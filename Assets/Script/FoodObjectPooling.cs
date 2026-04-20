using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObjectPooling : MonoBehaviour
{
    public class SpawnedFood
    {
        public int index;
        public GameObject go;
    }
    public static FoodObjectPooling Instance;

    public GameObject foodPrefab;
    public int maxSpawn = 20;

    private List<SpawnedFood> spawnedFoods = new List<SpawnedFood>();

    void Awake()
    {
        Instance = this;

        for (int i = 0; i < maxSpawn; i++)
        {
            GameObject go = Instantiate(foodPrefab,transform.position, foodPrefab.transform.rotation);
            go.SetActive(false);
            go.GetComponent<Food>().foodIndex = i;
            SpawnedFood spawned = new();
            spawned.index = i;
            spawned.go = go;
            spawnedFoods.Add(spawned);
        }
    }

    public GameObject SpawnFood(Vector3 position)
    {
        for (int i = 0; i < spawnedFoods.Count; i++)
        {
            if (!spawnedFoods[i].go.activeInHierarchy)
            {
                spawnedFoods[i].go.transform.position = position;
                spawnedFoods[i].go.SetActive(true);
                return spawnedFoods[i].go;
            }
        }

        // if food is null=
        GameObject go = Instantiate(foodPrefab, transform.position, foodPrefab.transform.rotation);
        go.SetActive(false);
        go.GetComponent<Food>().foodIndex = spawnedFoods.Count - 1;
        SpawnedFood spawned = new();
        spawned.index = 0;
        spawned.go = go;
        spawnedFoods.Add(spawned);
        return go;
    }

    public void RemoveFood(int index)
    {
        for (int i = 0; i < spawnedFoods.Count; i++)
        {
            if (spawnedFoods[i].index == index)
            {
                spawnedFoods[i].go.SetActive(false);
            }
        }
    }
}
