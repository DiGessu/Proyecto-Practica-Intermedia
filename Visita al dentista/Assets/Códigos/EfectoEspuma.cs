using UnityEngine;

public class EfectoEspuma : MonoBehaviour
{
    [Header("Configuración de Movimiento Simple")]
    [Tooltip("Qué tan rápido se mueve/burbujea la espuma.")]
    [SerializeField] private float velocidadAnimacion = 5f;

    [Tooltip("Qué tan grande es el movimiento (valores pequeños para que sea sutil).")]
    [SerializeField] private float intensidadAnimacion = 0.05f;

    private Vector3 escalaInicial;
    private float tiempoAleatorio;

    void Start()
    {
        // Guardamos la escala inicial (que ya incluye el tamaño aleatorio)
        float escalaAleatoria = Random.Range(0.8f, 1.2f);
        transform.localScale *= escalaAleatoria;
        escalaInicial = transform.localScale;

        // Le damos una rotación aleatoria para que no se vean todas iguales
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

        // Desincronizamos las espumas para que no se muevan todas exactamente al mismo tiempo
        tiempoAleatorio = Random.Range(0f, 100f);
    }

    void Update()
    {
        // Usamos Mathf.Sin para crear un bucle de vaivén (vibración/burbujeo)
        // Sumamos tiempoAleatorio para que cada burbuja lleve su propio ritmo
        float factorMovimiento = Mathf.Sin(Time.time * velocidadAnimacion + tiempoAleatorio) * intensidadAnimacion;

        // Opción A: Modificar la ESCALA (Efecto de inflarse y desinflarse sutilmente)
        transform.localScale = escalaInicial + new Vector3(factorMovimiento, factorMovimiento, 0f);

        // Opción B: Si prefieres que VIBRE DE LADO A LADO en vez de inflarse, 
        // desmarca las barras de abajo y borra la línea de la Escala de arriba:
        // transform.position += new Vector3(factorMovimiento * Time.deltaTime, 0f, 0f);
    }
}