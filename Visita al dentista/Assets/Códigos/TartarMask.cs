using UnityEngine;

public class TartarMask : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Igual que el cepillo normal
    public void EraseAt(Vector2 worldPos)
    {
        ClearTartar();
    }

    // Baja opacidad poco a poco
    public void ClearTartar()
    {
        if (spriteRenderer == null) return;

        Color c = spriteRenderer.color;

        c.a -= 0.05f;

        c.a = Mathf.Clamp01(c.a);

        spriteRenderer.color = c;
    }

    // RESTAURA COMPLETAMENTE EL SARRO
    public void RestoreTartar()
    {
        if (spriteRenderer == null) return;

        // Asegurar que el objeto esté visible
        spriteRenderer.enabled = true;

        // Restaurar color completo
        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);

        Debug.Log("Sarro restaurado completamente.");
    }
}