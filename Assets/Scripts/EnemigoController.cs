using UnityEngine;

public class EnemigoController : MonoBehaviour
{
    [Header("Vida")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    [Header("Movimiento")]
    public float velocidadMovimiento = 3f;
    public float distanciaPersecucion = 5f;

    [Header("Ataque")]
    public float rangoAtaque = 5f;
    public float danoAtaque = 20f;
    public float cadenciaAtaque = 2f;

    private Transform jugador;
    private Rigidbody rb;
    private float tiempoUltimoAtaque = -999f;

    void Start()
    {
        vidaActual = vidaMaxima;
        rb = GetComponent<Rigidbody>();

        GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
        if (jugadorObj != null)
        {
            jugador = jugadorObj.transform;
        }
    }
    void Update()
    {
        if (vidaActual <= 0 || jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaPersecucion)
        {
            MirarAlJugador();

            if (distancia <= rangoAtaque)
            {
                Atacar();
            }
        }
    }
    void FixedUpdate()
    {
        if (vidaActual <= 0 || jugador == null) return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia <= distanciaPersecucion)
        {
            Vector3 direccion = (jugador.position - transform.position).normalized;
            direccion.y = 0f;

            Vector3 nuevaVelocidad = direccion * velocidadMovimiento;
            nuevaVelocidad.y = rb.velocity.y;
            rb.velocity = nuevaVelocidad;
        }
        else
        {
            Vector3 velocidadActual = rb.velocity;
            rb.velocity = new Vector3(0f, velocidadActual.y, 0f);
        }
    }

    void MirarAlJugador()
    {
        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0f;
        if (direccion.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direccion);
        }
    }

    void Atacar()
    {
        if (Time.time >= tiempoUltimoAtaque + cadenciaAtaque)
        {
            tiempoUltimoAtaque = Time.time;

            Ray rayo = new Ray(transform.position, (jugador.position - transform.position).normalized);
            RaycastHit impacto;

            if (Physics.Raycast(rayo, out impacto, rangoAtaque))
            {
                PlayerController jugadorController = impacto.collider.GetComponent<PlayerController>();
                if (jugadorController != null)
                {
                    jugadorController.RecibirDano(danoAtaque);
                }
            }
        }
    }

    public void RecibirDano(float cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0f, vidaMaxima);
    }
}