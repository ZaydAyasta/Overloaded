using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenTransition : MonoBehaviour
{
    public CanvasGroup fadeCg;          
    public float fadeDuration = 0.6f;
    public string nextSceneName = "GameScene";

    bool isTransitioning = false;

    void Start()
    {
        if (fadeCg != null)
        {
            fadeCg.alpha = 0f;
            fadeCg.blocksRaycasts = false;
        }
    }

    void Update()
    {
        if (!isTransitioning && Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(DoTransition());
        }
    }

    System.Collections.IEnumerator DoTransition()
    {
        isTransitioning = true;
        fadeCg.blocksRaycasts = true;

        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            fadeCg.alpha = Mathf.Lerp(0f, 1f, t / fadeDuration);
            yield return null;
        }

        fadeCg.alpha = 1f;

        SceneManager.LoadScene(nextSceneName);
    }
}
