using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour {

	public GUIText scoreText;

	private static int score = 0;


	void Start ()
	{

		UpdateScore ();

		//StartCoroutine (SpawnWaves ());
	}

	public void AddScore (int newScoreValue)
	{

		score += newScoreValue;
		UpdateScore ();
	}
	
	void UpdateScore ()
	{
		scoreText.text = "X " + score;
	}


}
