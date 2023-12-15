using System.Collections;
using System.Collections.Generic;
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
      
      transform.RotateAround(_enemy.Target.transform.position, Vector3.up, _circlingSpeed * _circlingDir * Time.deltaTime);
    }
 
    _enemy.Animator.SetFloat("moveAmount", _enemy.NavAgent.velocity.magnitude / _enemy.NavAgent.speed);
  }

  public override void Exit()
  {

  }

  private void StartChaseStance()
  {
    _stance = ECombatStance.Chase;
    _enemy.Animator.SetBool("IsCombatMode", false);
    _enemy.Animator.SetBool("IsCircling", false);
  }
  private void StartIdleStance()
  {
    _stance = ECombatStance.Idle;
    _timer = Random.Range(_idleTimeRange.x, _idleTimeRange.y);
    
    _enemy.Animator.SetBool("IsCombatMode", true);
    _enemy.Animator.SetBool("IsCircling", false);
  }
  private void StartCirclingStance()
  {
    _stance = ECombatStance.Circling;
    _timer = Random.Range(_circlingTimeRange.x, _circlingTimeRange.y);

    _circlingDir = Random.Range(0, 2) == 0 ? 1 : -1;
    
    _enemy.Animator.SetBool("IsCircling", true);
    _enemy.Animator.SetFloat("circlingDir", _circlingDir);
  }
}

public enum ECombatStance { Idle, Chase, Circling }
