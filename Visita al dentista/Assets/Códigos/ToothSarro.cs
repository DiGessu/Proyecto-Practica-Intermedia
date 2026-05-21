using UnityEngine;

public class ToothSarro : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private bool yaSeLimpio = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Esta función hace exactamente lo mismo que el ClearTooth() de tus dientes sucios
    public void LimpiarSarro(float cantidad)
    {
        if (yaSeLimpio || spriteRenderer == null) return;

        Color c = spriteRenderer.color;
        c.a -= cantidad; // Le baja la opacidad
        spriteRenderer.color = c;

        // Si ya es casi invisible, lo desactivamos y avisamos al diente
        if (c.a <= 0.05f)
        {
            yaSeLimpio = true;
            c.a = 1f;
            spriteRenderer.color = c;

            // Avisa de forma segura a tu script EstadoDiente que se actualice
            gameObject.SendMessage("ActualizarSpriteInicial", SendMessageOptions.DontRequireReceiver);
            gameObject.SetActive(false); // Apaga el objeto de sarro
        }
    }

    // Por si necesitas resetear el diente en un nuevo nivel
    public void ResetearSarro()
    {
        yaSeLimpio = false;
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }
    }
}