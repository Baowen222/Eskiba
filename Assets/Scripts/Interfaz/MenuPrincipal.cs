using TMPro;
using UnityEngine;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textoRecord;
    IGuardado guardado;

    void Awake()
    {
        Time.timeScale = 1f;
        guardado = new GuardadoLocal();
    }

    void Start()
    {
        textoRecord.text = guardado.CargarRecord().ToString("000");
    }

    public void Jugar()
    {
        NavegacionEscenas.Jugar();
    }

    public void Salir()
    {
        Application.Quit();
        // En el Editor se detiene Play; en Android se cierra la aplicación.
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
