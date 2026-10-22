
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
        textoPuntosFinales.text = "Puntuación: " + puntuacion.ObtenerPuntos();
        textoRecordFinal.text = "Récord: " + puntuacion.ObtenerRecord();

        textoPuntuacion.SetActive(false);
        textoRecord.SetActive(false);

        panelGameOver.SetActive(true);
        Time.timeScale = 0f;
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
