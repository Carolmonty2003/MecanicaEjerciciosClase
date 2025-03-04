using UnityEngine;
using System.Collections.Generic;

public class MassSpringSystemVerlet : MonoBehaviour
{
    public float mass = 1f;
    public float k = 20f;
    public float x0 = 0f;
    public float b = 0f;
    public float stepTime = 0.001f;
    public float totalTime = 5f;

    private float position;
    private float previousPosition; 
    private float time;

    private List<Vector3> trajectoryPoints = new List<Vector3>();
    private LineRenderer lineRenderer;

    void Start()
    {
        position = 0.5f; 
        previousPosition = position; 
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
            (position, previousPosition, time) = VerletStep(position, previousPosition, time);

            transform.position = new Vector3(position, 0, 0);
            trajectoryPoints.Add(new Vector3(position, 0, 0));
            UpdateTrajectory();
        }
    }

    (float, float, float) VerletStep(float pos, float prevPos, float t)
    {
        // v aproximada
        float velocity = (pos - prevPos) / stepTime;

        // a con amortiguamiento
        float acceleration = (-k * (pos - x0) - b * velocity) / mass;

        // new pos
        float newPosition = 2 * pos - prevPos + acceleration * stepTime * stepTime;

        // actualiza pos anterior
        float newPreviousPosition = pos;

        return (newPosition, newPreviousPosition, t + stepTime);
    }

    void UpdateTrajectory()
    {
        lineRenderer.positionCount = trajectoryPoints.Count;
        lineRenderer.SetPositions(trajectoryPoints.ToArray());
    }
}