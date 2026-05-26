using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DienteConfig
{
    public GameObject dienteGO;
    [Header("Sprites por estado")]
    public Sprite spriteSucio;
    public Sprite spriteSarro;
    public Sprite spriteCarie;
}

public class BocaController : MonoBehaviour
{
    [Header("Configuracion de Dientes")]
    public List<DienteConfig> configuracionDientes = new List<DienteConfig>();

    private List<EstadoDiente> dientes = new List<EstadoDiente>();
    private int indexDiente;

    [Header("Configuracion de Tiempos")]
    [Tooltip("Tiempo en segundos que esperara el juego al inicio antes de empezar a ensuciar los dientes.")]
    [SerializeField] private float tiempoEsperaInicial = 3f;
    [SerializeField] private float timerToChangeSprite = 5f;

    private float currentTime = 0;
    private bool juegoComenzado = false;
    private bool bucleAleatorioActivo = false;

    void Start()
    {
        InicializarDientes();
        Invoke("ComenzarAEnsuciar", tiempoEsperaInicial);
    }

    void InicializarDientes()
    {
        foreach (var config in configuracionDientes)
        {
            if (config.dienteGO == null) continue;

            var estado = config.dienteGO.GetComponent<EstadoDiente>();
            if (estado == null)
                estado = config.dienteGO.AddComponent<EstadoDiente>();

            estado.Inicializar(config.spriteSucio, config.spriteSarro, config.spriteCarie);

            dientes.Add(estado);
        }
    }

    void ComenzarAEnsuciar()
    {
        if (bucleAleatorioActivo) return;
        juegoComenzado = true;
        Debug.Log("[BocaController] Comienza a ensuciar. Dientes en lista: " + dientes.Count);
    }

    private void OnEnable()
    {
        EstadoDiente.OnDienteLimpiado += ReiniciarTimer;
        EstadoDiente.OnCualquierCambioDeEstado += VerificarSiTodaLaBocaEstaLimpia;
    }

    private void OnDisable()
    {
        EstadoDiente.OnDienteLimpiado -= ReiniciarTimer;
        EstadoDiente.OnCualquierCambioDeEstado -= VerificarSiTodaLaBocaEstaLimpia;
    }

    private void OnDestroy()
    {
        EstadoDiente.OnDienteLimpiado -= ReiniciarTimer;
        EstadoDiente.OnCualquierCambioDeEstado -= VerificarSiTodaLaBocaEstaLimpia;
    }

    private void ReiniciarTimer()
    {
        if (this == null) return;
        currentTime = 0;
    }

    private void Update()
    {
        if (bucleAleatorioActivo)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= timerToChangeSprite * 0.5f)
                EnsuciarDienteAleatorio();
            return;
        }

        if (juegoComenzado)
        {
            if (currentTime < timerToChangeSprite)
                currentTime += Time.deltaTime;
            else
                EnsuciarDienteAleatorio();
        }
    }

    private void EnsuciarDienteAleatorio()
    {
        if (dientes == null || dientes.Count == 0) return;

        indexDiente = Random.Range(0, dientes.Count);
        currentTime = 0;

        if (dientes[indexDiente] != null)
        {
            Debug.Log("[BocaController] Ensuciando diente: " + dientes[indexDiente].gameObject.name);
            dientes[indexDiente].EnsuciarDiente();
        }
    }

    private void VerificarSiTodaLaBocaEstaLimpia()
    {
        if (this == null || dientes == null || dientes.Count == 0) return;

        bool bocaLimpia = true;

        foreach (EstadoDiente diente in dientes)
        {
            if (diente != null && diente.estadoActual != TipoEstado.LIMPIO)
            {
                bocaLimpia = false;
                break;
            }
        }

        if (bocaLimpia && !bucleAleatorioActivo)
        {
            CancelInvoke("ComenzarAEnsuciar");
            bucleAleatorioActivo = true;
            juegoComenzado = true;

            int cantidadInicial = Random.Range(2, 4);
            for (int i = 0; i < cantidadInicial; i++)
            {
                int randomIdx = Random.Range(0, dientes.Count);
                if (dientes[randomIdx] != null)
                    dientes[randomIdx].EnsuciarDiente();
            }
            currentTime = 0;
        }
    }

    public void NotificarLimpiezaRealizada()
    {
        currentTime = 0;
    }
}