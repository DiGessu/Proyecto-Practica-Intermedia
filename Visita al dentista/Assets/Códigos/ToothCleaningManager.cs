using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manager para múltiples dientes independientes.
/// Cada diente tiene su propio progreso y se limpia por separado.
/// </summary>
public class ToothCleaningManager : MonoBehaviour
{
    [System.Serializable]
    public class ToothUnit
    {
        public string toothName = "Diente";
        public ToothDirtMask dirtyTooth;
        public SpriteRenderer cleanToothRenderer;
        [HideInInspector] public bool isCleaned = false;
    }

    [Header("Dientes")]
    [Tooltip("Agrega aquí cada unidad de diente (sucio + limpio)")]
    public List<ToothUnit> teeth = new List<ToothUnit>();

    [Header("Cepillo")]
    public ToothBrush brush;

    [Header("UI General")]
    [Tooltip("Slider que muestra el progreso TOTAL de todos los dientes")]
    public Slider totalProgressBar;
    [Tooltip("Texto con el porcentaje total")]
    public Text totalPercentText;
    [Tooltip("Panel que aparece cuando TODOS los dientes están limpios")]
    public GameObject allCleanPanel;

    [Header("Configuración")]
    public float cleanRevealDuration = 1.5f;

    private int cleanedCount = 0;

    void Start()
    {
        if (allCleanPanel != null)
            allCleanPanel.SetActive(false);

        foreach (ToothUnit tooth in teeth)
        {
            if (tooth.dirtyTooth == null) continue;

            ToothUnit captured = tooth;
            tooth.dirtyTooth.OnToothCleaned += () => HandleToothCleaned(captured);

            if (tooth.cleanToothRenderer != null)
            {
                Color c = tooth.cleanToothRenderer.color;
                c.a = 0f;
                tooth.cleanToothRenderer.color = c;
            }
        }
    }

    void Update()
    {
        UpdateTotalProgress();
    }

    void UpdateTotalProgress()
    {
        if (teeth.Count == 0) return;

        float total = 0f;
        foreach (ToothUnit tooth in teeth)
        {
            if (tooth.dirtyTooth != null)
                total += tooth.isCleaned ? 1f : tooth.dirtyTooth.GetCleanPercent();
        }

        float average = total / teeth.Count;

        if (totalProgressBar != null)
            totalProgressBar.value = average;

        if (totalPercentText != null)
            totalPercentText.text = Mathf.RoundToInt(average * 100f) + "%";
    }

    void HandleToothCleaned(ToothUnit tooth)
    {
        if (tooth.isCleaned) return;
        tooth.isCleaned = true;
        cleanedCount++;

        Debug.Log($"[ToothCleaningManager] '{tooth.toothName}' limpio. ({cleanedCount}/{teeth.Count})");

        StartCoroutine(RevealCleanTooth(tooth));

        if (cleanedCount >= teeth.Count)
            StartCoroutine(ShowAllCleanPanel());
    }

    IEnumerator RevealCleanTooth(ToothUnit tooth)
    {
        float elapsed = 0f;
        SpriteRenderer dirtyRenderer = tooth.dirtyTooth.GetComponent<SpriteRenderer>();

        while (elapsed < cleanRevealDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / cleanRevealDuration;
            float smooth = t * t * (3f - 2f * t);

            if (dirtyRenderer != null)
            {
                Color dc = dirtyRenderer.color;
                dc.a = 1f - smooth;
                dirtyRenderer.color = dc;
            }

            if (tooth.cleanToothRenderer != null)
            {
                Color cc = tooth.cleanToothRenderer.color;
                cc.a = smooth;
                tooth.cleanToothRenderer.color = cc;
            }

            yield return null;
        }

        if (dirtyRenderer != null)
        {
            Color dc = dirtyRenderer.color; dc.a = 0f;
            dirtyRenderer.color = dc;
        }
        if (tooth.cleanToothRenderer != null)
        {
            Color cc = tooth.cleanToothRenderer.color; cc.a = 1f;
            tooth.cleanToothRenderer.color = cc;
            StartCoroutine(ShineEffect(tooth.cleanToothRenderer.transform));
        }
    }

    IEnumerator ShowAllCleanPanel()
    {
        yield return new WaitForSeconds(0.8f);

        if (brush != null)
            brush.gameObject.SetActive(false);

        if (allCleanPanel != null)
            allCleanPanel.SetActive(true);
    }

    IEnumerator ShineEffect(Transform target)
    {
        Vector3 original = target.localScale;
        Vector3 big = original * 1.08f;
        float duration = 0.35f;
        float t = 0;

        while (t < 1f) { t += Time.deltaTime / duration; target.localScale = Vector3.Lerp(original, big, t * t); yield return null; }
        t = 0;
        while (t < 1f) { t += Time.deltaTime / duration; target.localScale = Vector3.Lerp(big, original, t); yield return null; }
        target.localScale = original;
    }

    public void ResetAllTeeth()
    {
        cleanedCount = 0;

        foreach (ToothUnit tooth in teeth)
        {
            tooth.isCleaned = false;

            if (tooth.dirtyTooth != null)
                Graphics.Blit(Texture2D.whiteTexture, tooth.dirtyTooth.maskTexture);

            SpriteRenderer dirtyRenderer = tooth.dirtyTooth?.GetComponent<SpriteRenderer>();
            if (dirtyRenderer != null) { Color dc = dirtyRenderer.color; dc.a = 1f; dirtyRenderer.color = dc; }
            if (tooth.cleanToothRenderer != null) { Color cc = tooth.cleanToothRenderer.color; cc.a = 0f; tooth.cleanToothRenderer.color = cc; }
        }

        if (allCleanPanel != null) allCleanPanel.SetActive(false);
        if (brush != null) brush.gameObject.SetActive(true);
    }
}
