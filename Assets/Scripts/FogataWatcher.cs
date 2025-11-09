using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class FogataWatcher : MonoBehaviour
{
    [SerializeField] private Transform ollaTransform; // arrastra la olla (Transform)
    [SerializeField] private bool forceTrigger = true;

    public bool IsOllaInside { get; private set; }

    private Collider2D _col;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        if (forceTrigger) _col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (ollaTransform != null && other.transform == ollaTransform)
            IsOllaInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (ollaTransform != null && other.transform == ollaTransform)
            IsOllaInside = false;
    }
}
