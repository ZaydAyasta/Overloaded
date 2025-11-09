using UnityEngine;
using UnityEngine.UI;   // solo si usas Text clásico
using TMPro;            // solo si usas TextMeshPro
using System.Collections.Generic; // si usas listas/diccionarios


public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public float moveSpeed = 4f;

    [Header("Camera")]
    public Transform cam;
    public Vector3 offset = new Vector3(0, 5, -6);
    public float camSpeed = 5f;

    [Header("Task Settings")]
    public Transform caldera;
    public Transform targetZone;
    public float pickupRange = 2f;
    public float dropRange = 1.5f;
    public KeyCode actionKey = KeyCode.E;
    private bool isCarrying = false;

    [Header("Timer")]
    public float totalTime = 60f;
    private float currentTime;
    public Text timerText;

    [Header("UI")]
    public Text taskText;

    void Start()
    {
        currentTime = totalTime;
        taskText.text = "Tarea: Cargar la caldera y colocarla en su base";
    }

    void Update()
    {
        // Movimiento del personaje
        PlayerMovement();

        // Cámara suave
        UpdateCamera();

        // Interacción de tareas
        HandleCalderaTask();

        // Temporizador global
        UpdateTimer();
    }

    void PlayerMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v).normalized;
        if (dir.magnitude > 0)
        {
            player.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
            player.forward = dir; // orientación del personaje
        }
    }

    void UpdateCamera()
    {
        Vector3 desiredPosition = player.position + offset;
        cam.position = Vector3.Lerp(cam.position, desiredPosition, Time.deltaTime * camSpeed);
        cam.LookAt(player);
    }

    void HandleCalderaTask()
    {
        float distanceToCaldera = Vector3.Distance(player.position, caldera.position);
        float distanceToTarget = Vector3.Distance(player.position, targetZone.position);

        // Recoger caldera
        if (!isCarrying && distanceToCaldera < pickupRange && Input.GetKeyDown(actionKey))
        {
            isCarrying = true;
            taskText.text = "Tarea: Lleva la caldera a su base";
        }

        // Mientras la lleva
        if (isCarrying)
        {
            caldera.position = player.position + player.forward * 0.7f + Vector3.up * 0.5f;

            // Soltar caldera
            if (Input.GetKeyDown(actionKey))
            {
                if (distanceToTarget <= dropRange)
                {
                    isCarrying = false;
                    caldera.position = targetZone.position;
                    StartCoroutine(StartAutoTask());
                }
                else
                {
                    isCarrying = false;
                    caldera.position = player.position + Vector3.down * 0.3f;
                    taskText.text = "Lugar incorrecto ❌ Intenta de nuevo";
                }
            }
        }
    }

    System.Collections.IEnumerator StartAutoTask()
    {
        taskText.text = "Tarea automática: Hervir agua...";
        yield return new WaitForSeconds(5f);
        taskText.text = "✅ Tarea completada: Agua lista";
    }

    void UpdateTimer()
    {
        currentTime -= Time.deltaTime;
        timerText.text = "Tiempo: " + Mathf.Round(currentTime).ToString() + "s";

        if (currentTime <= 0)
        {
            taskText.text = "⏰ Tiempo agotado. No completaste la tarea.";
            enabled = false;
        }
    }
}
