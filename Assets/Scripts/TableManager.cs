using System.Collections.Generic;
using UnityEngine;

public class TableManager : MonoBehaviour
{
    [Header("UI de Tareas (opcional)")]
    public MonoBehaviour tareasUI;   // componente que tiene CompletarTarea(int) / CompletarTareaPorTexto(string)
    public int tareaIndex = -1;      // usa índice si >= 0
    public string tareaTexto = "Limpiar las mesas";

    // Conjuntos para evitar duplicados
    private readonly HashSet<TableTask> all = new HashSet<TableTask>();
    private readonly HashSet<TableTask> done = new HashSet<TableTask>();

    [Header("Estado (solo lectura)")]
    [SerializeField] private int totalTables = 0;
    [SerializeField] private int completedTables = 0;
    [SerializeField] private bool tareaCompletada = false;

    // =====================================================================
    //                          REGISTRO DE MESAS
    // =====================================================================

    public void RegisterTable(TableTask t, bool isCompletedInitial = false)
    {
        if (t == null) return;

        if (all.Add(t))
        {
            if (isCompletedInitial)
                done.Add(t);

            totalTables = all.Count;
            completedTables = done.Count;

            if (tareaCompletada && completedTables < totalTables)
                tareaCompletada = false;

            CheckEstado();
        }
    }

    public void UnregisterTable(TableTask t)
    {
        if (t == null) return;

        if (all.Remove(t) | done.Remove(t))
        {
            totalTables = all.Count;
            completedTables = done.Count;

            if (tareaCompletada && completedTables < totalTables)
                tareaCompletada = false;

            CheckEstado();
        }
    }

    // =====================================================================
    //                          CUANDO SE COMPLETA UNA MESA
    // =====================================================================

    public void OnTableCleaned(TableTask t)
    {
        if (t == null) return;

        all.Add(t); // por si aún no estaba (seguridad)
        if (done.Add(t)) // solo suma si antes no era true
        {
            completedTables = done.Count;
            CheckEstado();
        }
    }

    // =====================================================================
    //                          CONTROL DE ESTADO
    // =====================================================================

    void CheckEstado()
    {
        if (!tareaCompletada &&
            totalTables > 0 &&
            completedTables >= totalTables)
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
            t.GetMethod("CompletarTarea")?.Invoke(tareasUI, new object[] { tareaIndex });
        }
        else if (!string.IsNullOrEmpty(tareaTexto))
        {
            t.GetMethod("CompletarTareaPorTexto")?.Invoke(tareasUI, new object[] { tareaTexto });
        }
    }

    public int GetTotal() => totalTables;
    public int GetRemaining() => Mathf.Max(0, totalTables - completedTables);
}
