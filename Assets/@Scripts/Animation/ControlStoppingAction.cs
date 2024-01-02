using UnityEngine;

public class ControlStoppingAction : StateMachineBehaviour
{
  private PlayerController _player;
  
  public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    if(_player == null)
      _player = animator.GetComponent<PlayerController>();
        
    _player.HasControl = false;
  }
  
  public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    _player.HasControl = true;
  }
}
