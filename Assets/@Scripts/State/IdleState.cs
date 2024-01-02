public class IdleState : State<EnemyController>
{
  private EnemyController _enemy;
  
  public override void Enter(EnemyController owner)
  {
    _enemy = owner;
    _enemy.Animator.SetBool("IsCombatMode", false);
  }

  public override void Execute()
  {
    base.Execute();

    _enemy.Target = _enemy.FindTarget();
    if(_enemy.Target != null)
      _enemy.ChangeState(EEnemyStates.CombatMovement);
  }
}
