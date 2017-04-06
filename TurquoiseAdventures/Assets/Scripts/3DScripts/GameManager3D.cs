using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager3D : MonoBehaviour {


	#region Highlighted Box Attributes
	[SerializeField]
	private BoxController3D _StartBox3D;

	public static BoxController3D _PreviouslyHighlighted3D;

	public static BoxController3D _CurrentlyHighlighted3D;
	#endregion

	#region SpritesManager Attributes
	private SpritesManager _SM;
	#endregion
	/*
	// Use this for initialization
	void Start () {

		// Need to initialise SpritesManager's attribuetes, and then firstHighlight in that order
		// so that the firstHighlighted Box doesn't think the highlighted sprite is its default sprite
		_SM = GetComponent<SpritesManager> ();
		_SM.setMaterials ();
		firstHighlight3D();
	}

	private void handleInput()
	{
		float moveAmount = 2f;
		//-------------------



		// STANDARD DISPLACEMENT -------------------
		// Keypress to make currentlyHighlighted box move UPWARDS
		if (Input.GetKeyDown (KeyCode.Q)) 
		{

			_CurrentlyHighlighted3D.setTextTo ("q");

			_CurrentlyHighlighted3D.setDisplacementAmount (moveAmount);
			_CurrentlyHighlighted3D.setState (BoxController.BoxState.POSITIVE);

		}

		// Keypress to make currentlyHighlighted box move DOWNWARDS
		if (Input.GetKeyDown (KeyCode.W)) 
		{

			_CurrentlyHighlighted3D.setTextTo ("w");

			_CurrentlyHighlighted3D.setDisplacementAmount (-1 * moveAmount);
			_CurrentlyHighlighted3D.setState (BoxController.BoxState.NEGATIVE);

		}

		// Keypress to make currentlyHighlighted box return to its startposition
		if (Input.GetKeyDown (KeyCode.Backspace)) 
		{

			_CurrentlyHighlighted3D.setTextTo ("");

			_CurrentlyHighlighted3D.setState (BoxController.BoxState.RETURNING);

		}
		//----------------------------------

		//GLIDES -------------------------
		// Keypress to make currentlyHighlighted box return to its startposition
		if (Input.GetKeyDown (KeyCode.P)) 
		{
			_CurrentlyHighlighted3D.setTextTo ("p");
		}

		if (Input.GetKeyDown (KeyCode.O)) 
		{
			_CurrentlyHighlighted3D.setTextTo ("o");
		}
	}


	/// <summary>
	/// Highlight the specified p_Box.
	/// </summary>
	/// <param name="p_Box">P box.</param>
	public static void highlight(BoxController p_Box)
	{
		// Get the Image component of the newly selected box
		Image _boxImage = p_Box.GetComponent<Image> ();

		// Set the sprite of newly selected box to the 'selected' sprite
		_boxImage.sprite = SpritesManager.Sprites [0];

		// Set the currently highlighted box to now become the previously highlighted box
		_PreviouslyHighlighted3D = _CurrentlyHighlighted3D;

		// Unhighlight this box
		_PreviouslyHighlighted3D.unHighlight();
		_PreviouslyHighlighted3D.setIsHighlighted (false);

		// Now set the currently highlighted box to be the parameter that was passed to this method
		_CurrentlyHighlighted3D = p_Box;
		_CurrentlyHighlighted3D.setIsHighlighted (true);

	}


	/// <summary>
	/// Called at start to initially highlight the StartBox
	/// </summary>
	private void firstHighlight()
	{
		_CurrentlyHighlighted3D = _StartBox3D;

		// Get  Image component of the _StartBox
		Image _boxImage = _StartBox3D.GetComponent<Image> ();

		// Set the sprite of _StartBox to 'selected' sprite
		_boxImage.sprite = SpritesManager.Sprites [0];

		_CurrentlyHighlighted3D.setIsHighlighted (true);

	}

	/// <summary>
	/// Called at start to initially highlight the StartBox
	/// </summary>
	private void firstHighlight3D()
	{
		_CurrentlyHighlighted3D = _StartBox3D;

		// Get  Image component of the _StartBox
		Renderer _boxImage = _StartBox3D.GetComponent<Renderer> ();

		// Set the sprite of _StartBox to 'selected' sprite
		_boxImage.material = SpritesManager.Materials [1];

		_CurrentlyHighlighted3D.setIsHighlighted (true);

	}


	// Update is called once per frame
	void Update () {
		handleInput ();
	}*/
}
