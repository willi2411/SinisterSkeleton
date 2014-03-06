using UnityEngine;
using System.Collections;

public class PlayerAnimation : MonoBehaviour {

	public float runSpeedScale = 1.0f;

	// Use this for initialization
	void Start () {
		// By default loop all animations
		animation.wrapMode = WrapMode.Loop;
		
		animation["run"].layer = -1;
		animation["idle"].layer = -1;

		animation.SyncLayer(-1);

		AnimationState attack = animation["attack"];
		attack.wrapMode = WrapMode.Once;

		AnimationState Gothit = animation["gethit"];
		Gothit.wrapMode = WrapMode.Once;



		animation.Stop();
		animation.Play("idle");
		}

	void Update()
	{
		PlayerController playerController = GetComponent<PlayerController>();
		var moveSpeed = playerController.GetSpeed();


		if (moveSpeed > .1f) 
			animation.CrossFade ("run");
		else
			animation.CrossFade("idle");


		animation["run"].normalizedSpeed = runSpeedScale;





				}
	}


