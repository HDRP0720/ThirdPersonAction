using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New parkour action")]
public class ParkourData : ScriptableObject
{
  #region Variables
  [SerializeField] private AnimationClip  _animationClip;   // 재생할 animation의 클립 
  [SerializeField] private float _minHeight;                // animation이 재생될 최소 제한 높이 (character controller의 step offset 보다 높아야함)
  [SerializeField] private float _maxHeight;                // animation이 재생될 최대 제한 높이 
  [SerializeField] private bool _shouldRotateToObstacle;    // animation이 재생될 때, 캐릭터를 obstacle로 회전 시킬지 여부를 결정
  
  [Header("Target Matching Settings")][Space]
  [SerializeField] private bool _canTargetMatching = true;  // target matching을 할지 여부를 결정
  [SerializeField] private AvatarTarget _matchBodyPart;     // target matching 적용 부위를 결정
  [SerializeField] private float _matchStartTime;           // target mathcing이 시작될 애니메이션 클립의 시간 (퍼센트를 백분율로 )
  [SerializeField] private float _matchEndTime;             // target mathcing이 적용될 애니메이션 클립의 시간 (퍼센트를 백분율로 )
  #endregion

  #region Properties
  public AnimationClip GetAnimClip => _animationClip;
  public string GetAnimationClipName => _animationClip.name;
  public bool ShouldRotateToObstacle => _shouldRotateToObstacle;
  public Quaternion TargetRotation { get; set; }
  public bool CanTargetMatching => _canTargetMatching;
  public AvatarTarget MatchBodyPart => _matchBodyPart;
  public float MatchStartTime => _matchStartTime;
  public float MatchEndTime => _matchEndTime;
  public Vector3 MatchPos { get; set; }
  #endregion

  public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
  {
    float height = hitData.heightHit.point.y - player.position.y;
    if (height < _minHeight || height > _maxHeight) 
      return false;

    if (_shouldRotateToObstacle)
      TargetRotation = Quaternion.LookRotation(-hitData.forwardHit.normal);

    if (_canTargetMatching)
      MatchPos = hitData.heightHit.point;

    return true;
  }
}
