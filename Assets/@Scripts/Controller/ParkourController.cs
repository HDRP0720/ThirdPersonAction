using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
  private EnvironmentScanner _environmentScanner;

  private void Awake()
  {
    _environmentScanner = GetComponent<EnvironmentScanner>();
  }
  private void Update()
  {
    var hitData = _environmentScanner.CheckObstacle();
    if (hitData.isForwardHitFound)
    {
      Debug.Log($"장애물 발견: {hitData.forwardHit.transform.name}");
    }
  }
}
