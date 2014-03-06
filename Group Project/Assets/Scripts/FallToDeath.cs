using UnityEngine;
using System.Collections;

public class FallToDeath : MonoBehaviour {
	
	int maxfalldis = -20;
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y <= maxfalldis)
		{
			/*if(Application.loadedLevelName == "levelone")
			Application.LoadLevel("levelone");
			else*/
				Application.LoadLevel("testone");

		}
	}
}