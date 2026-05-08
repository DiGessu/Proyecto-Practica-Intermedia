using UnityEngine;

public class CavityMask : MonoBehaviour
{
    public void EraseAt(Vector2 position)
    {
        // Aquí va tu lógica para borrar la carie
        // puede ser textura, máscara o sprite
        Debug.Log("Quitando carie en: " + position);
    }

    public void ClearCavity()
    {
        gameObject.SetActive(false);
    }
}
