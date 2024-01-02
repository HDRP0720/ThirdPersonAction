using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
  private EnvironmentScanner _environmentScanner;

  private void Awake()
  {
    _environmentScanner = GetComponent<EnvironmentScanner>();
  }
  private void Update()
  {
    if (Input.GetButton("Jump"))
    {
      if (_environmentScanner.IsNearClimbLedge(transform.forward, out RaycastHit ledgeHit))
      {
        Debug.Log("Climb ledge found!!!");
      }
    }
  }
}
