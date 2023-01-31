using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TransitionController : MonoBehaviour {
	#region Variables
#pragma warning disable 0649
	// Reference
	[SerializeField]
	private Animator animComp;

	// Control
	private bool initialized = false;                               // Bool to check whether fade control has been initialized

	private int numberOfTransitions;

	// Animation hash
	private int transitionInHash;
	private int transitionOutHash;
	private int transitionHash;
#pragma warning restore 0649
	#endregion

	// Use this for initialization
	private void Start () {
		// Initialize fade controller
		InitTransitionControl();
	}

	#region Transition functions
	public void ChooseRandomTransitionIn () {
		InitTransitionControl();

		animComp.SetInteger(transitionHash, Random.Range(0, numberOfTransitions));
		animComp.SetTrigger(transitionInHash);
	}

	public void ChooseRandomTransitionOut () {
		InitTransitionControl();

		animComp.SetInteger(transitionHash, Random.Range(0, Random.Range(0, numberOfTransitions)));
		animComp.SetTrigger(transitionOutHash);
	}
	#endregion

	#region Helper functions
	// Initialize transition controller
	private void InitTransitionControl () {
		if (!initialized) {
			// Initialize variables
			initialized = true;

			numberOfTransitions = 1;

			transitionInHash = Animator.StringToHash("TransitionIn");
			transitionOutHash = Animator.StringToHash("TransitionOut");
			transitionHash = Animator.StringToHash("Transition");
		}
	}

	// When script is disabled
	private void OnDisable () {
		// Cancel all invokes
		CancelInvoke();
	}
	#endregion
}
