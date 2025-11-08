using UnityEngine;

public abstract class GrabbableBehaviour : MonoBehaviour
{
    // Llamado cada frame mientras el objeto está siendo cargado
    public virtual void OnCarriedUpdate(PlayerGrab player, Vector2 moveInput) {}

    // Llamado al ser agarrado
    public virtual void OnPickedUp(PlayerGrab player) {}

    // Llamado al soltarse
    public virtual void OnDropped(PlayerGrab player) {}
}
