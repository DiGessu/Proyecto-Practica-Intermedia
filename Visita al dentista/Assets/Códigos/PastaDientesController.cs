using UnityEngine;

// Controla la lógica para destapar la pasta de dientes mediante una animación
public class PastaDientesController : MonoBehaviour
{
    // Referencia al componente Animator para controlar las animaciones del objeto
    private Animator animator;

    // Variable de control para saber si la pasta ya ha sido abierta y evitar repetir la acción
    private bool destapada = false;

    // Se ejecuta al iniciar el juego
    void Start()
    {
        // Obtiene y guarda el componente Animator que está en el mismo objeto que este script
        animator = GetComponent<Animator>();
    }

    // Se ejecuta una vez por cada frame (cuadro) del juego
    void Update()
    {
        // Detecta si el usuario hizo clic izquierdo o tocó la pantalla (0 es el clic principal)
        // y verifica que la pasta no esté ya destapada
        if (Input.GetMouseButtonDown(0) && !destapada)
        {
            // Convierte la posición del mouse de píxeles de pantalla a coordenadas del mundo 2D
            Vector2 posicionToque = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Lanza un "rayo" invisible en esa posición para ver si golpea algún objeto con Collider 2D
            RaycastHit2D hit = Physics2D.Raycast(posicionToque, Vector2.zero);

            // Si el rayo golpeó algo Y ese algo es este mismo objeto (la pasta de dientes)
            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // Llama a la función para realizar la acción de abrir
                Destapar();
            }
        }
    }

    // Método encargado de cambiar el estado y ejecutar la animación
    void Destapar()
    {
        // Cambia el estado a verdadero para que el código del Update no vuelva a entrar aquí
        destapada = true;

        // Activa el disparador (Trigger) llamado "Destapar" en el Animator Controller de Unity
        animator.SetTrigger("Destapar");
    }
}