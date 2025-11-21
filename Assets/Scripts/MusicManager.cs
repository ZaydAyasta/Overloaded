using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource musicSource;   // Arrastra un AudioSource aquí
    public AudioClip musicClip;       // Tu música de fondo

    [Range(0f, 1f)]
    public float volume = 0.6f;

    void Start()
    {
        if (musicSource == null) musicSource = GetComponent<AudioSource>();

        if (musicSource != null)
        {
            musicSource.clip = musicClip;
            musicSource.volume = volume;
            musicSource.loop = true;
            musicSource.Play();
        }
    }
}
