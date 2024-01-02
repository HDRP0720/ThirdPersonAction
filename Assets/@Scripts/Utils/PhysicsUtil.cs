using System.Collections.Generic;
using UnityEngine;

public class PhysicsUtil
{
  /// <summary>
  /// ledge 근처에서 벗어나지 않기 위해 Raycast를 3번 사용하여 간격을 맞춰주는 함수
  /// </summary>
  /// <param name="origin">Raycast를 사용할 위치</param>
  /// <param name="dir">Raycast 방향</param>
  /// <param name="spacing">3가지 Raycast들의 간격 </param>
  /// <param name="transform">spacing 간격의 기준이 될 위치</param>
  /// <param name="hits">3가지 RaycastHit가 저장될 리스트 타입 변수</param>
  /// <param name="distance">Raycast가 적용될 거리 범위</param>
  /// <param name="layer">Raycast가 적용될 layer</param>
  /// <param name="showDebugDrawLine">Raycast가 제대로 작동되는지 확인하기 위한 debug 용도</param>
  /// <returns></returns>
  public static bool ThreeRaycasts(Vector3 origin, Vector3 dir, float spacing, Transform transform, 
    out List<RaycastHit> hits, float distance, LayerMask layer, bool showDebugDrawLine = false)
  {
    bool isCenterHitFound = Physics.Raycast(origin, Vector3.down, out RaycastHit centerHit, distance, layer);
    bool isLeftHitFound = Physics.Raycast(origin-transform.right*spacing, Vector3.down, out RaycastHit leftHit, distance, layer);
    bool isRightHitFound = Physics.Raycast(origin+transform.right*spacing, Vector3.down, out RaycastHit rightHit, distance, layer);

    hits = new List<RaycastHit>() { centerHit, leftHit, rightHit };

    bool isHitFound = isCenterHitFound || isLeftHitFound || isRightHitFound;
    if (isHitFound && showDebugDrawLine)
    {
      Debug.DrawLine(origin, centerHit.point, Color.red);
      Debug.DrawLine(origin-transform.right*spacing, leftHit.point, Color.red);
      Debug.DrawLine(origin+transform.right*spacing, rightHit.point, Color.red);
    }
    
    return isHitFound;
  }
}
