using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
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

  private bool _isGrounded;
  private float _ySpeed;
  
  private static readonly int MoveAmount = Animator.StringToHash("moveAmount");

  private void Awake()
  {
    if (Camera.main != null) 
      _cameraController = Camera.main.GetComponent<CameraController>();
    
    _cc = GetComponent<CharacterController>();
    _animator = GetComponent<Animator>();
    _meeleCombat = GetComponent<MeeleCombat>();
  }
  private void Update()
  {
    if (_meeleCombat.IsInAction)
    {
      _animator.SetFloat(MoveAmount, 0f);
      return;
    }
    
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    float moveAmount =Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

    var moveInput = new Vector3(h, 0, v).normalized;
    var moveDir = _cameraController.GetPlanarRotation * moveInput;
    
    CheckGround();

    if (_isGrounded)
      _ySpeed = -0.5f;
    else
      _ySpeed += Physics.gravity.y * Time.deltaTime;

    var velocity = moveDir * _moveSpeed;
    velocity.y = _ySpeed;
    
    _cc.Move(velocity * Time.deltaTime);
    
    if (moveAmount > 0)
    {
      _targetRotation = Quaternion.LookRotation(moveDir);
    }

    transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);
    _animator.SetFloat(MoveAmount, moveAmount, 0.2f, Time.deltaTime);
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
