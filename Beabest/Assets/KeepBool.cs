using UnityEngine;

public class KeepBool : StateMachineBehaviour
{

	public string boolName;
	public bool status;
	public bool resetOnExit = true;
	
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetBool(boolName,status);
	}

	private void OnStateExit(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
	{
		if (resetOnExit)
			animator.SetBool(boolName,!status);
	}
}
