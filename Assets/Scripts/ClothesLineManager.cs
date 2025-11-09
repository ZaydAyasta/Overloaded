using System.Collections.Generic;
using UnityEngine;

public class ClotheslineManager : MonoBehaviour
{
    [Header("UI de Tareas (opcional)")]
    public MonoBehaviour tareasUI;      // componente con CompletarTarea(int) / CompletarTareaPorTexto(string)
    public int tareaIndex = -1;         // usa índice si >= 0
    public string tareaTexto = "Tender la ropa";

    // Conjuntos para evitar duplicados
    private readonly HashSet<Clothesline> all = new HashSet<Clothesline>();
    private readonly HashSet<Clothesline> done = new HashSet<Clothesline>();

    [Header("Estado (solo lectura)")]
    [SerializeField] private int totalClotheslines = 0;
    [SerializeField] private int completedClotheslines = 0;
    [SerializeField] private bool tareaCompletada = false;

    // --- API llamada por Clothesline ---
    public void RegisterClothesline(Clothesline c, bool isCompletedInitial = false)
    {
        if (c == null) return;
        if (all.Add(c))
        {
            if (isCompletedInitial) done.Add(c);
            totalClotheslines = all.Count;
            completedClotheslines = done.Count;
            if (tareaCompletada && completedClotheslines < totalClotheslines) tareaCompletada = false;
            CheckEstado();
        }
    }

    public void UnregisterClothesline(Clothesline c)
    {
        if (c == null) return;
        if (all.Remove(c) | done.Remove(c))
        {
            totalClotheslines = all.Count;
            completedClotheslines = done.Count;
            if (tareaCompletada && completedClotheslines < totalClotheslines) tareaCompletada = false;
            CheckEstado();
        }
    }

    public void OnClotheslineCompleted(Clothesline c)
    {
        if (c == null) return;
        all.Add(c); // por si aún no estaba
        if (done.Add(c)) // solo suma si no estaba marcado
        {
            completedClotheslines = done.Count;
            CheckEstado();
        }
    }

    // --- Lógica de finalización de tarea ---
    void CheckEstado()
    {
        if (!tareaCompletada && totalClotheslines > 0 && completedClotheslines >= totalClotheslines)
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
            t.GetMethod("CompletarTarea")?.Invoke(tareasUI, new object[] { tareaIndex });
        else if (!string.IsNullOrEmpty(tareaTexto))
            t.GetMethod("CompletarTareaPorTexto")?.Invoke(tareasUI, new object[] { tareaTexto });
    }

    // Helpers (HUD)
    public int GetTotal() => totalClotheslines;
    public int GetRemaining() => Mathf.Max(0, totalClotheslines - completedClotheslines);
}
