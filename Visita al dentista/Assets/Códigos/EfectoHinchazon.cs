using System.Collections;
using UnityEngine;

public class EfectoHinchazon : MonoBehaviour
{
    [Header("Configuración de la Animación")]
    [SerializeField] private float escalaMaxima = 1.15f;
    [SerializeField] private float duracionHinchazon = 0.4f;

    private Vector3 escalaOriginal;
    private bool estaHinchando = false;

    void Awake()
    {
        escalaOriginal = transform.localScale;
    }

    void OnEnable()
    {
        // Nos conectamos ÚNICAMENTE al evento de la carie
        EstadoDiente.OnAparicionDeCarie += IniciarAnimacionSegura;
    }

    void OnDisable()
    {
        EstadoDiente.OnAparicionDeCarie -= IniciarAnimacionSegura;
    }

    private void IniciarAnimacionSegura()
    {
        if (!estaHinchando)
        {
            StartCoroutine(AnimarHinchazonUnica());
        }
    }

    private IEnumerator AnimarHinchazonUnica()
    {
        estaHinchando = true;
        float tiempo = 0f;
        float mitadDuracion = duracionHinchazon / 2f;

        // Crecer
        while (tiempo < mitadDuracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / mitadDuracion;
            transform.localScale = Vector3.Lerp(escalaOriginal, escalaOriginal * escalaMaxima, progreso);
            yield return null;
        }

        tiempo = 0f;

        // Volver al tamaño normal
        while (tiempo < mitadDuracion)
        {
            tiempo += Time.deltaTime;
            float progreso = tiempo / mitadDuracion;
            transform.localScale = Vector3.Lerp(escalaOriginal * escalaMaxima, escalaOriginal, progreso);
            yield return null;
        }

        transform.localScale = escalaOriginal;
        estaHinchando = false;
    }
}