// PotStateHandler.cs  (EN LA OLLA)
using UnityEngine;

[DisallowMultipleComponent]
public class PotStateHandler : MonoBehaviour
{
    [SerializeField] private bool resetVelocityOnRespawn = true;

    private Vector3 _spawnPos;
    private Quaternion _spawnRot;
    private Transform _spawnParent;
    private Rigidbody2D _rb;
    private Collider2D _col;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        _spawnPos = transform.position;
        _spawnRot = transform.rotation;
        _spawnParent = transform.parent;
    }

    // Llama la fogata al empezar el round (oculta la olla)
    public void HideOnStart()
    {
        gameObject.SetActive(false);
    }

    // Llama la fogata si fallas (reaparece en punto de spawn)
    public void ResetToSpawnAndShow()
    {
        transform.SetParent(_spawnParent);
        transform.SetPositionAndRotation(_spawnPos, _spawnRot);

        if (resetVelocityOnRespawn && _rb != null)
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
        if (_col != null && !_col.isTrigger) _col.enabled = true;
        gameObject.SetActive(true);
    }
}
