using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
  // Auto-Property
  public NavMeshAgent NavAgent { get; private set; }
  public Animator Animator { get; private set; }
  public StateMachine<EnemyController> StateMachine { get; private set; }
  public List<MeeleCombat> TargetsInRange { get; set; } = new List<MeeleCombat>();
  
  [field: SerializeField] public MeeleCombat Target { get; set; }
  [field: SerializeField] public float FOV { get; set; } = 180f;
  
  // Caching states for ~~~
  private Dictionary<EEnemyStates, State<EnemyController>> _stateDict;


  private void Start()
  {
    NavAgent = GetComponent<NavMeshAgent>();
    Animator = GetComponent<Animator>();
    StateMachine = new StateMachine<EnemyController>(this);
    
    _stateDict = new Dictionary<EEnemyStates, State<EnemyController>>
    {
      [EEnemyStates.Idle] = GetComponent<IdleState>(),
      [EEnemyStates.Chase] = GetComponent<ChaseState>()
    };
  
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
