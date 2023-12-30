using System;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
  public static PlayerController Instance { get; private set; }
  
  [SerializeField] private float _moveSpeed = 5f;
  [SerializeField] private float _rotationSpeed = 500f;
  
  [Header("Ground Check Settings")]
  [SerializeField] private float _groundCheckRadius = 0.2f;
  [SerializeField] private Vector3 _groundCheckOffset;
  [SerializeField] private LayerMask _groundLayer;
  
  private Quaternion _targetRotation;
  
  private CameraController _cameraController;
  private CharacterController _cc;
  private Animator _animator;
  private MeeleCombat _meeleCombat;
  private CombatController _combatController;
  private EnvironmentScanner _environmentScanner;
  
  private Vector3 _desiredMoveDir;
  private Vector3 _moveDir;
  private Vector3 _velocity;
  private bool _hasControl = true;
  private bool _isGrounded;
  private float _ySpeed;
  
  // Property
  public float GetRotationSpeed => _rotationSpeed;
  public Vector3 InputDir { get; private set; }
  public bool IsOnLedge { get; set; }
  public LedgeData LedgeData { get; set; }
  
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
        // Debug.Log("I'm On Ledge!!!");
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
    float angle = Vector3.Angle(LedgeData.surfaceHit.normal, _desiredMoveDir);
    if (angle < 90)
    {
      _velocity = Vector3.zero;
      _moveDir = Vector3.zero;
    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = new Color(0, 1, 0, 0.5f);
    Gizmos.DrawSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius);
  }
}
