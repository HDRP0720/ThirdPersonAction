using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/New custom action")]
public class VaultData : ParkourData
{
  public override bool CheckIfPossible(ObstacleHitData hitData, Transform player)
  {
    if (!base.CheckIfPossible(hitData, player))
      return false;

    var hitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);
    if (hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0)
    {
      // UnMirror
      IsMirror = false;
      _matchBodyPart = AvatarTarget.RightHand;
    }
    else
    {
      // Mirror
      IsMirror = true;
      _matchBodyPart = AvatarTarget.LeftHand;
    }

    return true;
  }
}