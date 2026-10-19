
using UnityEngine;

public class MoverObstaculo : MonoBehaviour
{
    public float velocidad = 3f;

    void Update()
    {
        // El obstaculo baja constantemente
        transform.position += Vector3.down * velocidad * Time.deltaTime;

        // Si sale por debajo de la pantalla se elimina
        float limiteInferior = -Camera.main.orthographicSize - 1f;

        if (transform.position.y < limiteInferior)
        {
            Destroy(gameObject);
        }
    }
}
