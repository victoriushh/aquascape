using UnityEngine;
using System.Collections.Generic;
using System.Collections;


[SerializeField]
public class SpawnedObject
{
    public Vector3 position;
    public float radius;
}


public class SpawnManager : MonoBehaviour
{

    public static SpawnManager Instance;

    [Header("Spawn Settings")]
    public SpawnArea spawnArea;
    public int maxTry = 30;

    public float fishSize;
    public float trashSize;

    public List<GameObject> trashPrefabs = new List<GameObject>();
    public List<GameObject> fishPrefabs = new List<GameObject>();

    public List<SpawnedObject> spawnedObjects = new List<SpawnedObject>();

    public float trashSpawnIntervalMin;
    public float trashSpawnIntervalMax;
    void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        trashSpawnIntervalMin = ConfigManager.Instance.config.trashSpawnIntervalMin;
        trashSpawnIntervalMax = ConfigManager.Instance.config.trashSpawnIntervalMax;
        if (trashSpawnIntervalMin <= 0)
        {
            trashSpawnIntervalMin = 5;
        }
        if (trashSpawnIntervalMax <= 0)
        {
            trashSpawnIntervalMax = 15;
        }

        StartCoroutine(SpawnTrashRoutine());
    }

    public void RemoveFish(GameObject fish)
    {
        for (int i = fishPrefabs.Count - 1; i >= 0; i--)
        {
            if(fishPrefabs[i] == fish)
            {
                fishPrefabs.RemoveAt(i);
                Destroy(fish);
            }
        }

        if(fishPrefabs.Count == 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    IEnumerator SpawnTrashRoutine()
    {
        yield return new WaitForSeconds(3);

        Debug.Log("trashPrefabs : " + trashPrefabs.Count);
        while (true)
        {
            if (trashPrefabs.Count == 0)
            {
                yield return null;
                continue;
            }

            float randomDelay = Random.Range(trashSpawnIntervalMin, trashSpawnIntervalMax);

            float finalDelay = Mathf.Max(randomDelay, 2f);

            yield return new WaitForSeconds(finalDelay);

            SpawnTrash();
        }
    }

    void SpawnTrash()
    {
        if (trashPrefabs.Count == 0) return;

        GameObject prefab = trashPrefabs[Random.Range(0, trashPrefabs.Count)];

        Vector3 spawnPos = GetSpawnPosition(0.5f);

        GameObject go = Instantiate(prefab, spawnPos, prefab.transform.rotation);
        go.SetActive(true);
    }

    public void RegisterEntity(ParsedFileData data, Texture2D tex)
    {
        GameObject go = CreatePrefab(data, tex);

        go.SetActive(false);

        if (data.category == EntityCategory.Trash)
        {
            trashPrefabs.Add(go);
            go.AddComponent<TrashBehaviour>();
            go.AddComponent<PolygonCollider2D>().isTrigger = true;
            go.AddComponent<Rigidbody2D>();
        }
        else if (data.category == EntityCategory.Fish)
        {
            fishPrefabs.Add(go);
            go.AddComponent<FishBehaviour>();
            go.AddComponent<PolygonCollider2D>().isTrigger = true;

            Vector3 spawnPos = GetSpawnPosition(1f);
            go.transform.position = spawnPos;
            go.SetActive(true);
        }
    }
    public GameObject CreatePrefab(ParsedFileData data, Texture2D tex)
    {
        GameObject go = new GameObject(data.category + "_" + data.type);

        go.SetActive(false);

        SpriteRenderer spriteRenderer = go.AddComponent<SpriteRenderer>();

        Sprite sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f),
            100f
        );

        spriteRenderer.sprite = sprite;

        float targetSize = GetTargetSize(data.category);
        float spriteSize = sprite.bounds.size.x;
        float scale = targetSize / spriteSize;

        go.transform.localScale = Vector3.one * scale;


        return go;
    }

    float GetTargetSize(EntityCategory category)
    {
        switch (category)
        {
            case EntityCategory.Fish:
                return fishSize;
            case EntityCategory.Trash:
                return trashSize;
            default:
                return 1f;
        }
    }

    public Vector3 GetSpawnPosition(float radius)
    {
        for (int i = 0; i < maxTry; i++)
        {
            Vector3 candidate = spawnArea.GetRandomPosition();

            if (IsPositionValid(candidate, radius))
            {
                spawnedObjects.Add(new SpawnedObject
                {
                    position = candidate,
                    radius = radius
                });

                return candidate;
            }
        }

        return spawnArea.GetRandomPosition();
    }


    private bool IsPositionValid(Vector3 pos, float radius)
    {
        foreach (var obj in spawnedObjects)
        {
            float distance = Vector3.Distance(pos, obj.position);

            if (distance < (radius + obj.radius))
                return false;
        }

        return true;
    }
}

