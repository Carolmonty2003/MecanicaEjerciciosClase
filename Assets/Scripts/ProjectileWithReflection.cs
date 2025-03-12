using UnityEngine;
using System.Collections.Generic;
using UnityEngine.PlayerLoop;

public class ProjectileWithReflection : MonoBehaviour
{
    private Vector2 position;
    private Vector2 oldPosition;
    private Vector2 velocity;
    private Vector2 acceleration;
    private Vector2 dPosition;
    private Vector2 dVelocity;
    private float time;

    public float mass = 1f;
    public float totalTime = 10f;
    public float stepTime = 0.01f;
    public float theta = 45f;
    public float gravity = 9.81f;
    public float speed = 10f;
    public float k = 0.1f;
    public float e = 0.5f;
    public float d = 0;
    public Vector2 normal = new Vector2(0, 1);

    private List<Vector3> trajectoryPoints = new List<Vector3>();
    private LineRenderer lineRenderer;

    void Start()
    {
        position = new Vector2(0, 0);
        oldPosition = new Vector2(0, 0);
        velocity = new Vector2(speed * Mathf.Cos(theta * Mathf.PI / 180), speed * Mathf.Sin(theta * Mathf.PI / 180));
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

            if ((Vector2.Dot(normal, position) + d) * (Vector2.Dot(normal, oldPosition) + d) < 0)
            {
                HandleCollision();
                return;
            }
            else
            {
                transform.position = position;
                trajectoryPoints.Add(new Vector3(position.x, position.y, 0));
                UpdateTrajectory();
            }
        }
    }

    (Vector2, Vector2, float) EulerMethod(Vector2 position, Vector2 velocity, float time)
    {
        Vector2 forceGravity = new Vector2(0, -mass * gravity);
        Vector2 forceDrag = -k * velocity;

        acceleration = (forceGravity + forceDrag) / mass;

        Vector2 newVelocity = velocity + acceleration * stepTime;
        Vector2 newPosition = position + newVelocity * stepTime;

        time += stepTime;

        return (newPosition, newVelocity, time);
    }

    void UpdateTrajectory()
    {
        lineRenderer.positionCount = trajectoryPoints.Count;
        lineRenderer.SetPositions(trajectoryPoints.ToArray());
    }

    void HandleCollision()
    {
        dPosition = position;
        dVelocity = velocity;

        position = dPosition - (1 + e) * (Vector2.Dot(normal, dPosition) + d) * normal;
        velocity = dVelocity - (1 + e) * Vector2.Dot(normal, dVelocity) * normal;

        position.y = Mathf.Max(position.y, 0);

    }
}
