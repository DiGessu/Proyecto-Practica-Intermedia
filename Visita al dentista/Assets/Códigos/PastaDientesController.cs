using UnityEngine;

public class PastaDientesController : MonoBehaviour
{
    private Animator animator;
    private bool destapada = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Detecta toque en pantalla (funciona en móvil y PC)
        if (Input.GetMouseButtonDown(0) && !destapada)
        {
            Vector2 posicionToque = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(posicionToque, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                Destapar();
            }
        }
    }

    void Destapar()
    {
        destapada = true;
        animator.SetTrigger("Destapar");
    }
}
