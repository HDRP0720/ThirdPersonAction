using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleCombat : MonoBehaviour
{
  [SerializeField] private GameObject _weapon;
  [SerializeField] private List<AttackData> _attackData;
  
  private Animator _animator;
  private BoxCollider _weaponCollider;
  private SphereCollider _leftHandCollider, _rightHandCollider, _leftFootCollider, _rightFootCollider;
  private bool _isInCombo;
  private int _comboCount = 0;
  
  // Property
  public bool IsInAction { get; private set; } = false;
  public EAttackStance AttackStance { get; private set; }
  
  public List<AttackData> GetAttackData => _attackData;

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
      StartCoroutine(CoPlayHitReaction());
      Debug.Log($"{this.gameObject.name} was hit!!");
    }
  }

  public void TryToAttack()
  {
    if (!IsInAction)
    {
      StartCoroutine(CoAttack());
    }
    else if (AttackStance == EAttackStance.Impact || AttackStance == EAttackStance.Cooldown)
    {
      _isInCombo = true;
    }
  }

  private IEnumerator CoAttack()
  {
    IsInAction = true;
    AttackStance = EAttackStance.Windup;
    
    _animator.CrossFade(_attackData[_comboCount].AnimName, 0.2f);
    yield return null;
    
    var animState = _animator.GetNextAnimatorStateInfo(1);

    float timer = 0f;
    while (timer <=animState.length)
    {
      timer += Time.deltaTime;
      float normalizedTime = timer / animState.length;

      if (AttackStance == EAttackStance.Windup)
      {
        if (normalizedTime >= _attackData[_comboCount].ImpactStartTime)
        {
          AttackStance = EAttackStance.Impact;
          EnableHitboxCollider(_attackData[_comboCount]);
        }
      }
      else if (AttackStance == EAttackStance.Impact)
      {
        if (normalizedTime >= _attackData[_comboCount].ImpactEndTime)
        {
          AttackStance = EAttackStance.Cooldown;
          DisableAllHitboxColliders();
        }
      }
      else if (AttackStance == EAttackStance.Cooldown)
      {
        // TODO: Handle combo attack
        if (_isInCombo)
        {
          _isInCombo = false;
          _comboCount = (_comboCount + 1) % _attackData.Count;

          StartCoroutine(CoAttack());
          yield break;
        }
      }
      
      yield return null;
    }

    AttackStance = EAttackStance.Idle;
    _comboCount = 0;
    IsInAction = false;
  }
  
  private IEnumerator CoPlayHitReaction()
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

public enum EAttackStance {Idle, Windup, Impact, Cooldown}
