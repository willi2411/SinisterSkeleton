using UnityEngine;
using System.Collections;

public class walkInPlace : MonoBehaviour {

		
	// Update is called once per frame
	void Start(){
		animation ["Die"].wrapMode = WrapMode.Once;
		}

	void play () {

		animation.CrossFade("Die");
	}
}
