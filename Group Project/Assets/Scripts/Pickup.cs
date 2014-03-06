using UnityEngine;
using System.Collections;

public class Pickup : MonoBehaviour {


	void OnControllerColliderHit(ControllerColliderHit hit){
				if (hit.transform.name == "Hp(Clone)") { // compare the object name
						
						
				SendMessage("gainHealth",1f);
				Destroy (hit.gameObject);
				//SendMessage ("AddScore", 1);

				}
		if (hit.transform.name == "Damageplus(Clone)") {
			SendMessage("gainDamage" ,1);
			Destroy (hit.gameObject);
			SendMessage ("AddDamage", 1);

				}
		if (hit.transform.name == "HealthPot(Clone)") {

			SendMessage("AddHealthPot", 1);
			Destroy (hit.gameObject);
		
		}
	
	}

}