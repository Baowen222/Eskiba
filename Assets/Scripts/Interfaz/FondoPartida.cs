using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FondoPartida : MonoBehaviour
{
    Camera camara;
    SpriteRenderer dibujo;

    void Awake()
    {
        camara = Camera.main;
        dibujo = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // Cubre la cámara sin cambiar sus límites ni las coordenadas del jugador.
        float alto = camara.orthographicSize * 2f;
        Vector2 tamano = dibujo.sprite.bounds.size;
        transform.localScale = new Vector3(alto * camara.aspect / tamano.x,
            alto / tamano.y, 1f);
        transform.position = new Vector3(camara.transform.position.x,
            camara.transform.position.y, 5f);
    }
}
