
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOver : MonoBehaviour
{
    public GameObject panelGameOver;
    public TextMeshProUGUI textoPuntosFinales;
    public TextMeshProUGUI textoRecordFinal;
    public Puntuacion puntuacion;
    public GameObject textoPuntuacion;
    public GameObject textoRecord;
    public void TerminarPartida()
    {
        textoPuntosFinales.text = "PUNTUACIÓN\n<size=100>" + puntuacion.ObtenerPuntos().ToString("000") + "</size>";
        textoRecordFinal.text = "RÉCORD PERSONAL   " + puntuacion.ObtenerRecord().ToString("000");

        textoPuntuacion.SetActive(false);
        textoRecord.SetActive(false);

        panelGameOver.SetActive(true);
        Time.timeScale = 0f;
    }

    public void VolverAlMenu()
    {
        NavegacionEscenas.VolverAlMenu();
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
