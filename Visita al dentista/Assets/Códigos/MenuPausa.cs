using UnityEngine;
using UnityEngine.SceneManagement; // Requerido para recargar escenas

public class MenuPausa : MonoBehaviour
{
    [SerializeField] GameObject menuPausa; //

    // Esta función la debe llamar el botón de la esquina (II)
    public void Pausa()
    {
        menuPausa.SetActive(true); //
        Time.timeScale = 0f; // Esto congela el juego
    }

    public void Resume()
    {
        menuPausa.SetActive(false); //
        Time.timeScale = 1f; // Esto descongela el juego
    }

    public void Inicio()
    {
        Time.timeScale = 1f; // ¡OJO! Siempre resetea el tiempo antes de cambiar de escena
        SceneManager.LoadScene("Pantalla inicio"); //
    }

    // ==========================================
    // NUEVAS FUNCIONES PARA EL PANEL DE GAME OVER
    // ==========================================

    public void VolverAEmpezar()
    {
        Debug.Log("Reiniciando la clínica dental...");

        // Al igual que en tu función Inicio(), restauramos el tiempo a 1
        // de lo contrario, la escena reiniciada cargará completamente congelada.
        Time.timeScale = 1f;

        // Obtiene el nombre o índice de la escena que está abierta actualmente y la recarga
        string escenaActual = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(escenaActual);
    }

    public void SalirDelJuego()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit(); // Cierra el juego
    }
}