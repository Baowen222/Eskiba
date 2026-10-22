
using UnityEngine;

public class GenerarObstaculos : MonoBehaviour
{
    public GameObject obstaculo;

    public float tiempoEntreObstaculos = 1.2f;
    public float velocidadInicial = 4f;

    float tiempo = 0f;
    float tiempoJugado = 0f;

    void Update()
    {
        tiempo += Time.deltaTime;
        tiempoJugado += Time.deltaTime;

        // Los obstaculos aparecen cada vez mas rapido
        float intervalo = Mathf.Max(0.4f, tiempoEntreObstaculos - tiempoJugado * 0.01f);

        if (tiempo >= intervalo)
        {
            CrearObstaculos();
            tiempo = 0f;
        }
    }

    void CrearObstaculos()
    {
        float alto = Camera.main.orthographicSize;
        float ancho = alto * Camera.main.aspect;

        // Al principio aparecen 1 o 2 y luego igual 3
        int cantidadMaxima = tiempoJugado < 20f ? 2 : 3;
        int cantidad = Random.Range(1, cantidadMaxima + 1);

        for (int i = 0; i < cantidad; i++)
        {
            float x = Random.Range(-ancho + 0.4f, ancho - 0.4f);
            Vector3 posicion = new Vector3(x, alto + 1f, 0);

            GameObject nuevo = Instantiate(obstaculo, posicion, Quaternion.identity);

            MoverObstaculo movimiento = nuevo.GetComponent<MoverObstaculo>();

            movimiento.velocidad = Mathf.Min(
                8f,
                velocidadInicial + tiempoJugado * 0.05f
            );
        }
    }
}
