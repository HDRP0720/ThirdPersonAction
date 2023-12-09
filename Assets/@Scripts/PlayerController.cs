using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
  [SerializeField] private float _moveSpeed = 5f;
  [SerializeField] private float _rotationSpeed = 500f;
  
  private Quaternion _targetRotation;
  
  private CameraController _cameraController;
  private Animator _animator;
  
  private static readonly int MoveAmount = Animator.StringToHash("moveAmount");

  private void Awake()
  {
    if (Camera.main != null) 
      _cameraController = Camera.main.GetComponent<CameraController>();

    _animator = GetComponent<Animator>();
  }
  private void Update()
  {
    float h = Input.GetAxis("Horizontal");
    float v = Input.GetAxis("Vertical");

    float moveAmount =Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

    var moveInput = new Vector3(h, 0, v).normalized;
    var moveDir = _cameraController.GetPlanarRotation * moveInput;

    if (moveAmount > 0)
    {
      transform.position += moveDir * (_moveSpeed * Time.deltaTime);
      _targetRotation = Quaternion.LookRotation(moveDir);
    }

    transform.rotation = Quaternion.RotateTowards(transform.rotation, _targetRotation, _rotationSpeed * Time.deltaTime);
    _animator.SetFloat(MoveAmount, moveAmount, 0.2f, Time.deltaTime);
  }
}
