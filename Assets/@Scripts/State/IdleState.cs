using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State<EnemyController>
{
  private EnemyController _enemy;
  
  public override void Enter(EnemyController owner)
  {
    base.Enter(owner);
    _enemy = owner;
  }

  public override void Execute()
  {
    base.Execute();
    foreach (var target in _enemy.TargetsInRange)
    {
      var vecToTarget = target.transform.position - transform.position;
      float angle = Vector3.Angle(transform.forward, vecToTarget);
      if (angle <= _enemy.FOV / 2)
      {
        _enemy.Target = target;
        _enemy.ChangeState(EEnemyStates.CombatMovement);
        break;
      }
    }
  }

  public override void Exit()
  {
    base.Exit();
  }
}
