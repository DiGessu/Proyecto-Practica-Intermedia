using UnityEngine;

public class DetectorSonidoDiente : MonoBehaviour
{
    private SonidoCepilloNormal sistemaSonido;

    void Awake()
    {
        // Busca el script de sonido en la escena
        sistemaSonido = Object.FindFirstObjectByType<SonidoCepilloNormal>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si lo que entró al trigger es el cepillo normal, activa el sonido
        if (collision.GetComponent<ToothBrush>() != null)
        {
            if (sistemaSonido != null) sistemaSonido.SetTocandoDiente(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // Mantiene el sonido activo mientras sigas tallando el mismo diente
        if (collision.GetComponent<ToothBrush>() != null)
        {
            if (sistemaSonido != null) sistemaSonido.SetTocandoDiente(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si el cepillo sale del diente, apaga el sonido
        if (collision.GetComponent<ToothBrush>() != null)
        {
            if (sistemaSonido != null) sistemaSonido.SetTocandoDiente(false);
        }
    }
}