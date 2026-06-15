using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // PROPIEDAD GLOBAL AUTOMÁTICA: Con "I" mayúscula y protegida para todo el proyecto
    public static AudioManager Instancia { get; private set; }

    [Header("Mixer Principal")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource fuenteMusica;
    [SerializeField] private AudioSource fuenteSFX;

    void Awake()
    {
        // Usamos Instancia con mayúscula de forma coherente internamente
        if (Instancia == null)
        {
            Instancia = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
}