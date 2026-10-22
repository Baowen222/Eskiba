using UnityEngine;
using UnityEngine.EventSystems;

public class AnimacionBoton : MonoBehaviour, IPointerDownHandler,
    IPointerUpHandler, IPointerExitHandler
{
    Vector3 escalaInicial;
    bool pulsado;

    void Awake()
    {
        escalaInicial = transform.localScale;
    }

    void Update()
    {
        // Sigue funcionando en los paneles con Time.timeScale a cero.
        Vector3 destino = escalaInicial * (pulsado ? 0.96f : 1f);
        transform.localScale = Vector3.Lerp(transform.localScale, destino,
            1f - Mathf.Exp(-20f * Time.unscaledDeltaTime));
    }

    public void OnPointerDown(PointerEventData datos) { pulsado = true; }
    public void OnPointerUp(PointerEventData datos) { pulsado = false; }
    public void OnPointerExit(PointerEventData datos) { pulsado = false; }

    void OnDisable()
    {
        pulsado = false;
        transform.localScale = escalaInicial;
    }
}
