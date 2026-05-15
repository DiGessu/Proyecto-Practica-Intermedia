using UnityEngine;

public class HerramientaManager : MonoBehaviour
{
    public TipoHerramienta herramientaSeleccionada;

    void Update()
    {
        // Detectar clic en un diente (requiere que el diente tenga un Collider2D)
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 rayPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero);

            if (hit.collider != null)
            {
                EstadoDiente diente = hit.collider.GetComponent<EstadoDiente>();
                if (diente != null)
                {
                    diente.IntentarLimpiar(herramientaSeleccionada);
                }
            }
        }
    }

    // Métodos para botones de la UI
    public void SeleccionarCepillo() => herramientaSeleccionada = TipoHerramienta.CEPILLO;
    public void SeleccionarPasta() => herramientaSeleccionada = TipoHerramienta.CEPILLO_CON_PASTA;
    public void SeleccionarAlgodon() => herramientaSeleccionada = TipoHerramienta.ALGODON;
}