using System;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
{
    [Header("Unit Parameters")]
    public float AttackRange;
    public float AttackSpeed = 1f;
    public int Attack = 1;
    public int Health = 100;
    public float Armor = 0f;
    public int Gold = 1;
    public float Speed = 50f;

    public GameObject SelectionCirclePrefab;
    public GameObject HealthbarPrefab;

    public Vector3 CurrentTargetPosition { get; private set; }
    public Collider UnitCollider { get; private set; }
    public Unit CurrentTarget { get; private set; }
    public bool IsSelected { get; private set; }
    public bool IsMoving { get; private set; }
    public TeamID Team { get; private set; }

    public Action OnDestinationReached;
    public Action OnForceDestinationSet;
    public Action<Unit> OnUnitDead;
    public Action OnTargetUpdated;

    public MeshRenderer meshRenderer;

    private bool isCurrentDestinationForced;
    private float timeSinceLastAttack;
    private int maxHealth;


    protected NavMeshAgent navMeshAgent;
    private GameObject selectionCircle;
    private HealthBar healthBar;

    private int stoppingThreshold = 2;

    void Start()
    {
        Init();
    }

    public void SetTeam(TeamID team)
    {
        Team = team;
    }

    protected virtual void Init()
    {
        InitNavMeshAgent();
        InitHealthBar();
        InitSelectionCircle();

        maxHealth = Health;

        UnitCollider = GetComponent<Collider>();
    }

    protected void InitSelectionCircle()
    {
        selectionCircle = Instantiate(SelectionCirclePrefab);
        selectionCircle.transform.position = transform.position;
        selectionCircle.transform.parent = transform;
        selectionCircle.transform.Rotate(new Vector3(90, 0, 0));
        selectionCircle.transform.localPosition = new Vector3(0, -0.5f, 0);
        selectionCircle.transform.localScale = Vector3.one;
        selectionCircle.SetActive(false);
    }

    protected void InitHealthBar()
    {
        if (HealthbarPrefab == null)
            return;
        var healthBarInstance = Instantiate(HealthbarPrefab);
        healthBarInstance.transform.position = transform.position;
        healthBarInstance.transform.parent = transform;
        healthBar = healthBarInstance.GetComponentInChildren<HealthBar>();
    }

    protected void InitNavMeshAgent()
    {
        if (Speed <= 0)
            return;
        navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        navMeshAgent.isStopped = true;
        navMeshAgent.speed = Speed;
        navMeshAgent.acceleration = 1000f;
        navMeshAgent.radius = 0.5f;
        navMeshAgent.autoBraking = true;
        navMeshAgent.autoRepath = true;
        navMeshAgent.avoidancePriority = 100;
    }

    void Update()
    {
        if (IsMoving)
            OnMove();
        timeSinceLastAttack += Time.deltaTime;
    }

    private void OnMove()
    {
        if (!navMeshAgent.isStopped && !navMeshAgent.pathPending && navMeshAgent.remainingDistance <= stoppingThreshold)
            OnMoveDone();
    }

    private void OnTargetDead()
    {
        timeSinceLastAttack = 0f;
        CurrentTarget = null;
    }

    private void OnMoveDone()
    {
        navMeshAgent.isStopped = true;
        IsMoving = false;
        if (OnDestinationReached != null)
            OnDestinationReached();
        isCurrentDestinationForced = false;
    }

    public void SetDestination(Vector3 destination, bool forced = false)
    {
        if (Speed <= 0f)
            return;
        if (navMeshAgent == null)
            InitNavMeshAgent();
        if (navMeshAgent.destination == destination)
            return;
        if (forced && OnForceDestinationSet != null)
            OnForceDestinationSet();
        isCurrentDestinationForced = forced;

        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        navMeshAgent.isStopped = false;

        CurrentTargetPosition = destination;

        if (CurrentTarget != null && CurrentTarget.IsFrendly(this))
            navMeshAgent.stoppingDistance = 0;
        else
            navMeshAgent.stoppingDistance = AttackRange + stoppingThreshold;

        navMeshAgent.SetDestination(destination);
        IsMoving = true;
    }

    public void SetMaterial(Material material)
    {
        meshRenderer.material = material;
    }

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
    }

    public void AttackTarget()
    {
        if (AttackSpeed > 0 && timeSinceLastAttack < 1.0f / AttackSpeed)
            return;
        CurrentTarget.TakeDamageFrom(this);
        timeSinceLastAttack = 0.0f;
        if (CurrentTarget.IsDead)
            CurrentTarget = null;
    }

    public void TakeDamageFrom(Unit attacker)
    {
        var damageDealt = attacker.Attack * Armor;
        ChangeHP(-attacker.Attack);
    }

    public void ChangeHP(int delta)
    {
        if (Health + delta > maxHealth)
            Health = maxHealth;
        else
            Health += delta;
        if (Health <= 0)
            Die();
        else if (healthBar != null)
            healthBar.UpdateHealth((Mathf.Min(1.0f, (float)Health / maxHealth)));
    }
    
    public bool IsFrendly(Unit unit)
    {
        return Team == unit.Team;
    }

    protected virtual void Die()
    {
        if (OnUnitDead != null)
            OnUnitDead(this);
    }

    public float GetDistance(Vector3 targetPosition)
    {
        return Vector3.Distance(targetPosition, transform.position);
    }

    public virtual bool IsDead
    {
        get
        {
            return Health <= 0;
        }
    }

    public bool HasTarget
    {
        get
        {
            return CurrentTarget != null;
        }
    }

    public void SetTarget(Unit target)
    {
        if (target == CurrentTarget)
            return;
        CurrentTarget = target;
        if (OnTargetUpdated != null)
            OnTargetUpdated();
    }

    public void SetSelection(bool isSelected)
    {
        if (IsSelected == isSelected)
            return;
        IsSelected = isSelected;
        selectionCircle.SetActive(IsSelected);
    }
}
