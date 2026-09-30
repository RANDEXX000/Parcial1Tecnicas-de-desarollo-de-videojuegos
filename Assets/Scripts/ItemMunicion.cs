using UnityEngine;

public class ItemMunicion : MonoBehaviour
{
    public int cantidadBalas = 5;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController jugador = other.GetComponent<PlayerController>();
            if (jugador != null)
            {
                jugador.AgregarBalas(cantidadBalas);
                Destroy(gameObject);
            }
        }
    }
}