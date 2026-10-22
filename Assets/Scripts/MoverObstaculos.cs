
using UnityEngine;

public class MoverObstaculo : MonoBehaviour
{
    public float velocidad = 3f;

    void Update()
    {
        // Movemos el obstaculo hacia abajo
        transform.position += Vector3.down * velocidad * Time.deltaTime;

        // Eliminamos el obstaculo cuando sale de la pantalla
        float limiteInferior = -Camera.main.orthographicSize - 1f;

        if (transform.position.y < limiteInferior)
        {
            Destroy(gameObject);
        }
    }
}
