using UnityEngine;
using System.Collections;

public class PlayerGuiText: MonoBehaviour {

	public GUIText damageText;
	private static float damage;

	void Start ()
	{
		//damage = 1;
		UpdateDamage ();
		}

	public void AddDamage (int newdamageValue)
	{
		damage += newdamageValue;
		UpdateDamage ();
		
	}
	void UpdateDamage()
	{
		damageText.text = "X " + damage;
	}
}
