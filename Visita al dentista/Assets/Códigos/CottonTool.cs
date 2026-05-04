using UnityEngine;

public class CottonTool : MonoBehaviour
{
    public CavityMask currentCavity;

    void Update()
    {
        if (Input.GetMouseButton(0) && currentCavity != null)
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            currentCavity.EraseAt(pos);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var cavity = collision.GetComponent<CavityMask>();
        if (cavity != null)
        {
            currentCavity = cavity;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<CavityMask>() != null)
        {
            currentCavity = null;
        }
    }
}
