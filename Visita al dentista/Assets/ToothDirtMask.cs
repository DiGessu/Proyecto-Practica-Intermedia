using UnityEngine;

/// <summary>
/// Coloca este script en el GameObject del diente SUCIO.
/// Requiere un SpriteRenderer y un Material que use el shader "Sprites/Mask Erase".
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ToothDirtMask : MonoBehaviour
{
    [Header("Configuración de la máscara")]
    [Tooltip("Tamaño en píxeles de la textura de máscara (potencia de 2 recomendado)")]
    public int maskResolution = 512;

    [Tooltip("Tamaño del pincel al cepillar (en píxeles sobre la máscara)")]
    [Range(10, 120)]
    public int brushSize = 40;

    [Tooltip("Suavidad del borde del pincel (0 = duro, 1 = muy suave)")]
    [Range(0f, 1f)]
    public float brushSoftness = 0.5f;

    [Tooltip("Porcentaje de máscara borrada para considerar el diente 'limpio' (0-1)")]
    [Range(0.5f, 1f)]
    public float cleanThreshold = 0.85f;

    // Referencia pública para que el cepillo pinte sobre esta textura
    [HideInInspector] public RenderTexture maskTexture;

    private SpriteRenderer spriteRenderer;
    private Material eraseMaterial;
    private Texture2D brushTexture;

    // Para calcular el progreso de limpieza
    private Texture2D readbackTexture;
    private int totalPixels;
    private float cleanPercent = 0f;

    // Evento que se dispara cuando el diente queda limpio
    public System.Action OnToothCleaned;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeMask();
    }

    void InitializeMask()
    {
        // Crear la RenderTexture de máscara (blanca = sucia visible)
        maskTexture = new RenderTexture(maskResolution, maskResolution, 0, RenderTextureFormat.ARGB32);
        maskTexture.filterMode = FilterMode.Bilinear;
        maskTexture.Create();

        // Llenar la máscara de BLANCO (todo el diente sucio visible al inicio)
        Graphics.Blit(Texture2D.whiteTexture, maskTexture);

        // Crear el pincel suave con gradiente radial
        brushTexture = CreateSoftBrush(64, brushSoftness);

        // Asignar la máscara al material del SpriteRenderer
        // El material debe usar el shader "Sprites/MaskErase" (incluido en el paquete)
        eraseMaterial = spriteRenderer.material;
        if (eraseMaterial == null)
        {
            Debug.LogError("[ToothDirtMask] El SpriteRenderer no tiene material asignado. " +
                           "Asigna el material 'DirtMaskMaterial' creado con el shader personalizado.");
            return;
        }
        eraseMaterial.SetTexture("_MaskTex", maskTexture);

        // Para leer píxeles y calcular progreso
        readbackTexture = new Texture2D(maskResolution / 4, maskResolution / 4, TextureFormat.RGBA32, false);
        totalPixels = readbackTexture.width * readbackTexture.height;
    }

    /// <summary>
    /// Llamar desde ToothBrush.cs cuando el cepillo está sobre el diente.
    /// worldPos = posición en espacio mundo donde pinta el cepillo.
    /// </summary>
    public void EraseAt(Vector2 worldPos)
    {
        // Convertir posición mundo → UV de la máscara
        Vector2 uv = WorldToMaskUV(worldPos);

        if (uv.x < 0 || uv.x > 1 || uv.y < 0 || uv.y > 1)
            return; // Fuera del diente

        // Calcular posición en píxeles
        int px = Mathf.RoundToInt(uv.x * maskResolution);
        int py = Mathf.RoundToInt(uv.y * maskResolution);

        // Pintar negro (transparente) en la máscara usando un pincel suave
        PaintBlackOnMask(px, py);
    }

    void PaintBlackOnMask(int centerX, int centerY)
    {
        RenderTexture prev = RenderTexture.active;
        RenderTexture.active = maskTexture;

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, maskResolution, maskResolution, 0);

        // Calcular el rect del pincel
        float half = brushSize * 0.5f;
        float x = centerX - half;
        float y = centerY - half;

        // Usar Graphics.DrawTexture para pintar el pincel (negro = borra)
        // Se usa blending aditivo inverso para suavizar
        Graphics.DrawTexture(
            new Rect(x, y, brushSize, brushSize),
            brushTexture,
            new Rect(0, 0, 1, 1),
            0, 0, 0, 0,
            new Color(0, 0, 0, 1f) // Negro opaco
        );

        GL.PopMatrix();
        RenderTexture.active = prev;
    }

    /// <summary>
    /// Convierte posición en espacio mundo a coordenadas UV de la máscara.
    /// </summary>
    Vector2 WorldToMaskUV(Vector2 worldPos)
    {
        // Bounds del sprite en espacio mundo
        Bounds bounds = spriteRenderer.bounds;
        float u = (worldPos.x - bounds.min.x) / bounds.size.x;
        float v = (worldPos.y - bounds.min.y) / bounds.size.y;
        return new Vector2(u, v);
    }

    /// <summary>
    /// Crea una textura circular con degradado suave (blanco en centro, negro en bordes).
    /// El shader interpretará: blanco = mantener suciedad, negro = borrar suciedad.
    /// </summary>
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

                // Pincel: blanco con alpha variable (negro al multiplicar invertido en shader)
                pixels[y * size + x] = new Color(0, 0, 0, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return tex;
    }

    /// <summary>
    /// Calcula qué porcentaje del diente ya fue cepillado (0 a 1).
    /// Llama esto periódicamente, no cada frame (es costoso).
    /// </summary>
    public float GetCleanPercent()
    {
        return cleanPercent;
    }

    // Actualizar porcentaje de limpieza cada 0.5 segundos
    float nextCheckTime = 0f;
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
        RenderTexture prev = RenderTexture.active;

        // Usar versión reducida para no ser tan costoso
        RenderTexture small = RenderTexture.GetTemporary(
            readbackTexture.width, readbackTexture.height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(maskTexture, small);
        RenderTexture.active = small;

        readbackTexture.ReadPixels(
            new Rect(0, 0, readbackTexture.width, readbackTexture.height), 0, 0);
        readbackTexture.Apply();

        RenderTexture.active = prev;
        RenderTexture.ReleaseTemporary(small);

        // Contar píxeles que se han vuelto oscuros (borrados)
        Color[] pixels = readbackTexture.GetPixels();
        int erasedCount = 0;
        foreach (Color c in pixels)
        {
            // Si el rojo del canal es menor que 0.5, está "borrado"
            if (c.r < 0.5f)
                erasedCount++;
        }

        cleanPercent = (float)erasedCount / totalPixels;

        // Disparar evento si el diente está suficientemente limpio
        if (cleanPercent >= cleanThreshold)
        {
            OnToothCleaned?.Invoke();
        }
    }

    void OnDestroy()
    {
        if (maskTexture != null)
            maskTexture.Release();
    }
}
