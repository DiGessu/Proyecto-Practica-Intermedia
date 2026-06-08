using UnityEngine;

// ESTA LÍNEA OBLIGA A UNITY A CREAR EL AUDIO SOURCE AUTOMÁTICAMENTE
[RequireComponent(typeof(AudioSource))]
public class SonidoAlgodon : MonoBehaviour
{
    [Header("Configuración de Audio")]
    public AudioClip sonidoFrotado;

    [Range(0f, 1f)]
    public float volumenMaximo = 0.6f;

    [Tooltip("Velocidad con la que aparece/desaparece el sonido")]
    public float velocidadFade = 15f;

    [Header("Ajustes de Movimiento")]
    [Tooltip("Umbral de movimiento físico en el mundo")]
    public float umbralMovimiento = 0.005f;

    private AudioSource audioSource;
    private Vector3 ultimaPosicionHerramienta;
    private Transform transformAlgodon;
    private float volumenObjetivo = 0f;
    private bool estaTocandoDiente = false;

    void Awake()
    {
        // ASIGNAMOS EL AUDIO SOURCE CORRECTAMENTE
        audioSource = GetComponent<AudioSource>();

        audioSource.clip = sonidoFrotado;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f; // Inicia en silencio
    }

    void Start()
    {
        if (sonidoFrotado != null)
        {
            audioSource.Play();
        }

        // Buscamos la herramienta de algodón en la escena
        CottonTool algodon = Object.FindFirstObjectByType<CottonTool>();
        if (algodon != null)
        {
            transformAlgodon = algodon.transform;
            ultimaPosicionHerramienta = transformAlgodon.position;
        }
    }

    void Update()
    {
        if (transformAlgodon == null || !transformAlgodon.gameObject.activeInHierarchy)
        {
            audioSource.volume = 0f;
            return;
        }

        // 1. Comprobar clic o touch
        bool estaHaciendoClic = Input.GetMouseButton(0) || (Input.touchCount > 0);

        // 2. Calcular movimiento real de la herramienta
        float distanciaMovida = Vector3.Distance(transformAlgodon.position, ultimaPosicionHerramienta);
        bool seEstaMoviendo = distanciaMovida > umbralMovimiento;
        ultimaPosicionHerramienta = transformAlgodon.position;

        // 3. Condición: Clic + Se Mueve + Toca Diente
        if (estaHaciendoClic && seEstaMoviendo && estaTocandoDiente)
        {
            volumenObjetivo = volumenMaximo;
        }
        else
        {
            volumenObjetivo = 0f;
        }

        // 4. Interpolación para suavizar el sonido por completo
        audioSource.volume = Mathf.Lerp(audioSource.volume, volumenObjetivo, Time.deltaTime * velocidadFade);
    }

    // Método para recibir el aviso desde el CottonTool
    public void SetTocandoDiente(bool tocando)
    {
        estaTocandoDiente = tocando;
    }
}