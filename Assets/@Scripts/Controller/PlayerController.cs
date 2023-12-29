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

  private bool _hasControl = true;
  private bool _isGrounded;
  private float _ySpeed;
  
  // Property
  public float GetRotationSpeed => _rotationSpeed;
  public Vector3 InputDir { get; private set; }
  public bool IsOnLedge { get; set; }
  
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
    var moveDir = _cameraController.GetPlanarRotation * moveInput;
    InputDir = moveDir;
    
    if (!_hasControl) return;
    
    CheckGround();

    if (_isGrounded)
    {
      _ySpeed = -0.5f;

      IsOnLedge = _environmentScanner.IsNearLedge(moveDir);
      if(IsOnLedge)
        Debug.Log("I'm On Ledge!!!");
    }
    else
    {
      _ySpeed += Physics.gravity.y * Time.deltaTime;
    }
    
    var velocity = moveDir * _moveSpeed;
    
    // 전투 또는 에너미 타겟이 설정되면 Lock-On 모드로 변경
    if (_combatController.IsCombatMode)
    {
      velocity /= 4f;
      
      var targetVec = _combatController.TargetEnemy.transform.position - transform.position;
      targetVec.y = 0f;

      if (moveAmount > 0)
      {
        _targetRotation = Quaternion.LookRotation(targetVec);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);
      }
    
      float forwardSpeed = Vector3.Dot(velocity, transform.forward);
      _animator.SetFloat(ForwardSpeed, forwardSpeed / _moveSpeed, 0.2f, Time.deltaTime);

      float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
      float strafeSpeed = Mathf.Sin(angle * Mathf.Deg2Rad);
      _animator.SetFloat(StrafeSpeed, strafeSpeed, 0.2f, Time.deltaTime);
    }
    else
    {
      if (moveAmount > 0)
        _targetRotation = Quaternion.LookRotation(moveDir);

      transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);
      _animator.SetFloat(ForwardSpeed, moveAmount, 0.2f, Time.deltaTime);
    }
    
    velocity.y = _ySpeed;
    _cc.Move(velocity * Time.deltaTime);
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
  
  private void CheckGround()
  {
    _isGrounded = Physics.CheckSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius, _groundLayer);
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = new Color(0, 1, 0, 0.5f);
    Gizmos.DrawSphere(transform.TransformPoint(_groundCheckOffset), _groundCheckRadius);
  }
}
