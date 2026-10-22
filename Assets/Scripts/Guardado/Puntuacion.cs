
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

        textoRecord.text = "RÉCORD\n<size=44>" + record.ToString("000") + "</size>";
    }

    void Update()
    {
        puntos += Time.deltaTime;

        int puntuacionActual = Mathf.FloorToInt(puntos);
        textoPuntuacion.text = "PUNTOS\n<size=64>" + puntuacionActual.ToString("000") + "</size>";

        if (puntuacionActual > record)
        {
            record = puntuacionActual;
            guardado.GuardarRecord(record);
            textoRecord.text = "RÉCORD\n<size=44>" + record.ToString("000") + "</size>";
        }
    }


    public int ObtenerPuntos()
    {
        return Mathf.FloorToInt(puntos);
    }

    public int ObtenerRecord()
    {
        return record;
    }

}
