using System;
using UnityEngine;
using UnityEngine.Serialization;

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
    
    Debug.DrawRay(forwardRayOrigin, transform.forward * _forwardRayLength, hitData.isForwardHitFound ? Color.red: Color.white);

    if (hitData.isForwardHitFound)
    {
      var heightRayOrigin = hitData.forwardHit.point + Vector3.up * _heightRayLength;
      hitData.isHeightHitFound = Physics.Raycast(heightRayOrigin, Vector3.down, 
                                                out hitData.heightHit, _heightRayLength, _obstacleLayer);
      
      Debug.DrawRay(heightRayOrigin, Vector3.down*_heightRayLength, hitData.isHeightHitFound ? Color.red: Color.white);
    }

    return hitData;
  }

  public bool IsNearLedge(Vector3 moveDir, out LedgeData ledgeData)
  {
    ledgeData = new LedgeData();
    if (moveDir == Vector3.zero) return false;
  
    float originOffset = 0.5f;
    var origin = transform.position + moveDir * originOffset + Vector3.up;
    
    if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, _ledgeRayLength, _obstacleLayer))
    {
      Debug.DrawRay(origin, Vector3.down*_ledgeRayLength, Color.green);

      var surfaceRayOrigin = transform.position + moveDir - new Vector3(0, 0.1f, 0);
      if (Physics.Raycast(surfaceRayOrigin, -moveDir, out RaycastHit surfaceHit, 2f, _obstacleLayer))
      {
        float height = transform.position.y - hit.point.y;
        if (height > _ledgeHeightThreshHold)
        {
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
