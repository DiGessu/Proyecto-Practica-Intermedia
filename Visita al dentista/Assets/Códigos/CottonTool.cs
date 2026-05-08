using UnityEngine;

public class CottonTool : MonoBehaviour
{
    [Range(5f, 30f)] public float followSpeed = 15f;
    public bool followMouse = true;

    public Vector2 offset = new Vector2(0f, 0.5f);

    [Header("Referencia a la carie")]
    public CavityMask cavityMask;

    private Camera mainCamera;
    private bool isCleaning = false;
    private Vector2 currentVelocity;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        HandleInput();
        HandleCleaning();
    }

    void HandleInput()
    {
        if (!followMouse) return;

        Vector2 targetPos;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            targetPos = mainCamera.ScreenToWorldPoint(touch.position);
            isCleaning = (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary);
        }
        else
        {
            targetPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            isCleaning = Input.GetMouseButton(0);
        }

        targetPos += offset;

        Vector2 newPos = Vector2.SmoothDamp(
            transform.position,
            targetPos,
            ref currentVelocity,
            1f / followSpeed
        );

        transform.position = new Vector3(newPos.x, newPos.y, transform.position.z);
    }

    void HandleCleaning()
    {
        if (!isCleaning || cavityMask == null) return;

        Vector2 contactPoint = (Vector2)transform.position - offset;

        cavityMask.EraseAt(contactPoint);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<CavityMask>())
        {
            collision.gameObject.GetComponent<CavityMask>().ClearCavity();
        }
    }
}