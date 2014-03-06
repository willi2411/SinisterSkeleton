using UnityEngine;
using System.Collections;

public class CloneLifeSpan : MonoBehaviour {

	public float lifespan;
	
	void Awake() {
		
		Destroy(gameObject, lifespan);
		
	}
}
