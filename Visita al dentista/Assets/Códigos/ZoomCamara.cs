using UnityEngine;

public class ZoomCamara : MonoBehaviour
{
    private Camera camara;

    [Header("Configuración de Zoom")]
    [SerializeField] private float tamañoNormal = 5f;
    [SerializeField] private float tamañoZoom = 2.5f;
    [SerializeField] private float velocidadZoom = 5f;

    private float tamañoObjetivo;

    void Awake()
    {
        camara = GetComponent<Camera>();
        tamañoObjetivo = tamañoNormal;
        camara.orthographicSize = tamañoNormal;
    }

    void Update()
    {
        // --- DETECTAR LA TECLA SHIFT ---

        // Si dejas presionado Shift (ya sea el izquierdo o el derecho del teclado)
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            tamañoObjetivo = tamañoZoom;
        }
        else
        {
            // Si no estás presionando Shift, la cámara vuelve a su tamaño normal
            tamañoObjetivo = tamañoNormal;
        }

        // Movemos el tamaño de la cámara suavemente hacia el objetivo
        camara.orthographicSize = Mathf.Lerp(camara.orthographicSize, tamañoObjetivo, Time.deltaTime * velocidadZoom);
    }
}