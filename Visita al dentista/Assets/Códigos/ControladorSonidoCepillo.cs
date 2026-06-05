using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ControladorSonidoCepillo : MonoBehaviour
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
    private bool estaTocandoDiente = false;
    private Transform transformCepillo;
    private float volumenObjetivo = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = sonidoCepillado;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f; // Iniciamos en silencio absoluto

        ToothpasteBrush cepillo = Object.FindFirstObjectByType<ToothpasteBrush>();
        if (cepillo != null)
        {
            transformCepillo = cepillo.transform;
            ultimaPosicionCepillo = transformCepillo.position;
        }
    }

    void Start()
    {
        // Forzamos a que el AudioSource empiece a reproducirse en bucle silencioso desde el inicio
        if (sonidoCepillado != null)
        {
            audioSource.Play();
        }
    }

    void Update()
    {
        if (transformCepillo == null) return;

        // 1. Comprobar clic del jugador e interacción con el trigger del diente
        bool estaHaciendoClic = Input.GetMouseButton(0);

        // 2. Calcular la distancia real recorrida por el cepillo en este frame
        float distanciaMovida = Vector3.Distance(transformCepillo.position, ultimaPosicionCepillo);
        bool seEstaMoviendo = distanciaMovida > umbralMovimiento;

        ultimaPosicionCepillo = transformCepillo.position;

        // 3. Determinar el volumen objetivo en base a las condiciones
        if (estaHaciendoClic && estaTocandoDiente && seEstaMoviendo)
        {
            volumenObjetivo = volumenMaximo;
        }
        else
        {
            volumenObjetivo = 0f;
        }

        // 4. Aplicar Interpolación Lineal (Lerp) al volumen para suavizar por completo cualquier micro-corte
        audioSource.volume = Mathf.Lerp(audioSource.volume, volumenObjetivo, Time.deltaTime * velocidadFade);
    }

    public void SetTocandoDiente(bool tocando)
    {
        estaTocandoDiente = tocando;
    }
}