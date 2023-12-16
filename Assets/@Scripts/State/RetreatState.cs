using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetreatState : State<EnemyController>
{
  [SerializeField] private float _backwardWalkSpeed = 1.5f;
  [SerializeField] private float _distanceToRetreat = 3f;
  
  private EnemyController _enemy;
  
  public override void Enter(EnemyController owner)
  {
    _enemy = owner;
  }
  
  public override void Execute()
  {
    if (Vector3.Distance(_enemy.transform.position, _enemy.Target.transform.position) >= _distanceToRetreat)
    {
      _enemy.ChangeState(EEnemyStates.CombatMovement);
      return;
    }
    
    var vecToTarget = _enemy.Target.transform.position - _enemy.transform.position;
    _enemy.NavAgent.Move(-vecToTarget.normalized * (_backwardWalkSpeed * Time.deltaTime));

    vecToTarget.y = 0f;
    transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(vecToTarget), 500 * Time.deltaTime);
  }
}