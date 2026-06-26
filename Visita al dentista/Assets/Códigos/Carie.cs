using UnityEngine;

public class Carie : MonoBehaviour
{
    // Aquí guardamos nuestra cajita mágica de partículas
    public ParticleSystem particulasSuciedad;

    // Esta función se activa SOLA cuando algo toca la carie
    private void OnTriggerEnter2D(Collider2D objetoQueMeToco)
    {
        // ¿El objeto que me tocó tiene la etiqueta "Cepillo"?
        if (objetoQueMeToco.CompareTag("Cepillo"))
        {
            // 1. Movemos la cajita de partículas a donde está la carie
            particulasSuciedad.transform.position = transform.position;

            // 2. ¡Le damos PLAY para que salga la suciedad!
            particulasSuciedad.Play();

            // 3. Destruimos la carie porque ya se limpió
            Destroy(gameObject);
        }
    }
}