using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	
	public Transform target;
	public float distance = -10f;
	public float lift = 1.5f;
	
	// Update is called once per frame
	void Update () {
		transform.position = target.position + new Vector3(0,lift,distance);
		transform.LookAt (target);
	}
}
