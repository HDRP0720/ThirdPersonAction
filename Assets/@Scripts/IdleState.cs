using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : State<EnemyController>
{
  private EnemyController _enemy;
  
  public override void Enter(EnemyController owner)
  {
    _enemy = owner;
    Debug.Log("Enter Idle State");
  }

  public override void Execute()
  {
    Debug.Log("Execute Idle State");
    
    if (Input.GetKeyDown(KeyCode.T))
      _enemy.ChangeState(EEnemyStates.Chase);
  }

  public override void Exit()
  {
    Debug.Log("Exit Idle State");
  }
}
