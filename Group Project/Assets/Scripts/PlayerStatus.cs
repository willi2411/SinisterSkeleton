using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour {

	private float health;
	public GUIText HealthText;
	public GUIText HealthPotText;
	private float onePercent;
	private static int healthPot =2;
	public static float MaxHealth = 15f;
	public Texture2D[] TextureList;
	Rect barPosition = new Rect(Screen.width - 500,Screen.height - 40,300,40); //position of hp bar

	void  ApplyDamage ( int damage  ){
		animation.CrossFade ("gethit");

		if(health <=0)
			return;

		health -= damage;
		UpdateHealth ();

		if (health <= 0)
		{
			animation.CrossFade("die");
			StartCoroutine(Death());
		}
	}
	float getOnePercent()
	{float hpvalue = MaxHealth / 100; 

		return hpvalue;
		}
	void Update()
	{
		onePercent = getOnePercent ();
		if (health <= MaxHealth) {

			health += onePercent * Time.deltaTime; // * .1f;

						UpdateHealth ();
				}
		if(Input.GetButtonDown ("Fire3") && health <MaxHealth && healthPot > 0)
		{
			if(MaxHealth - health < 5){
				health += MaxHealth - health;
				healthPot -= 1;
				UpdateHealthPot ();

			}
			else{
			health +=5;
			healthPot -= 1;
			UpdateHealthPot ();
			}
		}
		
	}

	void AddHealthPot(int addHPot)
	{
		healthPot += addHPot;
		UpdateHealthPot ();
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

	void OnGUI()
	{
		int number = GetPercent (health, MaxHealth);
			//print (number);
		if(number>TextureList.Length){
			number=TextureList.Length;
		}
		if(number<0){
			number=0;
		}
		//now we draw the bar!
		GUI.DrawTexture(barPosition,TextureList[number],ScaleMode.StretchToFill);
	

	}

	void gainHealth(int healthPack)
	{
		MaxHealth += healthPack;
		UpdateHealth ();

		}
	void Start ()
	{
		health = MaxHealth;

		UpdateHealth ();
		UpdateHealthPot ();
		//StartCoroutine (SpawnWaves ());
	}


	void UpdateHealth ()
	{
		HealthText.text = "HP: " + Mathf.RoundToInt(health) +"/" + MaxHealth;

	}

	void UpdateHealthPot ()
	{
		HealthPotText.text = "X " + Mathf.RoundToInt (healthPot);
		
	}

	IEnumerator Death()
	{	
		
		
				if (animation.IsPlaying ("die")) 
						yield return new WaitForSeconds (2f);
					
		Application.LoadLevel("testone");
	}
}
