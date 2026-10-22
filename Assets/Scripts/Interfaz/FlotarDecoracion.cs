using UnityEngine;

public class FlotarDecoracion : MonoBehaviour
{
    [SerializeField] float amplitud = 12f;
    [SerializeField] float desfase;
    RectTransform rectangulo;
    Vector2 posicionInicial;

    void Awake()
    {
        rectangulo = GetComponent<RectTransform>();
        posicionInicial = rectangulo.anchoredPosition;
    }

    void Update()
    {
        rectangulo.anchoredPosition = posicionInicial + Vector2.up *
            (Mathf.Sin(Time.unscaledTime * 1.2f + desfase) * amplitud);
    }
}
