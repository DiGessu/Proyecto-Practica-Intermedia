using UnityEngine;

public class ToothDirtMask : MonoBehaviour
{
    [Header("Configuración de la máscara")]
    public int maskResolution = 512;
    [Range(10, 120)] public int brushSize = 40;
    [Range(0f, 1f)] public float brushSoftness = 0.5f;
    [Range(0.5f, 1f)] public float cleanThreshold = 0.85f;

    [HideInInspector] public RenderTexture maskTexture;

    private SpriteRenderer spriteRenderer;
    private Material eraseMaterial;
    private Texture2D brushTexture;
    private float alphaValue = 1f;

    private Texture2D readbackTexture;
    private int totalPixels;
    private float cleanPercent = 0f;

    [Header("Respawn del diente")]
    public float respawnTime = 10f;
    private bool isRespawning = false;

    public System.Action OnToothCleaned;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeMask();
        //OnToothCleaned += HandleToothCleaned;
    }

  
    // NUEVO: Este método permite reiniciar la máscara cuando cambia el estado (ej: a Sarro)
    public void ResetearMascaraParaNuevoEstado()
    {
        if (maskTexture != null)
        {
            // Restaurar suciedad
            Graphics.Blit(Texture2D.whiteTexture, maskTexture);

            cleanPercent = 0f;
            isRespawning = false;

            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = true;

                // Restaurar opacidad completa
                Color c = spriteRenderer.color;
                c.a = 1f;
                spriteRenderer.color = c;
            }

            Debug.Log("El diente se ha restaurado correctamente.");
        }
    }

    void InitializeMask()
    {
        maskTexture = new RenderTexture(maskResolution, maskResolution, 0, RenderTextureFormat.ARGB32);
        maskTexture.filterMode = FilterMode.Bilinear;
        maskTexture.Create();

        // Llenar la máscara de BLANCO al inicio
        Graphics.Blit(Texture2D.whiteTexture, maskTexture);

        brushTexture = CreateSoftBrush(64, brushSoftness);

        eraseMaterial = spriteRenderer.material;
        if (eraseMaterial == null)
        {
            Debug.LogError("[ToothDirtMask] El SpriteRenderer no tiene material asignado.");
            return;
        }
        eraseMaterial.SetTexture("_MaskTex", maskTexture);

        readbackTexture = new Texture2D(maskResolution / 4, maskResolution / 4, TextureFormat.RGBA32, false);
        totalPixels = readbackTexture.width * readbackTexture.height;
    }

    public void EraseAt(Vector2 worldPos)
    {
        Vector2 uv = WorldToMaskUV(worldPos);
        if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1) return;

        int px = Mathf.RoundToInt(uv.x * maskResolution);
        int py = Mathf.RoundToInt(uv.y * maskResolution);

        PaintBlackOnMask(px, py);
    }

    void PaintBlackOnMask(int centerX, int centerY)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = maskTexture;

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, maskResolution, maskResolution, 0);

        float half = brushSize * 0.5f;
        float x = centerX - half;
        float y = centerY - half;

        Graphics.DrawTexture(
            new Rect(x, y, brushSize, brushSize),
            brushTexture,
            new Rect(0, 0, 1, 1),
            0, 0, 0, 0,
            new Color(0, 0, 0, 1f)
        );

        GL.PopMatrix();
        RenderTexture.active = prev;
    }

    Vector2 WorldToMaskUV(Vector2 worldPos)
    {
        Bounds bounds = spriteRenderer.bounds;
        float u = (worldPos.x - bounds.min.x) / bounds.size.x;
        float v = (worldPos.y - bounds.min.y) / bounds.size.y;
        return new Vector2(u, v);
    }

    Texture2D CreateSoftBrush(int size, float softness)
    {
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        Color[] pixels = new Color[size * size];
        float half = size * 0.5f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(half, half)) / half;
                float alpha;
                if (softness < 0.01f)
                    alpha = dist <= 1f ? 1f : 0f;
                else
                    alpha = 1f - Mathf.Clamp01((dist - (1f - softness)) / softness);

                pixels[y * size + x] = new Color(0, 0, 0, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    public float GetCleanPercent() { return cleanPercent; }

    float nextCheckTime = 0f;
    public float AlphaValue { get => alphaValue; set => alphaValue = value; }

    void Update()
    {
        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + 0.5f;
            UpdateCleanPercent();
        }
    }

    void UpdateCleanPercent()
    {
        if (maskTexture == null || !maskTexture.IsCreated()) return;

        RenderTexture prev = RenderTexture.active;
        RenderTexture small = RenderTexture.GetTemporary(readbackTexture.width, readbackTexture.height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(maskTexture, small);
        RenderTexture.active = small;

        readbackTexture.ReadPixels(new Rect(0, 0, readbackTexture.width, readbackTexture.height), 0, 0);
        readbackTexture.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(small);

        Color[] pixels = readbackTexture.GetPixels();
        int erasedCount = 0;
        foreach (Color c in pixels)
        {
            if (c.r < 0.5f) erasedCount++;
        }

        cleanPercent = (float)erasedCount / totalPixels;

        if (cleanPercent >= cleanThreshold && !isRespawning)
        {
            OnToothCleaned?.Invoke();
        }
    }

    void OnDestroy()
    {
        if (maskTexture != null) maskTexture.Release();
    }

    public void ClearTooth()
    {
        if (spriteRenderer == null) return;

        Color c = spriteRenderer.color;

        // Baja la opacidad poco a poco
        c.a -= 0.05f;

        // Evita valores negativos
        c.a = Mathf.Clamp01(c.a);

        spriteRenderer.color = c;
    }

    public void RestaurarDiente()
    {
        if (spriteRenderer == null) return;

        // Restaurar opacidad
        Color c = spriteRenderer.color;
        c.a = 1f;
        spriteRenderer.color = c;

        // Restaurar máscara
        Graphics.Blit(Texture2D.whiteTexture, maskTexture);

        cleanPercent = 0f;

        Debug.Log("Diente restaurado.");
    }

    System.Collections.IEnumerator RespawnTooth()
    {
        isRespawning = true;

        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(respawnTime);

        Graphics.Blit(Texture2D.whiteTexture, maskTexture);

        cleanPercent = 0f;

        spriteRenderer.enabled = true;

        // Restaurar opacidad
        Color c = spriteRenderer.color;
        c.a = 1f;
        spriteRenderer.color = c;

        isRespawning = false;
    }
}
