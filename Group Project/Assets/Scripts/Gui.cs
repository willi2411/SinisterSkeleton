using UnityEngine;
using System.Collections;

public class Gui : MonoBehaviour {

	public Texture2D SwordSheild;
	public Texture2D Knightpic;
	public Texture2D Potionpic;
	Rect barPosition = new Rect (Screen.width - 150,Screen.height - 80,80,80);
	Rect KnightPosition = new Rect (Screen.width - 114,Screen.height - 160,80,80);
	Rect PotionPosition = new Rect (0,Screen.height - 80,80,80);

	void OnGUI(){

		//GUI.Box (new Rect (0,Screen.height - 50,875,50), "");
		if(GUI.Button(new Rect(2,0,50,20), "Exit")) {
			Application.Quit();
		}
		
		// Make the second button.
		if(GUI.Button(new Rect(55,0,80,20), "Level 2")) {
			Application.LoadLevel(1);


		}
		GUI.Label(barPosition,SwordSheild);
		GUI.Label(KnightPosition,Knightpic);
		GUI.Label(PotionPosition,Potionpic);
	}
}
