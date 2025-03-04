using UnityEngine;
using System.Collections.Generic;

public class ChargedParticleInMagneticField : MonoBehaviour
{
    public float q = 1f; 
    public float B0 = 2f;
    public float mass = 1f; 
    public float v0 = 5f; 
    public float stepTime = 0.01f; 
    private float totalTime;

    private Vector3 position; 
    private Vector3 velocity; 
    private float time;

    private List<Vector3> trajectoryPoints = new List<Vector3>();
    private LineRenderer lineRenderer;

    void Start()
    {
        totalTime = 10f; 
        position = Vector3.zero; 
        velocity = new Vector3(v0, 0, v0); 
        time = 0;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        if (time < totalTime)
        {
            (position, velocity, time) = RK4Step(position, velocity, time);

            transform.position = position;
            trajectoryPoints.Add(position);
            UpdateTrajectory();
        }
    }

    (Vector3, Vector3, float) RK4Step(Vector3 pos, Vector3 vel, float t)
    {
        System.Func<Vector3, Vector3, Vector3> acceleration = (p, v) => {
            Vector3 B = new Vector3(0, 0, B0);
            return (q / mass) * Vector3.Cross(v, B);
        };

        // k1
        Vector3 k1_v = acceleration(pos, vel) * stepTime;
        Vector3 k1_x = vel * stepTime;

        // k2
        Vector3 k2_v = acceleration(pos + 0.5f * k1_x, vel + 0.5f * k1_v) * stepTime;
        Vector3 k2_x = (vel + 0.5f * k1_v) * stepTime;

        // k3
        Vector3 k3_v = acceleration(pos + 0.5f * k2_x, vel + 0.5f * k2_v) * stepTime;
        Vector3 k3_x = (vel + 0.5f * k2_v) * stepTime;

        // k4
        Vector3 k4_v = acceleration(pos + k3_x, vel + k3_v) * stepTime;
        Vector3 k4_x = (vel + k3_v) * stepTime;

        Vector3 newVelocity = vel + (k1_v + 2 * k2_v + 2 * k3_v + k4_v) / 6f;
        Vector3 newPosition = pos + (k1_x + 2 * k2_x + 2 * k3_x + k4_x) / 6f;

        return (newPosition, newVelocity, t + stepTime);
    }

    void UpdateTrajectory()
    {
        lineRenderer.positionCount = trajectoryPoints.Count;
        lineRenderer.SetPositions(trajectoryPoints.ToArray());
    }
}