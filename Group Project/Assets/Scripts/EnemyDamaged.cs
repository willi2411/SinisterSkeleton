using UnityEngine;
using System.Collections;

public class EnemyDamaged : MonoBehaviour {

	public float hitPoints = 3f;
	private float curHitPoints;
	public int dropMin = 0;
	public int dropMax = 1;
	public Transform DropedHP;
	public Transform DropedDamage;
	public Transform DropedPotion;
	public Transform DieingAn;
	public Texture2D[] TextureList;
	private Vector3 screenPos;
	private bool visible = true;

	 bool dead = false;
	// Use this for initialization

	void Start()
	{
		curHitPoints = hitPoints; 
	}

	void ApplyDamage (float damage)
	{
		if (curHitPoints <= 0) {
			dead = true;
				return;
				}

		curHitPoints -= damage;
		animation.CrossFade ("Defending");

		if (!dead && curHitPoints <= 0) {

			animation.Play("Die");


			Die ();
			SendMessage ("AddScore", 1);

			dead = true;
				}

	}

	int GetPercent (float curent, float Max)
	{
		float percent = curent / Max * (TextureList.Length - 1);
		if(percent<1&&percent>0){
			percent=1;//The reason for this is so that we will never show an empty bar (0) until we really have 0
		}
		int roundP = Mathf.RoundToInt(percent);
		return roundP;
	}



	void OnBecameVisible () {
		visible= true;
	}
	
	void OnBecameInvisible () {
		visible= false;
	}


	//display health bar
	void OnGUI(){
		
				if (visible) { 
						// > Healthbar code<
						screenPos = Camera.main.WorldToScreenPoint (transform.position);
						int number = GetPercent (curHitPoints, hitPoints);

						if (number > TextureList.Length) {
								number = TextureList.Length;
						}
						if (number < 0) {
								number = 0;
						}
						GUI.DrawTexture (new Rect (screenPos.x - 20, screenPos.y, 50, 10), TextureList [number], ScaleMode.StretchToFill);
		
				}
		}
	



	public bool isDead()
	{return dead;}


	 void Die()
	{	

			//Destroy (gameObject, 2.9f);
		Destroy (gameObject);
		         //testing
		Instantiate(DieingAn,transform.position,transform.rotation);

		// drop a random number of pickups 
		var toDrop = Random.Range(dropMin, dropMax + 1);	// how many shall we drop?
		int droptype = Random.Range (0, 3);
		for (var i=0;i<toDrop;i++)
		{			

			var direction = Random.onUnitSphere;	// pick a random direction to throw the pickup.
			if(direction.y < 0)
				direction.y = -direction.y;	// make sure the pickup isn't thrown downwards
			
			// initial position of the pickup
			var dropPosition = transform.TransformPoint(Vector3.up * 1.5f) + (direction / 2);

			// select a pickup type at random
			if( droptype == 0)
			Instantiate(DropedHP, dropPosition, Quaternion.identity);
			else if (droptype == 1)
				Instantiate(DropedDamage, dropPosition, Quaternion.identity);
			else 
				Instantiate(DropedPotion, dropPosition, Quaternion.identity);
		}

	}

	 


	public void Reset ()
	{
		gameObject.tag = "Enemy";
	}
}
