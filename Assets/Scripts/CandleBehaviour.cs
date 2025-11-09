using UnityEngine;

public class CandleBehaviour : GrabbableBehaviour
{
    [Header("Interacción")]
    public KeyCode useKey = KeyCode.E;

    [Header("Detección del candelabro")]
    public Transform tip;
    public float detectRadius = 0.6f;
    public LayerMask candlestickMask;

    private Candlestick nearby;
    private bool nearCandlestick = false;

    // Edge detection para que GetKeyDown funcione en FixedUpdate o Update
    private bool prevKeyHeld = false;

    public override bool WantsToBlockDrop() =>
        nearCandlestick || (nearby != null && nearby.IsFlashing);

    public override string GetDropBlockHint() =>
        nearCandlestick ? "No puedes soltar mientras interactúas." : null;

    public override void OnPickedUp(PlayerGrab player)
    {
        nearby = null;
        nearCandlestick = false;
        prevKeyHeld = false;
    }

    public override void OnDropped(PlayerGrab player)
    {
        nearby = null;
        nearCandlestick = false;
        prevKeyHeld = false;
    }

    public override void OnCarriedUpdate(PlayerGrab player, Vector2 moveInput)
    {
        // --- detectar candelabro cercano ---
        nearby = null;
        nearCandlestick = false;

        if (tip != null)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(tip.position, detectRadius, candlestickMask);
            float best = float.MaxValue;

            foreach (var h in hits)
            {
                Candlestick c = h.GetComponent<Candlestick>() ?? h.GetComponentInParent<Candlestick>();
                if (c == null) continue;

                float d = (tip.position - h.bounds.ClosestPoint(tip.position)).sqrMagnitude;
                if (d < best) { best = d; nearby = c; nearCandlestick = true; }
            }
        }

        // --- borde real del input ---
        bool keyHeld = Input.GetKey(useKey);
        bool justPressed = keyHeld && !prevKeyHeld;

        if (nearby != null && justPressed)
        {
            nearby.LightUp();
        }

        prevKeyHeld = keyHeld;
    }

    private void OnDrawGizmosSelected()
    {
        if (tip == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(tip.position, detectRadius);
    }
}
