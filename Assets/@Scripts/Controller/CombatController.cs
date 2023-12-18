using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
  private MeeleCombat _meeleCombat;
  private Animator _animator;

  private void Awake()
  {
    _meeleCombat = GetComponent<MeeleCombat>();
    _animator = GetComponent<Animator>();
  }
  private void Update()
  {
    if (Input.GetButtonDown("Attack"))
    {
      var enemy = EnemyManager.Instance.GetAttackingEnemy();
      if (enemy != null && enemy.MeeleCombat.CanCounter && !_meeleCombat.IsInAction)
      {
        StartCoroutine(_meeleCombat.CoPerformCounterAttack(enemy));
      }
      else
      {
        _meeleCombat.TryToAttack();
      }
    }
  }
  
  // 카운터 어택 발동시, 루트 모션의 포지션을 제어하기 위함
  private void OnAnimatorMove()
  {
    if(!_meeleCombat.IsInCounter)
      transform.position += _animator.deltaPosition;
    
    transform.rotation *= _animator.deltaRotation;
  }
}
