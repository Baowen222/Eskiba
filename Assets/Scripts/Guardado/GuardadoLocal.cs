
using UnityEngine;

public class GuardadoLocal : IGuardado
{
    public int CargarRecord()
    {
        return PlayerPrefs.GetInt("Record", 0);
    }

    public void GuardarRecord(int puntos)
    {
        PlayerPrefs.SetInt("Record", puntos);
        PlayerPrefs.Save();
    }
}
