using UnityEngine;

public class CavityMask : MonoBehaviour
{
    public void EraseAt(Vector2 position)
    {
        // Aqui esta la logica para borrar la carie
        // Sera en este caso con el sprite
        Debug.Log("Quitando carie en: " + position);
    }

    public void ClearCavity()
    {
        gameObject.SetActive(false);
    }
}
