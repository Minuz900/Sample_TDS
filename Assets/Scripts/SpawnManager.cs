using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SpawnManager : MonoBehaviour
{
    public GameObject zombiePrefab;
    public Transform spawn;

    [SerializeField]
    Transform objectPoolParent;

    [SerializeField]
    int defaultPoolSize = 50;
    [SerializeField]
    int maxPoolSize = 200;

    [SerializeField]
    float row2Probability = 0.3f;
    [SerializeField]
    float row3Probability = 0.3f;


    [SerializeField] 
    float spawnStartDelay = 3f;
    [SerializeField] 
    float startInterval = 2f;         // 처음 스폰 간격(초)
    [SerializeField] 
    float minInterval = 0.5f;         // 최소 스폰 간격(초)
    [SerializeField] 
    float intervalDecreaseTime = 20f; // 몇 초마다 스폰 간격이 감소하는지
    [SerializeField] 
    float intervalDecreaseAmount = 0.2f; // 한 번에 얼마나 감소할지

    [SerializeField]
    int maxSpawnCount = 100;
    int currentSpawnCount;

    float currentInterval;
    float timeSinceDecrease = 0f;

    IObjectPool<Zombie> m_Pool;
    public IObjectPool<Zombie> Pool
    {
        get
        {
            return m_Pool;
        }
    }

    public void StartSpawn()
    {
        currentInterval = startInterval;
        currentSpawnCount = 0;
        StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        if (spawnStartDelay > 0f)
            yield return new WaitForSeconds(spawnStartDelay);

        while (currentSpawnCount < maxSpawnCount)
        {
            SpawnZombie();
            currentSpawnCount++;

            timeSinceDecrease += currentInterval;
            if (timeSinceDecrease >= intervalDecreaseTime)
            {
                timeSinceDecrease = 0f;
                currentInterval = Mathf.Max(minInterval, currentInterval - intervalDecreaseAmount);
            }

            yield return new WaitForSeconds(currentInterval);
        }
    }

    public void InitPools()
    {
        // 오브젝트 생성, 풀에서 꺼낼 때, 풀로 회수할 때, 풀에서 제거될 때
        // 각각에 대한 이벤트 함수를 등록.
        m_Pool = new ObjectPool<Zombie>(CreateZombie, OnGetZombie, OnReleaseZombie, OnDestroyZombie, false, defaultPoolSize, maxPoolSize);

        for (int i = 0; i < defaultPoolSize; i++)
        {
            var zombie = this.CreateZombie();
            Pool.Release(zombie);
        }
    }

    Zombie CreateZombie()
    {
        GameObject obj = Instantiate(zombiePrefab, new Vector3(10000, 10000, 0), Quaternion.identity, objectPoolParent);
        Zombie zombie = obj.GetComponent<Zombie>();

        return zombie;
    }

    void OnReleaseZombie(Zombie zombie)
    {
        zombie.gameObject.SetActive(false);
    }

    void OnGetZombie(Zombie zombie)
    {
        zombie.gameObject.SetActive(true);
    }

    void OnDestroyZombie(Zombie zombie)
    {
        Destroy(zombie.gameObject);
    }

    void SpawnZombie()
    {
        Vector2 spawnPos = (Vector2)spawn.position;
        Zombie zombie = Pool.Get();

        zombie.transform.position = spawnPos;

        float value = Random.value;
        int layer;
        int layerIndex;
        if (value < row3Probability)
        {
            layer = LayerCache.ZombieRow3;
            layerIndex = 2;
            SetLayerRecursively(zombie.gameObject, layer);
        }
        else if (value < row3Probability + row2Probability)
        {
            layer = LayerCache.ZombieRow2;
            layerIndex = 1;
            SetLayerRecursively(zombie.gameObject, layer);
        }
        else
        {
            layer = LayerCache.ZombieRow1;
            layerIndex = 0;
            SetLayerRecursively(zombie.gameObject, layer);
        }

        zombie.SetZombieLayer(layerIndex);

    }

    public void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (obj == null) return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            if (child.gameObject.name != "Trigger")
                SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
