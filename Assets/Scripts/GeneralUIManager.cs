using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIData
{
    public float currentCash;
    public float currentTurretCost;
    public float currentDifficulty;
    public float currentEnemiesKilled;
}

public class GeneralUIManager : MonoBehaviour
{
    [SerializeField] private TMP_Text CashText;
    [SerializeField] private TMP_Text EnemiesKilledText;
    [SerializeField] private TMP_Text CurrentDifficultyText;
    [SerializeField] private TMP_Text TurretCostText;

    [Space(10)]
    [SerializeField] private float StartCash = 100f; 
    [SerializeField] private float StartDifficulty = 1f;
    [SerializeField] private float StartTurretCost = 15f;

    [SerializeField] private EnemySpawner enemySpawner;

    private float CurrentCash;
    private float CurrentTurretCost;
    private float CurrentDifficulty;
    private float CurrentEnemiesKilled;

    // Start is called before the first frame update

    private void OnEnable()
    {
        Events.EnemyKilled += OnEnemyKilled;
        Events.DifficultyIncrease += OnDifficultyIncreased;
    }

    private void OnDisable()
    {
        Events.EnemyKilled -= OnEnemyKilled;
        Events.DifficultyIncrease -= OnDifficultyIncreased;
    }

    public void Save()
    {
        Events.SaveTowers?.Invoke();
        SaveSpawnerData();
        SaveUIData();

    }

    private void SaveSpawnerData()
    {
        SpawnerData spDataNew = new SpawnerData();

        spDataNew.DifficultySpawnTimeReducer = enemySpawner.DifficultySpawnTimeReducer;
        spDataNew.DifficultySpeedIncreaseAmount = enemySpawner.DifficultySpeedIncreaseAmount;
        spDataNew.enemyStartSpeed = enemySpawner.enemyStartSpeed;
        spDataNew.maxSpawnTime = enemySpawner.maxSpawnTime;
        spDataNew.minSpawnTime = enemySpawner.minSpawnTime;
        spDataNew.spawnIn = enemySpawner.spawnIn;
        spDataNew.startDifficulty = enemySpawner.startDifficulty;
        spDataNew.timeBetweenDifficultyUpgrades = enemySpawner.timeBetweenDifficultyUpgrades;
        spDataNew.timePassed = enemySpawner.timePassed;
        spDataNew.Waypoints = enemySpawner.Waypoints;

        string json = JsonUtility.ToJson(spDataNew);
        PlayerPrefs.SetString("SpawnerData", json);
    }

    private void SaveUIData() 
    {
        UIData uIData = new UIData();
        uIData.currentCash = CurrentCash;
        uIData.currentDifficulty = CurrentDifficulty;
        uIData.currentTurretCost = CurrentTurretCost;
        uIData.currentEnemiesKilled = CurrentEnemiesKilled;

        string json = JsonUtility.ToJson(uIData);
        PlayerPrefs.SetString("UIData", json);
    }

    private void OnEnemyKilled()
    {
        CurrentEnemiesKilled++;
        CurrentCash += Random.Range(1, 3);

        RefreshUI();
    }

    private void OnDifficultyIncreased()
    {
        CurrentDifficulty++;
        CurrentCash += 4;
        Save();
        RefreshUI();
    }

    public void BuyTurret()
    {
        if (CurrentTurretCost > CurrentCash) return;

        CurrentCash -= CurrentTurretCost;
        CurrentTurretCost += 5;
        Events.BoughtTurret?.Invoke();

        RefreshUI();   
    }

    void Start()
    {
        string dataStr = PlayerPrefs.GetString("UIData");
        UIData ths = JsonUtility.FromJson<UIData>(dataStr);
        if (ths != null) 
        {
            CurrentCash = ths.currentCash;
            CurrentDifficulty = ths.currentDifficulty;
            CurrentEnemiesKilled = ths.currentEnemiesKilled;
            CurrentTurretCost = ths.currentTurretCost;
        }
        else 
        {
            CurrentCash = StartCash;
            CurrentTurretCost = StartTurretCost;
            CurrentDifficulty = StartDifficulty;
            CurrentEnemiesKilled = 0;
        }
        RefreshUI();
    }

    private void RefreshUI()
    {
        CashText.text = CurrentCash.ToString();
        EnemiesKilledText.text = "Enemies Killed: " + CurrentEnemiesKilled.ToString();
        CurrentDifficultyText.text = "Current Difficulty: " + CurrentDifficulty.ToString();
        TurretCostText.text = CurrentTurretCost.ToString() + "$";
    }

    
}
