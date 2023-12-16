using UnityEngine;

public class CombatMovementState : State<EnemyController>
{
  [SerializeField] private float _distanceToStop = 3f;
  [SerializeField] private float _distanceThreshold = 1f;
  [SerializeField] private Vector2 _idleTimeRange = new Vector2(2, 5);
  [SerializeField] private Vector2 _circlingTimeRange = new Vector2(3, 6);
  [SerializeField] private float _circlingSpeed = 20f;

  private float _timer = 0f;
  private int _circlingDir = 1;     // 1 for left, -1 for right
  private EnemyController _enemy;
  private ECombatStance _stance;
  
  public override void Enter(EnemyController owner)
  {
    _enemy = owner;

    _enemy.NavAgent.stoppingDistance = _distanceToStop;
    _enemy.CombatMovementTimer = 0f;
  }

  public override void Execute()
  {
    if(_timer > 0)
      _timer -= Time.deltaTime;
    
    if (Vector3.Distance(_enemy.Target.transform.position, _enemy.transform.position) > _distanceToStop + _distanceThreshold)
      StartChaseStance();
    
    if (_stance == ECombatStance.Idle)
    {
      if (_timer <= 0)
      {
        if (Random.Range(0, 2) == 0)
          StartIdleStance();
        else
          StartCirclingStance();
      }
    }
    else if (_stance == ECombatStance.Chase)
    {
      if (Vector3.Distance(_enemy.Target.transform.position, _enemy.transform.position) <= _distanceToStop + 0.03f)
        StartIdleStance();
      
      _enemy.NavAgent.SetDestination(_enemy.Target.transform.position);
    }
    else if (_stance == ECombatStance.Circling)
    {
      if (_timer <= 0)
      {
        StartIdleStance();
        return;
      }
      
      var vecToTarget = _enemy.transform.position - _enemy.Target.transform.position;
      var rotatedPos = Quaternion.Euler(0, _circlingSpeed * _circlingDir * Time.deltaTime, 0) * vecToTarget;
      
      _enemy.NavAgent.Move(rotatedPos - vecToTarget);
      _enemy.transform.rotation = Quaternion.LookRotation(-rotatedPos);
    }

    _enemy.CombatMovementTimer += Time.deltaTime;
  }

  public override void Exit()
  {
    _enemy.CombatMovementTimer = 0f;
  }

  private void StartChaseStance()
  {
    _stance = ECombatStance.Chase;
    _enemy.Animator.SetBool("IsCombatMode", false);

  }
  private void StartIdleStance()
  {
    _stance = ECombatStance.Idle;
    _timer = Random.Range(_idleTimeRange.x, _idleTimeRange.y);
    
    _enemy.Animator.SetBool("IsCombatMode", true);

  }
  private void StartCirclingStance()
  {
    _stance = ECombatStance.Circling;
    
    _enemy.NavAgent.ResetPath();
    _timer = Random.Range(_circlingTimeRange.x, _circlingTimeRange.y);

    _circlingDir = Random.Range(0, 2) == 0 ? 1 : -1;
  }
}

public enum ECombatStance { Idle, Chase, Circling }
