using UnityEngine;

// Alguien o algo del campus que da una pista, un sospechoso o un objeto
// Mientras su pista no está en la libreta tiene un punto que late encima
public class ClueSpot : Interactable
{
    [SerializeField] int clue;      // número de la pista en el caso, empezando por 0
    [SerializeField] string source; // quién o qué la da, por ejemplo "Josep" o "Registro de entrada"
    [SerializeField] string line;   // lo que dice, por ejemplo "Estuve toda la noche en una videollamada."
    [SerializeField] Transform dot; // el punto que late

    public int Clue => clue;
    public string Source => source;
    public string Line => line;

    void Update()
    {
        // el punto se hace un poco más grande y más pequeño todo el rato
        float scale = 1f + 0.15f * Mathf.Sin(Time.time * 4f);
        dot.localScale = new Vector3(scale, scale, 1f);
    }

    // Cuando su pista ya está en la libreta el punto se quita, porque no queda nada nuevo
    public void MarkFound(bool found)
    {
        dot.gameObject.SetActive(!found);
    }
}