using UnityEngine;
using System.Collections.Generic;

public class ParticleInTriangle : MonoBehaviour
{
    private Vector3 position;
    private Vector3 oldPosition;
    private Vector3 velocity;
    private Vector3 acceleration;
    private float time;

    public float totalTime = 10f;
    public float stepTime = 0.01f;
    public float epsilon = 1f; 

    private List<Vector3> trajectoryPoints = new List<Vector3>();
    private LineRenderer lineRenderer;

    private List<(Vector3 normal, Vector3 point)> edges = new List<(Vector3, Vector3)>();

    void Start()
    {
        position = new Vector3(1f, 1f, 0f);
        velocity = new Vector3(3f, 4f, 0f);
        time = 0;
        transform.position = position;

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 0;
        lineRenderer.useWorldSpace = true;

        float sqrt5 = Mathf.Sqrt(5f);

        edges.Add((new Vector3(0, 1, 0), new Vector3(0, 0, 0)));

        Vector3 bcNormal = new Vector3(-2f / sqrt5, -1f / sqrt5, 0);
        edges.Add((bcNormal, new Vector3(5f, 0f, 0f)));

        Vector3 caNormal = new Vector3(2f / sqrt5, -1f / sqrt5, 0);
        edges.Add((caNormal, new Vector3(2.5f, 5f, 0f)));
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
        Vector3 newPosition = position + velocity * stepTime;
        Vector3 newVelocity = velocity;
        time += stepTime;

        return (newPosition, newVelocity, time);
    }

    void CheckCollisions()
    {
        foreach (var edge in edges)
        {
            Vector3 normal = edge.normal;
            Vector3 edgePoint = edge.point;

            float distanceOld = Vector3.Dot(normal, oldPosition - edgePoint);
            float distanceNew = Vector3.Dot(normal, position - edgePoint);

            if (distanceOld >= 0 && distanceNew < 0)
            {
                HandleCollision(normal, epsilon);
                ClampPosition(normal, distanceNew);
            }
        }
    }

    void HandleCollision(Vector3 normal, float e)
    {
        velocity = velocity - (1 + e) * Vector3.Dot(normal, velocity) * normal;
    }

    void ClampPosition(Vector3 normal, float distance)
    {
        position -= normal * distance;
    }

    void UpdateTrajectory()
    {
        lineRenderer.positionCount = trajectoryPoints.Count;
        lineRenderer.SetPositions(trajectoryPoints.ToArray());
    }
}