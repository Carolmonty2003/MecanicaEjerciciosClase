using UnityEngine;
using System.Collections.Generic;

public class ParticleInCube : MonoBehaviour
{
    private Vector3 position;
    private Vector3 oldPosition;
    private Vector3 velocity;
    private Vector3 acceleration;
    private float time;

    public float mass = 1f;
    public float totalTime = 10f;
    public float stepTime = 0.01f;
    public float gravity = 9.81f;
    public float k = 0.1f;
    public float cubeSize = 5f;
    public float d = 0;

    public float[] e = { 0.5f };

    private List<Vector3> trajectoryPoints = new List<Vector3>();
    private LineRenderer lineRenderer;

    void Start()
    {
        position = new Vector3(1, 1, 1);
        velocity = new Vector3(5, 5, 5); 
        time = 0;
        transform.position = position;

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
            oldPosition = position;
            (position, velocity, time) = EulerMethod(position, velocity, time);
            CheckCollisions();

            transform.position = position;
            trajectoryPoints.Add(position);
            UpdateTrajectory();
        }
    }

    (Vector3, Vector3, float) EulerMethod(Vector3 position, Vector3 velocity, float time)
    {
        Vector3 forceGravity = new Vector3(0, -mass * gravity, 0);
        Vector3 forceDrag = -k * velocity;

        acceleration = (forceGravity + forceDrag) / mass;

        Vector3 newVelocity = velocity + acceleration * stepTime;
        Vector3 newPosition = position + newVelocity * stepTime;

        time += stepTime;

        return (newPosition, newVelocity, time);
    }

    void CheckCollisions()
    {
        var walls = new List<(Vector3 normal, float limit, int epsilonIndex)>()
        {
            (Vector3.left, 0, 0),    // x-
            (Vector3.right, cubeSize, 1),  // x+
            (Vector3.down, 0, 2),       // y+
            (Vector3.up, cubeSize, 3),  // y-
            (Vector3.back, 0, 4),   // z-
            (Vector3.forward, cubeSize, 5)   // z+
        };

        foreach (var wall in walls)
        {
            float dotOld = Vector3.Dot(wall.normal, oldPosition) - wall.limit;
            float dotNew = Vector3.Dot(wall.normal, position) - wall.limit;

            if (dotOld * dotNew < 0)
            {
                HandleCollision(wall.normal, e[wall.epsilonIndex]);
                ClampPosition(wall.normal, wall.limit);
            }
        }
    }

    void HandleCollision(Vector3 normal, float e)
    {
        velocity = velocity - (1 + e) * Vector3.Dot(normal, velocity) * normal;
    }

    void ClampPosition(Vector3 normal, float limit)
    {
        position = position - normal * (Vector3.Dot(normal, position) - limit);
    }

    void UpdateTrajectory()
    {
        lineRenderer.positionCount = trajectoryPoints.Count;
        lineRenderer.SetPositions(trajectoryPoints.ToArray());
    }
}