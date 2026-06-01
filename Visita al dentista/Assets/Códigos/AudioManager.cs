using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instancia;

    [Header("Mixer Principal")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource fuenteMusica;
    [SerializeField] private AudioSource fuenteSFX;

    void Awake()
    {
        // Patron Singleton: evita que se duplique el AudioManager al cambiar de escena
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Funciones públicas para cambiar el volumen desde los Sliders (UI)
    // El volumen de un Mixer va de -80dB (silencio) a 0dB (volumen máximo).
    public void CambiarVolumenMusica(float valorSlider)
    {
        float deciembeleos = Mathf.Log10(Mathf.Clamp(valorSlider, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("VolMusica", deciembeleos);
    }

    public void CambiarVolumenSFX(float valorSlider)
    {
        float deciembeleos = Mathf.Log10(Mathf.Clamp(valorSlider, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat("VolSFX", deciembeleos);
    }

    // Funciones para reproducir audio desde otros scripts
    public void ReproducirMusica(AudioClip clip)
    {
        fuenteMusica.clip = clip;
        fuenteMusica.Play();
    }

    public void ReproducirSFX(AudioClip clip)
    {
        fuenteSFX.PlayOneShot(clip);
    }
}