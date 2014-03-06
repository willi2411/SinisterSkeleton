using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class AttackFrdSkl : MonoBehaviour {
		
		public float attackTurnTime= 0.7f;
		public float rotateSpeed= 120.0f;
		public float attackDistance= 5.0f;
		public float extraRunTime= 1f;
		public float damage= 1;
		
		public float movementSpeed= 1.0f;
		public float attackRotateSpeed= 20.0f;
		
		public float idleTime= 1.6f;
		
		public Vector3 punchPosition= new Vector3 (0.4f, 0, 0.7f);
		public float punchRadius= 1.1f;
		
		private float lastPunchTime= 0.0f;
		
		 Transform target;
		private Vector3 tarPos;	
		private EnemyDamaged dead;
		private float runSpeedScale = .5f;
		// Cache a reference to the controller
		CharacterController characterController; 
		
		IEnumerator Start(){
			
			characterController  = GetComponent<CharacterController>();
			dead = GetComponent<EnemyDamaged>();
			
			
				target = GameObject.FindWithTag("FriendlyNPC").transform;
			
			animation.Play("Idle");
			animation["Run"].wrapMode = WrapMode.Loop;
			animation ["Attack_Medium_01"].wrapMode = WrapMode.Once;
			animation["Attack_Medium_01"].layer = 2;
			animation["Idle"].layer = 1;
			animation["Walk"].layer = -1;
			
			yield return new WaitForSeconds(Random.value);
			
			
			while (true)	
			{
				// Don't do anything when idle. And wait for player to be in range!
				// This is the perfect time for the player to attack us
				yield return StartCoroutine (wander());
				//wander();
				
				// Prepare, turn to player and attack him
				
				yield return StartCoroutine( Attack());
				
			}
			
			
		}
		
		void GetNextPosition()
		{
			tarPos = new Vector3(Random.Range(-45f, 45f), 0.5f, Random.Range(-45f, 45f));
		}
		
		IEnumerator  wander (){
			
		animation.CrossFade ("Idle");
			if(Vector3.Distance(tarPos, transform.position) <= 5.0f)
				GetNextPosition();
			
			if (tarPos == Vector3.zero) {
				GetNextPosition();
			}
			// And if the player is really far away.
			// We just wander around until he comes back
			
			while (true)
			{	Vector3 offset= transform.position - target.position;
				// if player is in range again, stop wandering
				// Good Hunting!		
				if (offset.magnitude < attackDistance){		
					return false;
				}
				
				if(Vector3.Distance(tarPos, transform.position) <= 5.0f)
					GetNextPosition();
				
				Quaternion tarRot = Quaternion.LookRotation(tarPos - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, tarRot, rotateSpeed * Time.deltaTime);
				
				transform.Translate(new Vector3(0, 0, movementSpeed * Time.deltaTime+ .1f));
				
				animation.CrossFade ("Walk");
				yield return new WaitForSeconds(0.2f);
			}
		} 
		
		float RotateTowardsPosition ( Vector3 targetPos ,   float rotateSpeed  ){
			// Compute relative point and get the angle towards it
			var relative= transform.InverseTransformPoint(targetPos);
			var angle= Mathf.Atan2 (relative.x, relative.z) * Mathf.Rad2Deg;
			// Clamp it with the max rotation speed
			var maxRotation= rotateSpeed * Time.deltaTime;
			var clampedAngle= Mathf.Clamp(angle, -maxRotation, maxRotation);
			// Rotate
			transform.Rotate(0, clampedAngle, 0);
			// Return the current angle
			return angle;
		}
		
		IEnumerator  Attack (){
			
			// Already queue up the attack run animation but set it's blend wieght to 0
			// it gets blended in later
			// it is looping so it will keep playing until we stop it.
			
			
			animation.CrossFade ("Run");
			
			
			
			// First we wait for a bit so the player can prepare while we turn around
			// As we near an angle of 0, we will begin to move
			float angle;
			angle = 180.0f;
			float time;
			time = 0.0f;
			Vector3 direction;
			
			while (angle > 5 || time < attackTurnTime) {
				time += Time.deltaTime;
				angle = Mathf.Abs (RotateTowardsPosition (target.position, rotateSpeed));
				float move = Mathf.Clamp01 ((90 - angle) / 90);
				// depending on the angle, start moving
				animation ["Run"].weight = animation ["Run"].speed = move;
				direction = transform.TransformDirection (Vector3.forward * movementSpeed * move);
				characterController.SimpleMove (direction);
			}
			
			yield return 0;
			
			// Run towards player
			float timer = 0.0f;
			bool lostSight = false;
			while (timer < extraRunTime) {
				angle = RotateTowardsPosition (target.position, attackRotateSpeed);
				
				// The angle of our forward direction and the player position is larger than 50 degrees
				// That means he is out of sight
				if (Mathf.Abs (angle) > 40)
					lostSight = true;
				
				// If we lost sight then we keep running for some more time (extraRunTime). 
				// then stop attacking 
				if (lostSight)
					timer += Time.deltaTime;	
				
				// Just move forward at constant speed
				direction = transform.TransformDirection (Vector3.forward * movementSpeed);
				characterController.SimpleMove (direction);
				
				// Keep looking if we are hitting our target
				// If we are, deal damage
				var pos = transform.TransformPoint (punchPosition);
				if (Time.time > lastPunchTime + 1f && (pos - target.position).magnitude < punchRadius) {
					// deal damage
				animation.CrossFade ("Attack_Medium_01");
					//target.SendMessage ("ApplyDamage", damage);
					lastPunchTime = Time.time;
					
					
				}
				
				// We are not actually moving forward.
				// This probably means we ran into a wall or something. Stop attacking the player.
				if (characterController.velocity.magnitude < movementSpeed * 0.3f)
					break;
				
				// yield for one frame
				yield return 0;
			}
			
		animation.CrossFade ("Idle");
			
		}
		
		void  OnDrawGizmosSelected (){
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere (transform.TransformPoint(punchPosition), punchRadius);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere (transform.position, attackDistance);
		}
		
		
	}
	
	
	

