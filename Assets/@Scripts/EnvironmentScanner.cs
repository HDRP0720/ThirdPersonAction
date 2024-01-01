using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnvironmentScanner : MonoBehaviour
{
  [SerializeField] private Vector3 _forwardRayOffset = new Vector3(0, 0.25f, 0);
  [SerializeField] private float _forwardRayLength = 0.8f;
  [SerializeField] private float _heightRayLength = 5f;
  [SerializeField] private float _ledgeRayLength = 10f;
  [SerializeField] private float _ledgeHeightThreshHold = 0.75f;
  [SerializeField] private LayerMask _obstacleLayer;
  
  public ObstacleHitData CheckObstacle()
  {
    var hitData = new ObstacleHitData();
    
    var forwardRayOrigin = transform.position + _forwardRayOffset;
    hitData.isForwardHitFound = Physics.Raycast(transform.position + _forwardRayOffset, transform.forward, 
                                                out hitData.forwardHit, _forwardRayLength, _obstacleLayer);
    
    // Debug.DrawRay(forwardRayOrigin, transform.forward * _forwardRayLength, hitData.isForwardHitFound ? Color.red: Color.white);

    if (hitData.isForwardHitFound)
    {
      var heightRayOrigin = hitData.forwardHit.point + Vector3.up * _heightRayLength;
      hitData.isHeightHitFound = Physics.Raycast(heightRayOrigin, Vector3.down, 
                                                out hitData.heightHit, _heightRayLength, _obstacleLayer);
      
      Debug.DrawRay(heightRayOrigin, Vector3.down*_heightRayLength, hitData.isHeightHitFound ? Color.red: Color.white);
    }

    return hitData;
  }

  public bool IsOnLedge(Vector3 moveDir, out LedgeData ledgeData)
  {
    ledgeData = new LedgeData();
    
    if (moveDir == Vector3.zero) return false;
  
    float originOffset = 0.5f;
    var origin = transform.position + moveDir * originOffset + Vector3.up;
    if (PhysicsUtil.ThreeRaycasts(origin, Vector3.down, 0.3f, transform, out List<RaycastHit> hits, _ledgeRayLength, _obstacleLayer, true))
    {
      var validHits = hits.Where(h => transform.position.y - h.point.y > _ledgeHeightThreshHold).ToList();
      if (validHits.Count > 0)
      {
        // var surfaceRayOrigin = transform.position + moveDir - new Vector3(0, 0.1f, 0);
        var surfaceRayOrigin = validHits[0].point;
        surfaceRayOrigin.y = transform.position.y - 0.1f;
        if (Physics.Raycast(surfaceRayOrigin, transform.position - surfaceRayOrigin, out RaycastHit surfaceHit, 2f, _obstacleLayer))
        {
          Debug.DrawLine(surfaceRayOrigin, transform.position, Color.cyan);
          
          float height = transform.position.y - validHits[0].point.y;
        
          ledgeData.angle = Vector3.Angle(transform.forward, surfaceHit.normal);
          ledgeData.height = height;
          ledgeData.surfaceHit = surfaceHit;
          return true;
        }
      }
    }

    return false;
  }
}

public struct ObstacleHitData
{
  public bool isForwardHitFound;
  public bool isHeightHitFound;
  public RaycastHit forwardHit;
  public RaycastHit heightHit;
}

public struct LedgeData
{
  public float height;
  public float angle;
  public RaycastHit surfaceHit;
}
