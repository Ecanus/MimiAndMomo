using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {


	/// <summary>
	/// List of all movable boxes ingame
	/// </summary>
	public static List<BoxController> Boxes;

	/// <summary>
	/// Input Manager component of this instance
	/// </summary>
	private InputManager _IM;

	#region Highlighted Box Attributes
	/// <summary>
	/// The box highlighted on startup
	/// </summary>
	[SerializeField]
	private BoxController _StartBox;

	[SerializeField]
	private BoxController3D _StartBox3D;

	/// <summary>
	/// The previously highlighted box
	/// </summary>
	public static BoxController _PreviouslyHighlighted;

	public static BoxController3D _PreviouslyHighlighted3D;

	/// <summary>
	/// The currently highlighted box
	/// </summary>
	public static BoxController _CurrentlyHighlighted;
	public static BoxController3D _CurrentlyHighlighted3D;
	#endregion


	#region SpritesManager Attributes
	private SpritesManager _SM;
	#endregion

	void Awake()
	{
		Boxes = new List<BoxController> ();
	}

	// Use this for initialization
	void Start () {

		// Need to initialise SpritesManager's attribuetes, and then firstHighlight in that order
		// so that the firstHighlighted Box doesn't think the highlighted sprite is its default sprite
		_SM = GetComponent<SpritesManager> ();
		_IM = GetComponent<InputManager> ();
		_SM.setSprites ();
		firstHighlight ();
	
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

	/// <summary>
	/// Called at start to initially highlight the StartBox
	/// </summary>
	private void firstHighlight3D()
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
		_IM.handleInput (_CurrentlyHighlighted);
	}
}
