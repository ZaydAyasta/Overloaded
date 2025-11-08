using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5.5f;
    public float velocidadEscalera = 3f;

    private Animator anim;
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private float movX;
    private float movY;
    private bool enEscalera;

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
            rb.linearVelocity = new Vector2(movX * velocidad, movY * velocidadEscalera);
        }
        else
        {
            rb.gravityScale = 2f;
            rb.linearVelocity = new Vector2(movX * velocidad, rb.linearVelocity.y);
        }

        //anim.SetFloat("Speed", Mathf.Abs(movX) + Mathf.Abs(movY));

        if (movX != 0)
            sr.flipX = movX < 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Escalera")) { 
            Debug.Log("TOY EN ESCALERA", gameObject);
            enEscalera = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Escalera")) { 
            Debug.Log("TOY SALIENDO DE ESCALERA", gameObject);
            enEscalera = false;
        }
    }
}
