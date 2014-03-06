using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Enemy : MonoBehaviour {
	

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
	
	public Transform target;
	private Vector3 tarPos;	
	private EnemyDamaged dead;
	private float runSpeedScale = .5f;
	// Cache a reference to the controller
	CharacterController characterController; 
	
	IEnumerator Start(){
		
		characterController  = GetComponent<CharacterController>();
		dead = GetComponent<EnemyDamaged>();
		
		if (!target)
			target = GameObject.FindWithTag("Player").transform;
		
		animation.Play("Attack_Idle");

		animation["Run"].wrapMode = WrapMode.Loop;
		animation ["Attack_Medium_01"].wrapMode = WrapMode.Once;
		animation["Attack_Medium_01"].layer = 1;
				
		
		yield return new WaitForSeconds(Random.value);

		// Just attack for now
		while (true)	
		{
			// Don't do anything when idle. And wait for player to be in range!
			// This is the perfect time for the player to attack us
			yield return StartCoroutine (Idle());
			//wander();
			
			// Prepare, turn to player and attack him

			yield return StartCoroutine( Attack());

		}

	}


	IEnumerator  Idle (){

		animation.CrossFade ("Attack_Idle");
		yield return new WaitForSeconds(idleTime);

		// And if the player is really far away.
		// stand and wait

		while (true)
		{

			characterController.SimpleMove(Vector3.zero);
			yield return new WaitForSeconds(0.2f);

			Vector3 offset= transform.position - target.position;
			
			// if player is in range again, stop lazyness
			// Good Hunting!		
			if (offset.magnitude < attackDistance)
			{		
				return false;
				
			}
			
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
		
		animation.CrossFade ("Run");
		
		//if (!dead.isDead ()) {
		
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
				target.SendMessage ("ApplyDamage", damage);
				lastPunchTime = Time.time;
				
				
			}
			
			// We are not actually moving forward.
			// This probably means we ran into a wall or something. Stop attacking the player.
			if (characterController.velocity.magnitude < movementSpeed * 0.3f)
				break;
			
			// yield for one frame
			yield return 0;
		}

		}
		
	void  OnDrawGizmosSelected (){
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere (transform.TransformPoint(punchPosition), punchRadius);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position, attackDistance);
	}
	
	
}

