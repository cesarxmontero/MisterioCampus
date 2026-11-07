using System.Collections;
using System.Collections.Generic;
using Misterio.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Demo jugable de la PEC 2 con el caso de la memoria
// El jugador anda por el campus con A y D. Con E habla con los sospechosos, mira los objetos y cruza
// las puertas. Las pistas se apuntan en la libreta, que se abre con Tab, y desde ella se acusa
// El juego sabe quién fue porque al empezar le pide la solución al solucionador completo
public class DemoGame : MonoBehaviour
{
    const float FadeTime = 0.25f; // lo que tarda el fundido a negro, en segundos

    [Header("Pantallas")]
    [SerializeField] GameObject startScreen;
    [SerializeField] GameObject hud;
    [SerializeField] GameObject notebookScreen;
    [SerializeField] GameObject endScreen;
    [SerializeField] Image fade; // la imagen negra del fundido

    [Header("Campus")]
    [SerializeField] GameObject[] places;        
    [SerializeField] Player player;
    [SerializeField] float interactRange = 1.3f; // a qué distancia se puede usar algo con E

    [Header("Textos del campus")]
    [SerializeField] TMP_Text placeText;
    [SerializeField] TMP_Text messageText;

    [Header("Libreta y final")]
    [SerializeField] Notebook notebook;
    [SerializeField] TMP_Text endText;

    CaseDefinition currentCase;
    Solution solution;
    HumanResult humanResult;
    List<int> found = new List<int>();         // las pistas encontradas, en el orden en que se encuentran
    List<string> sources = new List<string>(); // quién o qué dio cada una
    int current;        // la sala en la que está el jugador
    bool changingPlace; // true mientras dura el fundido

    void Start()
    {
        currentCase = ExampleCases.Basic();

        // El solucionador completo da la solución, y de paso se comprueba que solo hay una
        List<Solution> solutions = CompleteSolver.FindSolutions(currentCase, 2);
        if (solutions.Count != 1)
        {
            Debug.LogError("El caso de la demo tiene " + solutions.Count + " soluciones y tendría que tener una");
            return;
        }
        solution = solutions[0];

        // El humano dice cuántos pasos hacen falta y lo difícil que es, para enseñarlo al final
        humanResult = HumanSolver.Solve(currentCase);

        ShowStart();
    }

    void Update()
    {
        FitCamera();

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || changingPlace)
            return;

        // En la pantalla de inicio y en la final, E hace lo mismo que el botón
        if (startScreen.activeSelf)
        {
            if (keyboard.eKey.wasPressedThisFrame)
                StartGame();
            return;
        }
        if (endScreen.activeSelf)
        {
            if (keyboard.eKey.wasPressedThisFrame)
                ShowStart();
            return;
        }

        // La libreta se abre y se cierra con Tab, y también se cierra con Escape
        if (keyboard.tabKey.wasPressedThisFrame)
        {
            if (notebookScreen.activeSelf)
                CloseNotebook();
            else
                OpenNotebook();
            return;
        }
        if (notebookScreen.activeSelf)
        {
            if (keyboard.escapeKey.wasPressedThisFrame)
                CloseNotebook();
            return;
        }

        // Lo que está más cerca enseña la E, y con E se usa
        Interactable nearest = NearestInteractable();
        foreach (Interactable thing in places[current].GetComponentsInChildren<Interactable>())
            thing.SetNear(thing == nearest);
        if (nearest == null || !keyboard.eKey.wasPressedThisFrame)
            return;

        ClueSpot spot = nearest as ClueSpot;
        if (spot != null)
            Talk(spot);
        Door door = nearest as Door;
        if (door != null)
            StartCoroutine(GoThrough(door));
    }

    // Pantalla de inicio. También es lo que hace el botón de volver a empezar

    public void ShowStart()
    {
        startScreen.SetActive(true);
        hud.SetActive(false);
        notebookScreen.SetActive(false);
        endScreen.SetActive(false);
        player.CanMove = false;
        SetFade(0f);
    }

    public void StartGame()
    {
        // libreta en blanco y todos los puntos otra vez encendidos. El true es para que también
        // encuentre los de las salas apagadas
        found.Clear();
        sources.Clear();
        notebook.Setup(currentCase, found, sources);
        foreach (GameObject place in places)
        {
            foreach (ClueSpot spot in place.GetComponentsInChildren<ClueSpot>(true))
                spot.MarkFound(false);
        }

        startScreen.SetActive(false);
        hud.SetActive(true);

        current = currentCase.CrimePlace; // se empieza en la biblioteca, donde fue el robo
        ShowPlace();
        player.MoveTo(0f);
        player.CanMove = true;
        Deselect();
        messageText.text = "Usa A y D para moverte y E para hablar, mirar o cruzar puertas. Tab abre la libreta.";
    }

    // Campus

    void ShowPlace()
    {
        for (int i = 0; i < places.Length; i++)
            places[i].SetActive(i == current);

        placeText.text = Capitalize(currentCase.Places[current]);

        // ninguna E encendida al entrar, la que toque se enciende cuando el jugador se acerque
        foreach (Interactable thing in places[current].GetComponentsInChildren<Interactable>())
            thing.SetNear(false);

        // cuántas pistas de esta sala faltan por apuntar
        int missing = 0;
        foreach (ClueSpot spot in places[current].GetComponentsInChildren<ClueSpot>())
        {
            if (!found.Contains(spot.Clue))
                missing++;
        }
        if (missing == 0)
            messageText.text = "En esta sala ya no queda nada nuevo.";
        else
            messageText.text = "Acércate a lo que tenga un punto encima y pulsa E.";
    }

    // Lo más cercano al jugador en esta sala con lo que se puede hacer algo, o null si no hay nada a tiro
    Interactable NearestInteractable()
    {
        Interactable nearest = null;
        float best = interactRange;
        foreach (Interactable thing in places[current].GetComponentsInChildren<Interactable>())
        {
            float distance = Mathf.Abs(thing.transform.position.x - player.transform.position.x);
            if (distance <= best)
            {
                nearest = thing;
                best = distance;
            }
        }
        return nearest;
    }

    // Habla con un sospechoso o mira un objeto. La frase sale siempre, pero la pista se apunta solo la primera vez
    void Talk(ClueSpot spot)
    {
        string text = "<b>" + spot.Source + "</b>   " + spot.Line + "\n<size=80%>";
        if (found.Contains(spot.Clue))
            text += "Esto ya lo tienes en la libreta.";
        else
        {
            found.Add(spot.Clue);
            sources.Add(spot.Source);
            text += "Pista " + found.Count + " de " + currentCase.Clues.Count + " en la libreta  ·  "
                + currentCase.Clues[spot.Clue].GetText(currentCase);
        }
        spot.MarkFound(true);
        messageText.text = text + "</size>";
    }

    // Cruza una puerta. Funde a negro, cambia de sala y deja al jugador junto a la puerta que lleva
    // de vuelta.
    IEnumerator GoThrough(Door door)
    {
        changingPlace = true;
        player.CanMove = false;
        for (float t = 0f; t < 1f; t += Time.deltaTime / FadeTime)
        {
            SetFade(t);
            yield return null;
        }
        SetFade(1f);

        int from = current;
        current = door.Target;
        ShowPlace();
        foreach (Door back in places[current].GetComponentsInChildren<Door>())
        {
            if (back.Target == from)
            {
                float x = back.transform.position.x;
                player.MoveTo(x < 0f ? x + 1.5f : x - 1.5f);
            }
        }

        for (float t = 1f; t > 0f; t -= Time.deltaTime / FadeTime)
        {
            SetFade(t);
            yield return null;
        }
        SetFade(0f);
        player.CanMove = true;
        changingPlace = false;
    }

    void SetFade(float alpha)
    {
        fade.color = new Color(0f, 0f, 0f, alpha);
    }

    // Llibreta

    public void OpenNotebook()
    {
        notebookScreen.SetActive(true);
        notebook.Refresh();
        player.CanMove = false;
        Deselect();
    }

    public void CloseNotebook()
    {
        notebookScreen.SetActive(false);
        player.CanMove = true;
        Deselect();
    }

    // Final. Los botones de acusar pasan el número del sospechoso, Josep es el 0
    public void Accuse(int suspect)
    {
        int culprit = solution.WhoWasIn(currentCase.CrimePlace);

        string text;
        if (suspect == culprit)
            text = "<size=130%><b>¡Has acertado!</b></size>\n" + Story(culprit) + " Fue quien robó el libro.\n\n";
        else
            text = "<size=130%><b>No fue " + currentCase.Suspects[suspect] + "</b></size>\n" + Story(suspect)
                + " Quien robó el libro fue " + currentCase.Suspects[culprit] + ".\n\n";

        text += "<b>Lo que pasó esa noche</b>\n";
        for (int s = 0; s < currentCase.Suspects.Count; s++)
            text += Story(s) + "\n";

        text += "\nHas encontrado " + found.Count + " de " + currentCase.Clues.Count + " pistas.";
        text += "\nEl solucionador humano resuelve este caso en " + humanResult.Steps.Count
            + " pasos, con una dificultad de " + humanResult.TotalCost() + ".";
        endText.text = text;

        notebookScreen.SetActive(false);
        hud.SetActive(false);
        endScreen.SetActive(true);
        player.CanMove = false;
    }

    // Por ejemplo "Ari estaba en la biblioteca estudiando."
    string Story(int suspect)
    {
        return currentCase.Suspects[suspect] + " estaba en " + currentCase.Places[solution.PlaceOf[suspect]]
            + " " + currentCase.Activities[solution.ActivityOf[suspect]] + ".";
    }

    // "la biblioteca" pasa a "La biblioteca"
    static string Capitalize(string text)
    {
        return char.ToUpper(text[0]) + text.Substring(1);
    }

    // Quita la selección de los botones, si no A, D y Espacio pueden mover la selección o pulsarlos
    static void Deselect()
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    // Cada sala mide 19,2 x 10,8 unidades. Si la ventana es más estrecha que 16:9 se aleja la cámara
    // pra que la sala se vea entera de lado a lado
    static void FitCamera()
    {
        Camera cam = Camera.main;
        if (cam != null)
            cam.orthographicSize = Mathf.Max(5.4f, 9.6f / cam.aspect);
    }
}