using UnityEngine;

public class ToothpasteBrush : MonoBehaviour
{
    [Header("Configuración del cepillo")]
    [Range(5f, 30f)]
    public float followSpeed = 15f;

    public bool followMouse = true;

    public Vector2 brushOffset =
        new Vector2(0f, 0.5f);

    [Header("Referencia al sarro")]
    public TartarMask tartarMask;

    private Camera mainCamera;

    private bool isBrushing = false;

    private Vector2 currentVelocity;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleInput();
        HandleBrushing();
    }

    void HandleInput()
    {
        if (!followMouse) return;

        Vector2 targetPos;

        targetPos =
            mainCamera.ScreenToWorldPoint(
                Input.mousePosition
            );

        isBrushing =
            Input.GetMouseButton(0);

        targetPos += brushOffset;

        Vector2 newPos =
            Vector2.SmoothDamp(
                transform.position,
                targetPos,
                ref currentVelocity,
                1f / followSpeed
            );

        transform.position =
            new Vector3(
                newPos.x,
                newPos.y,
                transform.position.z
            );
    }

    void HandleBrushing()
    {
        if (!isBrushing || tartarMask == null)
            return;

        Vector2 contactPoint =
            (Vector2)transform.position
            - brushOffset;

        tartarMask.EraseAt(contactPoint);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        TartarMask tartar =
            collision.GetComponent<TartarMask>();

        if (tartar != null)
        {
            tartar.ClearTartar();
        }
    }
}