using UnityEngine;
using TMPro; // Requerido si usas TextMeshPro. Si usas el texto clásico, cambia esto por 'using UnityEngine.UI;'

public class TimeManager : MonoBehaviour
{
    [Header("Configuración del Tiempo")]
    [Tooltip("Tiempo en segundos que durará el nivel (ej. 60 para 1 minuto).")]
    [SerializeField] private float tiempoRestante = 60f;

    [Header("Componentes de UI")]
    [Tooltip("Arrastra aquí el componente de texto que mostrará el reloj.")]
    [SerializeField] private TextMeshProUGUI textoTimer; // Cambiar a 'public Text textoTimer' si no usas TMP

    [Tooltip("Arrastra aquí el Panel de Victoria (PanelWin).")]
    [SerializeField] private GameObject panelWin;

    private bool nivelTerminado = false;
    private PainManager painManager;

    void Start()
    {
        // Nos aseguramos de que el panel de victoria empiece oculto
        if (panelWin != null)
            panelWin.SetActive(false);

        // Buscamos el PainManager en la escena para asegurarnos de que no calcule el tiempo si ya perdimos
        painManager = FindFirstObjectByType<PainManager>();
    }

    void Update()
    {
        if (nivelTerminado) return;

        // Si el tiempo llega a cero, el jugador gana
        if (tiempoRestante > 0f)
        {
            tiempoRestante -= Time.deltaTime;
            ActualizarTextoReloj(tiempoRestante);
        }
        else
        {
            tiempoRestante = 0f;
            ActualizarTextoReloj(tiempoRestante);
            GanarNivel();
        }
    }

    // Convierte los segundos flotantes en un formato limpio de minutos:segundos (00:00)
    void ActualizarTextoReloj(float tiempoEnSegundos)
    {
        if (textoTimer == null) return;

        int minutos = Mathf.FloorToInt(tiempoEnSegundos / 60f);
        int segundos = Mathf.FloorToInt(tiempoEnSegundos % 60f);

        // Formatea el texto para que siempre muestre dos dígitos (ej: 05:09)
        textoTimer.text = string.Format("{0:00}:{1:00}", minutos, segundos);
    }

    void GanarNivel()
    {
        nivelTerminado = true;
        Debug.Log("[TimeManager] ¡Felicidades, el tiempo se acabó y el paciente resistió!");

        // Mostramos el cartel de Victoria
        if (panelWin != null)
        {
            panelWin.SetActive(true);
        }

        // Congelamos el tiempo del juego para detener las mecánicas de ensuciamiento y herramientas
        Time.timeScale = 0f;
    }

    // Función pública por si necesitas saber desde otros scripts si el tiempo ya acabó
    public bool ElNivelYaTermino()
    {
        return nivelTerminado;
    }
}
