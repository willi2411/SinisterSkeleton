using UnityEngine;
using System.Collections;

public class MagePlayerAttack : MonoBehaviour {
	
	public float attackSpeed= 1;
	public float attackHitTime= 0.2f;
	public float attackTime= 0.4f;

	

	private bool busy= false; 

	//bullettype
	public Transform BulletPrefab;
	public Transform Spawn;
	
	void  Update (){
		
		if(!busy && Input.GetButtonDown ("Fire1"))
		{	
			SendMessage ("Didattack");
			busy = true;
		}
	}
	
	IEnumerator Didattack (){
		
				animation.CrossFadeQueued ("attack", 0.1f, QueueMode.PlayNow);
				yield return new WaitForSeconds (attackHitTime);
				
		
			var instanceBullet = (Transform)Instantiate(BulletPrefab, Spawn.position, Quaternion.identity);
			instanceBullet.rigidbody.AddForce(transform.forward * 2000);
				


		yield return new WaitForSeconds(attackTime - attackHitTime);
		busy = false;
	}


	void  OnDrawGizmosSelected (){
		Gizmos.color = Color.yellow;
		//Gizmos.DrawWireSphere (transform.TransformPoint(attackPosition), attackRadius);
	}
	
	
}
