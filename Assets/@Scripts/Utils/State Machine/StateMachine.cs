using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
  private T _owner;
  
  // Property
  public State<T> CurrentState { get; private set; }
  
  // Constructor
  public StateMachine(T owner)
  {
    _owner = owner;
  }

  public void ChangeState(State<T> newState)
  {
    if(CurrentState != null)
      CurrentState.Exit();
    
    CurrentState = newState;
    CurrentState.Enter(_owner);
  }

  public void Execute()
  {
    if(CurrentState != null)
      CurrentState.Execute();
  }
}
