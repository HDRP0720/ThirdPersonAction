using System.Collections;
using UnityEngine;

public class AttackState : State<EnemyController>
{
  [SerializeField] private float _distanceToAttack = 1f;
  
  private EnemyController _enemy;
  private bool _isAttacking;
  
  public override void Enter(EnemyController owner)
  {
    base.Enter(owner);
    
    _enemy = owner;
    
    _enemy.NavAgent.stoppingDistance = _distanceToAttack;
  }
  public override void Execute()
  {
    base.Execute();
    
    if (_isAttacking) return;
    
    _enemy.NavAgent.SetDestination(_enemy.Target.transform.position);

    if (Vector3.Distance(_enemy.Target.transform.position, _enemy.transform.position) <= _distanceToAttack + 0.03f)
      StartCoroutine(CoAttack(Random.Range(0, _enemy.MeeleCombat.GetAttackData.Count + 1)));
  }
  public override void Exit()
  {
    _enemy.NavAgent.ResetPath();
  }

  private IEnumerator CoAttack(int comboCount=1)
  {
    _isAttacking = true;
    _enemy.Animator.applyRootMotion = true;
    
    _enemy.MeeleCombat.TryToAttack(_enemy.Target);
    
    for (int i = 0; i < comboCount; i++)
    {
      yield return new WaitUntil(() => _enemy.MeeleCombat.AttackStance == EAttackStance.Cooldown);
      _enemy.MeeleCombat.TryToAttack(_enemy.Target);
    }
    
    yield return new WaitUntil(() => _enemy.MeeleCombat.AttackStance == EAttackStance.Idle);

    _enemy.Animator.applyRootMotion = false;
    _isAttacking = false;
    
    if(_enemy.IsInState(EEnemyStates.Attack))
      _enemy.ChangeState(EEnemyStates.Retreat);
  }
}
