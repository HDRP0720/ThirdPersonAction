using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New parkour action")]
public class ParkourData : ScriptableObject
{
  #region Variables
  [SerializeField] private AnimationClip  _animationClip;   // 재생할 animation의 클립
  [SerializeField] private string _obstacleTag;             // 특정 태그에 따라 다른 모션 활성화
  [SerializeField] private float _minHeight;                // animation이 재생될 최소 제한 높이 (character controller의 step offset 보다 높아야함)
  [SerializeField] private float _maxHeight;                // animation이 재생될 최대 제한 높이 
  [SerializeField] private bool _shouldRotateToObstacle;    // animation이 재생될 때, 캐릭터를 obstacle로 회전 시킬지 여부를 결정
  [SerializeField] private float _postAnimDelay;            // 추가 애니메이션이 존재하여 input 조작 시간을 조절해야할 경우 사용 (예시. wall climb 참조)  
  
  [Header("Target Matching Settings")][Space]
  [SerializeField] private bool _canTargetMatching = true;  // target matching을 할지 여부를 결정
  [SerializeField] protected AvatarTarget _matchBodyPart;   // target matching 적용 부위를 결정
  [SerializeField] private float _matchStartTime;           // target mathcing이 시작될 애니메이션 클립의 시간 (퍼센트를 백분율로 )
  [SerializeField] private float _matchEndTime;             // target mathcing이 적용될 애니메이션 클립의 시간 (퍼센트를 백분율로 )
  [SerializeField] private Vector3 _matchPosWeight = new Vector3(0, 1, 0);
  #endregion

  #region Properties
  public AnimationClip AnimClip => _animationClip;
  public string AnimationClipName => _animationClip.name;
  public bool ShouldRotateToObstacle => _shouldRotateToObstacle;
  public float PostAnimDelay => _postAnimDelay;
  public Quaternion TargetRotation { get; set; }
  public bool CanTargetMatching => _canTargetMatching;
  public AvatarTarget MatchBodyPart => _matchBodyPart;
  public float MatchStartTime => _matchStartTime;
  public float MatchEndTime => _matchEndTime;
  public Vector3 MatchPosWeight => _matchPosWeight;
  public Vector3 MatchPos { get; set; }
  public bool IsMirror { get; set; }
  #endregion

  public virtual bool CheckIfPossible(ObstacleHitData hitData, Transform player)
  {
    // Compare tag value
    if (!string.IsNullOrEmpty(_obstacleTag) && !hitData.forwardHit.transform.CompareTag(_obstacleTag))
      return false;
      
    // Compare Height value
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
