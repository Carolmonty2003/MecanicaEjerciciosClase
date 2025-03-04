using UnityEngine;
using System.Collections.Generic;

public class PendulumSimulationWithDamping : MonoBehaviour
{
    public float length_l = 2f;
    public float mass_m = 1f;
    public float gravity_g = 9.81f; 
    public float initialAngle_O = 45f; 
    public float stepTime_t = 0.01f;
    public float dampingCoefficient_b = 0.1f; 
    public bool useVerlet = true; 
    public bool resistenceEnergyDecay = false; 

    private float theta; 
    private float omega; 
    private float time;

    private List<Vector3> trajectoryPoints = new List<Vector3>();
    private List<float> energyPoints = new List<float>();
    private LineRenderer lineRenderer;

    void Start()
    {
        theta = initialAngle_O * Mathf.Deg2Rad; 
        omega = 0f; 
        time = 0;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        if (time < 10f) 
        {
            if (useVerlet)
            {
                (theta, omega, time) = VerletStep(theta, omega, time);
            }
            else
            {
                (theta, omega, time) = EulerStep(theta, omega, time);
            }

            Vector3 position = new Vector3(length_l * Mathf.Sin(theta), -length_l * Mathf.Cos(theta), 0);
            transform.position = position;
            trajectoryPoints.Add(position);
            UpdateTrajectory();

            float energy = CalculateEnergy(theta, omega);
            energyPoints.Add(energy);
            PlotEnergy();
        }
    }

    (float, float, float) EulerStep(float theta, float omega, float t)
    {
        float alpha = -(gravity_g / length_l) * Mathf.Sin(theta) - (dampingCoefficient_b / (mass_m * length_l * length_l)) * omega; 
        float newOmega = omega + alpha * stepTime_t;
        float newTheta = theta + newOmega * stepTime_t;

        return (newTheta, newOmega, t + stepTime_t);
    }

    (float, float, float) VerletStep(float theta, float omega, float t)
    {
        float alpha = -(gravity_g / length_l) * Mathf.Sin(theta) - (dampingCoefficient_b / (mass_m * length_l * length_l)) * omega; 
        float newTheta = theta + omega * stepTime_t + 0.5f * alpha * stepTime_t * stepTime_t;
        float newAlpha = -(gravity_g / length_l) * Mathf.Sin(newTheta) - (dampingCoefficient_b / (mass_m * length_l * length_l)) * omega; 
        float newOmega = omega + 0.5f * (alpha + newAlpha) * stepTime_t;

        return (newTheta, newOmega, t + stepTime_t);
    }

    float CalculateEnergy(float theta, float omega)
    {
        // E potencial: U = m * g * h = m * g * l * (1 - cos(theta))
        float potentialEnergy = mass_m * gravity_g * length_l * (1 - Mathf.Cos(theta));

        // E cinética: K = 0.5 * I * omega^2 = 0.5 * m * l^2 * omega^2
        float kineticEnergy = 0.5f * mass_m * length_l * length_l * omega * omega;

        // E total
        return kineticEnergy + potentialEnergy;
    }

    void UpdateTrajectory()
    {
        lineRenderer.positionCount = trajectoryPoints.Count;
        lineRenderer.SetPositions(trajectoryPoints.ToArray());
    }

    void PlotEnergy()
    {
        Debug.Log("Energía total: " + energyPoints[energyPoints.Count - 1]);
    }
}