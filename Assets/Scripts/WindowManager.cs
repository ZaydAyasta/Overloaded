using UnityEngine;

public class WindowManager : MonoBehaviour
{
    [Header("UI de Tareas")]
    public MonoBehaviour tareasUI;       // componente con CompletarTarea / CompletarTareaPorTexto
    public int tareaIndex = -1;          // usa índice si >= 0
    public string tareaTexto = "Limpiar ventanas";

    [Header("Estado (solo lectura)")]
    [SerializeField] private int totalWindows = 0;
    [SerializeField] private int cleanedWindows = 0;
    [SerializeField] private bool tareaCompletada = false;

    public void RegisterWindow(WindowPanel w)
    {
        totalWindows++;
        if (tareaCompletada && cleanedWindows < totalWindows)
            tareaCompletada = false;
    }

    public void OnWindowCleaned(WindowPanel w)
    {
        cleanedWindows++;
        CheckEstado();
    }

    void CheckEstado()
    {
        if (!tareaCompletada && totalWindows > 0 && cleanedWindows >= totalWindows)
        {
            tareaCompletada = true;
            TryMarcarUI();
        }
    }

    void TryMarcarUI()
    {
        if (tareasUI == null) return;
        var type = tareasUI.GetType();

        if (tareaIndex >= 0)
        {
            var m = type.GetMethod("CompletarTarea");
            if (m != null) m.Invoke(tareasUI, new object[] { tareaIndex });
        }
        else if (!string.IsNullOrEmpty(tareaTexto))
        {
            var m2 = type.GetMethod("CompletarTareaPorTexto");
            if (m2 != null) m2.Invoke(tareasUI, new object[] { tareaTexto });
        }
    }

    // opcional HUD
    public int GetTotal() => totalWindows;
    public int GetRemaining() => Mathf.Max(0, totalWindows - cleanedWindows);
}
