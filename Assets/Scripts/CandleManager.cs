using UnityEngine;

public class CandleManager : MonoBehaviour
{
    [Header("UI de Tareas")]
    public MonoBehaviour tareasUI;
    public int tareaIndex = -1;
    public string tareaTexto = "Encender velas";

    [Header("Estado (solo lectura)")]
    [SerializeField] private int totalCandles = 0;
    [SerializeField] private int litCandles = 0;
    [SerializeField] private bool tareaCompletada = false;

    public void RegisterCandlestick(Candlestick c)
    {
        totalCandles++;
        if (tareaCompletada && litCandles < totalCandles)
            tareaCompletada = false;
    }

    public void OnCandlestickLit(Candlestick c)
    {
        litCandles++;
        CheckEstado();
    }

    public void OnCandlestickUnlit(Candlestick c)
    {
        litCandles = Mathf.Max(0, litCandles - 1);
        CheckEstado();
    }

    void CheckEstado()
    {
        if (!tareaCompletada && totalCandles > 0 && litCandles >= totalCandles)
        {
            tareaCompletada = true;
            TryMarcarUI();
        }
    }

    void TryMarcarUI()
    {
        if (tareasUI == null) return;

        var t = tareasUI.GetType();

        if (tareaIndex >= 0)
        {
            var m = t.GetMethod("CompletarTarea");
            if (m != null) m.Invoke(tareasUI, new object[] { tareaIndex });
        }
        else if (!string.IsNullOrEmpty(tareaTexto))
        {
            var m2 = t.GetMethod("CompletarTareaPorTexto");
            if (m2 != null) m2.Invoke(tareasUI, new object[] { tareaTexto });
        }
    }
}
