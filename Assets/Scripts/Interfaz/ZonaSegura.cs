using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ZonaSegura : MonoBehaviour
{
    RectTransform rectangulo;
    Rect ultimaZona;
    Vector2Int ultimaResolucion;

    void Awake()
    {
        rectangulo = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        Aplicar();
    }

    void Update()
    {
        if (ultimaZona != Screen.safeArea ||
            ultimaResolucion != new Vector2Int(Screen.width, Screen.height))
            Aplicar();
    }

    void Aplicar()
    {
        if (Screen.width == 0 || Screen.height == 0) return;
        ultimaZona = Screen.safeArea;
        ultimaResolucion = new Vector2Int(Screen.width, Screen.height);
        Vector2 pantalla = new Vector2(Screen.width, Screen.height);
        rectangulo.anchorMin = ultimaZona.min / pantalla;
        rectangulo.anchorMax = ultimaZona.max / pantalla;
        rectangulo.offsetMin = Vector2.zero;
        rectangulo.offsetMax = Vector2.zero;
    }
}
