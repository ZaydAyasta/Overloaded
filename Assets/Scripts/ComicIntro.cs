using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class ComicIntro : MonoBehaviour
{
    public RectTransform comicImage;        // Imagen gigante del comic
    public CanvasGroup cg;                  // Para fades del panel
    public float fadeTime = 0.4f;

    public Vector2 topLeft;
    public Vector2 topRight;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;

    public TextMeshProUGUI pressToPlayText;  // TEXTO TMP
    public string nextScene = "Fusion";

    int index = 0;
    bool isMoving = false;
    bool ending = false;

    Vector2[] positions;

    void Start()
    {
        positions = new Vector2[] {
            topLeft,
            topRight,
            bottomLeft,
            bottomRight
        };

        // Desactivar texto inicial
        pressToPlayText.gameObject.SetActive(false);

        cg.alpha = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        comicImage.anchoredPosition = positions[0];
    }

    void Update()
    {
        if (isMoving) return;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (ending)
            {
                SceneManager.LoadScene(nextScene);
                return;
            }

            NextStep();
        }
    }

    void NextStep()
    {
        index++;

        if (index < positions.Length)
        {
            StartCoroutine(SmoothMove(positions[index]));
        }
        else
        {
            pressToPlayText.gameObject.SetActive(true);
            StartCoroutine(FadeInTMP());
            ending = true;
        }
    }

    IEnumerator SmoothMove(Vector2 target)
    {
        isMoving = true;

        Vector2 start = comicImage.anchoredPosition;
        float t = 0f;

        while (t < fadeTime)
        {
            t += Time.deltaTime;
            comicImage.anchoredPosition = Vector2.Lerp(start, target, t / fadeTime);
            yield return null;
        }

        comicImage.anchoredPosition = target;
        isMoving = false;
    }

    IEnumerator FadeInTMP()
    {
        Color c = pressToPlayText.color;
        c.a = 0;
        pressToPlayText.color = c;

        float t = 0f;

        while (t < 0.7f)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, t / 0.7f);
            pressToPlayText.color = c;
            yield return null;
        }

        c.a = 1f;
        pressToPlayText.color = c;
    }
}
