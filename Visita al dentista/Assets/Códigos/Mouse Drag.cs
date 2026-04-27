using UnityEngine;

// Esta clase permite arrastrar un objeto 3D con el ratón manteniendo su profundidad inicial
public class MouseDrag : MonoBehaviour
{
    // Almacena la diferencia de posición entre el centro del objeto y el punto exacto donde se hizo clic
    private Vector3 mOffset;

    // Almacena la distancia del objeto respecto a la cámara en el eje Z
    private float mZCoord;

    // Se ejecuta una vez en el momento en que el usuario hace clic sobre el objeto
    void OnMouseDown()
    {
        // 1. Obtiene la posición del objeto en el espacio de la pantalla y extrae la profundidad (z)
        // Esto es necesario para saber a qué "distancia" de la cámara está el objeto
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // 2. Calcula la diferencia (offset) entre la posición real del objeto y el punto del clic en el mundo
        // Esto evita que el objeto "salte" bruscamente hacia el puntero del ratón
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    // Método auxiliar para convertir la posición del ratón de píxeles (2D) a coordenadas del mundo (3D)
    private Vector3 GetMouseWorldPos()
    {
        // Obtiene las coordenadas X e Y del ratón en píxeles
        Vector3 mousePoint = Input.mousePosition;

        // Asigna la profundidad guardada previamente para que el cálculo sea coherente en el espacio 3D
        mousePoint.z = mZCoord;

        // Convierte el punto de la pantalla (Screen) al espacio global del juego (World)
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    // Se ejecuta continuamente mientras el usuario mantiene presionado el botón del ratón sobre el objeto
    void OnMouseDrag()
    {
        // Actualiza la posición del objeto sumando la nueva posición del ratón y el offset calculado al inicio
        // Esto hace que el arrastre sea fluido y mantenga la posición relativa del clic
        transform.position = GetMouseWorldPos() + mOffset;
    }
}

