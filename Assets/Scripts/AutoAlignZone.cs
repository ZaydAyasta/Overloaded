using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class AutoAlignZone2D : MonoBehaviour
{
    [Header("Oscilación (unidades de mundo)")]
    [SerializeField, Range(0f, 2f)] private float amplitude = 0.35f;
    [SerializeField, Range(0.1f, 5f)] private float speed = 1.1f;
    [SerializeField] private bool onlyWhenCarrying = true;

    [Header("Bordes y seguridad")]
    [SerializeField, Range(0f, 0.5f)] private float margin = 0.06f; // margen interno
    [SerializeField, Range(0f, 0.4f)] private float edgeSoftness = 0.12f; // suaviza al acercarse a bordes
    [SerializeField] private bool keepEntryY = false;

    [Header("Control (PD + límites)")]
    [SerializeField, Range(1f, 400f)] private float kp = 12f; // rigidez (pos)
    [SerializeField, Range(0.1f, 40f)] private float kd = 3.5f; // amortiguación (vel)
    [SerializeField, Range(2f, 800f)] private float maxAccel = 35f; // máx. aceleración (m/s^2)
    [SerializeField, Range(1f, 150f)] private float maxSpeedX = 7f; // tope de velocidad en X
    [SerializeField, Range(0f, 10f)] private float dragX = 0.08f; // freno leve por step

    [Header("Identificación del Player")]
    [SerializeField] private string playerGrabComponentName = "PlayerGrab"; // debe exponer bool IsCarryingSomething

    private Collider2D _zoneCol;
    private Rigidbody2D _rb;
    private Collider2D _rbCol;
    private Component _playerGrab;
    private int _isCarryingPropID; // acceso rápido a propiedad

    private float _centerX;
    private float _entryY;
    private bool _inside;
    private float _t;
    private float _phaseJitter;

    private void Awake()
    {
        _zoneCol = GetComponent<Collider2D>();
        _zoneCol.isTrigger = true;
        // cache de reflección para evitar GetComponent cada frame
        _isCarryingPropID = Shader.PropertyToID("IsCarryingSomething"); // solo para nombre único, no se usa con Shader.
        _phaseJitter = Random.Range(-0.6f, 0.6f); // hace menos predecible el vaivén
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var rb = other.attachedRigidbody;
        if (rb == null) return;

        // Identificamos Player por la presencia del componente "PlayerGrab"
        var grab = other.GetComponent(playerGrabComponentName);
        if (grab == null) return;

        _rb = rb;
        _rbCol = other;
        _playerGrab = grab;
        _inside = true;

        _centerX = _zoneCol.bounds.center.x;
        _entryY = _rb.position.y;
        _t = 0f;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.attachedRigidbody == _rb)
        {
            _inside = false;
            _rb = null;
            _rbCol = null;
            _playerGrab = null;
        }
    }

    private bool IsCarrying()
    {
        if (_playerGrab == null) return false;
        // Intentamos leer propiedad pública IsCarryingSomething de forma segura.
        var prop = _playerGrab.GetType().GetProperty("IsCarryingSomething");
        if (prop != null && prop.PropertyType == typeof(bool))
            return (bool)prop.GetValue(_playerGrab);
        return false;
    }

    private void FixedUpdate()
    {
        if (!_inside || _rb == null) return;
        //if (onlyWhenCarrying && !IsCarrying()) return;
        if (onlyWhenCarrying)
        {
            if (!IsCarrying()) return;

            var propName = _playerGrab.GetType().GetProperty("CurrentGrabbedName");
            if (propName != null && propName.PropertyType == typeof(string))
            {
                string grabbedName = (string)propName.GetValue(_playerGrab);
                if (grabbedName != "olla")
                    return; 
            }
            else
            {
                return;
            }
        }

        Bounds zb = _zoneCol.bounds;
        float halfX = (_rbCol != null) ? _rbCol.bounds.extents.x : 0.1f;

        // límites internos (zona menos margen y semiancho del player)
        float minX = zb.min.x + margin + halfX;
        float maxX = zb.max.x - margin - halfX;

        // fase y objetivo del vaivén
        _t += Time.fixedDeltaTime * speed;
        float rawTargetX = _centerX + Mathf.Sin(_t + _phaseJitter) * amplitude;

        // suavizado de borde: acercándonos al borde, reducimos amplitud objetivo
        float zoneHalf = (maxX - minX) * 0.5f;
        float distFromCenterToEdge = zoneHalf;
        float ampAllowed = Mathf.Max(0f, distFromCenterToEdge * (1f - edgeSoftness));
        float targetX = Mathf.Clamp(_centerX + Mathf.Sin(_t + _phaseJitter) * Mathf.Min(amplitude, ampAllowed), minX, maxX);

        Vector2 pos = _rb.position;
        Vector2 vel = _rb.linearVelocity;

        // Control PD en X
        float error = targetX - pos.x;          // posición deseada - actual
        float desiredVel = error * kp;          // proporcional
        float accel = (desiredVel - vel.x) * kd; // derivativo sobre vel (crítico ≈ 2*sqrt(kp))

        // clamp de aceleración por paso
        float maxA = maxAccel;
        accel = Mathf.Clamp(accel, -maxA, maxA);

        // fuerza = m * a
        float forceX = accel * _rb.mass;
        _rb.AddForce(new Vector2(forceX, 0f), ForceMode2D.Force);

        // drag suave en X (reducimos jitter sin matar control)
        float damp = Mathf.Clamp01(dragX);
        _rb.linearVelocity = new Vector2(Mathf.MoveTowards(_rb.linearVelocity.x, 0f, damp * Time.fixedDeltaTime * maxAccel), _rb.linearVelocity.y);

        // limitador de velocidad en X
        if (Mathf.Abs(_rb.linearVelocity.x) > maxSpeedX)
        {
            _rb.linearVelocity = new Vector2(Mathf.Sign(_rb.linearVelocity.x) * maxSpeedX, _rb.linearVelocity.y);
        }

        // opcional: mantener Y de entrada (no toques fuerzas Y del RB)
        if (keepEntryY)
        {
            // corrijo posición Y de manera imperceptible para no luchar con la gravedad
            float y = Mathf.Lerp(_rb.position.y, _entryY, 0.2f);
            _rb.position = new Vector2(_rb.position.x, y);
        }
    }
}
