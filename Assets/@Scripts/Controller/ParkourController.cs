using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
  #region Variables
  [SerializeField] private List<ParkourData> _parkourData;
  [SerializeField] private ParkourData _jumpDownData;
  [SerializeField] private float _autoJumpHeightLimit = 1f;   // 지정된 높이값 이하에선 auto-jump
  
  private Animator _animator;
  private PlayerController _player;
  private EnvironmentScanner _environmentScanner;
  #endregion
  
  private void Awake()
  {
    _animator = GetComponent<Animator>();
    _player = GetComponent<PlayerController>();
    _environmentScanner = GetComponent<EnvironmentScanner>();
  }
  private void Update()
  {
    var hitData = _environmentScanner.CheckObstacle();
    
    if (Input.GetButton("Jump") && !_player.IsInAction && !_player.IsHanging)
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
    
    if (_player.IsOnLedge && !_player.IsInAction && !hitData.isForwardHitFound)
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
    _player.SetControl(false);

    MatchTargetParams mp = null;
    if (data.CanTargetMatching)
    {
      mp = new MatchTargetParams()
      {
        matchPos = data.MatchPos,
        matchBodyPart = data.MatchBodyPart,
        matchPosWeight = data.MatchPosWeight,
        matchStartTime = data.MatchStartTime,
        matchEndTime = data.MatchEndTime
      };
    }

    yield return _player.CoAction(data.AnimationClipName, mp, data.TargetRotation, data.ShouldRotateToObstacle, data.PostAnimDelay, data.IsMirror);
    
    _player.SetControl(true);
  }

  // private void MatchTarget(ParkourData data)
  // {
  //   if (_animator.isMatchingTarget) return;
  //   
  //   _animator.MatchTarget(data.MatchPos, transform.rotation, data.MatchBodyPart, 
  //     new MatchTargetWeightMask(data.MatchPosWeight, 0), data.MatchStartTime, data.MatchEndTime);
  // }
}
