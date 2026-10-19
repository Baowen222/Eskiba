
using UnityEngine;

public class ColisionJugador : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Obstaculo"))
        {
            Debug.Log("Game Over");
            Time.timeScale = 0f;
        }
    }
}
