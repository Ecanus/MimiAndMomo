using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoalBoxController : MonoBehaviour {

	[SerializeField]
	private Text _Score;
	private int _ScoreCount;

	// Use this for initialization
	void Start () {
		_ScoreCount = 0;
		_Score.text = "" + _ScoreCount;
	}


	private void updateScoreText()
	{
		_Score.text = "" + _ScoreCount;
	}

	public void OnTriggerEnter2D(Collider2D coll)
	{
		_ScoreCount++;
		updateScoreText ();

		destroyBox (coll.gameObject);
	}


	private void destroyBox(GameObject p_Object)
	{
		BoxController box = p_Object.GetComponent<BoxController> ();
		GameManager.Boxes.Remove (box);

		BoxController temp = GameManager.Boxes [Random.Range (0, GameManager.Boxes.Count)];
		GameManager.highlight (temp);

		Destroy (p_Object);
	}
}
