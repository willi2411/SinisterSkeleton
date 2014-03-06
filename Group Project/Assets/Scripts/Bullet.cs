using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	 


	float attackHitPoints = PlayerAttack.attackDamage /4f; 



	void OnCollisionEnter(Collision  hit){

		Vector3 pos= transform.TransformPoint(0,0,.8f);
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		foreach (GameObject go in enemies) {
			EnemyDamaged enemy = go.GetComponent<EnemyDamaged> ();
		
			if (hit.gameObject.tag == "Enemy" && Vector3.Distance(enemy.transform.position, pos) < 1f) { // compare the object name
			
				enemy.SendMessage("ApplyDamage",attackHitPoints);
				Destroy(gameObject);
			
			}
		}
	}
}
