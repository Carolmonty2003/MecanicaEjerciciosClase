using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    private Vector2 position;
    private Vector2 velocity;
    private Vector2 acceleration;
    private float time;

    public float mass = 1f; 
    public float totalTime = 10f;
    public float stepTime = 0.01f;
    public float theta = 45f; 
    public float gravity = 9.81f;
    public float speed = 10f;
    public float k = 0.1f; 

    private List<Vector3> trajectoryPoints = new List<Vector3>();
    private LineRenderer lineRenderer;

    void Start()
    {
        position = new Vector2(0, 0);
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
            (position, velocity, time) = EulerMethod(position, velocity, time);

            if (position.y < 0) 
            {
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
}
