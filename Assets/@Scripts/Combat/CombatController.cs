using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatController : MonoBehaviour
{
  private MeeleCombat _meeleCombat;

  private void Awake()
  {
    _meeleCombat = GetComponent<MeeleCombat>();
  }
  private void Update()
  {
    if (Input.GetButtonDown("Attack"))
    {
      _meeleCombat.TryToAttack();
    }
  }
}
