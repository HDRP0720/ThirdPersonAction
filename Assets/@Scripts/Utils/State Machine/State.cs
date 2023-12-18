using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<T> : MonoBehaviour
{
  public virtual void Enter(T owner) { }

  public virtual void Execute()
  {
    Debug.Log($"실행중인 상태:{this}");
  }

  public virtual void Exit() { }
}
