
using UnityEngine;

public class ColisionJugador : MonoBehaviour
{
    public GameOver gameOver;

    void OnTriggerEnter2D(Collider2D otro)
    {
        if (otro.CompareTag("Obstaculo"))
        {
            gameOver.TerminarPartida();
        }
    }
}
