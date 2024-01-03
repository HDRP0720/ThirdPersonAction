using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
  private PlayerController _player;
  private EnvironmentScanner _environmentScanner;

  private void Awake()
  {
    _player = GetComponent<PlayerController>();
    _environmentScanner = GetComponent<EnvironmentScanner>();
  }
  private void Update()
  {
    if (!_player.IsHanging)
    {
      if (!Input.GetButton("Jump") || _player.IsInAction) return;
      
      if (!_environmentScanner.IsNearClimbLedge(transform.forward, out RaycastHit ledgeHit)) return;
      
      _player.SetControl(false);
      StartCoroutine(CoJumpToLedge("Idle To Braced Hang", ledgeHit.transform, 0.41f, 0.54f));
    }
    else
    {
      // TODO: Ledge to ledge jump
    }
  }

  private IEnumerator CoJumpToLedge(string anim, Transform ledge, float matchStartTime, float matchEndTime)
  {
    var matchParams = new MatchTargetParams()
    {
      matchPos = GetHandPos(ledge),
      matchBodyPart = AvatarTarget.RightHand,
      matchPosWeight = Vector3.one,
      matchStartTime = matchStartTime,
      matchEndTime = matchEndTime
    };

    var targetRot = Quaternion.LookRotation(-ledge.forward);

    yield return _player.CoAction(anim, matchParams, targetRot, true);

    _player.IsHanging = true;
  }

  private Vector3 GetHandPos(Transform ledge)
  {
    return ledge.position + ledge.forward * 0.1f + Vector3.up * 0.1f - ledge.right * 0.2f;
  }
}
