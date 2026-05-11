using System.Collections.Generic;
using UnityEngine;

public class BocaController : MonoBehaviour
{
    public List<EstadoDiente> dientes = new List<EstadoDiente>();
    private int indexDiente;

    [Header("Configuración de Estado")]
    [SerializeField] private float timerToChangeSprite;
    private float currentTime = 0;

    private void OnEnable()
    {
       
        EstadoDiente.OnDienteLimpiado += ReiniciarTimer;
    }

    private void OnDisable()
    {
       
        EstadoDiente.OnDienteLimpiado -= ReiniciarTimer;
    }

    private void ReiniciarTimer()
    {
        currentTime = 0;
       
    }

    private void FixedUpdate()
    {
        if (currentTime < timerToChangeSprite)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            if (dientes.Count > 0)
            {
                indexDiente = Random.Range(0, dientes.Count);
                currentTime = 0;
                dientes[indexDiente].EnsuciarDiente(dientes[indexDiente].estadoActual);
            }
        }
    }
    public void NotificarLimpiezaRealizada()
    {
        currentTime = 0;
        Debug.Log("Un diente ha sido limpiado. Reiniciando temporizador de la boca.");
    }
}

