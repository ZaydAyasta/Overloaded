using System;
using UnityEngine;

public class CleaningManager : MonoBehaviour
{
    [Header("UI de Tareas")]
    [Tooltip("Arrastra aquí el componente de tu UI que tiene CompletarTarea / CompletarTareaPorTexto.")]
    public MonoBehaviour tareasUI;

    [Tooltip("Usa índice (>=0) o texto para marcar la tarea.")]
    public int tareaIndex = -1;
    public string tareaTexto = "Barrer polvos";

    [Header("Estado (solo lectura)")]
    [SerializeField] private int totalPolvos = 0;
    [SerializeField] private int cleanedPolvos = 0;
    [SerializeField] private bool tareaCompletada = false;

    // --- Llamado por cada Polvo en Start() ---
    public void RegisterPolvo(Polvo p)
    {
        totalPolvos++;
        // Si ya estaba completa y aparece uno nuevo, reabrimos (opcional)
        if (tareaCompletada && cleanedPolvos < totalPolvos)
        {
            tareaCompletada = false;
            // Si tu UI no tiene "desmarcar", simplemente no hagas nada aquí
            // (o implementa tu propio método para desmarcar si lo necesitas)
        }
    }

    // --- Llamado por Polvo cuando se limpia ---
    public void OnPolvoCleaned(Polvo p)
    {
        cleanedPolvos++;
        CheckEstado();
    }

    private void CheckEstado()
    {
        if (!tareaCompletada && totalPolvos > 0 && cleanedPolvos >= totalPolvos)
        {
            tareaCompletada = true;
            TryMarcarUI();
        }
        else if (tareaCompletada && cleanedPolvos < totalPolvos)
        {
            tareaCompletada = false;
            // Aquí podrías desmarcar la tarea si tu UI lo soporta.
        }
    }

    private void TryMarcarUI()
    {
        if (tareasUI == null) return;
        Type t = tareasUI.GetType();

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

    // Accesores opcionales (por si quieres mostrar progreso)
    public int GetTotal() => totalPolvos;
    public int GetRemaining() => Mathf.Max(0, totalPolvos - cleanedPolvos);
}
