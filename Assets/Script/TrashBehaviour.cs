using UnityEngine;

public class TrashBehaviour : MonoBehaviour
{
    public float minSpeed = 0.5f;
    public float maxSpeed = 1.2f;

    public float waveHeight = 0.2f;
    public float waveSpeed = 2f;

    public float rotationAmount = 10f;

    private float speed;
    private Vector3 direction;
    private float baseY;
    private float timeOffset;

    void Start()
    {
        speed = Random.Range(minSpeed, maxSpeed);

        float random = Random.value;

        if (random > 0.5f)
        {
            direction = Vector3.right;
        }
        else
        {
            direction = Vector3.left;
        }

        baseY = transform.position.y;
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        MoveHorizontal();
        WaveObject();
        RotateObject();
    }

    void MoveHorizontal()
    {
        Vector3 pos = transform.position;
        pos += direction * speed * Time.deltaTime;

        Vector2 min = SpawnManager.Instance.spawnArea.minBounds;
        Vector2 max = SpawnManager.Instance.spawnArea.maxBounds;

        if (pos.x <= min.x)
        {
            pos.x = min.x;         
            direction = Vector3.right;
        }
        else if (pos.x >= max.x)
        {
            pos.x = max.x;
            direction = Vector3.left;
        }

        transform.position = pos;
    }

    void WaveObject()
    {
        float wave = Mathf.Sin(Time.time * waveSpeed + timeOffset) * waveHeight;

        Vector3 pos = transform.position;
        pos.y = baseY + wave;

        transform.position = pos;
    }

    void RotateObject()
    {
        float rot = Mathf.Sin(Time.time * waveSpeed + timeOffset) * rotationAmount;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }
}