using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.UI;
public class GameEnd : MonoBehaviour
{
    public static GameEnd Instance { get; private set; }
    public GameObject PanelTareas;  

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
    }

    public void Win()
{
    if (ended) return;
    ended = true;

    if (PanelTareas != null)
        PanelTareas.SetActive(false);

    // Instanciamos el prefab de pantalla de victoria
    var g = Instantiate(pantallaPrefab);
    var p = g.GetComponent<Pantalla>();
    if (p != null)
        p.win();
    else
        Debug.LogWarning("El prefab de pantalla no tiene componente Pantalla");

    // Obtener el panel de Win
    Transform win = g.transform.Find("win");  // Buscar el panel de victoria dentro del prefab
    if (win != null)
    {
        // Obtener los botones dentro del panel de victoria
        Button[] buttons = win.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.name == "Button1") // Si es el primer botón (Reiniciar)
            {
                button.onClick.AddListener(ResetGame);
                Debug.Log("Se ha asignado ResetGame al Button1 (Reiniciar).");
            }
            else if (button.name == "Button2") // Si es el segundo botón (Ir al Menú)
            {
                button.onClick.AddListener(GoToMainMenu);
                Debug.Log("Se ha asignado GoToMainMenu al Button2 (Ir al Menú).");
            }
        }
    }
}

public void Lose()
{
    if (ended) return;
    ended = true;

    if (PanelTareas != null)
        PanelTareas.SetActive(false);

    // Instanciamos el prefab de pantalla de derrota
    var g = Instantiate(pantallaPrefab);
    var p = g.GetComponent<Pantalla>();
    if (p != null)
        p.lose();
    else
        Debug.LogWarning("El prefab de pantalla no tiene componente Pantalla");

    // Obtener el panel de Lose
    Transform lose = g.transform.Find("lose");  // Buscar el panel de derrota dentro del prefab
    if (lose != null)
    {
        // Obtener los botones dentro del panel de derrota
        Button[] buttons = lose.GetComponentsInChildren<Button>();
        foreach (Button button in buttons)
        {
            if (button.name == "Button3") // Si es el primer botón de derrota (Reiniciar)
            {
                button.onClick.AddListener(ResetGame);
                Debug.Log("Se ha asignado ResetGame al Button3 (Reiniciar).");
            }
            else if (button.name == "Button4") // Si es el segundo botón de derrota (Ir al Menú)
            {
                button.onClick.AddListener(GoToMainMenu);
                Debug.Log("Se ha asignado GoToMainMenu al Button4 (Ir al Menú).");
            }
        }
    }
    else
    {
        Debug.LogWarning("No se encontró el panel de derrota (lose) dentro del prefab.");
    }
}


    public void ResetGame()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void GoToMainMenu()
{
    // Asegúrate de que el nombre de la escena coincida con el nombre de tu escena del menú principal
    SceneManager.LoadScene("Title");  // O usa el nombre de tu escena de menú principal
}
}
