using System;
using UnityEngine;

class EnemyWaveManager : MonoBehaviour
{
    private MinionSpawner[] minionSpawners;

    void Start()
    {
        minionSpawners = GetComponents<MinionSpawner>();
        if (minionSpawners == null || minionSpawners.Length == 0)
            throw new Exception("You must add and setup MinionSpawners in order for EnemyWaveManager to work");

        MainGameManager.Instance.OnCountDownFinish += OnCountDownFinish;
        MainGameManager.Instance.OnWaveBeaten += TryDefineNextWave;

        TryDefineNextWave();
    }

    private void TryDefineNextWave()
    {
        bool allMaxLevelsReached = false;
        for (int i = 0; i < minionSpawners.Length; i++)
        {
            allMaxLevelsReached = minionSpawners[i].IsMaxLevelReached;
            if (!allMaxLevelsReached)
                break;
        }
        if (allMaxLevelsReached)
        {
            MainGameManager.Instance.OnAllWavesBeaten();
            return;
        }
        UpgradeSpawners();
        MainGameManager.Instance.BeginNewRound();
    }

    private void UpgradeSpawners()
    {
        for (int i = 0; i < minionSpawners.Length; i++)
            minionSpawners[i].Upgrade();
    }

    private void OnCountDownFinish()
    {
        Debug.Log("OnCDFINISH");
        for (int i = 0; i < minionSpawners.Length; i++)
        {
            var waveUnitCount = minionSpawners[i].CurrentMinionLevel.MaxUnits;
            for (int j = 0; j < waveUnitCount; j++)
                minionSpawners[i].RequestUnitSpawn();
        }
    }
}
