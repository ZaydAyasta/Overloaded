using UnityEngine;

[DisallowMultipleComponent]
public class OllaCarryController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Grabbable olla;      // arrastra la OLLA
    [SerializeField] private Transform holdPoint; // punto donde se lleva
    [SerializeField] private FogataWatcher fogata;// arrastra Fogata1_0 (con FogataWatcher)

    [Header("Ajustes")]
    [SerializeField] private KeyCode key = KeyCode.E;
    [SerializeField] private float pickupRange = 1.1f;

    private Rigidbody2D _rbOlla;
    private bool _carrying;

    private void Awake()
    {
        if (olla != null) _rbOlla = olla.GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(key))
        {
            if (_carrying) TryDrop();
            else TryPick();
        }
    }

    private void TryPick()
    {
        if (olla == null || holdPoint == null) return;

        if (Vector2.Distance(holdPoint.position, olla.transform.position) > pickupRange)
            return;

        _carrying = true;
        olla.OnPickedUp();

        olla.transform.SetParent(holdPoint);
        olla.transform.localPosition = Vector3.zero;

        if (_rbOlla != null)
        {
            _rbOlla.linearVelocity = Vector2.zero;
            _rbOlla.angularVelocity = 0f;
            _rbOlla.isKinematic = true;
        }
    }

    private void TryDrop()
    {
        if (olla == null) return;

        bool canDropHere = (fogata == null) ? true : fogata.IsOllaInside;
        if (!canDropHere) return; // bloquea soltar fuera de la fogata

        olla.transform.SetParent(null);
        if (_rbOlla != null) _rbOlla.isKinematic = false;

        _carrying = false;
        olla.OnDropped(); // disparar� PotReleaseNotifier por cambio de parent
    }
}
