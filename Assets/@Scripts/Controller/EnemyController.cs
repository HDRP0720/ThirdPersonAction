using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
  // Property
  public StateMachine<EnemyController> StateMachine { get; private set; }
  
  // Caching states for 
  private Dictionary<EEnemyStates, State<EnemyController>> _stateDict;

  private void Start()
  {
    _stateDict = new Dictionary<EEnemyStates, State<EnemyController>>
    {
      [EEnemyStates.Idle] = GetComponent<IdleState>(),
      [EEnemyStates.Chase] = GetComponent<ChaseState>()
    };

    StateMachine = new StateMachine<EnemyController>(this);
    StateMachine.ChangeState(_stateDict[EEnemyStates.Idle]);
  }
  private void Update()
  {
    StateMachine.Execute();
  }

  public void ChangeState(EEnemyStates state)
  {
    StateMachine.ChangeState(_stateDict[state]);
  }
}

public enum EEnemyStates { Idle, Chase }
