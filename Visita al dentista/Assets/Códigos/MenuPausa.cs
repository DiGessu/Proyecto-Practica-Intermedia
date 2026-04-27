using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPausa : MonoBehaviour
{
    [SerializeField] GameObject menuPausa;

    // Esta función la debe llamar el botón de la esquina (II)
    public void Pausa()
    {
        menuPausa.SetActive(true);
        Time.timeScale = 0f; // Esto congela el juego
    }

    public void Resume()
    {
        menuPausa.SetActive(false);
        Time.timeScale = 1f; // Esto descongela el juego
    }

    public void Inicio()
    {
        Time.timeScale = 1f; // ¡OJO! Siempre resetea el tiempo antes de cambiar de escena
        SceneManager.LoadScene("Menu Principal");
    }
}
