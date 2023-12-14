using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionSensor : MonoBehaviour
{
  [SerializeField] private EnemyController _enemy;
  
  private void OnTriggerEnter(Collider other)
  {
    var fighter = other.GetComponent<MeeleCombat>();
    if (fighter != null)
      _enemy.TargetsInRange.Add(fighter);
  }
  private void OnTriggerExit(Collider other)
  {
    var fighter = other.GetComponent<MeeleCombat>();
    if (fighter != null)
      _enemy.TargetsInRange.Remove(fighter);
  }
}
