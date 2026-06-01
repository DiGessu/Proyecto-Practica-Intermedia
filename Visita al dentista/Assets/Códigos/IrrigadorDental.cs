using UnityEngine;

public class IrrigadorDental : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private float zDepth;
    private Camera mainCamera; // Optimizador de cámara

    [Header("Configuración del Agua")]
    [SerializeField] private GameObject spriteAgua;
    [SerializeField] private Transform puntoSalida;

    void Start()
    {
        // Guardamos la cámara al inicio para no buscarla en cada frame
        mainCamera = Camera.main;
    }

    void OnMouseDown()
    {
        isDragging = true;
        zDepth = mainCamera.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorldPos();
    }

    void OnMouseUp()
    {
        isDragging = false;
    }

    void Update()
    {
        // 1. Manejo del Movimiento
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;
        }

        // 2. Manejo del Input (Independiente de si arrastras o no, más seguro)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (spriteAgua != null) spriteAgua.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            DesactivarAgua();
        }
    }

    // Usamos LateUpdate para evitar que el agua "parpadee" o se desfase al mover el irrigador
    void LateUpdate()
    {
        // Si el agua está activa, sigue a la punta
        if (spriteAgua != null && spriteAgua.activeSelf && puntoSalida != null)
        {
            spriteAgua.transform.position = puntoSalida.position;
            // Descomenta si decides que el agua rote con la punta:
            // spriteAgua.transform.rotation = puntoSalida.rotation;
        }
    }

    private void DesactivarAgua()
    {
        if (spriteAgua != null && spriteAgua.activeSelf)
        {
            spriteAgua.SetActive(false);
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zDepth;
        return mainCamera.ScreenToWorldPoint(mousePoint);
    }
}