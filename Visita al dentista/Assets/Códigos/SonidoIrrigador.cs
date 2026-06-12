using UnityEngine;

public class SonidoIrrigador : MonoBehaviour
{
    [Header("Configuración del Sonido")]
    [SerializeField] private AudioSource fuenteAudio; // Aquí arrastraremos el componente de audio
    [SerializeField] private AudioClip sonidoAgua;     // Aquí arrastraremos tu sonido descargado

    void Start()
    {
        // Nos aseguramos por código de que el sonido asignado sea el correcto
        if (fuenteAudio != null && sonidoAgua != null)
        {
            fuenteAudio.clip = sonidoAgua;
            fuenteAudio.loop = true; // Activa el bucle automáticamente
        }
    }

    void Update()
    {
        // Cuando el niño PRESIONA el Espacio, el sonido empieza a sonar y repetirse solo
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (fuenteAudio != null && !fuenteAudio.isPlaying)
            {
                fuenteAudio.Play();
            }
        }

        // Cuando el niño SUELTA el Espacio, el sonido se detiene de inmediato
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (fuenteAudio != null)
            {
                fuenteAudio.Stop();
            }
        }
    }
}