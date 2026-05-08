using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BocaController : MonoBehaviour
{
    public List<EstadoDiente> dientes = new List<EstadoDiente>();
    private int indexDiente;


    [Header("Configuración de Estado")]
    [SerializeField]private float timerToChangeSprite;
    private float currentTime = 0;

    private void FixedUpdate()
    {
        if (currentTime < timerToChangeSprite)
        {
            currentTime += Time.deltaTime;
            print((int)currentTime);
        }
        else
        {
            indexDiente = Random.Range(0, dientes.Count);
            currentTime = 0;
            dientes[indexDiente].EnsuciarDiente(dientes[indexDiente].estadoActual);
        }
    }
}
