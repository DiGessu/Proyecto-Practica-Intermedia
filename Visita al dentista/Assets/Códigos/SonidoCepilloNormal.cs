using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SonidoCepilloNormal : MonoBehaviour
{
    [Header("Configuración de Audio")]
    public AudioClip sonidoCepillado;

    [Range(0f, 1f)]
    public float volumenMaximo = 0.7f;

    [Tooltip("Velocidad con la que aparece/desaparece el sonido")]
    public float velocidadFade = 15f;

    [Header("Ajustes de Movimiento")]
    [Tooltip("Umbral de movimiento físico en el mundo")]
    public float umbralMovimiento = 0.005f;

    private AudioSource audioSource;
    private Vector3 ultimaPosicionCepillo;
    private Transform transformCepillo;
    private float volumenObjetivo = 0f;
    private bool estaTocandoDiente = false;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = sonidoCepillado;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f; // Inicia en silencio
    }

    void Start()
    {
        if (sonidoCepillado != null)
        {
            audioSource.Play();
        }

        // Buscamos el cepillo normal en la escena
        ToothBrush cepillo = Object.FindFirstObjectByType<ToothBrush>();
        if (cepillo != null)
        {
            transformCepillo = cepillo.transform;
            ultimaPosicionCepillo = transformCepillo.position;
        }
    }

    void Update()
    {
        if (transformCepillo == null || !transformCepillo.gameObject.activeInHierarchy)
        {
            audioSource.volume = 0f;
            return;
        }

        // 1. Comprobar clic o touch
        bool estaHaciendoClic = Input.GetMouseButton(0) || (Input.touchCount > 0);

        // 2. Calcular movimiento real del cepillo
        float distanciaMovida = Vector3.Distance(transformCepillo.position, ultimaPosicionCepillo);
        bool seEstaMoviendo = distanciaMovida > umbralMovimiento;
        ultimaPosicionCepillo = transformCepillo.position;

        // 3. Condición idéntica: Clic + Se Mueve + Toca Diente
        if (estaHaciendoClic && seEstaMoviendo && estaTocandoDiente)
        {
            volumenObjetivo = volumenMaximo;
        }
        else
        {
            volumenObjetivo = 0f;
        }

        // 4. Interpolación para evitar micro-cortes
        audioSource.volume = Mathf.Lerp(audioSource.volume, volumenObjetivo, Time.deltaTime * velocidadFade);
    }

    // Método para recibir la señal del cepillo
    public void SetTocandoDiente(bool tocando)
    {
        estaTocandoDiente = tocando;
    }
}