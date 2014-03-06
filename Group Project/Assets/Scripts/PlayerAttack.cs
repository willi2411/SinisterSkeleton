using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour {

		public float attackSpeed= 1;
		public float attackHitTime= 0.2f;
		public float attackTime= 0.4f;
		Vector3 attackPosition= new Vector3 (0, 0, 0.8f);
		public float attackRadius= 1.3f;
		public static float attackDamage = 1;
		
		public Transform BulletPrefab;
		public Transform Spawn;	

		private bool busy= false; 
		
				
	public float getAtttackDmgAmt(){
		return attackDamage;
	}
		void  Update (){
			 
			if(!busy && Input.GetButtonDown ("Fire1"))
			{	
				SendMessage ("Didattack");
				busy = true;
			}
		if(!busy && Input.GetButtonDown ("Fire2") && Application.loadedLevelName != "testone")
		{	
			SendMessage ("DidRangeAttack");
			busy = true;
		}
		}
		
	void Start(){

		if (Application.loadedLevelName == "testone")
						attackDamage = 1;

		}

	void gainDamage(int DamageIncrease)
	{
		attackDamage += DamageIncrease;

		
	}

	IEnumerator DidRangeAttack (){
		
		animation.CrossFadeQueued ("attack", 0.1f, QueueMode.PlayNow);
		yield return new WaitForSeconds (attackHitTime);
		
		
		var instanceBullet = (Transform)Instantiate(BulletPrefab, Spawn.position, Quaternion.identity);
		instanceBullet.rigidbody.AddForce(transform.forward * 1000);
		
		
		
		yield return new WaitForSeconds(attackTime - attackHitTime);
		busy = false;
	}


		IEnumerator Didattack (){

			animation.CrossFadeQueued("attack", 0.1f, QueueMode.PlayNow);
			yield return new WaitForSeconds(attackHitTime);
			Vector3 pos= transform.TransformPoint(attackPosition);
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			

			foreach(GameObject go in enemies)
			{
				EnemyDamaged enemy= go.GetComponent<EnemyDamaged>();
				if (enemy == null){
				Debug.Log("enemy damage = null");
				continue;}



				if (Vector3.Distance(enemy.transform.position, pos) < attackRadius)
				{
					enemy.SendMessage("ApplyDamage", attackDamage);
					
				/* Play sound.
					if (attackSound)
						audio.PlayOneShot(attackSound);*/
				}
			}
			yield return new WaitForSeconds(attackTime - attackHitTime);
			busy = false;
		}
		

		void  OnDrawGizmosSelected (){
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere (transform.TransformPoint(attackPosition), attackRadius);
		}
		

}
