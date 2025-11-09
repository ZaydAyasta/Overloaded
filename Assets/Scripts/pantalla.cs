using UnityEngine;
using UnityEngine.SceneManagement;
public class Pantalla : MonoBehaviour
{
    public Animator ani;

    void Awake()
    {
        // Buscar aunque esté en hijos
        if (ani == null)
            ani = GetComponentInChildren<Animator>();
    }

    public void win()
    {
        if (ani == null)      return;
        

        ani.CrossFade("WinNew", 0.01f);
    }

    public void lose()
    {
        if (ani == null) return;

        ani.CrossFade("LoseNew", 0.01f);
    }

    public void restart() => SceneManager.LoadScene("escena");
}
