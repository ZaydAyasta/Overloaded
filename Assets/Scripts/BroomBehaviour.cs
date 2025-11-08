using UnityEngine;

public class BroomBehaviour : GrabbableBehaviour
{
    [Header("Oscilación de la escoba")]
    public float rotationAmplitude = 15f;  // grados máximos
    public float rotationSpeed = 6f;       // velocidad del vaivén

    private float currentAngle = 0f;
    private float oscillationTime = 0f;

    private Transform t;

    void Awake()
    {
        t = transform;
    }

    public override void OnCarriedUpdate(PlayerGrab player, Vector2 moveInput)
    {
        if (Mathf.Abs(moveInput.x) > 0.01f)
        {
            oscillationTime += Time.deltaTime * rotationSpeed;
            float target = Mathf.Sin(oscillationTime) * rotationAmplitude;
            currentAngle = Mathf.Lerp(currentAngle, target, Time.deltaTime * 8f);
        }
        else
        {
            // volver suavemente al ángulo neutro
            currentAngle = Mathf.Lerp(currentAngle, 0f, Time.deltaTime * 8f);
        }

        // aplicar rotación
        t.localRotation = Quaternion.Euler(0f, 0f, currentAngle);
    }

    public override void OnDropped(PlayerGrab player)
    {
        // volver al ángulo neutro
        t.localRotation = Quaternion.identity;
    }
}
