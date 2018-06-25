
using System;
using System.Collections.Generic;
using UnityEngine;

public class MinionManager : MonoBehaviour
{
    public Action OnWaveBeaten;
    public Action<Minion> OnMinionSpawned;

    private Fountain fountain;
    private Couch couch;

    private Dictionary<Minion, List<Minion>> attackers;
    private Dictionary<TeamID, List<Minion>> unitDict;

    private Dictionary<TeamID, Material> materialsByTeam;

    public Unit SpawnMinion(GameObject prefab, TeamID team, Vector3 position)
    {
        return CreateMinion(prefab, team, position);
    }

    public int GetUnitCount(TeamID teamID)
    {
        if (!unitDict.ContainsKey(teamID))
            return 0;
        var list = unitDict[teamID];
        return list.Count;
    }

    public void Init(Couch couch, Fountain fountain)
    {
        this.couch = couch;
        this.fountain = fountain;
    }

    public MinionManager()
    {
        unitDict = new Dictionary<TeamID, List<Minion>>();
        attackers = new Dictionary<Minion, List<Minion>>();
        materialsByTeam = new Dictionary<TeamID, Material>();
    }

    public void AddTeamMaterial(TeamID teamID, Material material)
    {
        materialsByTeam[teamID] = material;
    }

    private Minion CreateMinion(GameObject prefab, TeamID teamID, Vector3 position)
    {
        GameObject gameObj = Instantiate(prefab, position, Quaternion.identity);
        Minion unit = gameObj.GetComponent<Minion>();
        var material = GetMaterial(teamID);

        unit.SetMaterial(material);
        unit.SetTeam(teamID);
        unit.OnUnitDead += OnUnitDead;

        List<Minion> unitList;
        if (!unitDict.ContainsKey(teamID))
        {
            unitList = new List<Minion>();
            unitDict[teamID] = unitList;
        }
        else
        {
            unitList = unitDict[teamID];
        }

        unitList.Add(unit);
        OnMinionSpawned(unit);
        return unit;
    }

    private Material GetMaterial(TeamID teamID)
    {
        if (materialsByTeam.ContainsKey(teamID))
            return materialsByTeam[teamID];
        else
            return null;
    }

    public void SearchObjectForAttack(Unit attacker)
    {
        Unit target;
        foreach (var item in unitDict)
        {
            bool isFriendly = (item.Key == attacker.Team);
            if (isFriendly)
                continue;
            List<Minion> unitList = item.Value;
            target = TryGetClosestUnit(attacker, unitList);
            if (target != null)
            {
                attacker.SetTarget(target);
                break;
            }
        }
        if (!attacker.HasTarget)
            DefineStaticTarget(attacker);
    }

    private void DefineStaticTarget(Unit attacker)
    {
        switch (attacker.Team)
        {
            case TeamID.player:
                attacker.SetTarget(fountain);
                break;
            case TeamID.enemy:
                attacker.SetTarget(couch);
                break;
        }
    }

    private Unit TryGetClosestUnit(Unit attacker, List<Minion> unitList)
    {
        if (attacker.IsDead)
            return null;
        if (unitList.Count == 0)
            return null;
        Unit closestUnit = unitList[0];
        for (int i = 0; i < unitList.Count; i++)
        {
            var unit = unitList[i];
            if (i == 0)
            {
                closestUnit = unit;
                continue;
            }
            var unitDistance = unit.GetDistance(attacker.position);
            var currentClosestDistance = closestUnit.GetDistance(attacker.position);
            if (unitDistance < currentClosestDistance)
                closestUnit = unit;
        }
        return closestUnit;
    }

    public void OnUnitDead(Unit deadUnit)
    {
        var minion = deadUnit as Minion;
        foreach (var item in unitDict) 
        {
            if (item.Key != deadUnit.Team)
                continue;
            List<Minion> unitList = item.Value;
            unitList.Remove(minion);
            Destroy(deadUnit.gameObject);
            break;
        }
        if (unitDict[TeamID.enemy].Count == 0)
            OnWaveBeaten.Invoke();
     }
}