using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class ComicIntro : MonoBehaviour
{
    [Header("Comic")]
    public RectTransform comicImage;
    public CanvasGroup comicCg;
    public float moveTime = 0.4f;

    [Header("Menu")]
    public CanvasGroup menuCg;
    public float menuFadeTime = 0.5f;

    [Header("Comic Positions")]
    public Vector2 topLeft;
    public Vector2 topRight;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;

    [Header("UI")]
    public TextMeshProUGUI pressToPlayText;

    [Header("Next")]
    public string nextScene = "Fusion";

    int index = 0;
    bool isMoving = false;
    bool ending = false;
    bool menuFinished = false;

    Vector2[] positions;

    void Start()
    {
        positions = new[] { topLeft, topRight, bottomLeft, bottomRight };

        comicCg.alpha = 0f;
        pressToPlayText.gameObject.SetActive(false);
        comicImage.anchoredPosition = positions[0];

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log("[ComicIntro] Start listo. Menú visible.");
    }

    void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Return))
            return;

        // PRIMER ENTER — OCULTAR MENÚ
        if (!menuFinished)
        {
            Debug.Log("[ComicIntro] ENTER → OCULTAR MENÚ");
            StartCoroutine(FadeOutMenu());
            return;
        }

        // ENTERS DURANTE EL CÓMIC
        if (!ending)
        {
            Debug.Log("[ComicIntro] ENTER → AVANZAR COMIC (" + index + ")");
            NextComicStep();
            return;
        }

        // ENTER FINAL → CAMBIAR ESCENA
        Debug.Log("[ComicIntro] ENTER FINAL → LOAD SCENE");
        SceneManager.LoadScene(nextScene);
    }

    // FADE DEL MENÚ
    IEnumerator FadeOutMenu()
    {
        float t = 0f;
        float start = menuCg.alpha;

        while (t < menuFadeTime)
        {
            t += Time.deltaTime;
            menuCg.alpha = Mathf.Lerp(start, 0f, t / menuFadeTime);
            yield return null;
        }

        menuCg.alpha = 0f;
        menuCg.gameObject.SetActive(false);

        StartCoroutine(FadeInComic());
    }

    IEnumerator FadeInComic()
    {
        float t = 0f;
        float duration = 0.6f;

        while (t < duration)
        {
            t += Time.deltaTime;
            comicCg.alpha = Mathf.Lerp(0, 1, t / duration);
            yield return null;
        }

        comicCg.alpha = 1f;
        menuFinished = true;

        Debug.Log("[ComicIntro] Comic visible. Puedes avanzar con Enter.");
    }

    // --- LOGICA DEL COMIC ---
    void NextComicStep()
    {
        index++;

        if (index < positions.Length)
        {
            StartCoroutine(SmoothMove(positions[index]));
        }
        else
        {
            // YA NO HAY MÁS COMICS — MOSTRAR PRESS TO PLAY
            pressToPlayText.gameObject.SetActive(true);
            StartCoroutine(FadeInTMP());
            ending = true;

            Debug.Log("[ComicIntro] Final del comic. Mostrar mensaje de continuar.");
        }
    }

    IEnumerator SmoothMove(Vector2 target)
    {
        isMoving = true;

        Vector2 start = comicImage.anchoredPosition;
        float t = 0f;

        while (t < moveTime)
        {
            t += Time.deltaTime;
            comicImage.anchoredPosition = Vector2.Lerp(start, target, t / moveTime);
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
