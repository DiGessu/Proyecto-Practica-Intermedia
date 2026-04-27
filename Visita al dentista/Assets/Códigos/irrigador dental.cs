using UnityEngine;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class Irrigador : MonoBehaviour
{
    public float grabDistance = 3f;
    public Transform holdPoint;
    public ParticleSystem waterParticles;

    private GameObject grabbedObject;

    void Update()
    {
        // Click para agarrar
        if (Input.GetMouseButtonDown(0))
        {
            TryGrab();
        }

        // Mantener objeto en la mano
        if (grabbedObject != null)
        {
            grabbedObject.transform.position = holdPoint.position;
            grabbedObject.transform.rotation = holdPoint.rotation;

            // Espacio para lanzar agua
            if (Input.GetKey(KeyCode.Space))
            {
                ShootWater();
            }
            else
            {
                StopWater();
            }
        }
    }

    void TryGrab()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            if (hit.collider.CompareTag("Grabbable"))
            {
                grabbedObject = hit.collider.gameObject;

                Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                }
            }
        }
    }

    void ShootWater()
    {
        if (waterParticles != null && !waterParticles.isPlaying)
        {
            waterParticles.Play();
        }
    }

    void StopWater()
    {
        if (waterParticles != null && waterParticles.isPlaying)
        {
            waterParticles.Stop();
        }
    }
}