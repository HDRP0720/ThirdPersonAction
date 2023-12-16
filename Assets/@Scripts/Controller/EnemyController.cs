using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
  // Navmesh SetDestination 대신 Move를 사용하여 Navmesh의 velocity 사용불가.
  // 이를 해결하기 위해 프레임간의 거리차이를 활용하여 velocity를 직접 계산
  private Vector3 prevPos;  
  
  // Auto-Property
  public NavMeshAgent NavAgent { get; private set; }
  public Animator Animator { get; private set; }
  public StateMachine<EnemyController> StateMachine { get; private set; }
  public List<MeeleCombat> TargetsInRange { get; set; } = new List<MeeleCombat>();
  
  [field: SerializeField] public MeeleCombat Target { get; set; }
  [field: SerializeField] public float FOV { get; set; } = 180f;
  
  // Caching states
  private Dictionary<EEnemyStates, State<EnemyController>> _stateDict;

  private void Start()
  {
    NavAgent = GetComponent<NavMeshAgent>();
    Animator = GetComponent<Animator>();
    StateMachine = new StateMachine<EnemyController>(this);
    
    _stateDict = new Dictionary<EEnemyStates, State<EnemyController>>
    {
      [EEnemyStates.Idle] = GetComponent<IdleState>(),
      [EEnemyStates.CombatMovement] = GetComponent<CombatMovementState>()
    };
  
    StateMachine.ChangeState(_stateDict[EEnemyStates.Idle]);
  }
  private void Update()
  {
    StateMachine.Execute();
    
    // v = dx / dt
    var deltaPos = transform.position - prevPos;
    var velocity = deltaPos / Time.deltaTime;
    
    float forwardSpeed = Vector3.Dot(velocity, transform.forward);
    Animator.SetFloat("forwardSpeed", forwardSpeed / NavAgent.speed, 0.2f, Time.deltaTime);

    float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
    float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
    Animator.SetFloat("strafeSpeed", strafeSpeed, 0.2f, Time.deltaTime);

    prevPos = transform.position;
  }

  public void ChangeState(EEnemyStates state)
  {
    StateMachine.ChangeState(_stateDict[state]);
  }
}

public enum EEnemyStates { Idle, CombatMovement }
