using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New parkour action")]
public class ParkourData : ScriptableObject
{
  [SerializeField] private AnimationClip  _animationClip;
  [SerializeField] private float _minHeight;              // character controller의 step offset 보다 높아야함
  [SerializeField] private float _maxHeight;
  
  // Property
  public AnimationClip GetAnimClip => _animationClip;
  public string GetAnimationClipName => _animationClip.name;

  public bool CheckIfPossible(ObstacleHitData hitData, Transform player)
  {
    float height = hitData.heightHit.point.y - player.position.y;
    if (height < _minHeight || height > _maxHeight) 
      return false;

    return true;
  }
}
