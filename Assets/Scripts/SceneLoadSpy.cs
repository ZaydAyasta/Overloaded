using UnityEngine;
using UnityEngine.SceneManagement;
using System.Diagnostics;

public class SceneLoadSpy : MonoBehaviour
{
    private static bool created = false;

    void Awake()
    {
        if (created)
        {
            Destroy(gameObject);
            return;
        }

        created = true;
        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        // Obtenemos stack trace desde 2 niveles arriba para ignorar llamadas internas
        StackTrace trace = new StackTrace(2, true);

        UnityEngine.Debug.Log(
            "\n\n DETECTADO CAMBIO DE ESCENA\n" +
            " De: " + oldScene.name + "\n" +
            " A:  " + newScene.name + "\n\n" +
            " Posible llamada desde:\n" + trace.ToString() +
            "\n----------------------------------------\n"
        );
    }
}
