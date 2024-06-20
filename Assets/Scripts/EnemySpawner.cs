using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class EnemySpawner : MonoBehaviour
{
    [Serializable]
    public struct EnemySpawnConfig
    {
        public int frequency;
        public GameObject enemyToSpawn;
        public Enemy.EnemyType enemyType;
    }

    public List<EnemySpawnConfig> spawnConfigs;
    private int totalFrequency = 0;
    [SerializeField] private float timeBetweenSpawns = 1f;
    [SerializeField] private float baseMovementSpeed = 0.5f;
    private float currentTimer = 0f;
    [SerializeField] private Transform FinishLine;
    [SerializeField] private Canon playerCanon;

    public UnityEvent OnCubeHealingTrigger = new UnityEvent();
    public UnityEvent OnWoundedHealingTrigger = new UnityEvent();
    public UnityEvent OnSpeedingUpTrigger = new UnityEvent();
    public UnityEvent OnSlowingDownTrigger = new UnityEvent();

    private List<Enemy> enemiesSpawned = new List<Enemy>();

    [Header("Prefabs")]
    [SerializeField] private GameObject SmallCubePrefab;
    [SerializeField] private GameObject BigCubePrefab;
    [SerializeField] private GameObject SmallBallPrefab;
    [SerializeField] private GameObject BigBallPrefab;

    private List<GameObject> smallCubes = new List<GameObject>();
    private List<GameObject> bigCubes = new List<GameObject>();
    private List<GameObject> smallBalls = new List<GameObject>();
    private List<GameObject> bigBalls = new List<GameObject>();

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        HandleSpawnTimer();
    }

    public void Reset()
    {
        OnCubeHealingTrigger.RemoveAllListeners();
        OnWoundedHealingTrigger.RemoveAllListeners();
        OnSpeedingUpTrigger.RemoveAllListeners();
        OnSlowingDownTrigger.RemoveAllListeners();
    }

    private void HandleSpawnTimer()
    {
        currentTimer += Time.deltaTime;
        if (currentTimer > timeBetweenSpawns)
        {
            currentTimer -= timeBetweenSpawns;
            SpawnRandomEnemy();
        }
    }

    private void Init()
    {
        totalFrequency = 0;
        foreach (EnemySpawnConfig config in spawnConfigs)
        {
            totalFrequency += config.frequency;
        }
    }

    private void SpawnRandomEnemy()
    {
        int randomNumber = UnityEngine.Random.Range(0, totalFrequency);
        int total = 0;
        foreach (EnemySpawnConfig config in spawnConfigs)
        {
            if (randomNumber < config.frequency + total)
            {
                GameObject enemyObject = SpawnEnemy(config.enemyType);
                enemyObject.transform.position = GetRandomPosition();
                enemyObject.transform.rotation = Quaternion.identity;
                Enemy enemy = enemyObject.GetComponent<Enemy>();
                enemiesSpawned.Add(enemy);
                enemy.Init(FinishLine.position.z, this, baseMovementSpeed);
                enemy.OnReachingPlayer.AddListener(() => playerCanon.TakeDamage(enemy.GetDamageValue()));
                SetupEffect(enemy, config.enemyType);
                return;
            }
            total += config.frequency;
        }
    }

    private GameObject SpawnEnemy(Enemy.EnemyType type)
    {
        switch (type)
        {
            case Enemy.EnemyType.SMALL_CUBE: return GetSmallCube();
            case Enemy.EnemyType.BIG_CUBE: return GetBigCube();
            case Enemy.EnemyType.SMALL_BALL: return GetSmallBall();
            case Enemy.EnemyType.BIG_BALL: return GetBigBall();
            default: return null;
        }
    }

    private GameObject GetSmallCube()
    {
        GameObject inactiveObject = smallCubes.Find(x => !x.activeSelf);
        if (inactiveObject)
        {
            inactiveObject.SetActive(true);
            return inactiveObject;
        }
        GameObject gameObject = Instantiate(SmallCubePrefab);
        smallCubes.Add(gameObject);
        return gameObject;
    }

    private GameObject GetBigCube()
    {
        GameObject inactiveObject = bigCubes.Find(x => !x.activeSelf);
        if (inactiveObject)
        {
            inactiveObject.SetActive(true);
            return inactiveObject;
        }
        GameObject gameObject = Instantiate(BigCubePrefab);
        bigCubes.Add(gameObject);
        return gameObject;
    }

    private GameObject GetSmallBall()
    {
        GameObject inactiveObject = smallBalls.Find(x => !x.activeSelf);
        if (inactiveObject)
        {
            inactiveObject.SetActive(true);
            return inactiveObject;
        }
        GameObject gameObject = Instantiate(SmallBallPrefab);
        smallBalls.Add(gameObject);
        return gameObject;
    }

    private GameObject GetBigBall()
    {
        GameObject inactiveObject = bigBalls.Find(x => !x.activeSelf);
        if (inactiveObject)
        {
            inactiveObject.SetActive(true);
            return inactiveObject;
        }
        GameObject gameObject = Instantiate(BigBallPrefab);
        bigBalls.Add(gameObject);
        return gameObject;
    }

    private Vector3 GetRandomPosition()
    {
        return transform.position + new Vector3(UnityEngine.Random.Range(-1f, 1f), 0.05f, 0);
    }

    private void SetupEffect(Enemy enemy, Enemy.EnemyType enemyType)
    {
        switch (enemyType)
        {
            case Enemy.EnemyType.SMALL_CUBE:
                {
                    enemy.OnReachingPlayer.AddListener(HealAllCubes);

                    UnityAction HealMaxHPAction = () => enemy.HealMaxHP(0.1f);
                    UnityAction HealWoundedToFullAction = () => enemy.HealWoundedToFull();

                    OnCubeHealingTrigger.AddListener(HealMaxHPAction);
                    OnWoundedHealingTrigger.AddListener(HealWoundedToFullAction);

                    enemy.OnDeath.AddListener(() => OnCubeHealingTrigger.RemoveListener(HealMaxHPAction));
                    enemy.OnDeath.AddListener(() => OnWoundedHealingTrigger.RemoveListener(HealWoundedToFullAction));
                }
                break;
            case Enemy.EnemyType.BIG_CUBE:
                {
                    enemy.OnDeath.AddListener(HealAllWoundedEnemies); 

                    UnityAction HealMaxHPAction = () => enemy.HealMaxHP(0.1f);
                    UnityAction HealWoundedToFullAction = () => enemy.HealWoundedToFull();

                    OnCubeHealingTrigger.AddListener(HealMaxHPAction);
                    OnWoundedHealingTrigger.AddListener(HealWoundedToFullAction);

                    enemy.OnDeath.AddListener(() => OnCubeHealingTrigger.RemoveListener(HealMaxHPAction));
                }
                break;
            case Enemy.EnemyType.SMALL_BALL:
                {
                    enemy.OnBeingHit.AddListener(SpeedUpSmallBalls);

                    UnityAction HealWoundedToFullAction = () => enemy.HealWoundedToFull();
                    UnityAction IncreaseMovementSpeedAction = () => enemy.IncreaseMovementSpeed(0.1f);
                    UnityAction DecreaseMovementSpeedAction = () => enemy.DecreaseMovementSpeed(0.1f);

                    OnSpeedingUpTrigger.AddListener(IncreaseMovementSpeedAction);
                    OnWoundedHealingTrigger.AddListener(HealWoundedToFullAction);
                    OnSlowingDownTrigger.AddListener(DecreaseMovementSpeedAction);

                    enemy.OnDeath.AddListener(() => OnSpeedingUpTrigger.RemoveListener(IncreaseMovementSpeedAction));
                    enemy.OnDeath.AddListener(() => OnWoundedHealingTrigger.RemoveListener(HealWoundedToFullAction));
                    enemy.OnDeath.AddListener(() => OnSlowingDownTrigger.RemoveListener(DecreaseMovementSpeedAction));
                }
                break;
            case Enemy.EnemyType.BIG_BALL:
                {
                    enemy.OnBeingHit.AddListener(SlowDownAllBalls);

                    UnityAction HealWoundedToFullAction = () => enemy.HealWoundedToFull();
                    UnityAction DecreaseMovementSpeed = () => enemy.DecreaseMovementSpeed(0.1f);

                    OnWoundedHealingTrigger.AddListener(HealWoundedToFullAction);
                    OnSlowingDownTrigger.AddListener(DecreaseMovementSpeed);

                    enemy.OnDeath.AddListener(() => OnWoundedHealingTrigger.RemoveListener(HealWoundedToFullAction));
                    enemy.OnDeath.AddListener(() => OnSlowingDownTrigger.RemoveListener(DecreaseMovementSpeed));
                }
                break;
            default:
                break;
        }
    }

    private void HealAllCubes()
    {
        OnCubeHealingTrigger?.Invoke();
    }

    private void HealAllWoundedEnemies()
    {
        OnWoundedHealingTrigger?.Invoke();
    }

    private void SpeedUpSmallBalls()
    {
        OnSpeedingUpTrigger?.Invoke();
    }

    private void SlowDownAllBalls()
    {
        OnSlowingDownTrigger?.Invoke();
    }

}
