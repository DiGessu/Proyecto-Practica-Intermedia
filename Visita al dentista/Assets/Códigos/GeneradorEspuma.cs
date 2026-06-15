using UnityEngine;

public class GeneradorEspuma : MonoBehaviour
{
    [Header("Prefab de la Espuma")]
    [Tooltip("Arrastra aquí el Prefab con el Sprite de tu espuma.")]
    [SerializeField] private GameObject prefabEspuma;

    [Header("Ajustes de Generación")]
    [Tooltip("Cada cuántos segundos se genera una espuma mientras limpias.")]
    [SerializeField] private float tiempoEntreEspuma = 0.15f;

    [Header("Sonido")]
    [SerializeField] private AudioClip sonidoBurbuja;

    private float timerGeneracion;

    public void SoltarEspuma(Vector3 posicionDiente)
    {
        timerGeneracion += Time.deltaTime;

        if (timerGeneracion >= tiempoEntreEspuma)
        {
            timerGeneracion = 0f;

            // 1. Calculamos la posición final una sola vez
            Vector3 offsetAleatorio = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0f);
            Vector3 posFinal = posicionDiente + offsetAleatorio;
            posFinal.z = -1f; // Forzamos el eje Z al frente para que se vea en el plano 2D

            // 2. Instanciamos la espuma física en la escena
            Instantiate(prefabEspuma, posFinal, Quaternion.identity);

            // 3. Evaluamos el audio usando la variable global unificada 'Instancia'
            if (sonidoBurbuja != null && AudioManager.Instancia != null)
            {
                AudioSource.PlayClipAtPoint(sonidoBurbuja, posFinal);
            }
        }
    }
}