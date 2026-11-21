using System.Collections.Generic;
using UnityEngine;

// Alguien o algo del campus que da pistas, un sospechoso o un indicio
// Mientras le quedan pistas por apuntar tiene un punto que late encima
public class ClueSpot : Interactable
{
    [SerializeField] string source; // quién da las pistas. En los sospechosos, su nombre tal cual sale en el caso
    [SerializeField] Transform dot; 

    // Solo en los indicios. Un dibujo y un nombre por cada tipo de pista, en el orden lugar, actividad y relación
    [SerializeField] GameObject[] drawings;
    [SerializeField] string[] names;

    List<int> clues = new List<int>();

    public string Source => source;
    public List<int> Clues => clues;
    public bool IsEvidence => drawings.Length > 0;

    void Update()
    {
        float scale = 1f + 0.15f * Mathf.Sin(Time.time * 4f);
        dot.localScale = new Vector3(scale, scale, 1f);
    }

    // Las pistas de un sospechoso
    public void SetClues(List<int> newClues)
    {
        clues = newClues;
    }

    // La pista de un indicio. Enseña el dibujo de su tipo y se pone su nombre
    public void SetEvidence(int clue, int kind)
    {
        clues = new List<int> { clue };
        source = names[kind];
        for (int i = 0; i < drawings.Length; i++)
            drawings[i].SetActive(i == kind);
    }

    public void MarkFound(bool found)
    {
        dot.gameObject.SetActive(!found);
    }
}