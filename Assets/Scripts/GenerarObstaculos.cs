
using UnityEngine;

public class GenerarObstaculos : MonoBehaviour
{
    public GameObject obstaculo;
    public float tiempoEntreObstaculos = 1.5f;

    float tiempo = 0f;

    void Update()
    {
        tiempo += Time.deltaTime;

        if (tiempo >= tiempoEntreObstaculos)
        {
            CrearObstaculo();
            tiempo = 0f;
        }
    }

    void CrearObstaculo()
    {
       
        float alto = Camera.main.orthographicSize;
        float ancho = alto * Camera.main.aspect;

        // Posicion random en X
        float x = Random.Range(-ancho + 0.4f, ancho - 0.4f);

        // Aparece justo por encima de la pantalla
        Vector3 posicion = new Vector3(x, alto + 1f, 0);

        Instantiate(obstaculo, posicion, Quaternion.identity);
    }
}
