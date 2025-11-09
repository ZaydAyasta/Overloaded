// FogataPotTimer_NoTag.cs  (EN Fogata1_0)  — versión con HIDE + TIMER + RESPAWN
using UnityEngine;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public class FogataPotTimer_NoTag : MonoBehaviour
{
    [Header("Referencias (arrastra desde Hierarchy)")]
    [SerializeField] private PotReleaseNotifier pot;      // olla (con PotReleaseNotifier)
    [SerializeField] private PotStateHandler potState;    // olla (con PotStateHandler)
    [SerializeField] private Animator chimneyAnimator;    // opcional: anim de la chimenea
    [SerializeField] private string hasPotBool = "hasPot";// bool para cambiar estado visual

    [Header("Reglas")]
    [SerializeField] private float durationSeconds = 3f;
    [SerializeField] private bool isTrigger = true;

    // UI opcional (conecta un TMP si quieres mostrar 3-2-1)
    public event Action<int> OnSecondChanged;

    private Collider2D _col;
    private bool _potInside;
    private bool _running;
    private float _remaining;
    private int _lastShown = -1;

    private void Awake()
    {
        _col = GetComponent<Collider2D>();
        _col.isTrigger = isTrigger;
    }

    private void OnEnable() { if (pot) pot.OnReleased += HandleReleased; }
    private void OnDisable() { if (pot) pot.OnReleased -= HandleReleased; }

    private void Update()
    {
        if (!_running) return;

        _remaining -= Time.deltaTime;
        int s = Mathf.CeilToInt(Mathf.Max(0f, _remaining));
        if (s != _lastShown) { _lastShown = s; OnSecondChanged?.Invoke(s); }

        if (_remaining <= 0f)
        {
            _running = false;
            _remaining = 0f;

            // TIEMPO AGOTADO ⇒ fallo: reaparecer la olla y chimenea sin olla
            if (chimneyAnimator) chimneyAnimator.SetBool(hasPotBool, false);
            if (potState) potState.ResetToSpawnAndShow();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (pot != null && other.transform == pot.transform)
            _potInside = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (pot != null && other.transform == pot.transform)
            _potInside = false;
    }

    // Se dispara cuando la olla se SUELTA (deja de ser hija del Player)
    private void HandleReleased(GameObject obj)
    {
        if (_running) return;
        if (!_potInside) return; // condición estricta: soltó FUERA ⇒ no arranca

        // ÉXITO DE ENTRADA: oculta la olla y arranca el conteo
        if (potState) potState.HideOnStart();
        if (chimneyAnimator) chimneyAnimator.SetBool(hasPotBool, true);

        _running = true;
        _remaining = Mathf.Max(0.01f, durationSeconds);
        _lastShown = -1;
        OnSecondChanged?.Invoke(Mathf.CeilToInt(_remaining));
    }

    // Si alguna vez quieres “confirmar éxito” manual antes de que termine el timer:
    public void CompleteSuccessEarly()
    {
        if (!_running) return;
        _running = false;
        OnSecondChanged?.Invoke(0);
        // mantener chimenea con olla y la olla oculta (entregada)
    }
}
