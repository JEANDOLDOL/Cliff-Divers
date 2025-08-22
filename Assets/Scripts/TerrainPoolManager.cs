using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;

public class TerrainPoolManager : MonoBehaviour
{
    [Tooltip("Terrain Prefab List")]
    [SerializeField] private List<GameObject> terrainPrefabs;

    [Tooltip("Units per Chunk")]
    [SerializeField] private int initialCountPerPrefab = 2;

    [Tooltip("Max pool Capacity")]
    [SerializeField] private int maxPoolSize = 10;

    private Dictionary<GameObject, ObjectPool<GameObject>> pools = new();

    private void Awake()
    {
        foreach (GameObject prefab in terrainPrefabs)
        {
            ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
                () => CreateNewInstance(prefab),
                OnGetFromPool,
                OnReleaseToPool,
                OnDestroyPooledObject,
                collectionCheck: false,
                defaultCapacity: initialCountPerPrefab,
                maxSize: maxPoolSize
            );

            pools.Add(prefab, pool);
        }
    }

    private GameObject CreateNewInstance(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab);
        obj.name = prefab.name + "_Instance";
        return obj;
    }

    private void OnGetFromPool(GameObject obj)
    {
        obj.SetActive(true);
    }

    private void OnReleaseToPool(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void OnDestroyPooledObject(GameObject obj)
    {
        Destroy(obj);
    }

    public GameObject GetRandomTerrain()
    {
        GameObject randomPrefab = terrainPrefabs[Random.Range(0, terrainPrefabs.Count)];
        return pools[randomPrefab].Get();
    }

    public void ReturnTerrain(GameObject terrainObj)
    {
        foreach (var kvp in pools)
        {
            if (terrainObj.name.Contains(kvp.Key.name))
            {
                kvp.Value.Release(terrainObj);
                return;
            }
        }

        Debug.LogWarning("해당 Terrain 오브젝트의 풀을 찾을 수 없어 Destroy 처리됨: " + terrainObj.name);
        Destroy(terrainObj);
    }
}
