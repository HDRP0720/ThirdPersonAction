using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleCombat : MonoBehaviour
{
  [SerializeField] private GameObject _weapon;
  [SerializeField] private EAttackState _attackState;
  
  private Animator _animator;
  private BoxCollider _weaponCollider;
  
  // Property
  public bool IsInAction { get; private set; } = false;
  
  private static readonly int SS_Slash01 = Animator.StringToHash("SS_Slash01");
  private static readonly int Hit_Fwd = Animator.StringToHash("Hit_Fwd");

  private void Awake()
  {
    _animator = GetComponent<Animator>();
  }
  private void Start()
  {
    if (_weapon != null)
    {
      _weaponCollider = _weapon.GetComponent<BoxCollider>();
      _weaponCollider.enabled = false;
    }
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
    _attackState = EAttackState.Windup;
    
    float impactStartTime = 0.14f;  // 공격 애니메이션에서 실제 공격 동작이 시작하는 순간 (전체 시간 대비 백분율)
    float impactEndTime = 0.42f;    // 공격 애니메이션에서 실제 공격 동작이 끝난 순간 (전체 시간 대비 백분율)
    
    _animator.CrossFade(SS_Slash01, 0.2f);
    yield return null;
    
    var animState = _animator.GetNextAnimatorStateInfo(1);

    float timer = 0f;
    while (timer <=animState.length)
    {
      timer += Time.deltaTime;
      float normalizedTime = timer / animState.length;

      if (_attackState == EAttackState.Windup)
      {
        if (normalizedTime >= impactStartTime)
        {
          _attackState = EAttackState.Impact;
          _weaponCollider.enabled = true;
        }
      }
      else if (_attackState == EAttackState.Impact)
      {
        if (normalizedTime >= impactEndTime)
        {
          _attackState = EAttackState.Cooldown;
          _weaponCollider.enabled = false;
        }
      }
      else if (_attackState == EAttackState.Cooldown)
      {
        // TODO: Handle combo attack
      }
      
      yield return null;
    }

    _attackState = EAttackState.Idle;
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

public enum EAttackState {Idle, Windup, Impact, Cooldown}
