using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleCombat : MonoBehaviour
{
  [SerializeField] private GameObject _weapon;
  [SerializeField] private List<AttackData> _attackDatas;
  
  private Animator _animator;
  private BoxCollider _weaponCollider;
  private SphereCollider _leftHandCollider, _rightHandCollider, _leftFootCollider, _rightFootCollider;
  private EAttackState _attackState;
  private bool _isInCombo;
  private int _comboCount = 0;
  
  // Property
  public bool IsInAction { get; private set; } = false;

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
      _leftHandCollider = _animator.GetBoneTransform(HumanBodyBones.LeftHand).GetComponent<SphereCollider>();
      _rightHandCollider= _animator.GetBoneTransform(HumanBodyBones.RightHand).GetComponent<SphereCollider>();
      _leftFootCollider= _animator.GetBoneTransform(HumanBodyBones.LeftFoot).GetComponent<SphereCollider>();
      _rightFootCollider= _animator.GetBoneTransform(HumanBodyBones.RightFoot).GetComponent<SphereCollider>();

      DisableAllHitboxColliders();
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
    else if (_attackState == EAttackState.Impact || _attackState == EAttackState.Cooldown)
    {
      _isInCombo = true;
    }
  }

  private IEnumerator Attack()
  {
    IsInAction = true;
    _attackState = EAttackState.Windup;
    
    _animator.CrossFade(_attackDatas[_comboCount].AnimName, 0.2f);
    yield return null;
    
    var animState = _animator.GetNextAnimatorStateInfo(1);

    float timer = 0f;
    while (timer <=animState.length)
    {
      timer += Time.deltaTime;
      float normalizedTime = timer / animState.length;

      if (_attackState == EAttackState.Windup)
      {
        if (normalizedTime >= _attackDatas[_comboCount].ImpactStartTime)
        {
          _attackState = EAttackState.Impact;
          EnableHitboxCollider(_attackDatas[_comboCount]);
        }
      }
      else if (_attackState == EAttackState.Impact)
      {
        if (normalizedTime >= _attackDatas[_comboCount].ImpactEndTime)
        {
          _attackState = EAttackState.Cooldown;
          DisableAllHitboxColliders();
        }
      }
      else if (_attackState == EAttackState.Cooldown)
      {
        // TODO: Handle combo attack
        if (_isInCombo)
        {
          _isInCombo = false;
          _comboCount = (_comboCount + 1) % _attackDatas.Count;

          StartCoroutine(Attack());
          yield break;
        }
      }
      
      yield return null;
    }

    _attackState = EAttackState.Idle;
    _comboCount = 0;
    IsInAction = false;
  }
  
  private IEnumerator PlayHitReaction()
  {
    IsInAction = true;
    _animator.CrossFade(Hit_Fwd, 0.2f);
    yield return null;
    
    var animState = _animator.GetNextAnimatorStateInfo(1);
    
    yield return new WaitForSeconds(animState.length * 0.8f);

    IsInAction = false;
  }

  private void EnableHitboxCollider(AttackData attackData)
  {
    switch (attackData.HitboxToUse)
    {
      case EAttackHitbox.LeftHand:
        _leftHandCollider.enabled = true;
        break;
      case EAttackHitbox.RightHand:
        _rightHandCollider.enabled = true;
        break;
      case EAttackHitbox.LeftFoot:
        _leftFootCollider.enabled = true;
        break;
      case EAttackHitbox.RightFoot:
        _rightFootCollider.enabled = true;
        break;
      case EAttackHitbox.Weapon:
        _weaponCollider.enabled = true;
        break;
      default:
        break;
    }
  }
  private void DisableAllHitboxColliders()
  {
    _weaponCollider.enabled = false;
    _leftHandCollider.enabled = false;
    _rightHandCollider.enabled = false;
    _leftFootCollider.enabled = false;
    _rightFootCollider.enabled = false;
  }
}

public enum EAttackState {Idle, Windup, Impact, Cooldown}
