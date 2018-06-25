using System;
using UnityEngine;

public class MinionAI : MonoBehaviour
{
    public Action<Minion> OnTargetRequest;

    private bool isManualModeOn;
    private MinionAIState currentState;
    private Minion minion;

    private bool targetWasUpdated;

    void Start()
    {
        minion = GetComponent<Minion>();
        if (minion == null)
            throw new Exception("gameObject must contain Unit component");
        minion.OnDestinationReached += OnDestinationReached;
        minion.OnForceDestinationSet += OnForceDestinationSet;
        minion.OnTargetUpdated += OnTargetUpdated;
        currentState = MinionAIState.idle;
    }

    private void OnForceDestinationSet()
    {
        isManualModeOn = true;
    }

    private void OnDestinationReached()
    {
        isManualModeOn = false;
    }

    private void OnTargetUpdated()
    {
        targetWasUpdated = true;
    }

    void Update()
    {
        if (isManualModeOn)
            return;
        if (minion.CurrentTarget != null && minion.CurrentTarget.IsDead)
            minion.SetTarget(null);

        switch (currentState)
        {
            case MinionAIState.move:
                OnStateMove();
                break;
            case MinionAIState.attack:
                OnStateAttack();
                break;
            case MinionAIState.idle:
                OnStateIdle();
                break;
            default:
                currentState = MinionAIState.idle;
                break;
        }
    }

    private void OnStateIdle()
    {
        if (minion.CurrentTarget == null || minion.CurrentTarget.IsFrendly(minion))
            SearchTarget();
        if (targetWasUpdated)
            SwitchState(MinionAIState.move);
    }

    private void OnStateAttack()
    {
        if (minion.CurrentTarget == null)
            SwitchState(MinionAIState.idle);
        else if (minion.CurrentTargetPosition != minion.CurrentTarget.position)
            SwitchState(MinionAIState.move);
        else
            minion.AttackTarget();
    }

    private void OnStateMove()
    {
        if (minion.CurrentTarget == null)
        {
            SwitchState(MinionAIState.idle);
            return;
        }
        if (targetWasUpdated || (minion.CurrentTarget.position != minion.CurrentTargetPosition))
        {
            minion.SetDestination(minion.CurrentTarget.position);
            targetWasUpdated = false;
        }
        if (!minion.IsMoving)
        {
            if (minion.CurrentTarget == null || minion.CurrentTarget.IsFrendly(minion))
                SwitchState(MinionAIState.idle);
            else
                SwitchState(MinionAIState.attack);
        }
    }

    private void SwitchState(MinionAIState state)
    {
        currentState = state;
    }

    private void SearchTarget()
    {
        if (OnTargetRequest != null)
            OnTargetRequest(minion);
    }
}
