// PotReleaseNotifier.cs  (EN LA OLLA)
using UnityEngine;
using System;

[DisallowMultipleComponent]
public class PotReleaseNotifier : MonoBehaviour
{
    [SerializeField] private string playerRootName = "Personaje";
    public event Action<GameObject> OnReleased;

    private bool _wasChild;

    private void Start() { _wasChild = IsChildOfPlayer(transform); }

    private void OnTransformParentChanged()
    {
        bool nowChild = IsChildOfPlayer(transform);
        if (_wasChild && !nowChild) OnReleased?.Invoke(gameObject); // soltó
        _wasChild = nowChild;
    }

    private bool IsChildOfPlayer(Transform t)
    {
        while (t != null) { if (t.name == playerRootName) return true; t = t.parent; }
        return false;
    }
}
