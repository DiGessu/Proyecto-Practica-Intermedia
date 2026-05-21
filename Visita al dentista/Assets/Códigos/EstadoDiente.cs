using System;
using UnityEngine;

// Declaramos los Enums al principio del archivo para que todo el proyecto los detecte
public enum TipoHerramienta
{
    CEPILLO,
    CEPILLO_CON_PASTA,
    ALGODON,
    NINGUNA
}

public enum TipoEstado
{
    LIMPIO,
    SUCIO,
    SARRO,
    CARIE
}

public class EstadoDiente : MonoBehaviour
{
    [Header("Configuración del Diente")]
    public bool esDienteInferior = false;

    [Header("Estado actual")]
    public TipoEstado estadoActual;

    [Header("Objetos por estado")]
    public GameObject dienteLimpio;
    public GameObject dienteSucio;
    public GameObject dienteSarro;
    public GameObject dienteCarie;

    public static event Action OnDienteLimpiado;
    public static event Action OnCualquierCambioDeEstado;

    void Start()
    {
        if (esDienteInferior)
        {
            estadoActual = TipoEstado.SARRO;
        }

        ActualizarEstadoVisual();
    }

    public void IntentarLimpiar(TipoHerramienta herramientaUsada)
    {
        bool limpiezaExitosa = false;

        switch (estadoActual)
        {
            case TipoEstado.CARIE:
                if (herramientaUsada == TipoHerramienta.ALGODON)
                {
                    estadoActual = TipoEstado.SARRO;
                    limpiezaExitosa = true;
                }
                break;

            case TipoEstado.SARRO:
                if (herramientaUsada == TipoHerramienta.CEPILLO_CON_PASTA)
                {
                    estadoActual = TipoEstado.SUCIO;
                    limpiezaExitosa = true;
                }
                break;

            case TipoEstado.SUCIO:
                if (herramientaUsada == TipoHerramienta.CEPILLO)
                {
                    estadoActual = TipoEstado.LIMPIO;
                    limpiezaExitosa = true;
                    OnDienteLimpiado?.Invoke();
                }
                break;
        }

        if (limpiezaExitosa)
        {
            ActualizarEstadoVisual();
            OnCualquierCambioDeEstado?.Invoke();
            Debug.Log(gameObject.name + " limpiado. Nuevo estado: " + estadoActual);
        }
    }

    public void EnsuciarDiente()
    {
        switch (estadoActual)
        {
            case TipoEstado.LIMPIO:
                estadoActual = TipoEstado.SUCIO;
                break;

            case TipoEstado.SUCIO:
                estadoActual = TipoEstado.SARRO;
                break;

            case TipoEstado.SARRO:
                if (!esDienteInferior)
                {
                    estadoActual = TipoEstado.CARIE;
                }
                break;
        }

        ActualizarEstadoVisual();
        OnCualquierCambioDeEstado?.Invoke();
        Debug.Log(gameObject.name + " se ensució a: " + estadoActual);
    }

    void ActualizarEstadoVisual()
    {
        if (dienteLimpio != null) dienteLimpio.SetActive(true);

        switch (estadoActual)
        {
            case TipoEstado.LIMPIO:
                if (dienteSucio != null) dienteSucio.SetActive(false);
                if (dienteSarro != null) dienteSarro.SetActive(false);
                if (dienteCarie != null) dienteCarie.SetActive(false);
                break;

            case TipoEstado.SUCIO:
                if (dienteSucio != null) dienteSucio.SetActive(true);
                if (dienteSarro != null) dienteSarro.SetActive(false);
                if (dienteCarie != null) dienteCarie.SetActive(false);
                break;

            case TipoEstado.SARRO:
                if (dienteSucio != null) dienteSucio.SetActive(true);
                if (dienteSarro != null) dienteSarro.SetActive(true);
                if (dienteCarie != null) dienteCarie.SetActive(false);
                break;

            case TipoEstado.CARIE:
                if (dienteSucio != null) dienteSucio.SetActive(true);
                if (dienteSarro != null) dienteSarro.SetActive(true);
                if (dienteCarie != null && !esDienteInferior) dienteCarie.SetActive(true);
                break;
        }
    }
}