
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausaJuego : MonoBehaviour
{
    public GameObject panelPausa;
    public GameObject panelGameOver;

    bool pausado = false;

    void OnApplicationPause(bool pausa)
    {
        if (pausa)
        {
            Pausar();
        }
    }

    void OnApplicationFocus(bool tieneFoco)
    {
        if (!tieneFoco)
        {
            Pausar();
        }
    }

    void Pausar()
    {
        // No abrimos la pausa si ya ha  perdido
        if (panelGameOver.activeSelf || pausado)
        {
            return;
        }

        pausado = true;
        panelPausa.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Continuar()
    {
        pausado = false;
        panelPausa.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
