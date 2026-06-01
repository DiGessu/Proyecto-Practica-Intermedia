using UnityEngine;
using UnityEngine.UI;

public class PainManager : MonoBehaviour
{
    [Header("Componentes UI")]
    [Tooltip("Asigna aquí la barra de UI (Slider) que representará el dolor.")]
    [SerializeField] private Slider barraDolorSlider;

    [Tooltip("Asigna aquí la imagen del 'Handle' del Slider para cambiar su cara.")]
    [SerializeField] private Image handleCaraImage;

    [Tooltip("Asigna el Panel o Objeto UI que creaste para el mensaje de Perdiste.")]
    [SerializeField] private GameObject panelGameOver;

    [Header("Sprites de Expresiones (Caras)")]
    [SerializeField] private Sprite cara1_Limpio;    // 0% a 25%
    [SerializeField] private Sprite cara2_Molesto;   // 26% a 50%
    [SerializeField] private Sprite cara3_Asustado;  // 51% a 75%
    [SerializeField] private Sprite cara4_Dolor;     // 76% a 100%

    [Header("Configuración de Dolor por Estado (%)")]
   
    [Range(0f, 100f)][SerializeField] private float dolorSucio = 5f;
    [Range(0f, 100f)][SerializeField] private float dolorSarro = 7f;
    [Range(0f, 100f)][SerializeField] private float dolorCarie = 15f;

    [Header("Suavizado de la Barra")]
    [Tooltip("Velocidad con la que la barra se mueve hacia el dolor actual.")]
    [SerializeField] private float velocidadSuavizado = 5f;

    private BocaController bocaController;
    private float dolorObjetivo = 0f;
    private float dolorActualVisual = 0f;
    private bool juegoTerminado = false; // Control para que no se ejecute el Game Over repetidamente

    void Start()
    {
        // Asegurarnos de que el panel empiece oculto por código también
        if (panelGameOver != null)
            panelGameOver.SetActive(false);

        // Asegurar que el tiempo corra normalmente al iniciar
        Time.timeScale = 1f;

        bocaController = FindFirstObjectByType<BocaController>();
        if (bocaController == null)
        {
            Debug.LogError("[PainManager] No se encontró BocaController en la escena.");
        }

        if (barraDolorSlider != null)
        {
            barraDolorSlider.minValue = 0f;
            barraDolorSlider.maxValue = 100f;
            barraDolorSlider.value = 0f;
        }

        CalcularDolorTotal();
    }

    private void OnEnable()
    {
        EstadoDiente.OnCualquierCambioDeEstado += CalcularDolorTotal;
    }

    private void OnDisable()
    {
        EstadoDiente.OnCualquierCambioDeEstado -= CalcularDolorTotal;
    }

    void Update()
    {
        if (barraDolorSlider == null || juegoTerminado) return;

        // === NUEVA LÍNEA DE SEGURIDAD ===
        // Si el TimeManager ya dio el juego por ganado, detenemos la lógica de dolor
        TimeManager timeManager = FindFirstObjectByType<TimeManager>();
        if (timeManager != null && timeManager.ElNivelYaTermino()) return;

        // ... (Todo el resto de tu código de Update del PainManager se queda exactamente igual)
        CalcularDolorTotal();
        dolorActualVisual = Mathf.MoveTowards(dolorActualVisual, dolorObjetivo, velocidadSuavizado * Time.deltaTime);
        barraDolorSlider.value = dolorActualVisual;
        ActualizarCaraUI(dolorActualVisual);

        if (dolorActualVisual >= 99.9f)
        {
            GameOver();
        }
    }

    private void CalcularDolorTotal()
    {
        if (juegoTerminado) return;

        EstadoDiente[] todosLosDientes = FindObjectsByType<EstadoDiente>(FindObjectsSortMode.None);
        if (todosLosDientes.Length == 0) return;

        float sumaDolorAcumulado = 0f;

        foreach (EstadoDiente diente in todosLosDientes)
        {
            if (diente == null) continue;

            Transform capaSucio = diente.transform.Find("Sucio");
            Transform capaSarro = diente.transform.Find("Sarro");
            Transform capaCarie = diente.transform.Find("Carie");

            float alphaSucio = 0f;
            float alphaSarro = 0f;
            float alphaCarie = 0f;

            if (capaSucio != null && capaSucio.TryGetComponent(out SpriteRenderer srSucio) && srSucio.enabled)
                alphaSucio = srSucio.color.a;

            if (capaSarro != null && capaSarro.TryGetComponent(out SpriteRenderer srSarro) && srSarro.enabled)
                alphaSarro = srSarro.color.a;

            if (capaCarie != null && capaCarie.TryGetComponent(out SpriteRenderer srCarie) && srCarie.enabled)
                alphaCarie = srCarie.color.a;

            sumaDolorAcumulado += alphaSucio * dolorSucio;
            sumaDolorAcumulado += alphaSarro * dolorSarro;
            sumaDolorAcumulado += alphaCarie * dolorCarie;
        }

        dolorObjetivo = Mathf.Clamp(sumaDolorAcumulado, 0f, 100f);
    }

    private void ActualizarCaraUI(float valorActual)
    {
        if (handleCaraImage == null) return;

        if (valorActual <= 25f)
        {
            if (cara1_Limpio != null) handleCaraImage.sprite = cara1_Limpio;
        }
        else if (valorActual <= 50f)
        {
            if (cara2_Molesto != null) handleCaraImage.sprite = cara2_Molesto;
        }
        else if (valorActual <= 75f)
        {
            if (cara3_Asustado != null) handleCaraImage.sprite = cara3_Asustado;
        }
        else
        {
            if (cara4_Dolor != null) handleCaraImage.sprite = cara4_Dolor;
        }
    }

    private void GameOver()
    {
        juegoTerminado = true;
        Debug.Log("[PainManager] ¡EL PACIENTE NO AGUANTÓ EL DOLOR! Fin del juego.");

        // Mostramos el cartel de Perdiste
        if (panelGameOver != null)
        {
            panelGameOver.SetActive(true);
        }

        // Congelamos el tiempo del juego para que los dientes dejen de ensuciarse 
        // y las herramientas ya no interactúen ni se muevan.
        Time.timeScale = 0f;
    }
}