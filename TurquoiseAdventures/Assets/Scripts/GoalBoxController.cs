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


	/// <summary>
	/// Updates the score text.
	/// </summary>
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

	/// <summary>
	/// Destroys the box.
	/// </summary>
	/// <param name="p_Object">P object.</param>
	private void destroyBox(GameObject p_Object)
	{
		// Remove this instance from the GameManager's list of onscreen boxes
		BoxController box = p_Object.GetComponent<BoxController> ();
		GameManager.Boxes.Remove (box);

		// Tell GameManager to highlight a new box, at random
		BoxController temp = GameManager.Boxes [Random.Range (0, GameManager.Boxes.Count)];
		GameManager.highlight (temp);

		// Finally, destroy this instance
		Destroy (p_Object);
	}
}
