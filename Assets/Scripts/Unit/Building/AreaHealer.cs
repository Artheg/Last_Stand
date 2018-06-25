using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class AreaHealer : MonoBehaviour
{
    public float HealRadius = 10f;
    public float HealInterval = 1f;
    public int HealAmount = 1;
    public TeamID Team;

    private SphereCollider sphereCollider;
    private Dictionary<Collider, Unit> unitDict;
    private Dictionary<Collider, Coroutine> coroutineDict;

    void Start()
    {
        sphereCollider = gameObject.AddComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = HealRadius / transform.localScale.x;
        gameObject.AddComponent<Rigidbody>();
        unitDict = new Dictionary<Collider, Unit>();
        coroutineDict = new Dictionary<Collider, Coroutine>();
    }
    private Unit TryGetUnitFromDict(Collider other)
    {
        if (unitDict.ContainsKey(other))
           return unitDict[other];
        return null;
    }

    void OnTriggerEnter(Collider other)
    {
        Unit unit = TryGetUnitFromDict(other);
        if (unit == null)
        {
            unit = other.GetComponent<Unit>();
            if (unit != null && unit.Team == Team)
                unitDict[other] = unit;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!unitDict.ContainsKey(other))
            return;
        Unit unit = unitDict[other];
        if (coroutineDict.ContainsKey(other)) //if coroutine started
            return;
        coroutineDict[other] = StartCoroutine(HealUnit(other));
    }

    void OnTriggerExit(Collider other)
    {
        if (!coroutineDict.ContainsKey(other))
            return;
        StopCoroutine(coroutineDict[other]);
        coroutineDict.Remove(other);
    }

    private IEnumerator HealUnit(Collider unitCollider)
    {
        yield return new WaitForSeconds(HealInterval);
        Unit unit = unitDict[unitCollider];
        unit.ChangeHP(HealAmount);
        coroutineDict.Remove(unitCollider);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, HealRadius);
    }
}
