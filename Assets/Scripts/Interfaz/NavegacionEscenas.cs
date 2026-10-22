using UnityEngine;
using UnityEngine.SceneManagement;

// Punto común para salir de una partida, incluso si estaba pausada.
public static class NavegacionEscenas
{
    public static void Jugar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Juego");
    }

    public static void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
