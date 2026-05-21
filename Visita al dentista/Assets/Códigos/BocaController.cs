using System.Collections.Generic;
using UnityEngine;

public class BocaController : MonoBehaviour
{
    public List<EstadoDiente> dientes = new List<EstadoDiente>();
    private int indexDiente;

    [Header("Configuración de Tiempos")]
    [Tooltip("Tiempo en segundos que esperará el juego al inicio antes de empezar a ensuciar solos los dientes.")]
    [SerializeField] private float tiempoEsperaInicial = 20f;
    [SerializeField] private float timerToChangeSprite = 5f;

    private float currentTime = 0;
    private bool juegoComenzado = false;
    private bool bucleAleatorioActivo = false;

    void Start()
    {
        // Programamos que el temporizador de ensuciar empiece a correr después del tiempo configurado
        Invoke("ComenzarAEnsuciar", tiempoEsperaInicial);
    }

    void ComenzarAEnsuciar()
    {
        if (bucleAleatorioActivo) return; // Si ya ganó limpiando todo antes, ignoramos esto
        juegoComenzado = true;
        Debug.Log("¡Tiempo de gracia terminado! BocaController empieza a ensuciar.");
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

    private void ReiniciarTimer()
    {
        if (this == null) return;
        currentTime = 0;
    }

    private void Update()
    {
        // Si logramos limpiar toda la boca, se activa la reaparición infinita acelerada
        if (bucleAleatorioActivo)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= timerToChangeSprite * 0.5f)
            {
                EnsuciarDienteAleatorio();
            }
            return;
        }

        // Si ya pasó el tiempo inicial de gracia, corre el temporizador normal
        if (juegoComenzado)
        {
            if (currentTime < timerToChangeSprite)
            {
                currentTime += Time.deltaTime;
            }
            else
            {
                EnsuciarDienteAleatorio();
            }
        }
    }

    private void EnsuciarDienteAleatorio()
    {
        if (dientes != null && dientes.Count > 0)
        {
            indexDiente = Random.Range(0, dientes.Count);
            currentTime = 0;

            if (dientes[indexDiente] != null)
            {
                dientes[indexDiente].EnsuciarDiente();
            }
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

        // Si limpiaste todo antes de tiempo o durante el juego, se activa el caos final aleatorio
        if (bocaLimpia && !bucleAleatorioActivo)
        {
            CancelInvoke("ComenzarAEnsuciar"); // Cancelamos el reloj viejo por seguridad
            Debug.Log("¡Boca perfectamente limpia! Desatando reaparición infinita.");
            bucleAleatorioActivo = true;
            juegoComenzado = true;

            // Ensuciamos un par de dientes inmediatamente para reactivar la partida
            int cantidadInicial = Random.Range(2, 4);
            for (int i = 0; i < cantidadInicial; i++)
            {
                int randomIdx = Random.Range(0, dientes.Count);
                if (dientes[randomIdx] != null)
                {
                    dientes[randomIdx].EnsuciarDiente();
                }
            }
            currentTime = 0;
        }
    }

    public void NotificarLimpiezaRealizada()
    {
        currentTime = 0;
    }
}