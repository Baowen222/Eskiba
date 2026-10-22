using UnityEngine;

public class FabricaObstaculos
{
    public GameObject CrearObstaculo(GameObject obstaculo, Vector3 posicion, float velocidad)
    {
        // Creamos el obstaculo y configuramos su movimiento.
        GameObject nuevo = Object.Instantiate(obstaculo, posicion, Quaternion.identity);
        MoverObstaculo movimiento = nuevo.GetComponent<MoverObstaculo>();
        movimiento.velocidad = velocidad;

        return nuevo;
    }
}
