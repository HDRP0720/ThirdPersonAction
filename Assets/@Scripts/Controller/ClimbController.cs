using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
  private PlayerController _player;
  private EnvironmentScanner _environmentScanner;

  private ClimbPoint currentPoint;

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

      currentPoint = ledgeHit.transform.GetComponent<ClimbPoint>();
      _player.SetControl(false);
      StartCoroutine(CoJumpToLedge("Idle To Braced Hang", ledgeHit.transform, 0.41f, 0.54f));
    }
    else
    {
      // TODO: Ledge to ledge jump
      float h = Mathf.Round(Input.GetAxisRaw("Horizontal"));
      float v = Mathf.Round(Input.GetAxisRaw("Vertical"));
      var inputDir = new Vector2(h, v);
      
      if(_player.IsInAction || inputDir == Vector2.zero) return;

      var neighbour = currentPoint.GetNeighbour(inputDir);
      if (neighbour == null) return;

      if (neighbour.connectionType == EConnectionType.Jump && Input.GetButton("Jump"))
      {
        currentPoint = neighbour.point;
        
        if (neighbour.direction.y == 1)
          StartCoroutine(CoJumpToLedge("Braced Hang Hop Up", currentPoint.transform, 0.35f, 0.65f));
        else if (neighbour.direction.y == -1)
          StartCoroutine(CoJumpToLedge("Braced Hang Drop", currentPoint.transform, 0.31f, 0.65f));
        else if (neighbour.direction.x == 1)
          StartCoroutine(CoJumpToLedge("Braced Hang Hop Right", currentPoint.transform, 0.20f, 0.50f));
        else if (neighbour.direction.x == -1)
          StartCoroutine(CoJumpToLedge("Braced Hang Hop Left", currentPoint.transform, 0.20f, 0.50f));
        
      }
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
