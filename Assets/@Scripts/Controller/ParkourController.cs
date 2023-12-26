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
      Debug.LogError("The parkour Data's anim name and animation name aren't matched. Please Check!!!");

    yield return new WaitForSeconds(animState.length);
    
    _player.SetControl(true);
    _isInAction = false;
  }
}
