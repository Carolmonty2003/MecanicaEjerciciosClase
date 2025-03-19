using UnityEngine;

public class EnergyVerification : MonoBehaviour
{
    public float mass = 1f;      //kg
    public float gravity = 9.81f; //m/s*s
    private float initialY;      //m
    private float initialU;      // Ep inicial (J)
    private float totalWork;     // Trabajo acumulado (J)
    private Rigidbody rb;        

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = mass; 
        initialY = transform.position.y;
        initialU = mass * gravity * initialY;
        totalWork = 0f;
    }

    void FixedUpdate()
    {
        rb.AddForce(new Vector3(0, -gravity * mass, 0), ForceMode.Force);

        float currentY = transform.position.y;
        float currentU = mass * gravity * currentY;
        float deltaU = currentU - initialU;

        float deltaY = currentY - initialY;
        float work = -mass * gravity * deltaY; // W = -F * dy
        totalWork += work;

        initialY = currentY;
        initialU = currentU;
    }
}