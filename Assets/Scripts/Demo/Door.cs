using UnityEngine;

// Una puerta que lleva a otra sala. Al pulsar E delante, la partida funde a negro y cambia de sala
public class Door : Interactable
{
    [SerializeField] int target; // la sala a la que lleva

    public int Target => target;
}