using UnityEngine;

public class SonidoIrrigador : MonoBehaviour
{
    [Header("Configuración del Sonido")]
    [SerializeField] private AudioSource fuenteAudio; // Aquí arrastraremos el componente de audio
    [SerializeField] private AudioClip sonidoAgua;     // Aquí arrastraremos tu sonido descargado

    void Update()
    {
        // Detecta el momento exacto en que el niño presiona el Espacio
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (fuenteAudio != null && sonidoAgua != null)
            {
                // Reproduce el sonido una sola vez (ideal para efectos)
                fuenteAudio.PlayOneShot(sonidoAgua);
            }
        }

        // OPCIONAL: Para que el sonido se detenga inmediatamente al soltar el espacio
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (fuenteAudio != null)
            {
                fuenteAudio.Stop();
            }
        }
    }
}