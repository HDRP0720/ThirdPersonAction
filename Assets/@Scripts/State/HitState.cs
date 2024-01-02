using System.Collections;
using UnityEngine;

public class HitState : State<EnemyController>
{
  [SerializeField] private float _stunnTime = 0.5f;
  
  private EnemyController _enemy;

  public override void Enter(EnemyController owner)
  {
    _enemy = owner;
    _enemy.MeeleCombat.OnHitComplete += () =>StartCoroutine(GoToCombatMovementState());
  }

  private IEnumerator GoToCombatMovementState()
  {
    yield return new WaitForSeconds(_stunnTime);
    _enemy.ChangeState(EEnemyStates.CombatMovement);
  }
}
