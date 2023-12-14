using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : State<EnemyController>
{
  [SerializeField] private float _distanceToStop = 3f;
  
  private EnemyController _enemy;
  
  public override void Enter(EnemyController owner)
  {
    _enemy = owner;

    _enemy.NavAgent.stoppingDistance = _distanceToStop;
  }

  public override void Execute()
  {
    _enemy.NavAgent.SetDestination(_enemy.Target.transform.position);
    _enemy.Animator.SetFloat("moveAmount", _enemy.NavAgent.velocity.magnitude / _enemy.NavAgent.speed);
  }

  public override void Exit()
  {

  }
}
