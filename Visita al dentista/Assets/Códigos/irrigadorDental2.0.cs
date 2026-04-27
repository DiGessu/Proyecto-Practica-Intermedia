using UnityEngine;

public class Irrigador : MonoBehaviour
{
    public float grabDistance = 5f;
    public Transform holdPoint;
    public ParticleSystem waterParticles;

    private GameObject grabbedObject;
    private Rigidbody grabbedRb;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // Clic izquierdo para agarrar o soltar
        if (Input.GetMouseButtonDown(0))
        {
            if (grabbedObject == null)
                TryGrab();
            else
                ReleaseObject();
        }

        // Si hay objeto agarrado, mantenerlo en la mano
        if (grabbedObject != null)
        {
            grabbedObject.transform.position = holdPoint.position;
            grabbedObject.transform.rotation = holdPoint.rotation;

            // Espacio para lanzar agua
            if (Input.GetKey(KeyCode.Space))
                ShootWater();
            else
                StopWater();
        }
        else
        {
            // Si se suelta el objeto, parar el agua
            StopWater();
        }
    }

    void TryGrab()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            if (hit.collider.CompareTag("Grabbable"))
            {
                grabbedObject = hit.collider.gameObject;
                grabbedRb = grabbedObject.GetComponent<Rigidbody>();

                if (grabbedRb != null)
                {
                    grabbedRb.isKinematic = true;
                    grabbedRb.useGravity = false;
                }
            }
        }
    }

    void ReleaseObject()
    {
        if (grabbedRb != null)
        {
            grabbedRb.isKinematic = false;
            grabbedRb.useGravity = true;
        }

        StopWater();
        grabbedObject = null;
        grabbedRb = null;
    }

    void ShootWater()
    {
        print("estoy disparando");
        if (waterParticles != null && !waterParticles.isPlaying)
            waterParticles.Play();
    }

    void StopWater()
    {
        if (waterParticles != null && waterParticles.isPlaying)
            waterParticles.Stop();
    }
}