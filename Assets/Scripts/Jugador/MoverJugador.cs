
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MoverJugador : MonoBehaviour
{
    void Update()
    {

        if (Time.timeScale == 0f)
        {
            return;
        }
        // Pulsar un botón de la interfaz no debe desplazar al jugador.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            Vector2 dedo = Touchscreen.current.primaryTouch.position.ReadValue();
            Mover(dedo);
        }
        else if (Mouse.current != null &&
                 Mouse.current.leftButton.isPressed)
        {
            Vector2 raton = Mouse.current.position.ReadValue();
            Mover(raton);
        }
    }

    void Mover(Vector2 posicionPantalla)
    {
        // Pasar la posicion de dedo a coordenadas
        float distancia = -Camera.main.transform.position.z;

        Vector3 posicion = Camera.main.ScreenToWorldPoint(
            new Vector3(posicionPantalla.x, posicionPantalla.y, distancia)
        );

        // Calcular tamaño de pantalla
        float alto = Camera.main.orthographicSize;
        float ancho = alto * Camera.main.aspect;

        // Evitamos que el jugador salga de la pantalla
        float x = Mathf.Clamp(posicion.x, -ancho + 0.4f, ancho - 0.4f);
        float y = Mathf.Clamp(posicion.y, -alto + 0.4f, alto - 0.4f);

        transform.position = new Vector3(x, y, transform.position.z);
    }
}
