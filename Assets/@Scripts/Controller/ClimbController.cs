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
      StartCoroutine(CoJumpToLedge("Idle To Braced Hang", ledgeHit.transform, 0.41f, 0.54f, handOffset: new Vector3(0.2f, 0.05f, 0.05f)));
    }
    else
    {
      if (Input.GetButton("Drop") && !_player.IsInAction)
      {
        StartCoroutine(CoJumpFromHang());
        return;
      }
     
      float h = Mathf.Round(Input.GetAxisRaw("Horizontal"));
      float v = Mathf.Round(Input.GetAxisRaw("Vertical"));
      var inputDir = new Vector2(h, v);
      
      if(_player.IsInAction || inputDir == Vector2.zero) return;
      
      // Mount from the hanging state
      if (currentPoint.IsMountPoint && Mathf.Approximately(inputDir.y, 1))
      {
        StartCoroutine(CoMountFromHang());
        return;
      }
      
      // Ledge to ledge jump
      var neighbour = currentPoint.GetNeighbour(inputDir);
      if (neighbour == null) return;

      if (neighbour.connectionType == EConnectionType.Jump && Input.GetButton("Jump"))
      {
        currentPoint = neighbour.point;
        
        if (Mathf.Approximately(neighbour.direction.y, 1))
          StartCoroutine(CoJumpToLedge("Braced Hang Hop Up", currentPoint.transform, 0.35f, 0.65f));
        else if (Mathf.Approximately(neighbour.direction.y, -1))
          StartCoroutine(CoJumpToLedge("Braced Hang Drop", currentPoint.transform, 0.31f, 0.65f, handOffset: new Vector3(0.25f, 0.1f, 0.1f)));
        else if (Mathf.Approximately(neighbour.direction.x, 1))
          StartCoroutine(CoJumpToLedge("Braced Hang Hop Right", currentPoint.transform, 0.20f, 0.50f, handOffset: new Vector3(0.25f, 0.05f, 0.1f)));
        else if (Mathf.Approximately(neighbour.direction.x, -1))
          StartCoroutine(CoJumpToLedge("Braced Hang Hop Left", currentPoint.transform, 0.20f, 0.50f, handOffset: new Vector3(0.25f, 0.05f, 0.1f)));
      }
      else if (neighbour.connectionType == EConnectionType.Move)
      {
        currentPoint = neighbour.point;
        
        if (Mathf.Approximately(neighbour.direction.x, 1))
          StartCoroutine(CoJumpToLedge("Right Braced Hang Shimmy", currentPoint.transform, 0f, 0.38f, handOffset: new Vector3(0.25f, 0.05f, 0.1f)));
        else if (Mathf.Approximately(neighbour.direction.x, -1))
          StartCoroutine(CoJumpToLedge("Left Braced Hang Shimmy", currentPoint.transform, 0f, 0.38f, AvatarTarget.LeftHand, handOffset: new Vector3(0.25f, 0.05f, 0.1f)));
      }
    }
  }

  private IEnumerator CoJumpToLedge(string anim, Transform ledge, float matchStartTime, float matchEndTime, 
    AvatarTarget matchHand = AvatarTarget.RightHand, Vector3? handOffset = null)
  {
    var matchParams = new MatchTargetParams()
    {
      matchPos = GetHandPos(ledge, matchHand, handOffset),
      matchBodyPart = matchHand,
      matchPosWeight = Vector3.one,
      matchStartTime = matchStartTime,
      matchEndTime = matchEndTime
    };

    var targetRot = Quaternion.LookRotation(-ledge.forward);

    yield return _player.CoAction(anim, matchParams, targetRot, true);

    _player.IsHanging = true;
  }

  private IEnumerator CoJumpFromHang()
  {
    _player.IsHanging = false;
    
    yield return _player.CoAction("Jump From Wall");
    
    _player.ResetTargetRotation();
    _player.SetControl(true);
  }

  private IEnumerator CoMountFromHang()
  {
    _player.IsHanging = false;
    
    yield return _player.CoAction("Braced Hang To Crouch");
    
    // _player.EnableCharacterController(true);

    yield return new WaitForSeconds(0.5f);
    
    _player.ResetTargetRotation();
    _player.SetControl(true);
  }

  private Vector3 GetHandPos(Transform ledge, AvatarTarget matchHand, Vector3? handOffset)
  {
    var offVal = handOffset ?? new Vector3(0.25f, 0.05f, 0.15f);
    var horizontalDir = (matchHand == AvatarTarget.RightHand) ? ledge.right : -ledge.right;
    
    return ledge.position + ledge.forward * offVal.z + Vector3.up * offVal.y - horizontalDir * offVal.x;
  }
}
