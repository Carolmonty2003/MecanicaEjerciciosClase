using UnityEngine;
using System.Collections.Generic;

public class OrbitalSimulation : MonoBehaviour
{
    public Transform sun;
    public float GM = 4 * Mathf.PI * Mathf.PI; 
    public float stepTime = 0.001f;
    private float totalTime;

    private Vector2 position;
    private Vector2 velocity;
    private float time;

    private List<Vector3> trajectoryPoints = new List<Vector3>();
    private LineRenderer lineRenderer;

    void Start()
    {
        totalTime = 5 * Mathf.PI; 
        position = new Vector2(1f, 0f); 
        velocity = new Vector2(0f, 2 * Mathf.PI);
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
            trajectoryPoints.Add(new Vector3(position.x, position.y, 0));
            UpdateTrajectory();
        }
    }

    (Vector2, Vector2, float) RK4Step(Vector2 pos, Vector2 vel, float t)
    {
        // a(r) = F/m = -GM * r / r^3
        System.Func<Vector2, Vector2, Vector2> acceleration = (p, v) => {
            Vector2 r = p - (Vector2)sun.position;
            float rMagnitude = r.magnitude;
            return -GM * r / (rMagnitude * rMagnitude * rMagnitude);
        };

        // k1
        Vector2 k1_v = acceleration(pos, vel) * stepTime;
        Vector2 k1_x = vel * stepTime;

        // k2
        Vector2 k2_v = acceleration(pos + 0.5f * k1_x, vel + 0.5f * k1_v) * stepTime;
        Vector2 k2_x = (vel + 0.5f * k1_v) * stepTime;

        // k3
        Vector2 k3_v = acceleration(pos + 0.5f * k2_x, vel + 0.5f * k2_v) * stepTime;
        Vector2 k3_x = (vel + 0.5f * k2_v) * stepTime;

        // k4
        Vector2 k4_v = acceleration(pos + k3_x, vel + k3_v) * stepTime;
        Vector2 k4_x = (vel + k3_v) * stepTime;

        Vector2 newVelocity = vel + (k1_v + 2 * k2_v + 2 * k3_v + k4_v) / 6f;
        Vector2 newPosition = pos + (k1_x + 2 * k2_x + 2 * k3_x + k4_x) / 6f;

        return (newPosition, newVelocity, t + stepTime);
    }

    void UpdateTrajectory()
    {
        lineRenderer.positionCount = trajectoryPoints.Count;
        lineRenderer.SetPositions(trajectoryPoints.ToArray());
    }
}