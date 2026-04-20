using UnityEngine;

public class SpawnArea : MonoBehaviour
{
    public Vector2 size = new Vector2(10, 5);

    public Vector2 minBounds;
    public Vector2 maxBounds;

    public float paddingX = 1f;
    public float paddingTop = 1f;
    void Awake()
    {
        CalculateCameraBounds();
    }

    void CalculateCameraBounds()
    {
        Camera cam = Camera.main;

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, 0));

        minBounds = bottomLeft;
        maxBounds = topRight;
    }

    public Vector3 GetRandomPosition()
    {
        Vector3 center = transform.position;

        float x = Random.Range(-size.x / 2f, size.x / 2f);
        float y = Random.Range(-size.y / 2f, size.y / 2f);

        Vector3 pos = new Vector3(center.x + x, center.y + y, 0f);

        float minX = minBounds.x + paddingX;
        float maxX = maxBounds.x - paddingX;

        float minY = minBounds.y + 0;
        float maxY = maxBounds.y - paddingTop;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        return pos;


    }

    public Vector3 ClampToScreen(Vector3 pos)
    {
        float minX = minBounds.x + paddingX;
        float maxX = maxBounds.x - paddingX;

        float maxY = maxBounds.y - paddingTop;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, 0, maxY);
        pos.z = 0f;

        return pos;
    }

    void OnDrawGizmos()
    {
        if (Camera.main == null) return;

        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, 0));

        Vector3 center = (bottomLeft + topRight) / 2f;
        Vector3 size = topRight - bottomLeft;

        // Outer (camera)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(center, size);

        // Inner (padding area)
        Vector3 paddedCenter = center;
        Vector3 paddedSize = new Vector3(
            size.x - paddingX * 2,
            size.y - (paddingTop + 0),
            0
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(paddedCenter, paddedSize);
    }
}