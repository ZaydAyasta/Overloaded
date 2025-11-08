using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Grabbable : MonoBehaviour
{
    [Header("Posición al llevar (offset desde el jugador)")]
    public Vector2 carryOffset = new Vector2(0.35f, 0.5f);
    public bool mirrorOffsetWithPlayerFlip = true;

    [Header("Parpadeo y retorno al soltar")]
    [Tooltip("Capas que cuentan como suelo (p. ej. 'Ground').")]
    public LayerMask groundLayer;
    public float blinkDuration = 0.8f;
    public float blinkInterval = 0.12f;
    public float delayBeforeBlink = 0.2f;

    // Estado interno
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sr;

    private Vector3 originalPos;
    private Quaternion originalRot;
    private float originalGravity;

    private bool isCarried = false;
    private bool returning = false;
    private bool allowBlinkOnGround = false;
    private Coroutine blinkCo;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();

        originalPos = transform.position;
        originalRot = transform.rotation;
        originalGravity = rb.gravityScale;

        // Ajustes recomendados
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.freezeRotation = true; // quítalo si quieres que rote
    }

    // Llamado por PlayerGrab
    public void OnPickedUp()
    {
        isCarried = true;
        returning = false;
        allowBlinkOnGround = false;

        if (blinkCo != null)
        {
            StopCoroutine(blinkCo);
            blinkCo = null;
            sr.enabled = true;
        }
    }

    // Llamado por PlayerGrab
    public void OnDropped()
    {
        isCarried = false;
        allowBlinkOnGround = true;

        // asegurar que use la gravedad original
        rb.gravityScale = originalGravity <= 0f ? 1f : originalGravity;
    }

    void OnCollisionEnter2D(Collision2D c)
    {
        if (isCarried || returning) return;
        if (!allowBlinkOnGround) return;

        // ¿tocó suelo?
        if (((1 << c.collider.gameObject.layer) & groundLayer) != 0)
        {
            if (blinkCo == null)
                blinkCo = StartCoroutine(BlinkThenReturn());
        }
    }

    IEnumerator BlinkThenReturn()
    {
        returning = true;

        if (delayBeforeBlink > 0f)
            yield return new WaitForSeconds(delayBeforeBlink);

        float t = 0f;
        bool vis = true;

        while (t < blinkDuration)
        {
            vis = !vis;
            sr.enabled = vis;
            yield return new WaitForSeconds(blinkInterval);
            t += blinkInterval;
        }
        sr.enabled = true;

        // Teletransporta de vuelta “seguro”
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

        transform.position = originalPos;
        transform.rotation = originalRot;

        // Espera un FixedUpdate por si reaparece dentro de algo
        yield return new WaitForFixedUpdate();

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = originalGravity; // restaura su gravedad original
        returning = false;
        allowBlinkOnGround = false;
        blinkCo = null;
    }
}
