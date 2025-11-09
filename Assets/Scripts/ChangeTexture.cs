using UnityEngine;

public class ChangeTexture : MonoBehaviour
{
    public Sprite nuevoSprite;        // Sprite de "chimenea encendida"
    private SpriteRenderer render;    // SpriteRenderer de la fogata
    private Sprite spriteOriginal;    // Guarda el sprite original

    private void Start()
    {
        render = GetComponent<SpriteRenderer>();
        if (render != null)
            spriteOriginal = render.sprite;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.name == "olla") // nombre exacto del objeto olla
        {
            if (render != null && nuevoSprite != null)
            {
                render.sprite = nuevoSprite;
                Debug.Log("Sprite cambiado a chimenea encendida");
            }
            else
            {
                Debug.LogWarning("No se asignó el nuevo sprite o el SpriteRenderer no existe");
            }
        }
    }
}
