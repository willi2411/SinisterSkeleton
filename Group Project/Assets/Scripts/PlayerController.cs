using UnityEngine;
using System.Collections;


// Require a character controller to be attached to the same game object
[RequireComponent(typeof(CharacterController))]

public class PlayerController : MonoBehaviour {

	public float runSpeed = 4.0f;

	private Vector3 moveDirection = Vector3.zero; //current x-z direction
	private float moveSpeed =0.0f; //current x-z plane
	private CollisionFlags collisionFlags;
	public float rotateSpeed = 500.0f;
	public float jumpHeight = 0.5f;
	public float gravity = 20.0f;// The gravity for the character
	public bool canJump = true;
	public float extraJumpHeight = 2.5f;


	private bool movingBack = false; //locks camera
	private bool isMoving = false; //user pressing keys?
	private bool isControllable = true;
	private float lockCameraTimer = 0.5f;
	private float speedSmoothing = 10.0f;


	private Vector3 inAirVelocity = Vector3.zero;
	private float verticalSpeed = 0.0f;

	//jumping 
	private float jumpRepeatTime = 0.05f;
	private bool jumping = false;
	private bool jumpingReachedApex = false;
	private float lastJumpButtonTime = -10.0f;
	private float lastJumpTime = -1.0f;// Last time we performed a jump
	// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
	private float lastJumpStartHeight = 0.0f;
	private float lastGroundedTime = 0.0f;
	private float jumpTimeout = 0.15f;
	private float groundedTimeout = 0.25f;

	void Awake()
	{
		moveDirection = transform.TransformDirection (Vector3.forward);

		}

	//Direction player is moving
	void UpdateMovementDirection()
	{
		Transform cameraTransform = Camera.main.transform;
		bool grounded = IsGrounded();

		// Forward vector relative to the camera along the x-z plane	
		Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
		forward.y = 0;
		forward = forward.normalized;
		
		// Right vector relative to the camera
		// Always orthogonal to the forward vector
		Vector3 right = new Vector3( forward.z, 0, -forward.x);
		
		float v = Input.GetAxisRaw("Vertical");
		float h = Input.GetAxisRaw("Horizontal");
		
		// Are we moving backwards or looking backwards
		if (v < -0.2f)
			movingBack = true;
		else
			movingBack = false;
		
		bool wasMoving = isMoving;
		isMoving = Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f;
		
		// Target direction relative to the camera
		Vector3 targetDirection = h * right + v * forward;
		

		// Grounded controls
		if (grounded)
		{

			// Lock camera for short period when transitioning moving & standing still
			lockCameraTimer += Time.deltaTime;
			if (isMoving != wasMoving)
				lockCameraTimer = 0.0f;
			
			// We store speed and direction seperately,
			// so that when the character stands still we still have a valid forward direction
			// moveDirection is always normalized, and we only update it if there is user input.
			if (targetDirection != Vector3.zero)
			{

				moveDirection = Vector3.RotateTowards(moveDirection, targetDirection, rotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);
				moveDirection = moveDirection.normalized;
			}

			// Smooth the speed based on the current target direction
			var curSmooth = speedSmoothing * Time.deltaTime;
			
			// Choose target speed
			//* We want to support analog input but make sure you cant walk faster diagonally than just forward or sideways
			var targetSpeed = Mathf.Min(targetDirection.magnitude, 1.0f);
				targetSpeed *= runSpeed;

			
			moveSpeed = Mathf.Lerp(moveSpeed, targetSpeed, curSmooth);
		}
		// In air controls
		else
		{
			// Lock camera while in air
			if (jumping)
				lockCameraTimer = 0.0f;
			
			if (isMoving)
				inAirVelocity += targetDirection.normalized * Time.deltaTime;
		}
	
}

	void ApplyJumping()
	{
		// Prevent jumping too fast after each other
		if (lastJumpTime + jumpRepeatTime > Time.time)
			return;


		if (IsGrounded())
		{
			// Jump
			// - Only when pressing the button down
			// - With a timeout so you can press the button slightly before landing		
			if (canJump && Time.time < lastJumpButtonTime + jumpTimeout)
			{
				verticalSpeed = CalculateJumpVerticalSpeed(jumpHeight);
				SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	
	void ApplyGravity()
	{

		if (isControllable)	// don't move player at all if not controllable.
		{

			// Apply gravity
			var jumpButton = Input.GetButton("Jump");


			// When we reach the apex of the jump we send out a message
			if (jumping && !jumpingReachedApex && verticalSpeed <= 0.0f)
			{
				jumpingReachedApex = true;
				SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);

			}
			var extraPowerJump = IsJumping() && verticalSpeed > 0.0f && jumpButton && transform.position.y < lastJumpStartHeight+ extraJumpHeight;

			if(extraPowerJump)
				return;
			else if (IsGrounded())
				verticalSpeed = 0.0f;
			else
				verticalSpeed -= gravity * Time.deltaTime;
		}
	}

	float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * targetJumpHeight * gravity);
	}
	
	void DidJump()
	{
		jumping = true;
		jumpingReachedApex = false;
		lastJumpTime = Time.time;
		lastJumpStartHeight = transform.position.y;
		lastJumpButtonTime = -10;
	}



	// Update is called once per frame
	void Update () {
	
		if (!isControllable)
		{
			// kill all inputs if not controllable.
			Input.ResetInputAxes();
		}
		
		if (Input.GetButtonDown("Jump"))
		{
			lastJumpButtonTime = Time.time;
		}
		//get direction
		UpdateMovementDirection ();

		ApplyGravity();

		// Apply jumping logic
		ApplyJumping();

		//Calculate actual motion
		var movement = moveDirection * moveSpeed+ new Vector3 (0, verticalSpeed, 0) + inAirVelocity;;
		movement *= Time.deltaTime;

		// Move the controller
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);

		// We are in jump mode but just became grounded
		if (IsGrounded())
		{
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if (jumping)
			{
				jumping = false;
				SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
	
		transform.rotation = Quaternion.LookRotation(moveDirection);

	}

	public bool IsJumping()
	{
		return jumping;
	}

	public bool HasJumpReachedApex()
	{
		return jumpingReachedApex;
	}
	
	public bool IsGroundedWithTimeout()
	{
		return lastGroundedTime + groundedTimeout > Time.time;
	}

	public bool IsGrounded()
	{
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
	public float GetSpeed () {
		return moveSpeed;
	}

	public Vector3 GetDirection () {
		return moveDirection;
	}

	public bool IsMovingBackwards () {
		return movingBack;
	}

	public bool IsMoving ()  
	{
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5;
	}

	public void Reset ()
	{
		gameObject.tag = "Player";
	}


}
