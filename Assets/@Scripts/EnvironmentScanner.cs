using System;
using UnityEngine;
using UnityEngine.Serialization;

public class EnvironmentScanner : MonoBehaviour
{
  [SerializeField] private Vector3 _forwardRayOffset = new Vector3(0, 0.25f, 0);
  [SerializeField] private float _forwardRayLength = 0.8f;
  [SerializeField] private float _heightRayLength = 5f;
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
}

public struct ObstacleHitData
{
  public bool isForwardHitFound;
  public bool isHeightHitFound;
  public RaycastHit forwardHit;
  public RaycastHit heightHit;
}