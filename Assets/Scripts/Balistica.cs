using UnityEngine;
public class Balistica : MonoBehaviour
{
    private Vector2 position;
    private Vector2 velocity;
    private Vector2 acceleration;
    private float time;

    public float totalTime = 10f;
    public float stepTime = 0.01f;
    public float theta = 30f;
    private float gravity = 9.8f;
    public float speed = 10f;

    void Start()
    {
        position = new Vector2(0, 0);
        velocity = new Vector2(speed * Mathf.Cos(theta * Mathf.PI / 180), speed * Mathf.Sin(theta * Mathf.PI / 180));
        acceleration = new Vector2(0, -gravity);
        time = 0;
        transform.position = position;
    }

    void Update()
    {
        if (time < totalTime)
        {
            (position, velocity, time) = EulearMethod(position, velocity, time);

            if (position.y < 0)
            {
                return;
            }
            else
            {
                transform.position = position;
            }
        }
        else { return; }
    }

    (Vector2, Vector2, float) EulearMethod(Vector2 position, Vector2 velocity, float time)
    {
        Vector2 newPosition = position + velocity * stepTime;
        Vector2 newVelocity = velocity + acceleration * stepTime;

        time += stepTime;

        return (newPosition, newVelocity, time);
    }
}
