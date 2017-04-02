using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoxController : MonoBehaviour, IPointerClickHandler {

	public enum BoxState 
	{FREEZE, STATIONARY, POSITIVE, NEGATIVE, 
		FLOATING, MOVINGUP, MOVINGDOWN, MOVINGLEFT, MOVINGRIGHT, RETURNING};

	public BoxState _state;

	/// <summary>
	/// Force applied to make the box start moving
	/// </summary>
	private float moveForce;

	/// <summary>
	/// Bool prevent multiple buttons to be pressed while this instance is having a force applied to it
	/// </summary>
	private bool _ButtonLock;

	/// <summary>
	/// The rigid body component of this instance
	/// </summary>
	private Rigidbody2D _rb;

	/// <summary>
	/// State of this instance being highlighted
	/// </summary>
	private  bool isHighlighted;

	#region TextValue Attributes
	/// <summary>
	/// The first child of this instance. A text gameObject
	/// </summary>
	private Text _text;

	/// <summary>
	/// String representing the upVal of this box
	/// </summary>
	[SerializeField]
	private string upChar;
	//private string upVal_2;
	//private string upVal_3;

	/// <summary>
	/// String representing the downVal of this box
	/// </summary>
	[SerializeField]
	private string downChar;
	//private string downVal_2;
	//private string downVal_3;

	#endregion


	#region Proximity Checks
	[SerializeField]
	private bool occupiedAbove;
	[SerializeField]
	private bool occupiedBelow;
	[SerializeField]
	private bool occupiedLeft;
	[SerializeField]
	private bool occupiedRight;
	#endregion

	#region Proximity Boxes
	[SerializeField]
	private BoxController aboveOccupant;
	[SerializeField]
	private BoxController belowOccupant;
	[SerializeField]
	private BoxController leftOccupant;
	[SerializeField]
	private BoxController rightOccupant;
	#endregion

	#region Position Vectors
	/// <summary>
	/// The position box was in before it began being displaced.
	/// </summary>
	[SerializeField]
	private Vector3 _startPosition;

	/// <summary>
	/// Vector updated per frame to hold the location Box was previously in
	/// </summary>
	private Vector3 _previousPosition;

	/// <summary>
	/// The target position of the box when its displacement began.
	/// </summary>
	[SerializeField]
	private Vector3 _targetPosition;

	private Vector3 _floatingPosition;

	/// <summary>
	/// Amount by which the box is expected to be displaced
	/// </summary>
	private float displacementAmount;
	#endregion

	/// <summary>
	/// The normal sprite of this instance, which is returnd to on unhighlight() call
	/// </summary>
	[SerializeField]
	private Sprite _NormalSprite;


	void Awake () {

		moveForce = 40f;

		// Get the unhighlighted image
		_NormalSprite = GetComponent<Image> ().sprite;

		// Get the text of this instance
		_text = transform.GetChild (0).GetComponent<Text>();

		_rb = GetComponent<Rigidbody2D> ();

		// Set this instance's isHighlighted to false at start
		isHighlighted = false;

		occupiedAbove = false;
		occupiedBelow = false;
	}

	void Start()
	{
		// Set initial position to this instance's startposition
		_startPosition = transform.position;
	}

	#region State Methods
	private void handleState()
	{
		switch (_state) 
		{
		case BoxState.FREEZE:
			resetStartPosition ();
			_rb.constraints = RigidbodyConstraints2D.FreezeAll;
			//_ButtonLock = false;
			_state = BoxState.STATIONARY;
			break;
		case BoxState.STATIONARY:
			break;
		case BoxState.FLOATING:
			break;
		case BoxState.POSITIVE:
			_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
			checkAbove ();
			break;
		case BoxState.NEGATIVE:
			_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
			checkBelow ();
			break;
		//----------MOVEMENT CASES------------
		case BoxState.MOVINGUP:
			displace ();
			break;
		case BoxState.MOVINGDOWN:
			displace();
			break;
		case BoxState.MOVINGRIGHT:
			glide();
			break;
		//-----------------------
		case BoxState.RETURNING:
			checkDistanceToStart ();
			break;
		default:
			break;
		}
	}

	public void setState(BoxState p_NewState)
	{
		_state = p_NewState;
	}

	#endregion

	#region Displacement Methods
	/// <summary>
	/// Sets the previousPosition and targetPosition vectors for motion to take place
	/// </summary>
	private void setPositions(string p_DisplaceType)
	{

		//TODO: Displacement is based on screen width and height.
		// Change displacement to be a percentage of screen space


		// Set previous position to where box is now
		_previousPosition = transform.position;

		// Set target position to an offset of box's current position
		_targetPosition = _startPosition;

		switch (p_DisplaceType) 
		{
		case "Vertical":
			_targetPosition.y += displacementAmount;
			break;
		case "Glide_Right":
			_targetPosition.x += 500f;
			break;
		case "Stay":
			_targetPosition.x += 0;
			_targetPosition.y += 0;
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Checks if Box has arrived at targetDestination. Returns true if it is the case
	/// </summary>
	/// <returns><c>true</c>, if arrived was hased, <c>false</c> otherwise.</returns>
	private bool hasArrived()
	{
		float xThresh = Mathf.Abs (transform.position.x - _targetPosition.x);
		float yThresh = Mathf.Abs (transform.position.y - _targetPosition.y);

		if (xThresh <= 0.1f && yThresh <= 0.1f)
			return true;
		else
			return false;
	}

	/// <summary>
	/// Displaces the Box upwards, or rightwards, depending on if another box exists immediately above
	/// </summary>
	private void displace()
	{
		// If at targetPosition, and was moving vertically, set state to FLOATINGV. Halt motion
		if (hasArrived()) 
		{
			_state = BoxState.FLOATING;
			_floatingPosition = transform.position;
			//_ButtonLock = false;
		}

		// Move the box to its targetPosition
		transform.position = Vector3.Lerp (_previousPosition, _targetPosition, 1f * Time.deltaTime);
		_previousPosition = transform.position;

	
	}

	/// <summary>
	/// Displaces the Box upwards, or rightwards, depending on if another box exists immediately above
	/// </summary>
	private void glide()
	{
		// If at targetPosition, and was moving vertically, set state to FLOATINGV. Halt motion
		if (hasArrived()) 
		{
			_state = BoxState.FLOATING;
			_floatingPosition = transform.position;
			//_ButtonLock = false;
		}

		// Move the box to its targetPosition
		transform.position = Vector3.MoveTowards (_previousPosition, _targetPosition, 40f * Time.deltaTime);
		_previousPosition = transform.position;


	}


	/// <summary>
	/// Checks whether the original start position is beneath/left or above/right of box's current position
	/// </summary>
	private void checkDistanceToStart()
	{
		// Set displacement amount to 0, so that box will return to startPosition
		setDisplacementAmount (0);

		Vector3 currPos = transform.position;
		bool xCheck = (currPos.x - _startPosition.x) >= 0;
		bool yCheck = (currPos.y - _startPosition.y) >= 0;

		if (yCheck) 
		{
			_state = BoxState.NEGATIVE;
			return;
		}

		else if (!yCheck) 
		{
			_state = BoxState.POSITIVE;
			return;
		}

	}

	private void resetStartPosition()
	{
		_startPosition = transform.position;
		//setTextTo ("");
	}

	/// <summary>
	/// Sets the displacement amount.
	/// </summary>
	/// <param name="p_Amount">P amount.</param>
	public void setDisplacementAmount(float p_Amount)
	{
		displacementAmount = p_Amount;
	}

	/// <summary>
	/// Ensures instance doesn't move from its start position
	/// if in STATIONARY or FLOATING states
	/// </summary>
	private void holdPosition()
	{
		if (_state == BoxState.STATIONARY) 
		{
			transform.position = _startPosition;
		}

		if (_state == BoxState.FLOATING)
			transform.position = _floatingPosition;
	}

	#endregion

	#region Glide Methods
	private void checkCanGlide()
	{
		switch (_text.text) 
		{
		// Right Glide 
		case "o":
			if (leftOccupant != null && leftOccupant.getTextValue () == "p") 
			{
				//Debug.Log ("LeftOccupant not Null");
				_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
				checkRight ();
			}
			break;
		case "":
			break;
		default:
			break;
		}
	}

	#endregion

	#region Proximity Checks
	/// <summary>
	/// Checks immediate above this instance to see if a box is present there
	/// if so, sets the occupiedAbove attribute to true
	/// </summary>
	private void checkAbove()
	{

		// if no box is located immediately above
		// move this instance up vertically
		if (!occupiedAbove) {
			
			setPositions ("Vertical");
			//_ButtonLock = true;
			_state = BoxState.MOVINGUP;
		}
			

		// If both checks fail, do not move at all
		else 
		{
			setPositions ("Stay");
			_state = BoxState.STATIONARY;
			//_ButtonLock = false;
		}
	}

	private void checkBelow()
	{

		// If no box is located immediately below, 
		// move this instance down vertically
		if (!occupiedBelow) 
		{
			setPositions ("Vertical");
			//_ButtonLock = true;
			_state = BoxState.MOVINGDOWN;
		}
			
		// If both checks fail, do not move at all
		else 
		{
			setPositions ("Stay");
			_state = BoxState.STATIONARY;
			//_ButtonLock = false;
		}
	}

	private void checkRight()
	{

		// If no box is located immediately below, 
		// move this instance down vertically
		if (!occupiedRight) 
		{
			setPositions ("Glide_Right");
			//_ButtonLock = true;
			_state = BoxState.MOVINGRIGHT;
		}

		// If both checks fail, do not move at all
		else 
		{
			setPositions ("Stay");
			_state = BoxState.STATIONARY;
			//_ButtonLock = false;
		}
	}
		

	/// <summary>
	/// Checks that distance between this instance and parameter does not exceed the width of a box
	/// </summary>
	/// <returns>The <see cref="System.Boolean"/>.</returns>
	/// <param name="p_Object">P object.</param>
	private bool distanceBetween(Transform p_Object)
	{
		float radiusCheck = p_Object.GetComponent<RectTransform> ().sizeDelta.y/2 +1;
		float proxCheck = Vector3.Magnitude (transform.position - p_Object.position);

		if (proxCheck < radiusCheck)
			return true;
		else
			return false;
	}
	#endregion


	#region Box Value modification
	/// <summary>
	/// Sets the text of this instance to given parameter
	/// </summary>
	/// <param name="p_NewValue">P new value.</param>
	public void setTextTo(string p_NewValue)
	{
		_text.text = p_NewValue;
		checkCanGlide ();
	}

	/// <summary>
	/// Gets the text value of this instance
	/// </summary>
	/// <returns>The text value.</returns>
	public string getTextValue()
	{
		return _text.text;
	}

	public bool getButtonLock()
	{
		return _ButtonLock;
	}
	#endregion


	#region Interface Methods
	public void OnPointerClick(PointerEventData eventData)
	{
		// If already highlighted, do not do anything
		if (isHighlighted) return;
		
		GameManager.highlight (this);
	}
	#endregion

	#region Highlight Methods
	/// <summary>
	/// Unhighlights this instance. Called by GamaManager when another Box has been highlighted.
	/// </summary>
	public void unHighlight()
	{
		// Get Image component, set sprite back to unselected sprite image
		Image _Image = GetComponent<Image> ();
		_Image.sprite = _NormalSprite;
	}

	/// <summary>
	/// Sets the is highlighted attribute to specified p_Value
	/// </summary>
	/// <param name="p_Value">If set to <c>true</c> p value.</param>
	public void setIsHighlighted(bool p_Value)
	{
		isHighlighted = p_Value;
		// If highlighted, make this instance light so it doesn't bump 
		// other boxes out of the way
		switch (p_Value) 
		{
		case true:
			_rb.mass = 1f;
			break;
		case false:
			_rb.mass = 10f;
			break;
		}
	}

	#endregion


	#region Collider2D Method
	public void OnCollisionEnter2D(Collision2D coll)
	{ 

		bool movingVertical = (_state == BoxState.MOVINGDOWN) || (_state == BoxState.MOVINGUP);
		bool movingHorizontal = (_state == BoxState.MOVINGLEFT) || (_state == BoxState.MOVINGRIGHT);
		bool floating = (_state == BoxState.FLOATING);



		foreach (ContactPoint2D _CP in coll.contacts) 
		{

			// Bools to check which side of the box is being made contact with
			bool xNormal = (Mathf.Abs (_CP.normal.x) >= 0f);
			bool yNormal = (Mathf.Abs (_CP.normal.y) >= 0f);


			// HEAD ON COLLISION CHECKS 

			// Moving Up collision
			if ((movingVertical || floating) && _CP.normal.y < 0f)
			{
				occupiedAbove = true;
				aboveOccupant = coll.gameObject.GetComponent<BoxController> ();

				_state = BoxState.FREEZE;
			}

			// Moving Down collision
			if ((movingVertical || floating) && _CP.normal.y > 0f)
			{
				occupiedBelow = true;
				belowOccupant = coll.gameObject.GetComponent<BoxController> ();

				_state = BoxState.FREEZE;
			}

			// Moving Right collision
			if ((movingHorizontal || floating) && _CP.normal.x < 0f)
			{
				occupiedRight = true;
				rightOccupant = coll.gameObject.GetComponent<BoxController> ();

				_state = BoxState.FREEZE;
			}
				
			//--------------------


			// BRUSH BY COLLISION CHECKS----------------

			// If Box is moving vertically, and makes contact with another box on its side while enroute then
			// ignore that collision
			// If Box is floating vertically, and makes contact with another box on its side while that 
			// other box is enroute then ignore as well
			if ((movingVertical && xNormal ) || floating && xNormal) 
			{

				continue;
			} 

			// TODO Make sure brushing when gliding doesn't halt motion
			// If Box is moving horizontally, and makes contact with another box (on the side) while enroute then
			// ignore that collision
			if (movingHorizontal && yNormal) 
			{
				continue;
			}
		
			//--------------------------
		}
	}

	public void OnCollisionStay2D(Collision2D coll)
	{

		bool stationary = (_state == BoxState.STATIONARY);
		bool floating = (_state == BoxState.FLOATING);


		//Debug.Log ("Collision Stay!");

		foreach (ContactPoint2D _CP in coll.contacts) 
		{

			// LINGERING COLLISION CHECKS-----------------------
			// distanceBetween check is to make sure a corner collision is not being mistaken
			// for a head on collision.
			// If FLOATING and with a lingering collision below, set occupiedBelow to true
			if ((floating || stationary) && _CP.normal.y > 0f && distanceBetween(coll.gameObject.transform))
			{
				occupiedBelow = true;
				belowOccupant = coll.gameObject.GetComponent <BoxController> ();
				_state = BoxState.FREEZE;
				//_ButtonLock = false;
			}

			// If FLOATING and with a lingering collision above, set occupiedAbove to true
			if ((floating || stationary) && _CP.normal.y < 0f &&  distanceBetween(coll.gameObject.transform))
			{
				occupiedAbove = true;
				aboveOccupant = coll.gameObject.GetComponent <BoxController> ();

				_state = BoxState.FREEZE;
				//_ButtonLock = false;
			}

			// If FLOATING and with a lingering collision to the left, set occupiedLeft to true
			if ((floating || stationary) && _CP.normal.x > 0f && distanceBetween(coll.gameObject.transform) )
			{
				occupiedLeft = true;
				leftOccupant = coll.gameObject.GetComponent <BoxController> ();

				_state = BoxState.FREEZE;
				//_ButtonLock = false;
			}

			// If FLOATING and with a lingering collision to the right, set occupiedRight to true
			if ((floating || stationary) && _CP.normal.x < 0f && distanceBetween(coll.gameObject.transform))
			{
				occupiedRight = true;
				rightOccupant = coll.gameObject.GetComponent<BoxController> ();

				_state = BoxState.FREEZE;
				//_ButtonLock = false;
			}

			//-----------------------------------
		}
	}


	public void OnCollisionExit2D(Collision2D coll)
	{

		foreach (ContactPoint2D _CP in coll.contacts) 
		{

			// If Collision Point with negative vertical normal exits collision,
			// assume that this instance no longer has another box immediately above it
			if ( _CP.normal.y < 0f ) 
			{
				//Debug.Log (name + " Exiting Colliison");
				occupiedAbove = false;
				aboveOccupant = null;

			}

			// If Collision Point with positive vertical normal exits collision,
			// assume that this instance no longer has another box immediately below it
			if ( _CP.normal.y > 0f ) 
			{
				occupiedBelow = false;
				belowOccupant = null;

			}

			// If Collision Point with positive horizonal normal exits collision,
			// assume that this instance no longer has another box immediately left of it
			if ( _CP.normal.x > 0f ) 
			{
				occupiedLeft = false;
				leftOccupant = null;

			}

			// If Collision Point with negative horizontal normal exits collision,
			// assume that this instance no longer has another box immediately right of it
			if ( _CP.normal.x < 0f ) 
			{
				occupiedRight = false;
				rightOccupant = null;
			}

		}
	}


	#endregion


	// Update is called once per frame
	void Update () {
		handleState ();
		holdPosition ();
	}


}
