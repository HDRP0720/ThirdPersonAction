using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  public static PlayerController Instance { get; private set; }
  
  # region Variables
  [SerializeField] private float _moveSpeed = 5f;
  [SerializeField] private float _rotationSpeed = 500f;
  
  [Header("Ground Check Settings")]
  [SerializeField] private float _groundCheckRadius = 0.2f;
  [SerializeField] private Vector3 _groundCheckOffset;
  [SerializeField] private LayerMask _groundLayer;
  
  private CameraController _cameraController;
  private CharacterController _cc;
  private Animator _animator;
  private MeeleCombat _meeleCombat;
  private CombatController _combatController;
  private EnvironmentScanner _environmentScanner;
  
  private Quaternion _targetRotation;
  private Vector3 _desiredMoveDir;
  private Vector3 _moveDir;
  private Vector3 _velocity;
  private bool _hasControl = true;
  private bool _isGrounded;
  private float _ySpeed;
  #endregion

  #region Properties
  public float GetRotationSpeed => _rotationSpeed;
  public bool HasControl { get => _hasControl; set => _hasControl = value; }
  public Vector3 InputDir { get; private set; }
  public bool IsInAction { get; private set; }
  public bool IsOnLedge { get; set; }
  public bool IsHanging { get; set; }
  public LedgeData LedgeData { get; set; }
  #endregion
  
  // For Animation parameters
  private static readonly int ForwardSpeed = Animator.StringToHash("forwardSpeed");
  private static readonly int StrafeSpeed = Animator.StringToHash("strafeSpeed");

  private void Awake()
  {
    Instance = this;
    
    if (Camera.main != null) 
      _cameraController = Camera.main.GetComponent<CameraController>();
    
    _cc = GetComponent<CharacterController>();
    _animator = GetComponent<Animator>();
    _meeleCombat = GetComponent<MeeleCombat>();
    _combatController = GetComponent<CombatController>();
    _environmentScanner = GetComponent<EnvironmentScanner>();
  }
  private void Update()
  {
    if (_meeleCombat.IsInAction)
    {
      _targetRotation = transform.rotation;
      _animator.SetFloat(ForwardSpeed, 0f);
      return;
    }
    
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    float moveAmount =Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

    var moveInput = new Vector3(h, 0, v).normalized;
    _desiredMoveDir = _cameraController.GetPlanarRotation * moveInput;
    _moveDir = _desiredMoveDir;
    InputDir = _desiredMoveDir;
    
    if (!_hasControl) return;

    if (IsHanging) return;

    _velocity = Vector3.zero;
    
    _isGrounded = CheckGround();
    _animator.SetBool("IsGrounded", _isGrounded);

    if (_isGrounded)
    {
      _ySpeed = -0.5f;
      _velocity = _desiredMoveDir * _moveSpeed;

      IsOnLedge = _environmentScanner.IsOnLedge(_desiredMoveDir, out LedgeData ledgeData);
      if (IsOnLedge)
      {
        LedgeData = ledgeData;
        MoveNearLedge();
      }
      
      _animator.SetFloat(ForwardSpeed, _velocity.magnitude / _moveSpeed, 0.2f, Time.deltaTime);
    }
    else
    {
      _ySpeed += Physics.gravity.y * Time.deltaTime;
      // velocity = transform.forward * (_moveSpeed * 0.5f);
    }
    
    // 전투 또는 에너미 타겟이 설정되면 Lock-On 모드로 변경
    if (_combatController.IsCombatMode)
    {
      _velocity /= 4f;
      
      var targetVec = _combatController.TargetEnemy.transform.position - transform.position;
      targetVec.y = 0f;

      if (moveAmount > 0)
      {
        _targetRotation = Quaternion.LookRotation(targetVec);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);
      }
    
      float forwardSpeed = Vector3.Dot(_velocity, transform.forward);
      _animator.SetFloat(ForwardSpeed, forwardSpeed / _moveSpeed, 0.2f, Time.deltaTime);

      float angle = Vector3.SignedAngle(transform.forward, _velocity, Vector3.up);
      float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
      _animator.SetFloat(StrafeSpeed, strafeSpeed, 0.2f, Time.deltaTime);
    }
    else
    {
      if (moveAmount > 0 && _moveDir.magnitude > 0.2f)
        _targetRotation = Quaternion.LookRotation(_moveDir);

      transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);
    }
    
    _velocity.y = _ySpeed;
    _cc.Move(_velocity * Time.deltaTime);
  }

  public void SetControl(bool isControl)
  {
    _hasControl = isControl;
    _cc.enabled = isControl;

    if (!isControl)
    {
      _animator.SetFloat(ForwardSpeed, 0f);
      _targetRotation = transform.rotation;
    }
  }
  
  private bool CheckGround()
  {
    return Physics.CheckSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius, _groundLayer);
  }

  private void MoveNearLedge()
  {
    var signedAngle = Vector3.SignedAngle(LedgeData.surfaceHit.normal, _desiredMoveDir, Vector3.up);
    var angle = Mathf.Abs(signedAngle);
    
    // Only for character rotation, Not move
    if (Vector3.Angle(_desiredMoveDir, transform.forward) >= 80)
    {
      _velocity = Vector3.zero;
      return;
    }
    
    // For joystick movement, if angle is b/w 60 and 90, then limit the velocity only to horizontal direction
    if (angle < 60)
    {
      _velocity = Vector3.zero;
      _moveDir = Vector3.zero;
    }
    else if (angle < 90)
    {
     
      var left = Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
      var dir = left * Mathf.Sign(signedAngle);

      _velocity = _velocity.magnitude * dir;
      _moveDir = dir;
    }
  }

  public IEnumerator CoAction(string animName, MatchTargetParams matchParams,
    Quaternion targetRotation, bool canRotate = false, float postDelay = 0f, bool isMirror = false)
  {
    IsInAction = true;
    
    _animator.SetBool("IsMirror", isMirror);
    _animator.CrossFadeInFixedTime(animName, 0.2f);
    yield return null;

    var animState = _animator.GetNextAnimatorStateInfo(0);
    if(!animState.IsName(animName))
      Debug.LogError("The parkour data's animation name does not match the specified animation.");

    float rotateStartTime = matchParams?.matchStartTime ?? 0f;
    float timer = 0f;
    while (timer <= animState.length)
    {
      timer += Time.deltaTime;
      float normalizedTime = timer / animState.length;
      
      if (canRotate && normalizedTime > rotateStartTime)
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
      
      if(matchParams != null)
        MatchTarget(matchParams);
      
      // vault animation 재생 마지막 부분의 height 값과 실제 값이 다른 경우를 위함
      // 강제로 root motion에서 character controller로 이전 (중력 적용을 위해서)
      if (_animator.IsInTransition(0) && timer > 0.5f)
        break;

      yield return null;
    }
    
    // 연속된 animation에서 재생되는 마지막 animation때문에 input 조작 시간을 늦춰야할 경우 적용됨
    // postDelay에 마지막 animation의 재생시간 입력
    yield return new WaitForSeconds(postDelay);
  
    IsInAction = false;
  }
  private void MatchTarget(MatchTargetParams mp)
  {
    if (_animator.isMatchingTarget) return;
    
    _animator.MatchTarget(mp.matchPos, transform.rotation, mp.matchBodyPart, 
      new MatchTargetWeightMask(mp.matchPosWeight, 0), mp.matchStartTime, mp.matchEndTime);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = new Color(0, 1, 0, 0.5f);
    Gizmos.DrawSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius);
  }
}

/// <summary>
/// Target Match에 쓰일 파라미터 모음
/// </summary>
public class MatchTargetParams
{
  public Vector3 matchPos;        // target match가 실행될 position
  public AvatarTarget matchBodyPart;   // target match가 적용될 body part
  public Vector3 matchPosWeight;       // target match가 얼마나 적용될지
  public float matchStartTime;    // target match가 시작될 시간 (예. 67% -> 0.67)
  public float matchEndTime;      // target match가 완료될 시간 (예. 67% -> 0.67)
}
