using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
  private Animator _animator;
  private PlayerController _player;
  private EnvironmentScanner _environmentScanner;

  private bool _isInAction;

  private void Awake()
  {
    _animator = GetComponent<Animator>();
    _player = GetComponent<PlayerController>();
    _environmentScanner = GetComponent<EnvironmentScanner>();
  }
  private void Update()
  {
    if (Input.GetButton("Jump") && !_isInAction)
    {
      var hitData = _environmentScanner.CheckObstacle();
      if (hitData.isForwardHitFound)
      {
        StartCoroutine(CoParkourAction());
      }
    }
  }

  private IEnumerator CoParkourAction()
  {
    _isInAction = true;
    _player.SetControl(false);
    
    _animator.CrossFade("StepUp", 0.2f);
    yield return null;

    var animState = _animator.GetNextAnimatorStateInfo(0);

    yield return new WaitForSeconds(animState.length);
    
    _player.SetControl(true);
    _isInAction = false;
  }
}
