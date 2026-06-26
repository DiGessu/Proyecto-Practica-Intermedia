using UnityEngine;

public class GeneradorEspumaAlgodon : MonoBehaviour
{
    [Header("Prefab de la Espuma Especial")]
    [Tooltip("Arrastra aquí el NUEVO Prefab de espuma para el algodón.")]
    [SerializeField] private GameObject prefabEspumaNueva;

    [Header("Ajustes de Generación")]
    [Tooltip("Cada cuántos segundos se genera esta espuma.")]
    [SerializeField] private float tiempoEntreEspuma = 0.15f;

    [Header("Sonido")]
    [SerializeField] private AudioClip sonidoBurbuja;

    private float timerGeneracion;

    public void SoltarEspumaNueva(Vector3 posicionDiente)
    {
        timerGeneracion += Time.deltaTime;

        if (timerGeneracion >= tiempoEntreEspuma)
        {
            timerGeneracion = 0f;

            // 1. Calculamos la posición final
            Vector3 offsetAleatorio = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0f);
            Vector3 posFinal = posicionDiente + offsetAleatorio;
            posFinal.z = -1f; // Al frente en el plano 2D

            // 2. Instanciamos la NUEVA espuma física
            Instantiate(prefabEspumaNueva, posFinal, Quaternion.identity);

            // 3. Audio
            if (sonidoBurbuja != null && AudioManager.Instancia != null)
            {
                AudioSource.PlayClipAtPoint(sonidoBurbuja, posFinal);
            }
        }
    }
}