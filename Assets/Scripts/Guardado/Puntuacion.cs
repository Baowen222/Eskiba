
using UnityEngine;
using TMPro;

public class Puntuacion : MonoBehaviour
{
    public TextMeshProUGUI textoPuntuacion;
    public TextMeshProUGUI textoRecord;

    float puntos = 0f;
    int record;

    IGuardado guardado;

    void Start()
    {
        guardado = new GuardadoLocal();
        record = guardado.CargarRecord();

        textoRecord.text = "Record: " + record;
    }

    void Update()
    {
        puntos += Time.deltaTime;

        int puntuacionActual = Mathf.FloorToInt(puntos);
        textoPuntuacion.text = "Score: " + puntuacionActual;

        if (puntuacionActual > record)
        {
            record = puntuacionActual;
            guardado.GuardarRecord(record);
            textoRecord.text = "Record: " + record;
        }
    }
}
