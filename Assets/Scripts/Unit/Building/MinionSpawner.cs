using System;
using System.Collections.Generic;
using UnityEngine;

class MinionSpawner : MonoBehaviour
{
    public Transform SpawnPoint;
    public TeamID Team;
    public float SpawnTime = 1f;
    public Action<Unit> OnMinionSpawn;
    public GameObject minionPrefab;
    public List<MinionLevel> MinionLevels = new List<MinionLevel>();

    public MinionLevel CurrentMinionLevel { get; private set; }

    private float timeSinceLastSpawn;
    private int unitRequestCount;
    private int currentLevelIndex = -1;
    private int currentUnitCount;

    void Start()
    {
        if (minionPrefab == null)
            throw new Exception("missing minion prefab");
        if (MinionLevels.Count == 0)
            throw new Exception("Setup minion levels first!");
        Upgrade();
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (IsMaxUnitCountSpawned)
            unitRequestCount = 0;
        if (unitRequestCount > 0)
            SpawnUnit();
    }

    public void Upgrade()
    {
        if (currentLevelIndex + 1 >= MinionLevels.Count)
        {
            Debug.Log("Max level reached!");
            return;
        }
        currentLevelIndex++;
        CurrentMinionLevel = MinionLevels[currentLevelIndex];
    }

    private void SpawnUnit()
    {
        if (timeSinceLastSpawn < SpawnTime)
            return;
        timeSinceLastSpawn = 0.0f;
        unitRequestCount--;

        var unit = MainGameManager.Instance.MinionManager.SpawnMinion(minionPrefab, Team, SpawnPoint.position);
        if (OnMinionSpawn != null)
            OnMinionSpawn(unit);
        SetUnitParams(unit);
        unit.OnUnitDead += OnUnitDead;
        currentUnitCount++;
    }

    private void OnUnitDead(Unit unit)
    {
        currentUnitCount--;
    }

    public bool IsMaxUnitCountSpawned
    {
        get
        {
            return currentUnitCount >= CurrentMinionLevel.MaxUnits;
        }
    }

    public bool IsMaxLevelReached
    {
        get
        {
            return currentLevelIndex >= MinionLevels.Count - 1;
        }
    }

    public void RequestUnitSpawn()
    {
        if (currentUnitCount - unitRequestCount <= CurrentMinionLevel.MaxUnits)
            unitRequestCount++;
    }

    private void SetUnitParams(Unit unit)
    {
        unit.AttackRange = CurrentMinionLevel.AttackRange;
        unit.AttackSpeed = CurrentMinionLevel.AttackSpeed;
        unit.Attack = CurrentMinionLevel.Attack;
        unit.Health = CurrentMinionLevel.Health;
        unit.Armor = CurrentMinionLevel.Armor;
        unit.Gold = CurrentMinionLevel.Gold;
        unit.Speed = CurrentMinionLevel.Speed;
    }
}
