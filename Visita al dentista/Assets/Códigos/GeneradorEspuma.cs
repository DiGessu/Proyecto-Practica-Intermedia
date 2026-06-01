using UnityEngine;

public class GeneradorEspuma : MonoBehaviour
{
    [Header("Prefab de la Espuma")]
    [Tooltip("Arrastra aquí el Prefab con el Sprite de tu espuma.")]
    [SerializeField] private GameObject prefabEspuma;

    [Header("Ajustes de Generación")]
    [Tooltip("Cada cuántos segundos se genera una espuma mientras limpias.")]
    [SerializeField] private float tiempoEntreEspuma = 0.15f;

    private float timerGeneracion;

    public void SoltarEspuma(Vector3 posicionDiente)
    {
        timerGeneracion += Time.deltaTime;

        if (timerGeneracion >= tiempoEntreEspuma)
        {
            timerGeneracion = 0f;

            // Reducimos el offset a algo mínimo para que brote pegadito al punto de contacto
            Vector3 offsetAleatorio = new Vector3(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f), 0f);
            Vector3 posFinal = posicionDiente + offsetAleatorio;

            // Forzamos el eje Z al frente
            posFinal.z = -1f;

            // Instancia la espuma justo ahí
            Instantiate(prefabEspuma, posFinal, Quaternion.identity);
        }
    }
}
