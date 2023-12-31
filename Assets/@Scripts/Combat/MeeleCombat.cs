using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleCombat : MonoBehaviour
{
  [SerializeField] private GameObject _weapon;
  [SerializeField] private List<AttackData> _attackData;
  [SerializeField] private List<AttackData> _longRageAttackDatas;
  [SerializeField] private float _longRangeAttackThreshold = 1.5f;
  [SerializeField] private float _rotationSpeed = 500f;
  
  private Animator _animator;
  private BoxCollider _weaponCollider;
  private SphereCollider _leftHandCollider, _rightHandCollider, _leftFootCollider, _rightFootCollider;
  private bool _isInCombo;
  private int _comboCount = 0;
  private MeeleCombat _currentTarget;
  
  // Delegate
  public event Action OnHitState;
  public event Action OnHitComplete;
  
  // Property
  public bool IsInAction { get; private set; } = false;
  public EAttackStance AttackStance { get; private set; }
  public bool IsInCounter { get; private set; } = false;
  
  public List<AttackData> GetAttackData => _attackData;
  public bool CanCounter => AttackStance == EAttackStance.Windup && _comboCount == 0;

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
      var attacker = other.GetComponentInParent<MeeleCombat>();
      if (attacker._currentTarget != this) return;
        
      StartCoroutine(CoPlayHitReaction(attacker.transform));
    }
  }

  public void TryToAttack(MeeleCombat target = null)
  {
    if (!IsInAction)
    {
      StartCoroutine(CoAttack(target));
    }
    else if (AttackStance == EAttackStance.Impact || AttackStance == EAttackStance.Cooldown)
    {
      _isInCombo = true;
    }
  }

  private IEnumerator CoAttack(MeeleCombat target = null)
  {
    IsInAction = true;
    AttackStance = EAttackStance.Windup;
    _currentTarget = target;

    var attack = _attackData[_comboCount];
    var attackDir = transform.forward;
    
    Vector3 startPos = transform.position;
    Vector3 targetPos = Vector3.zero;
    
    if (target != null)
    {
      var vecToTarget = target.transform.position - transform.position;
      vecToTarget.y = 0f;
      attackDir = vecToTarget.normalized;
      float distance = vecToTarget.magnitude - attack.DistanceFromTarget;

      if (distance > _longRangeAttackThreshold)
        attack = _longRageAttackDatas[0];

      if (attack.CanMoveToTarget)
      {
        if(distance < attack.MaxMoveDistance)
          targetPos = target.transform.position - attackDir * attack.DistanceFromTarget;
        else
          targetPos = startPos + attackDir * attack.MaxMoveDistance;
      }
    }
    
    _animator.CrossFade(attack.AnimName, 0.2f);
    yield return null;
    
    var animState = _animator.GetNextAnimatorStateInfo(1);

    float timer = 0f;
    while (timer <=animState.length)
    {
      timer += Time.deltaTime;
      float normalizedTime = timer / animState.length;

      if (target != null && attack.CanMoveToTarget)
      {
        float percentageTime = (normalizedTime - attack.MoveStartTime) / (attack.MoveEndTime - attack.MoveStartTime);
        transform.position = Vector3.Lerp(startPos, targetPos, percentageTime);
      }

      if (attackDir != null)
      {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(attackDir),
          _rotationSpeed * Time.deltaTime);
      }

      if (AttackStance == EAttackStance.Windup)
      {
        if (IsInCounter) break;
          
        if (normalizedTime >= attack.ImpactStartTime)
        {
          AttackStance = EAttackStance.Impact;
          EnableHitboxCollider(_attackData[_comboCount]);
        }
      }
      else if (AttackStance == EAttackStance.Impact)
      {
        if (normalizedTime >= attack.ImpactEndTime)
        {
          AttackStance = EAttackStance.Cooldown;
          DisableAllHitboxColliders();
        }
      }
      else if (AttackStance == EAttackStance.Cooldown)
      {
        if (_isInCombo)
        {
          _isInCombo = false;
          _comboCount = (_comboCount + 1) % _attackData.Count;

          StartCoroutine(CoAttack(target));
          yield break;
        }
      }
      
      yield return null;
    }

    AttackStance = EAttackStance.Idle;
    _comboCount = 0;
    IsInAction = false;
    _currentTarget = null;
  }
  
  private IEnumerator CoPlayHitReaction(Transform attacker)
  {
    IsInAction = true;
    
    // 맞는 방향에 맞게 캐릭터를 회전
    var hitDir = attacker.position - transform.position;
    hitDir.y = 0f;
    transform.rotation = Quaternion.LookRotation(hitDir);

    OnHitState?.Invoke();
    _animator.CrossFade(Hit_Fwd, 0.2f);
    yield return null;
    
    var animState = _animator.GetNextAnimatorStateInfo(1);
    
    yield return new WaitForSeconds(animState.length * 0.8f);
    
    OnHitComplete?.Invoke();
    IsInAction = false;
  }
  
  public IEnumerator CoPerformCounterAttack(EnemyController opponent)
  {
    IsInAction = true;
    
    IsInCounter = true;
    opponent.MeeleCombat.IsInCounter = true;
    opponent.ChangeState(EEnemyStates.Dead);
    
    // 카운터 어택 세트 애니메이션 플레이 직전, 플레이어 & 에너미 회전 방향 맞춰주기
    var vecToEnemyDir = opponent.transform.position - transform.position;
    vecToEnemyDir.y = 0f;
    transform.rotation = Quaternion.LookRotation(vecToEnemyDir);
    opponent.transform.rotation = Quaternion.LookRotation(-vecToEnemyDir);
    
    //
    var targetPos = opponent.transform.position - vecToEnemyDir.normalized * 1f;
    
    _animator.CrossFade("CounterAttack", 0.2f);
    opponent.Animator.CrossFade("CounterAttackVictim", 0.2f);
    
    yield return null;
    
    var animState = _animator.GetNextAnimatorStateInfo(1);

    float timer = 0f;
    while (timer <= animState.length)
    {
      transform.position = Vector3.MoveTowards(transform.position, targetPos, 5 * Time.deltaTime);

      yield return null;

      timer += Time.deltaTime;
    }
    
    IsInCounter = false;
    opponent.MeeleCombat.IsInCounter = false;

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
  public void DisableAllHitboxColliders()
  {
    _weaponCollider.enabled = false;
    _leftHandCollider.enabled = false;
    _rightHandCollider.enabled = false;
    _leftFootCollider.enabled = false;
    _rightFootCollider.enabled = false;
  }
}

public enum EAttackStance {Idle, Windup, Impact, Cooldown}
