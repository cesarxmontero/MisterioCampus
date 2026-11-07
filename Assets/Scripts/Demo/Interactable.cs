using UnityEngine;

// Algo del campus que se usa con E, como un sospechoso, un objeto o una puerta
// Cuando es lo que el jugador tiene más cerca, enseña la tecla E encima
public abstract class Interactable : MonoBehaviour
{
    [SerializeField] GameObject key; // la tecla E

    public void SetNear(bool near)
    {
        key.SetActive(near);
    }
}