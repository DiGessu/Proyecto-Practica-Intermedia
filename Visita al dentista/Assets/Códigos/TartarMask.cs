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

    // Baja opacidad igual que el diente normal
    public void ClearTartar()
    {
        Color c = spriteRenderer.color;

        c.a -= 0.05f;

        spriteRenderer.color = c;
    }
}