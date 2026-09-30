using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadMovimiento = 5f;
    public float sensibilidadMouse = 2f;

    [Header("Salto")]
    public float fuerzaSalto = 5f;
    public float costoEstaminaSalto = 5f;
    public float distanciaChequeoPiso = 1.1f;

    [Header("Vida")]
    public float vidaMaxima = 100f;
    public float vidaActual;

    [Header("Estamina")]
    public float estaminaMaxima = 10f;
    public float estaminaActual;
    public float regeneracionEstaminaPorSegundo = 2f;

    [Header("Cámara y FOV")]
    public Camera camaraJugador;
    public float fovMinimo = 50f;
    public float fovMaximo = 120f;
    public float velocidadCambioFOV = 30f;

    private Rigidbody rb;
    private float rotacionVertical = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        vidaActual = vidaMaxima;
        estaminaActual = estaminaMaxima;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (vidaActual <= 0) return;

        ManejarMouseLook();
        ManejarFOV();
        ManejarEstamina();
        ManejarSalto();
    }

    void FixedUpdate()
    {
        if (vidaActual <= 0) return;

        ManejarMovimiento();
    }

    void ManejarMovimiento()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direccion = (transform.right * horizontal + transform.forward * vertical).normalized;
        Vector3 nuevaVelocidad = direccion * velocidadMovimiento;
        nuevaVelocidad.y = rb.velocity.y;

        rb.velocity = nuevaVelocidad;
    }

    void ManejarMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadMouse;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadMouse;

        transform.Rotate(Vector3.up * mouseX);

        rotacionVertical -= mouseY;
        rotacionVertical = Mathf.Clamp(rotacionVertical, -80f, 80f);
        camaraJugador.transform.localRotation = Quaternion.Euler(rotacionVertical, 0f, 0f);
    }

    void ManejarFOV()
    {
        if (Input.GetKey(KeyCode.T))
        {
            camaraJugador.fieldOfView = Mathf.Clamp(camaraJugador.fieldOfView - velocidadCambioFOV * Time.deltaTime, fovMinimo, fovMaximo);
        }
        if (Input.GetKey(KeyCode.Y))
        {
            camaraJugador.fieldOfView = Mathf.Clamp(camaraJugador.fieldOfView + velocidadCambioFOV * Time.deltaTime, fovMinimo, fovMaximo);
        }
    }

    void ManejarEstamina()
    {
        if (estaminaActual < estaminaMaxima)
        {
            estaminaActual += regeneracionEstaminaPorSegundo * Time.deltaTime;
            estaminaActual = Mathf.Clamp(estaminaActual, 0f, estaminaMaxima);
        }
    }

    bool EstaEnElPiso()
    {
        return Physics.Raycast(transform.position, Vector3.down, distanciaChequeoPiso);
    }

    void ManejarSalto()
    {
        if (Input.GetKeyDown(KeyCode.Space) && EstaEnElPiso() && estaminaActual >= costoEstaminaSalto)
        {
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.Impulse);
            estaminaActual -= costoEstaminaSalto;
        }
    }

    public void RecibirDano(float cantidad)
    {
        vidaActual -= cantidad;
        vidaActual = Mathf.Clamp(vidaActual, 0f, vidaMaxima);
    }
}