using UnityEngine;

public class GameEnd : MonoBehaviour
{
    public static GameEnd Instance { get; private set; }

    [Header("Prefab de pantalla final")]
    [SerializeField] GameObject pantallaPrefab;   // 👉 aquí vas a arrastrar el prefab "Final"

    bool ended = false;
    public bool HasEnded => ended;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // opcional: DontDestroyOnLoad(gameObject);
    }

    public void Win()
    {
        if (ended) return;
        ended = true;

        var g = Instantiate(pantallaPrefab);
        var p = g.GetComponent<Pantalla>();
        if (p != null)
            p.win();
        else
            Debug.LogWarning("El prefab de pantalla no tiene componente Pantalla");
    }

    public void Lose()
    {
        if (ended) return;
        ended = true;

        var g = Instantiate(pantallaPrefab);
        var p = g.GetComponent<Pantalla>();
        if (p != null)
            p.lose();
        else
            Debug.LogWarning("El prefab de pantalla no tiene componente Pantalla");
    }
}
