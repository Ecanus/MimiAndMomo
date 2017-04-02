using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {


	#region Highlighted Box Attributes
	/// <summary>
	/// The box highlighted on startup
	/// </summary>
	[SerializeField]
	private BoxController _StartBox;

	/// <summary>
	/// The previously highlighted box
	/// </summary>
	public static BoxController _PreviouslyHighlighted;

	/// <summary>
	/// The currently highlighted box
	/// </summary>
	public static BoxController _CurrentlyHighlighted;
	#endregion

	#region SpritesManager Attributes
	private SpritesManager _SM;
	#endregion

	// Use this for initialization
	void Start () {

		// Need to initialise SpritesManager's attribuetes, and then firstHighlight in that order
		// so that the firstHighlighted Box doesn't think the highlighted sprite is its default sprite
		_SM = GetComponent<SpritesManager> ();
		_SM.setSprites ();
		firstHighlight ();
	}

	private void handleInput()
	{

		// NOT CURRENTLY IMPLEMENTED
		// If _CurrentlyHighlighted is in the process of having a force applied,
		// no other input will be registered until that force is done being applied
		if (_CurrentlyHighlighted.getButtonLock ())
			return; 
		//-------------------



		// STANDARD DISPLACEMENT -------------------
		// Keypress to make currentlyHighlighted box move UPWARDS
		if (Input.GetKeyDown (KeyCode.Q)) 
		{

			_CurrentlyHighlighted.setTextTo ("q");

			_CurrentlyHighlighted.setDisplacementAmount (45f);
			_CurrentlyHighlighted.setState (BoxController.BoxState.POSITIVE);

		}

		// Keypress to make currentlyHighlighted box move DOWNWARDS
		if (Input.GetKeyDown (KeyCode.W)) 
		{

			_CurrentlyHighlighted.setTextTo ("w");

			_CurrentlyHighlighted.setDisplacementAmount (-45f);
			_CurrentlyHighlighted.setState (BoxController.BoxState.NEGATIVE);

		}

		// Keypress to make currentlyHighlighted box return to its startposition
		if (Input.GetKeyDown (KeyCode.Backspace)) 
		{

			_CurrentlyHighlighted.setTextTo ("");

			_CurrentlyHighlighted.setState (BoxController.BoxState.RETURNING);

		}
		//----------------------------------

		//GLIDES -------------------------
		// Keypress to make currentlyHighlighted box return to its startposition
		if (Input.GetKeyDown (KeyCode.P)) 
		{
			_CurrentlyHighlighted.setTextTo ("p");
		}

		if (Input.GetKeyDown (KeyCode.O)) 
		{
			_CurrentlyHighlighted.setTextTo ("o");
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
		_PreviouslyHighlighted = _CurrentlyHighlighted;

		// Unhighlight this box
		_PreviouslyHighlighted.unHighlight();
		_PreviouslyHighlighted.setIsHighlighted (false);

		// Now set the currently highlighted box to be the parameter that was passed to this method
		_CurrentlyHighlighted = p_Box;
		_CurrentlyHighlighted.setIsHighlighted (true);

	}


	/// <summary>
	/// Called at start to initially highlight the StartBox
	/// </summary>
	private void firstHighlight()
	{
		_CurrentlyHighlighted = _StartBox;

		// Get  Image component of the _StartBox
		Image _boxImage = _StartBox.GetComponent<Image> ();

		// Set the sprite of _StartBox to 'selected' sprite
		_boxImage.sprite = SpritesManager.Sprites [0];

		_CurrentlyHighlighted.setIsHighlighted (true);

	}
	// Update is called once per frame
	void Update () {
		handleInput ();
	}
}
