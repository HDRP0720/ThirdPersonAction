using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
  // Navmesh SetDestination 대신 Move를 사용하여 Navmesh의 velocity 사용불가.
  // 이를 해결하기 위해 프레임간의 거리차이를 활용하여 velocity를 직접 계산
  private Vector3 prevPos;
  
  // Caching states
  private Dictionary<EEnemyStates, State<EnemyController>> _stateDict;
  
  // Auto-Property
  public Animator Animator { get; private set; }
  public CharacterController CharacterController { get; private set; } 
  public NavMeshAgent NavAgent { get; private set; }
  public SkinnedMeshHighlighter MeshHighlighter { get; private set; }
  public MeeleCombat MeeleCombat { get; private set; }
  public VisionSensor VisionSensor { get; set; }
  public StateMachine<EnemyController> StateMachine { get; private set; }
  public List<MeeleCombat> TargetsInRange { get; set; } = new List<MeeleCombat>();
  public float CombatMovementTimer { get; set; } = 0f;
  
  [field: SerializeField] public MeeleCombat Target { get; set; }
  [field: SerializeField] public float FOV { get; set; } = 180f;

  private void Start()
  {
    Animator = GetComponent<Animator>();
    CharacterController = GetComponent<CharacterController>();
    NavAgent = GetComponent<NavMeshAgent>();

    MeshHighlighter = GetComponent<SkinnedMeshHighlighter>();
    MeeleCombat = GetComponent<MeeleCombat>();
    StateMachine = new StateMachine<EnemyController>(this);
    
    _stateDict = new Dictionary<EEnemyStates, State<EnemyController>>
    {
      [EEnemyStates.Idle] = GetComponent<IdleState>(),
      [EEnemyStates.CombatMovement] = GetComponent<CombatMovementState>(),
      [EEnemyStates.Attack] = GetComponent<AttackState>(),
      [EEnemyStates.Retreat] = GetComponent<RetreatState>(),
      [EEnemyStates.Dead] = GetComponent<DeadState>(),
    };
  
    StateMachine.ChangeState(_stateDict[EEnemyStates.Idle]);
  }
  private void Update()
  {
    StateMachine.Execute();
    
    // v = dx / dt
    var deltaPos = Animator.applyRootMotion ? Vector3.zero : transform.position - prevPos;
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

  public bool IsInState(EEnemyStates state)
  {
    return StateMachine.CurrentState == _stateDict[state];
  }
}

public enum EEnemyStates { Idle, CombatMovement, Attack, Retreat, Dead }
