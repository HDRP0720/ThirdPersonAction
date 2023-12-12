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
  private static readonly int Hit_Fwd = Animator.StringToHash("Hit_Fwd");

  private void Awake()
  {
    _animator = GetComponent<Animator>();
  }
  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Hitbox") && !IsInAction)
    {
      StartCoroutine(PlayHitReaction());
      Debug.Log($"{this.gameObject.name} was hit!!");
    
    }
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
  
  private IEnumerator PlayHitReaction()
  {
    IsInAction = true;
    _animator.CrossFade(Hit_Fwd, 0.2f);
    yield return null;
    
    var animState = _animator.GetNextAnimatorStateInfo(1);
    
    yield return new WaitForSeconds(animState.length);

    IsInAction = false;
  }
}
