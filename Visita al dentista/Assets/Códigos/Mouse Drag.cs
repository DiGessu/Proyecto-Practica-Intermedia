using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;

    void OnMouseDown()
    {
        // Guardamos la distancia Z entre la cámara y el objeto
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;

        // Calculamos el offset para que el objeto no "salte" al centro del mouse
        mOffset = gameObject.transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos()
    {
        // Posición del mouse en píxeles (x, y)
        Vector3 mousePoint = Input.mousePosition;

        // Profundidad Z del objeto en el mundo
        mousePoint.z = mZCoord;

        // Convertimos a coordenadas del mundo
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        // Actualizamos la posición del objeto con el movimiento del mouse + el offset inicial
        transform.position = GetMouseWorldPos() + mOffset;
    }
}

