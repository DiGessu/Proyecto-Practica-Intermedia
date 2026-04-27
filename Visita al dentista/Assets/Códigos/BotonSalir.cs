using UnityEngine;

public class BotonSalir : MonoBehaviour
{
    // Asegúrate de que tenga las llaves { } y NO un ; al final de la línea del nombre
    public void Salir()
    {
        Debug.Log("Saliendo del juego..."); // Esto te avisa en la consola que funciona
        Application.Quit(); // Cierra el juego (solo funciona en el .exe compilado)
    }
}