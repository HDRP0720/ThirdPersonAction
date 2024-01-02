using UnityEngine;

public class CombatController : MonoBehaviour
{
  private CameraController _cameraController;
  private MeeleCombat _meeleCombat;
  private Animator _animator;

  private EnemyController _targetEnemy;
  private bool _isCombatMode;

  // Property
  public EnemyController TargetEnemy
  {
    get => _targetEnemy;
    set
    {
      _targetEnemy = value;

      if (_targetEnemy == null)
        IsCombatMode = false;
    }
  }
  public bool IsCombatMode
  {
    get => _isCombatMode;
    set
    {
      _isCombatMode = value;

      if (TargetEnemy == null)
        _isCombatMode = false;
      
      _animator.SetBool("IsCombatMode", _isCombatMode);
    }
  }

  private void Awake()
  {
    if (Camera.main != null) 
      _cameraController = Camera.main.GetComponent<CameraController>();
    
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
        var enemyToAttack = EnemyManager.Instance.GetClosestEnemyInDirection(PlayerController.Instance.InputDir);
        _meeleCombat.TryToAttack(enemyToAttack?.MeeleCombat);
        
        IsCombatMode = true;
      }
    }

    if (Input.GetButtonDown("LockOn"))
    {
      IsCombatMode = !IsCombatMode;
    }
  }
  
  // 카운터 어택 발동시, 루트 모션의 포지션을 제어하기 위함
  private void OnAnimatorMove()
  {
    if(!_meeleCombat.IsInCounter)
      transform.position += _animator.deltaPosition;
    
    transform.rotation *= _animator.deltaRotation;
  }

  public Vector3 GetTargetingDir()
  {
    if (IsCombatMode) return transform.forward;
    
    var vecFromCam = transform.position - _cameraController.transform.position;
    vecFromCam.y = 0f;

    return vecFromCam.normalized;
  }
}
