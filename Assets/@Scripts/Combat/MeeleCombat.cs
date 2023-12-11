using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleCombat : MonoBehaviour
{
  private Animator _animator;
  
  // Property
  public bool IsInAction { get; private set; } = false;
  
  private static readonly int SS_Slash01 = Animator.StringToHash("SS_Slash01");

  private void Awake()
  {
    _animator = GetComponent<Animator>();
  }

  public void TryToAttack()
  {
    if (!IsInAction)
    {
      StartCoroutine(Attack());
    }
  }

  private IEnumerator Attack()
  {
    IsInAction = true;
    _animator.CrossFade(SS_Slash01, 0.2f);
    yield return null;
    
    var animState = _animator.GetNextAnimatorStateInfo(1);
    
    yield return new WaitForSeconds(animState.length);

    IsInAction = false;
  }
}
