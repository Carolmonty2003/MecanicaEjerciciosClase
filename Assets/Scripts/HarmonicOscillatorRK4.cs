using UnityEngine;
using System.Collections.Generic;

public class HarmonicOscillatorRK4 : MonoBehaviour
{
    public float mass = 1f; 
    public float k = 4f;     
    public float stepTime = 0.001f;  
    public float totalTime = 5 * Mathf.PI;

    public float x0 = 0f;
    public float positonX = 2f;
    public float velocityX = 2f;
    private Vector2 position;
    private Vector2 velocity;
    private float time = 0;

    private List<Vector3> trajectoryPoints = new List<Vector3>();
    private LineRenderer lineRenderer;

    void Start()
    { 
        position = new Vector2(positonX, 0f); 
        velocity = new Vector2(velocityX, 0f);

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
        //a(x) = F/m = -kx/m
        System.Func<Vector2, Vector2, Vector2> acceleration = (p, v) => new Vector2(-k * (p.x - x0) / mass, 0);

        //k1
        Vector2 k1_v = acceleration(pos, vel);
        Vector2 k1_x = vel;

        //k2
        Vector2 k2_v = acceleration(pos + 0.5f * k1_x, vel + 0.5f * k1_v) * stepTime;
        Vector2 k2_x = (vel + 0.5f * k1_v) * stepTime;

        //k3
        Vector2 k3_v = acceleration(pos + 0.5f * k2_x, vel + 0.5f * k2_v) * stepTime;
        Vector2 k3_x = (vel + 0.5f * k2_v) * stepTime;

        //k4
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
