using UnityEngine;

public class CepilloDientes : MonoBehaviour
{
    // Referencias a los dientes
    public GameObject dienteVerde;
    public GameObject dienteAmarillo;
    public GameObject dienteLimpio;

    // Estado del diente
    private int etapa = 0; // 0: Verde, 1: Amarillo, 2: Limpio

    void Update()
    {
        // Detectar si el cepillo está en contacto con el diente
        if (Input.GetKeyDown(KeyCode.Space)) // Cambia esto por la forma que uses para limpiar
        {
            LimpiarDientes();
        }
    }

    void LimpiarDientes()
    {
        if (etapa == 0)
        {
            dienteVerde.SetActive(false);
            dienteAmarillo.SetActive(true);
            etapa = 1;
        }
        else if (etapa == 1)
        {
            dienteAmarillo.SetActive(false);
            dienteLimpio.SetActive(true);
            etapa = 2;
        }
    }
}
