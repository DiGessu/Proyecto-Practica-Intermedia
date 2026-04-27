using UnityEngine;

public class IrrigadorDental : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 offset;
    private float zDepth;
    [SerializeField] private ParticleSystem particulasAgua;

    void OnMouseDown()
    {
        isDragging = !isDragging; // Toggle: click activa/desactiva

        if (isDragging)
        {
            zDepth = Camera.main.WorldToScreenPoint(transform.position).z;
            offset = transform.position - GetMouseWorldPos();
        }
    }

    void Update()
    {
        if (isDragging)
        {
            transform.position = GetMouseWorldPos() + offset;

            // Activar partículas al presionar Space
            if (Input.GetKeyDown(KeyCode.Space))
            {
                print("ejecutar animacion");
                particulasAgua.Play();
            }

            // Detener partículas al soltar Space
            if (Input.GetKeyUp(KeyCode.Space))
            {
                particulasAgua.Stop();
            }
        }
        else
        {
            // Asegurar que las partículas se detengan si se suelta el objeto
            if (particulasAgua.isPlaying)
            {
                particulasAgua.Stop();
            }
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = zDepth;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
