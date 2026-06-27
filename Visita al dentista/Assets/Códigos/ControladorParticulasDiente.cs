using UnityEngine;

public class ControladorParticulasDiente : MonoBehaviour
{
    private GameObject objetoParticulasCarie;
    private ParticleSystem sistemaParticulas;
    private SpriteRenderer srCarie;

    void Awake()
    {
        // Buscamos el componente en los hijos, incluso si está desactivado (true)
        sistemaParticulas = GetComponentInChildren<ParticleSystem>(true);
        if (sistemaParticulas != null)
        {
            objetoParticulasCarie = sistemaParticulas.gameObject;
        }
    }

    void Start()
    {
        // Buscamos la capa llamada "Carie" que tu script de dientes crea dinámicamente
        Transform capaCarieTransform = transform.Find("Carie");
        if (capaCarieTransform != null)
        {
            srCarie = capaCarieTransform.GetComponent<SpriteRenderer>();
        }

        // Forzamos que empiece apagado para evitar impostores al arrancar
        if (objetoParticulasCarie != null)
        {
            objetoParticulasCarie.SetActive(false);
        }
    }

    void Update()
    {
        if (objetoParticulasCarie == null) return;

        // Si la capa carie aún no se había asignado, la intentamos buscar de nuevo
        if (srCarie == null)
        {
            Transform capaCarieTransform = transform.Find("Carie");
            if (capaCarieTransform != null)
            {
                srCarie = capaCarieTransform.GetComponent<SpriteRenderer>();
            }
            return; // Esperamos al siguiente frame a que aparezca
        }

        // *** AQUÍ ESTÁ EL CAMBIO CLAVE ***
        // Solo activamos las partículas si la capa de carie existe, está encendida 
        // y su opacidad (alpha) es visible en pantalla (mayor a 0.05)
        if (srCarie.enabled && srCarie.color.a > 0.05f)
        {
            if (!objetoParticulasCarie.activeSelf)
            {
                objetoParticulasCarie.SetActive(true);
                Debug.Log("[Magia] Carie visible en " + gameObject.name + ". Encendiendo partículas.");
            }
        }
        else
        {
            // Si la capa está apagada o el algodón bajó su opacidad a casi 0, se apaga de inmediato
            if (objetoParticulasCarie.activeSelf)
            {
                objetoParticulasCarie.SetActive(false);
                Debug.Log("[Magia] Carie invisible o limpiada en " + gameObject.name + ". Apagando partículas.");
            }
        }
    }
}