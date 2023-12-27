using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
  [SerializeField] private List<ParkourData> _parkourData;
  
  private Animator _animator;
  private PlayerController _player;
  private EnvironmentScanner _environmentScanner;

  private bool _isInAction;

  private void Awake()
  {
    _animator = GetComponent<Animator>();
    _player = GetComponent<PlayerController>();
    _environmentScanner = GetComponent<EnvironmentScanner>();
  }
  private void Update()
  {
    if (Input.GetButton("Jump") && !_isInAction)
    {
      var hitData = _environmentScanner.CheckObstacle();
      if (hitData.isForwardHitFound)
      {
        foreach (var data in _parkourData)
        {
          if (data.CheckIfPossible(hitData, transform))
          {
            StartCoroutine(CoParkourAction(data));
            break;
          }
        }
      }
    }
  }

  private IEnumerator CoParkourAction(ParkourData data)
  {
    _isInAction = true;
    _player.SetControl(false);
    
    _animator.CrossFade(data.GetAnimationClipName, 0.2f);
    yield return null;

    var animState = _animator.GetNextAnimatorStateInfo(0);
    if(!animState.IsName(data.GetAnimationClipName))
      Debug.LogError("The parkour data's animation name does not match the specified animation.");

    float timer = 0f;
    while (timer <= animState.length)
    {
      timer += Time.deltaTime;
      if (data.ShouldRotateToObstacle)
        transform.rotation = Quaternion.RotateTowards(transform.rotation, data.TargetRotation, 
                                        _player.GetRotationSpeed * Time.deltaTime);
      
      if(data.CanTargetMatching)
        MatchTarget(data);

      yield return null;
    }
    
    _player.SetControl(true);
    _isInAction = false;
  }

  private void MatchTarget(ParkourData data)
  {
    if (_animator.isMatchingTarget) return;
    
    _animator.MatchTarget(data.MatchPos, transform.rotation, data.MatchBodyPart, 
      new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), data.MatchStartTime, data.MatchEndTime);
  }
}
