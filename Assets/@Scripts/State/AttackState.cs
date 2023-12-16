using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State<EnemyController>
{
  [SerializeField] private float _distanceToAttack = 1f;
  
  private EnemyController _enemy;
  private bool _isAttacking;
  
  public override void Enter(EnemyController owner)
  {
    _enemy = owner;
    
    _enemy.NavAgent.stoppingDistance = _distanceToAttack;
  }
  public override void Execute()
  {
    if (_isAttacking) return;
    
    _enemy.NavAgent.SetDestination(_enemy.Target.transform.position);

    if (Vector3.Distance(_enemy.Target.transform.position, _enemy.transform.position) <= _distanceToAttack + 0.03f)
    {
      StartCoroutine(CoAttack());
    }
  }
  public override void Exit()
  {
    _enemy.NavAgent.ResetPath();
  }

  private IEnumerator CoAttack()
  {
    _isAttacking = true;
    _enemy.Animator.applyRootMotion = true;
    
    _enemy.MeeleCombat.TryToAttack();
    yield return new WaitUntil(() => _enemy.MeeleCombat.AttackStance == EAttackStance.Idle);

    _enemy.Animator.applyRootMotion = false;
    _isAttacking = false;
    
    _enemy.ChangeState(EEnemyStates.Retreat);
  }
}
