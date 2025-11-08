using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 10f;
    public float velocidadEscalera = 4f;

    private Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private float movX;
    private float movY;
    private bool enEscalera;
    public Collider2D pisoSuperiorCollider;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        movX = Input.GetAxisRaw("Horizontal");
        movY = Input.GetAxisRaw("Vertical");

        if (enEscalera)
        {
            rb.gravityScale = 0f;

            rb.linearVelocity = new Vector2(0f, movY * velocidadEscalera);

            if (movY == 0 && Mathf.Abs(movX) > 0.1f)
            {
                rb.linearVelocity = new Vector2(movX * velocidad, 0f);
                enEscalera = false;
                pisoSuperiorCollider.enabled = true;
            }
        }
        else
        {
            rb.gravityScale = 2f;
            rb.linearVelocity = new Vector2(movX * velocidad, rb.linearVelocity.y);
        }

        if (movX != 0)
            sr.flipX = movX < 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Escalera"))
        {
            pisoSuperiorCollider.enabled = false;
            enEscalera = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Escalera"))
        {
            pisoSuperiorCollider.enabled = true;
            enEscalera = false;
        }
    }
}
