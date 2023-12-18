using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : State<EnemyController>
{
  public override void Enter(EnemyController owner)
  {
    owner.VisionSensor.gameObject.SetActive(false);
    EnemyManager.Instance.RemoveEnemyInRange(owner);

    owner.NavAgent.enabled = false;
    owner.CharacterController.enabled = false;
    
    // TODO: 무기 콜라이더가 켜지는 순간이 종종 있어서 임시 조치
    owner.MeeleCombat.DisableAllHitboxColliders();
  }
}
