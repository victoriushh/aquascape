using UnityEngine;

public class Food : MonoBehaviour
{
    public int foodIndex;
    public float fallSpeed = 0.7f;
    public float waveAmplitude = 0.5f;
    public float waveFrequency = 2f;

    private float timeOffset;

    public float lifetime = 10f;
    float baseX;
    bool isGrounded;

    float timer = 10f;

    public GameObject takenBy;

    private void Start()
    {

        lifetime = ConfigManager.Instance.config.foodLifetime;
        if(lifetime <= 0)
        {
            lifetime = 10;
        }
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            FoodObjectPooling.Instance.RemoveFood(foodIndex);
        }
        Movement();
    }

    private void OnEnable()
    {
        timer = 10f;
        baseX = transform.position.x;
        isGrounded = false;
    }

    void Movement()
    {
        if (!isGrounded)
        {
            Vector3 pos = transform.position;
            pos.y -= fallSpeed * Time.deltaTime;

            float bottom = SpawnManager.Instance.spawnArea.minBounds.y;

            if (pos.y <= bottom)
            {
                pos.y = bottom;
                isGrounded = true;
            }

            transform.position = pos;

            float baseAngle = 90f;
            float amplitude = 10f;
            float speed = 2f;

            float angle = baseAngle + Mathf.Sin((Time.time + timeOffset) * speed) * amplitude;

            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

    }
}