using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
  [SerializeField] private List<ParkourData> _parkourData;
  [SerializeField] private ParkourData _jumpDownData;
  [SerializeField] private float _autoJumpHeightLimit = 1f;   // 지정된 높이값 이하에선 auto-jump
  
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
    var hitData = _environmentScanner.CheckObstacle();
    
    if (Input.GetButton("Jump") && !_isInAction)
    {
      if (hitData.isForwardHitFound)
      {
        foreach (var data in _parkourData)
        {
          if (data.CheckIfPossible(hitData, transform))
          {
            StartCoroutine(CoParkourAction(data));
            break;
          }
        }
      }
    }
    
    if (_player.IsOnLedge && !_isInAction && !hitData.isForwardHitFound)
    {
      bool shouldAutoJump = !(_player.LedgeData.height > _autoJumpHeightLimit && !Input.GetButton("Jump"));
      
      if (shouldAutoJump && _player.LedgeData.angle <= 50)
      {
        _player.IsOnLedge = false;
        StartCoroutine(CoParkourAction(_jumpDownData));
      }
    }
  }

  private IEnumerator CoParkourAction(ParkourData data)
  {
    _isInAction = true;
    _player.SetControl(false);
    
    _animator.SetBool("IsMirror", data.IsMirror);
    _animator.CrossFade(data.AnimationClipName, 0.2f);
    yield return null;

    var animState = _animator.GetNextAnimatorStateInfo(0);
    if(!animState.IsName(data.AnimationClipName))
      Debug.LogError("The parkour data's animation name does not match the specified animation.");

    float timer = 0f;
    while (timer <= animState.length)
    {
      timer += Time.deltaTime;
      if (data.ShouldRotateToObstacle)
        transform.rotation = Quaternion.RotateTowards(transform.rotation, data.TargetRotation, 
                                        _player.GetRotationSpeed * Time.deltaTime);
      
      if(data.CanTargetMatching)
        MatchTarget(data);
      
      // vault animation 재생 마지막 부분의 height 값과 실제 값이 다른 경우를 위함
      // 강제로 root motion에서 character controller로 이전 (중력 적용을 위해서)
      if (_animator.IsInTransition(0) && timer > 0.5f)
        break;

      yield return null;
    }
    
    // 연속된 animation 으로 input 조작을 늦춰야할 경우 적용됨
    yield return new WaitForSeconds(data.PostAnimDelay);
    
    _player.SetControl(true);
    _isInAction = false;
  }

  private void MatchTarget(ParkourData data)
  {
    if (_animator.isMatchingTarget) return;
    
    _animator.MatchTarget(data.MatchPos, transform.rotation, data.MatchBodyPart, 
      new MatchTargetWeightMask(data.MatchPosWeight, 0), data.MatchStartTime, data.MatchEndTime);
  }
}
