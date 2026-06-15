using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;

    void OnMouseDown()
    {
        // Asegura la profundidad Z respecto a la cámara
        mZCoord = Camera.main.WorldToScreenPoint(transform.position).z;

        // Calcula la distancia exacta desde donde hiciste click en la herramienta
        mOffset = transform.position - GetMouseWorldPos();
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = mZCoord;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }

    void OnMouseDrag()
    {
        // Mueve EXCLUSIVAMENTE a este objeto independiente
        transform.position = GetMouseWorldPos() + mOffset;
    }
}