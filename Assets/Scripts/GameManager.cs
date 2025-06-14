using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public PushEventController[] pushEvents;

    SpawnManager spawnManager;

    [SerializeField] 
    float baseTowerSpeed = 3f;
    [SerializeField] 
    float baseBackgroundSpeed = 1f;
    [SerializeField] 
    float baseBackgroundSecSpeed = 2f;
    [SerializeField] 
    float baseWheelSpeed = 560f;

    [HideInInspector] public float targetTowerSpeed;
    [HideInInspector] public float targetBackgroundSpeed;
    [HideInInspector] public float targetBackgroundSecSpeed;
    [HideInInspector] public float targetWheelSpeed;
    [HideInInspector] public float currentTowerSpeed;
    [HideInInspector] public float currentBackgroundSpeed;
    [HideInInspector] public float currentBackgroundSecSpeed;
    [HideInInspector] public float currentWheelSpeed;

    [SerializeField]
    float speedLerpTime = 0.7f;

    [SerializeField]
    float pushForce = 1f;

    [SerializeField]
    float pushTime = 0.2f;

    [SerializeField]
    int lineCount = 3;

    List<Zombie>[] lineZombies;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        DontDestroyOnLoad(gameObject);

        spawnManager = GetComponent<SpawnManager>();
        spawnManager.InitPools();

        InitSpeed();
        InitLineZombies();

        spawnManager.StartSpawn();
    }

    void Update()
    {
        currentTowerSpeed = Mathf.Lerp(currentTowerSpeed, targetTowerSpeed, Time.deltaTime / speedLerpTime);
        currentBackgroundSpeed = Mathf.Lerp(currentBackgroundSpeed, targetBackgroundSpeed, Time.deltaTime / speedLerpTime);
        currentBackgroundSecSpeed = Mathf.Lerp(currentBackgroundSecSpeed, targetBackgroundSecSpeed, Time.deltaTime / speedLerpTime);
        currentWheelSpeed = Mathf.Lerp(currentWheelSpeed, targetWheelSpeed, Time.deltaTime / speedLerpTime);
    }

    void InitSpeed()
    {
        targetTowerSpeed = currentTowerSpeed = baseTowerSpeed;
        targetBackgroundSpeed = currentBackgroundSpeed = baseBackgroundSpeed;
        targetBackgroundSecSpeed = currentBackgroundSecSpeed = baseBackgroundSecSpeed;
        targetWheelSpeed = currentWheelSpeed = baseWheelSpeed;
    }

    void InitLineZombies()
    {
        lineZombies = new List<Zombie>[lineCount];
        for (int i = 0; i < lineCount; i++)
            lineZombies[i] = new List<Zombie>();
    }

    public void RegisterZombie(Zombie z, int line)
    {
        if (!lineZombies[line].Contains(z))
            lineZombies[line].Add(z);
    }

    public void UnregisterZombie(Zombie z, int line)
    {
        if (lineZombies[line].Contains(z))
            lineZombies[line].Remove(z);
    }

    public void UpdateSpeedByZombieCount(int count)
    {
        if (count == 0)
        {
            targetTowerSpeed = baseTowerSpeed;
            targetBackgroundSpeed = baseBackgroundSpeed;
            targetBackgroundSecSpeed = baseBackgroundSecSpeed;
            targetWheelSpeed = baseWheelSpeed;
        }
        else if (count == 1)
        {
            targetTowerSpeed = baseTowerSpeed * 0.5f;
            targetBackgroundSpeed = baseBackgroundSpeed * 0.5f;
            targetBackgroundSecSpeed = baseBackgroundSecSpeed * 0.5f;
            targetWheelSpeed = baseWheelSpeed * 0.5f;
        }
        else
        {
            targetTowerSpeed = 0f;
            targetBackgroundSpeed = 0f;
            targetBackgroundSecSpeed = 0;
            targetWheelSpeed = 0;
        }
    }

    public List<Zombie> GetZombies(int index)
    {
        return lineZombies[index];
    }

    public void PushLineConnected(int line, float yThreshold = 0.2f)
    {
        var row = lineZombies[line];

        if (row.Count == 0) 
            return;

        // y값이 가장 낮은 좀비 찾기(맨 아랫줄 기준).
        float bottomY = row.Min(z => z.transform.position.y);

        // threshold 이내에 있는 좀비만 같은 줄로 간주.
        List<Zombie> filteredRow = new List<Zombie>();

        foreach (var z in row)
        {
            if (Mathf.Abs(z.transform.position.y - bottomY) < yThreshold)
            { 
                filteredRow.Add(z);
            }
        }

        if (filteredRow.Count == 0) 
            return;

        // x 오름차순 정렬.
        filteredRow.Sort((a, b) => a.transform.position.x.CompareTo(b.transform.position.x));

        // 연속 붙어있는 구간만 오른쪽으로 한 칸씩 밀기.
        float connectGap = 0.45f;
        List<Zombie> connectedGroup = new List<Zombie>();
        connectedGroup.Add(filteredRow[0]);
        for (int i = 1; i < filteredRow.Count; i++)
        {
            float prevX = filteredRow[i - 1].transform.position.x;
            float curX = filteredRow[i].transform.position.x;

            if (Mathf.Abs(curX - prevX) < connectGap)
            {
                connectedGroup.Add(filteredRow[i]);
            }
            else
                break;
        }

        if (connectedGroup.Count == 0)
            return;

        // 그룹 내 좀비들 전부 오른쪽으로 1칸씩 이동.
        foreach (var zombie in connectedGroup)
        {
            zombie.SmoothPushRight(pushForce, pushTime);
        }
    }
}
