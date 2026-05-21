using UnityEngine;

public class ToothSarroMask : MonoBehaviour
{
    [Header("Configuración de la máscara de Sarro")]
    public int maskResolution = 512;
    [Range(10, 120)] public int brushSize = 40;
    [Range(0f, 1f)] public float brushSoftness = 0.5f;
    [Range(0.5f, 1f)] public float cleanThreshold = 0.85f;

    [HideInInspector] public RenderTexture maskTexture;

    private SpriteRenderer spriteRenderer;
    private Material eraseMaterial;
    private Texture2D brushTexture;

    private Texture2D readbackTexture;
    private int totalPixels;
    private float cleanPercent = 0f;
    private bool isCleaned = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeMask();
    }

    public void ResetearMascaraSarro()
    {
        if (maskTexture != null)
        {
            Graphics.Blit(Texture2D.whiteTexture, maskTexture);
            cleanPercent = 0f;
            isCleaned = false;
            if (spriteRenderer != null) spriteRenderer.enabled = true;
        }
    }

    void InitializeMask()
    {
        maskTexture = new RenderTexture(maskResolution, maskResolution, 0, RenderTextureFormat.ARGB32);
        maskTexture.filterMode = FilterMode.Bilinear;
        maskTexture.Create();

        Graphics.Blit(Texture2D.whiteTexture, maskTexture);
        brushTexture = CreateSoftBrush(64, brushSoftness);

        eraseMaterial = spriteRenderer.material;
        if (eraseMaterial == null || eraseMaterial.shader.name == "Sprites/Default")
        {
            Debug.LogError($"[{gameObject.name}] El Sarro necesita un material con el Shader de borrador (MaskErase).");
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
        UpdateCleanPercent();
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
                float alpha = (softness < 0.01f) ? (dist <= 1f ? 1f : 0f) : (1f - Mathf.Clamp01((dist - (1f - softness)) / softness));
                pixels[y * size + x] = new Color(0, 0, 0, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    void UpdateCleanPercent()
    {
        if (maskTexture == null || !maskTexture.IsCreated() || isCleaned) return;

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

        if (cleanPercent >= cleanThreshold)
        {
            isCleaned = true;
            // Avisa de forma segura al script base del diente que ya se limpió el sarro
            gameObject.SendMessage("ActualizarSpriteInicial", SendMessageOptions.DontRequireReceiver);
            gameObject.SetActive(false); // Oculta el objeto de sarro limpio
        }
    }

    void OnDestroy()
    {
        if (maskTexture != null) maskTexture.Release();
    }
}
