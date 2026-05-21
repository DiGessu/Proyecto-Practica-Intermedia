using UnityEngine;

public class PastaDientesController : MonoBehaviour
{
    private Animator animator;

    // Al ser pública, el cepillo podrá leerla directamente
    public bool destapada = false;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Si hacen clic en la pasta y está cerrada
        if (Input.GetMouseButtonDown(0) && !destapada)
        {
            Vector2 posicionToque = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(posicionToque, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                destapada = true;
                animator.SetTrigger("Destapar");
            }
        }
    }
}