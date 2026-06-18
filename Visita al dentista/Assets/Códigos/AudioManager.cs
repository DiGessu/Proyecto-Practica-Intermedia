using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // ¡IMPORTANTE! Necesario para usar componentes Slider

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instancia { get; private set; }

    [Header("Mixer Principal")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Fuentes de Audio")]
    [SerializeField] private AudioSource fuenteMusica;
    [SerializeField] private AudioSource fuenteSFX;

    // --- NUEVO: Referencias a los sliders del menú ---
    [Header("UI Sliders")]
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderSFX;

    void Awake()
    {
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

    // --- NUEVO: Sincronizar volumen al iniciar ---
    void Start()
    {
        if (sliderMusica != null) CambiarVolumenMusica(sliderMusica.value);
        if (sliderSFX != null) CambiarVolumenSFX(sliderSFX.value);
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