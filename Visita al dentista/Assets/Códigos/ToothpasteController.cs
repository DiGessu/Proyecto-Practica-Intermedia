using UnityEngine;

public class ToothpasteController : MonoBehaviour
{
    public Sprite openedToothpasteSprite; // Sprite de la pasta abierta

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica si el objeto que toca es el cepillo de dientes
        if (collision.gameObject.CompareTag("ToothBrush"))
        {
            // Cambia el sprite a la pasta destapada
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = openedToothpasteSprite;
            }
            // Opcional: Destruir el collider para evitar más interacciones
            Destroy(GetComponent<Collider2D>());
        }
    }
}
