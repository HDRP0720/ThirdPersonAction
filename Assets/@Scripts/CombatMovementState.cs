using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatMovementState : State<EnemyController>
{
  [SerializeField] private float _distanceToStop = 3f;
  [SerializeField] private float _distanceThreshold = 1f;
  
  private EnemyController _enemy;
  private ECombatStance _stance;
  
  public override void Enter(EnemyController owner)
  {
    _enemy = owner;

    _enemy.NavAgent.stoppingDistance = _distanceToStop;
  }

  public override void Execute()
  {
    if (Vector3.Distance(_enemy.Target.transform.position, _enemy.transform.position) > _distanceToStop + _distanceThreshold)
      StartChaseStance();
    
    if (_stance == ECombatStance.Idle)
    {
      
    }
    else if (_stance == ECombatStance.Chase)
    {
      if (Vector3.Distance(_enemy.Target.transform.position, _enemy.transform.position) <= _distanceToStop + 0.03f)
        StartIdleStance();
      
      _enemy.NavAgent.SetDestination(_enemy.Target.transform.position);
    }
    else if (_stance == ECombatStance.Circling)
    {
      
    }
 
    _enemy.Animator.SetFloat("moveAmount", _enemy.NavAgent.velocity.magnitude / _enemy.NavAgent.speed);
  }

  public override void Exit()
  {

  }

  private void StartChaseStance()
  {
    _stance = ECombatStance.Chase;
    _enemy.Animator.SetBool("combatMode", false);
  }
  private void StartIdleStance()
  {
    _stance = ECombatStance.Idle;
    _enemy.Animator.SetBool("combatMode", true);
  }
}

public enum ECombatStance { Idle, Chase, Circling }
