using UnityEngine;

public class ToothBrush : MonoBehaviour
{
    public GameObject[] teeth; // Referencia a los dientes
    public Color cleanColor = Color.white; // Color limpio
    public Color dirtyColor = Color.yellow; // Color sucio

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el cepillo toca un diente
        if (collision.gameObject.CompareTag("Tooth"))
        {
            // Cambia el color del diente a limpio
            SpriteRenderer toothRenderer = collision.gameObject.GetComponent<SpriteRenderer>();
            if (toothRenderer != null)
            {
                toothRenderer.color = cleanColor;
            }
        }
    }
}
