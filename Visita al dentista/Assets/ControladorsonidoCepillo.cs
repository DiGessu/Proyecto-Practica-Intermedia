using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ControladorsonidoCepillo : MonoBehaviour
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

    private Transform transformCepilloActivo;
    private float volumenObjetivo = 0f;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = sonidoCepillado;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }

    void Start()
    {
        if (sonidoCepillado != null)
        {
            audioSource.Play();
        }
        BuscarCepilloActivo();
    }

    void Update()
    {
        if (transformCepilloActivo == null || !transformCepilloActivo.gameObject.activeInHierarchy)
        {
            BuscarCepilloActivo();
            if (transformCepilloActivo == null) return;
        }

        bool estaHaciendoClic = Input.GetMouseButton(0) || (Input.touchCount > 0);

        float distanciaMovida = Vector3.Distance(transformCepilloActivo.position, ultimaPosicionCepillo);
        bool seEstaMoviendo = distanciaMovida > umbralMovimiento;

        ultimaPosicionCepillo = transformCepilloActivo.position;

        if (estaHaciendoClic && estaTocandoDiente && seEstaMoviendo)
        {
            volumenObjetivo = volumenMaximo;
        }
        else
        {
            volumenObjetivo = 0f;
        }

        audioSource.volume = Mathf.Lerp(audioSource.volume, volumenObjetivo, Time.deltaTime * velocidadFade);
    }

    public void SetTocandoDiente(bool tocando)
    {
        estaTocandoDiente = tocando;
    }

    private void BuscarCepilloActivo()
    {
        // 1. Buscar cepillo con pasta
        ToothpasteBrush cepilloPasta = Object.FindFirstObjectByType<ToothpasteBrush>();
        if (cepilloPasta != null && cepilloPasta.gameObject.activeInHierarchy)
        {
            transformCepilloActivo = cepilloPasta.transform;
            ultimaPosicionCepillo = transformCepilloActivo.position;
            return;
        }

        // 2. Buscar cepillo normal (Utilizando el nombre exacto de tu clase)
        ToothBrush cepilloNormal = Object.FindFirstObjectByType<ToothBrush>();
        if (cepilloNormal != null && cepilloNormal.gameObject.activeInHierarchy)
        {
            transformCepilloActivo = cepilloNormal.transform;
            ultimaPosicionCepillo = transformCepilloActivo.position;
        }
    }
}