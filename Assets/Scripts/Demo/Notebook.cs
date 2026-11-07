using System.Collections.Generic;
using Misterio.Core;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// La libreta del jugador. A la izquierda salen las pistas encontradas y a la derecha la hoja
// para tachar. Pulsando un lugar o una actividad se tacha, y pulsándolo otra vez se destacha
public class Notebook : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] TMP_Text cluesText;
    [SerializeField] TMP_Text gridText;

    // Colores de tinta sobre el papel de la libreta
    const string HeaderColor = "#6B4FA0";  // morado, para los títulos
    const string SourceColor = "#6F6582";  // gris oscuro, para quién dio cada pista
    const string SureColor = "#C2571A";    // naranja, lo único que le queda a alguien
    const string CrossedColor = "#9A90A8"; // gris claro, tachado

    CaseDefinition currentCase;
    List<int> found;
    List<string> sources;
    bool[,] crossedPlace;    // [sospechoso, lugar]
    bool[,] crossedActivity; // [sospechoso, actividad]

    // Deja la libreta en blanco para este caso. foundClues y foundSources son las listas que va llenando el juego
    public void Setup(CaseDefinition c, List<int> foundClues, List<string> foundSources)
    {
        currentCase = c;
        found = foundClues;
        sources = foundSources;
        crossedPlace = new bool[c.Suspects.Count, c.Places.Count];
        crossedActivity = new bool[c.Suspects.Count, c.Activities.Count];
    }

    public void Refresh()
    {
        WriteClues();
        WriteGrid();
    }

    // Unity lo llama al hacer clic en la libreta. Si el clic cae en un nombre de la hoja, se tacha
    public void OnPointerClick(PointerEventData eventData)
    {
        int link = TMP_TextUtilities.FindIntersectingLink(gridText, eventData.position, null);
        if (link == -1)
            return;

        Toggle(gridText.textInfo.linkInfo[link].GetLinkID());
        WriteGrid();
    }

    // El id del enlace es por ejemplo "p_2_1", el lugar 1 del sospechoso 2. Las actividades empiezan por "a"
    void Toggle(string id)
    {
        string[] parts = id.Split('_');
        int suspect = int.Parse(parts[1]);
        int value = int.Parse(parts[2]);
        if (parts[0] == "p")
            crossedPlace[suspect, value] = !crossedPlace[suspect, value];
        else
            crossedActivity[suspect, value] = !crossedActivity[suspect, value];
    }

    // Cada pista sale con quién o qué la dio, por ejemplo "Josep estaba en una videollamada. (Josep)"
    void WriteClues()
    {
        string text = "<color=" + HeaderColor + "><b>Pistas</b></color>  <size=80%>" + found.Count + " de " + currentCase.Clues.Count + "</size>\n";
        text += "<size=85%><i>Quien estaba en la biblioteca a las 23:00 robó el libro.</i></size>\n\n";
        if (found.Count == 0)
            text += "Todavía no has encontrado ninguna. Busca por el campus.";
        for (int i = 0; i < found.Count; i++)
        {
            text += (i + 1) + ". " + currentCase.Clues[found[i]].GetText(currentCase)
                + "  <size=80%><color=" + SourceColor + ">(" + sources[i] + ")</color></size>\n";
        }
        cluesText.text = text;
    }

    // <pos=120> pone lo que va detrás a 120 píxeles del borde, así los nombres quedan alineados
    void WriteGrid()
    {
        string text = "<color=" + HeaderColor + "><b>Hoja de deducción</b></color>  <size=80%>pulsa para tachar</size>\n";
        for (int s = 0; s < currentCase.Suspects.Count; s++)
        {
            text += "\n<b>" + currentCase.Suspects[s] + "</b><pos=120>" + ValuesLine(s, "p", currentCase.Places, crossedPlace) + "\n";
            text += "<pos=120>" + ValuesLine(s, "a", currentCase.Activities, crossedActivity) + "\n";
        }
        gridText.text = text;
    }

    // Una línea de la hoja. Cada nombre va dentro de un enlace para poder pulsarlo
    // Si a un sospechoso solo le queda uno sin tachar, sale en naranja porque ya es seguro
    static string ValuesLine(int suspect, string kind, List<string> names, bool[,] crossed)
    {
        int left = 0;
        for (int v = 0; v < names.Count; v++)
        {
            if (!crossed[suspect, v])
                left++;
        }

        string line = "";
        for (int v = 0; v < names.Count; v++)
        {
            if (v > 0)
                line += "   ";

            string name = names[v];
            if (crossed[suspect, v])
                name = "<color=" + CrossedColor + "><s>" + name + "</s></color>";
            else if (left == 1)
                name = "<color=" + SureColor + "><b>" + name + "</b></color>";

            line += "<link=\"" + kind + "_" + suspect + "_" + v + "\">" + name + "</link>";
        }
        return line;
    }
}