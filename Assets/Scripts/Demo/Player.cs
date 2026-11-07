using UnityEngine;
using UnityEngine.InputSystem;

// El personaje que se mueve por el campus. A y D (o las flechas) lo mueven a izquierda y derecha
// La partida decide cuándo se puede mover y dónde aparece al cruzar una puerta
public class Player : MonoBehaviour
{
    [SerializeField] float speed = 5f;   // unidades por segundo
    [SerializeField] float limit = 8.9f; // hasta dónde puede llegar desde el centro de la sala

    public bool CanMove; // se apaga cuando hay una pantalla encima, como la libreta

    SpriteRenderer sprite;
    float baseY;
    float walkTime;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        baseY = transform.position.y;
    }

    void Update()
    {
        float direction = 0f;
        Keyboard keyboard = Keyboard.current;
        if (CanMove && keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                direction -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                direction += 1f;
        }

        Vector3 position = transform.position;
        position.x = Mathf.Clamp(position.x + direction * speed * Time.deltaTime, -limit, limit);

        // Mira hacia donde anda y da un pequeño bote al caminar, así se nota que anda sin tener animación
        if (direction != 0f)
        {
            sprite.flipX = direction < 0f;
            walkTime += Time.deltaTime;
        }
        else
            walkTime = 0f;
        position.y = baseY + Mathf.Abs(Mathf.Sin(walkTime * 10f)) * 0.08f;

        transform.position = position;
    }

    // Lo pone en otra x, por ejemplo al entrar en una sala por una puerta
    public void MoveTo(float x)
    {
        Vector3 position = transform.position;
        position.x = x;
        transform.position = position;
    }
}