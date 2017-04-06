using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class managing attributes, displacement and collisions of a box in-game
/// </summary>
public class BoxController : MonoBehaviour, IPointerClickHandler {

	/// <summary>
	/// States that a box can be in. Each state determines a distinct functionality and behaviour
	/// </summary>
	public enum BoxState 
	{FREEZE, STATIONARY, POSITIVE, NEGATIVE, SIDERIGHT, SIDELEFT,
		FLOATING, MOVINGUP, MOVINGDOWN, GLIDINGUP, GLIDINGDOWN, GLIDINGLEFT, GLIDINGRIGHT, RETURNING};

	/// <summary>
	/// The BoxState of this instance
	/// </summary>
	public BoxState _state;


	private float moveSpeed;
	private float glideSpeed;

	/// <summary>
	/// Bool prevent multiple buttons to be pressed while this instance is having a force applied to it
	/// </summary>
	/// NOT IMPLEMENTED YET
	private bool _ButtonLock;

	/// <summary>
	/// The rigid body component of this instance
	/// </summary>
	private Rigidbody2D _rb;

	/// <summary>
	/// The box collider component
	/// </summary>
	private BoxCollider2D _boxColl;
	private Vector2 boxCollVec;

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
	public bool occupiedAbove;
	public bool occupiedBelow;
	public bool occupiedLeft;
	public bool occupiedRight;
	#endregion

	#region Proximity Boxes
	public BoxController aboveOccupant;
	public BoxController belowOccupant;
	public BoxController leftOccupant;
	public BoxController rightOccupant;
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

		// Get the unhighlighted image
		_NormalSprite = GetComponent<Image> ().sprite;

		// Get the text of this instance
		_text = transform.GetChild (0).GetComponent<Text>();

		// Get the rigidbody2D component of this instance
		_rb = GetComponent<Rigidbody2D> ();
		// Lock it on all axes
		_rb.constraints = RigidbodyConstraints2D.FreezeAll;

		// Set this instance's isHighlighted to false at start
		isHighlighted = false;


		// Set its occupied values all to false
		occupiedAbove = false;
		occupiedBelow = false;
		occupiedLeft = false;
		occupiedRight = false;
	}

	void Start()
	{

		// Set initial position to this instance's startposition
		_startPosition = transform.position;

		// Get boxCollider2D component of this instance
		_boxColl = GetComponent<BoxCollider2D> ();
		boxCollVec = _boxColl.size;

		// Set movement speed attributes
		moveSpeed = 10f;
		glideSpeed = 9f;

		// Add this instance to the GameManager's list of all boxes currently on screen
		GameManager.Boxes.Add (this);
	
	}

	#region State Methods
	/// <summary>
	/// Handles behaviour of this instance based on its current state
	/// </summary>
	private void handleState()
	{
		switch (_state) 
		{
		case BoxState.FREEZE:
			_rb.constraints = RigidbodyConstraints2D.FreezeAll;
			resetStartPosition ();
			//_ButtonLock = false;
			_state = BoxState.STATIONARY;
			break;
		case BoxState.STATIONARY:
			boxCollVec.x = 30f;
			boxCollVec.y = 30f;
			_boxColl.size = boxCollVec;
			break;
		case BoxState.FLOATING:
			boxCollVec.x = 28f;
			boxCollVec.y = 28f;
			_boxColl.size = boxCollVec;
			break;
		case BoxState.POSITIVE:
			_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
			boxCollVec.x = 15f;
			boxCollVec.y = 30f;
			_boxColl.size = boxCollVec;
			checkAbove ();
			break;
		case BoxState.NEGATIVE:
			_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
			boxCollVec.x = 15f;
			boxCollVec.y = 30f;
			_boxColl.size = boxCollVec;
			checkBelow ();
			break;
		case BoxState.SIDELEFT:
			_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
			boxCollVec.x = 30f;
			boxCollVec.y = 15f;
			_boxColl.size = boxCollVec;
			checkLeft ();
			break;
		case BoxState.SIDERIGHT:
			_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
			boxCollVec.x = 30f;
			boxCollVec.y = 15f;
			_boxColl.size = boxCollVec;
			checkRight ();
			break;
		//----------MOVEMENT CASES------------
		case BoxState.MOVINGUP:
			displace ();
			break;
		case BoxState.MOVINGDOWN:
			displace();
			break;
		//------------GLDING CASES------------------
		case BoxState.GLIDINGUP:
			boxCollVec.x = 15f;
			boxCollVec.y = 30f;
			_boxColl.size = boxCollVec;
			glide();
			break;
		case BoxState.GLIDINGDOWN:
			boxCollVec.x = 15f;
			boxCollVec.y = 30f;
			_boxColl.size = boxCollVec;
			glide ();
			break;
		case BoxState.GLIDINGLEFT:
			boxCollVec.x = 30f;
			boxCollVec.y = 15f;
			_boxColl.size = boxCollVec;
			glide ();
			break;
		case BoxState.GLIDINGRIGHT:
			boxCollVec.x = 30f;
			boxCollVec.y = 15f;
			_boxColl.size = boxCollVec;
			glide ();
			break;
		//-----------------------------------------
		case BoxState.RETURNING:
			checkDistanceToStart ();
			break;
		default:
			break;
		}
	}

	/// <summary>
	/// Sets the state of this instance to the parameter p_NewState
	/// </summary>
	/// <param name="p_NewState">P new state.</param>
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

		// Determines how this instance should be displaced based on the given string, p_DisplaceType
		switch (p_DisplaceType) 
		{
		case "Vertical":
			_targetPosition.y += displacementAmount;
			break;
		case "Horizontal":
			_targetPosition.x += displacementAmount;
			break;
		case "Glide_Up":
			_targetPosition.y += 500f;
			break;
		case "Glide_Down":
			_targetPosition.y -= 500f;
			break;
		case "Glide_Right":
			_targetPosition.x += 500f;
			break;
		case "Glide_Left":
			_targetPosition.x -= 500f;
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

		if (xThresh <= 0.1f && yThresh <= 0.05f)
			return true;
		else
			return false;
	}

	/// <summary>
	/// Displaces the Box to its targetPosition
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
		transform.position = Vector3.MoveTowards(_previousPosition, _targetPosition, moveSpeed * Time.deltaTime);
		_previousPosition = transform.position;

	
	}

	/// <summary>
	/// Glides the instance towards its target position
	/// </summary>
	private void glide()
	{
		// If at targetPosition, and was moving vertically, set state to FLOATINGV. Halt motion
		if (hasArrived()) 
		{
			_state = BoxState.FLOATING;
			_floatingPosition = transform.position;
			_ButtonLock = false;
		}

		// Move the box to its targetPosition
		transform.position = Vector3.MoveTowards (_previousPosition, _targetPosition, glideSpeed * Time.deltaTime);
		_previousPosition = transform.position;


	}


	/// <summary>
	/// Checks whether the original start position is beneath/left or above/right of box's current position
	/// then returns box back to its start position
	/// </summary>
	private void checkDistanceToStart()
	{
		// Set displacement amount to 0, so that box will return to startPosition
		setDisplacementAmount (0);

		Vector3 currPos = transform.position;
		//bool xCheck = (currPos.x - _startPosition.x) >= 0;
		bool yCheck = (currPos.y - _startPosition.y) >= 0;

		// If higher than start position, move downwards to return
		if (yCheck) 
		{
			_state = BoxState.NEGATIVE;
			return;
		}

		// If lower than start position, move upwards
		else if (!yCheck) 
		{
			_state = BoxState.POSITIVE;
			return;
		}

	}

	/// <summary>
	/// Resets the start position of this instance to where it is currently
	/// </summary>
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

	#region Glide Methods ------------------
	/// <summary>
	/// Handles all checks for if this instance can begin gliding in any direction
	/// </summary>
	private void checkCanGlide()
	{
		switch (_text.text) 
		{
		case "":
			break;
		// IF THIS INSTANCE HAS "o" AS TEXT VALUE
		case "o":
			// If its left Occupant has "p", and instance is not blocked to the right, glide right
			// Prep right glide
			if (leftOccupant != null && leftOccupant.getTextValue () == "p") 
			{
				//Debug.Log ("LeftOccupant not Null");
				_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
				checkGlideRight ();
			}

			// If its right Occupant has "k", and instance is not blocked to the left, glide left
			// Prep left glide
			if (rightOccupant != null && rightOccupant.getTextValue () == "k") 
			{
				_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
				checkGlideLeft ();
			}
			break;

		// IF THIS INSTANCE HAS "d" AS TEXT VALUE, Prep Down Glide
		case "d":
			// If its left Occupant has a vowel, and instance is not blocked below, glide down
			if (leftOccupant != null && leftOccupant.getTextValue () == "o") 
			{
				//Debug.Log ("LeftOccupant not Null");
				_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
				checkGlideDown ();
			}
			break;
		// IF THIS INSTANCE HAS "p" AS TEXT VALUE, Prep Up Glide on RIGHT sides
		case "p":
			// If its left Occupant has a vowel, and instance is not blocked below, glide down
			if (leftOccupant != null && leftOccupant.getTextValue () == "o") 
			{
				_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
				checkGlideUp ();
			}
			break;
		// IF THIS INSTANCE HAS "b" AS TEXT VALUE, Prep Up Glide on LEFT side
		case "b":
			// If its left Occupant has a vowel, and instance is not blocked below, glide down
			if (rightOccupant != null && rightOccupant.getTextValue () == "o") 
			{
				_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
				checkGlideUp ();
			}
			break;
		case "v":
			// If its left Occupant has a vowel, and instance is not blocked below, glide up
			if (rightOccupant != null && rightOccupant.getTextValue () == "o") 
			{
				_rb.constraints = RigidbodyConstraints2D.FreezeRotation;
				checkGlideDown ();
			}
			break;
		default:
			break;
		}
	}

	#endregion

	#region Proximity Checks
	/// <summary>
	/// Checks immediate above this instance to see if a box is present there
	/// if not, begin moving upwards
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

	/// <summary>
	/// Checks that this instance can begin gliding upwards. If true, begin to glide
	/// else, remain in position
	/// </summary>
	private void checkGlideUp()
	{

		// If no box is located immediately below, 
		// move this instance down vertically
		if (!occupiedAbove) 
		{
			setPositions ("Glide_Up");
			//_ButtonLock = true;
			_state = BoxState.GLIDINGUP;
		}

		// If both checks fail, do not move at all
		else 
		{
			setPositions ("Stay");
			_state = BoxState.STATIONARY;
			_ButtonLock = false;
		}
	}


	/// <summary>
	/// Checks that this instance can begin moving downwards. If true, begin to move
	/// else, remain in position
	/// </summary>
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

	/// <summary>
	/// Checks that this instance can begin gliding downwards. If true, begin to move
	/// else, remain in position
	/// </summary>
	private void checkGlideDown()
	{

		// If no box is located immediately below, 
		// move this instance down vertically
		if (!occupiedBelow) 
		{
			setPositions ("Glide_Down");
			//_ButtonLock = true;
			_state = BoxState.GLIDINGDOWN;
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
	/// Checks that this instance can begin moving rightwards. If true, begin to move
	/// else, remain in position
	/// </summary>
	private void checkRight()
	{

		// If no box is located immediately below, 
		// move this instance down vertically
		if (!occupiedRight) 
		{
			setPositions ("Horizontal");
			//_ButtonLock = true;
			_state = BoxState.MOVINGUP;
		}

		// If both checks fail, do not move at all
		else 
		{
			setPositions ("Stay");
			_state = BoxState.STATIONARY;
			_ButtonLock = false;
		}
	}

	/// <summary>
	/// Checks that this instance can begin gliding rightwards. If true, begin to move
	/// else, remain in position
	/// </summary>
	private void checkGlideRight()
	{

		// If no box is located immediately below, 
		// move this instance down vertically
		if (!occupiedRight) 
		{
			setPositions ("Glide_Right");
			//_ButtonLock = true;
			_state = BoxState.GLIDINGRIGHT;
		}

		// If both checks fail, do not move at all
		else 
		{
			setPositions ("Stay");
			_state = BoxState.STATIONARY;
			_ButtonLock = false;
		}
	}


	/// <summary>
	/// Checks that this instance can begin moving left. If true, begin to move
	/// else, remain in position
	/// </summary>
	private void checkLeft()
	{

		// If no box is located immediately below, 
		// move this instance left horizontally
		if (!occupiedLeft) 
		{
			setPositions ("Horizontal");
			//_ButtonLock = true;
			_state = BoxState.MOVINGDOWN;
		}

		// If both checks fail, do not move at all
		else 
		{
			setPositions ("Stay");
			_state = BoxState.STATIONARY;
			_ButtonLock = false;
		}
	}

	/// <summary>
	/// Checks that this instance can begin gliding downwards. If true, begin to move
	/// else, remain in position
	/// </summary>
	private void checkGlideLeft()
	{

		// If no box is located immediately below, 
		// move this instance left horizontally
		if (!occupiedLeft) 
		{
			setPositions ("Glide_Left");
			//_ButtonLock = true;
			_state = BoxState.GLIDINGLEFT;
		}

		// If both checks fail, do not move at all
		else 
		{
			setPositions ("Stay");
			_state = BoxState.STATIONARY;
			_ButtonLock = false;
		}
	}
		

	/// <summary>
	/// Checks that distance between this instance and parameter does not exceed the width of a box
	/// </summary>
	/// <returns>The <see cref="System.Boolean"/>.</returns>
	/// <param name="p_Object">P object.</param>
	private bool distanceBetween(GameObject p_Object)
	{
		RectTransform collTransform = p_Object.GetComponent<RectTransform> ();
		float radiusCheck = collTransform.sizeDelta.y+1;

		float proxCheck = Vector3.Magnitude (GetComponent<RectTransform> ().anchoredPosition - collTransform.anchoredPosition);

		//Debug.Log ("proxCheck: " + proxCheck);

		if (proxCheck < radiusCheck) {
			//Debug.Log ("proxCheck: " + proxCheck);
			return true;
		}
		else
			return false;
	}
	#endregion



	#region Box Text Value modification
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

		// Else, inform GameManager to highlight this instance
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
			_rb.mass = 0f;
			break;
		case false:
			_rb.mass = 10f;
			break;
		}
	}

	#endregion


	#region Collider2D Method
	public void OnTriggerEnter2D(Collider2D coll)
	{ 

		bool movingVertical = (_state == BoxState.MOVINGDOWN) || (_state == BoxState.MOVINGUP) || (_state == BoxState.GLIDINGUP) || (_state == BoxState.GLIDINGDOWN);
		bool movingHorizontal = (_state == BoxState.GLIDINGLEFT) || (_state == BoxState.GLIDINGRIGHT);
		bool floating = (_state == BoxState.FLOATING);

		Vector2 _CP = coll.gameObject.GetComponent<RectTransform>().anchoredPosition;
		Vector2 myPos = GetComponent<RectTransform> ().anchoredPosition;
		float eps = GetComponent<RectTransform>().sizeDelta.x/2;



		// HEAD ON COLLISION CHECKS 

		// Moving Up collision
		if (_CP.y > myPos.y)
		{

			// If the distance between this instance's centre and the collision object's centre
			// is small enough, then set occupied accordingly
			if (isAbove(myPos, _CP, eps)) 
			{
				occupiedAbove = true;
				aboveOccupant = coll.gameObject.GetComponent<BoxController> ();
			}

			// If collision with border, push box to the border's peripheral
			if (coll.gameObject.CompareTag("Border_Horizontal") || movingVertical)
			{
				Vector2 myAnchor = GetComponent<RectTransform> ().anchoredPosition;
				Vector2 collAnchor = coll.gameObject.GetComponent<RectTransform> ().anchoredPosition;

				myAnchor.y = collAnchor.y - coll.gameObject.GetComponent<RectTransform> ().sizeDelta.y;
				GetComponent<RectTransform> ().anchoredPosition = myAnchor;
			}


			_state = BoxState.FREEZE;
		}

		// Moving Down collision
		if (_CP.y < myPos.y)
		{

			// If the distance between this instance's centre and the collision object's centre
			// is small enough, then set occupied accordingly
			if (isBelow(myPos, _CP, eps))
			{
				occupiedBelow = true;
				belowOccupant = coll.gameObject.GetComponent<BoxController> ();
			}

			// If collision with border, push box to the border's peripheral
			if (coll.gameObject.CompareTag("Border_Horizontal") || movingVertical)
			{
				Vector2 myAnchor = GetComponent<RectTransform> ().anchoredPosition;
				Vector2 collAnchor = coll.gameObject.GetComponent<RectTransform> ().anchoredPosition;

				myAnchor.y = collAnchor.y + coll.gameObject.GetComponent<RectTransform> ().sizeDelta.y;
				GetComponent<RectTransform> ().anchoredPosition = myAnchor;
			}


			_state = BoxState.FREEZE;
		}

		// Moving Right collision
		if (_CP.x > myPos.x)
		{

			// If the distance between this instance's centre and the collision object's centre
			// is small enough, then set occupied accordingly
			if (isRight(myPos, _CP, eps))
			{
				occupiedRight = true;
				rightOccupant = coll.gameObject.GetComponent<BoxController> ();
			}

			// If collision with border, push box to the border's peripheral
			if (coll.gameObject.CompareTag("Border_Vertical") || movingHorizontal)
			{
				Vector2 myAnchor = GetComponent<RectTransform> ().anchoredPosition;
				Vector2 collAnchor = coll.gameObject.GetComponent<RectTransform> ().anchoredPosition;
			
				myAnchor.x = collAnchor.x - coll.gameObject.GetComponent<RectTransform> ().sizeDelta.x;
				GetComponent<RectTransform> ().anchoredPosition = myAnchor;
			}

			_state = BoxState.FREEZE;
		}

		// Moving Left collision
		if (_CP.x < myPos.x)
		{

			// If the distance between this instance's centre and the collision object's centre
			// is small enough, then set occupied accordingly
			if (isLeft(myPos, _CP, eps)) 
			{
				occupiedLeft = true;
				leftOccupant = coll.gameObject.GetComponent<BoxController> ();
			}

			// If collision with border, push box to the border's peripheral
			if (coll.gameObject.CompareTag("Border_Vertical") || movingHorizontal)
			{
				Vector2 myAnchor = GetComponent<RectTransform> ().anchoredPosition;
				Vector2 collAnchor = coll.gameObject.GetComponent<RectTransform> ().anchoredPosition;

				myAnchor.x = collAnchor.x + coll.gameObject.GetComponent<RectTransform> ().sizeDelta.x;
				GetComponent<RectTransform> ().anchoredPosition = myAnchor;
			}


			_state = BoxState.FREEZE;
		}
			
	}

	public void OnTriggerStay2D(Collider2D coll)
	{
		bool movingVertical = (_state == BoxState.MOVINGDOWN) || (_state == BoxState.MOVINGUP) || (_state == BoxState.GLIDINGUP) || (_state == BoxState.GLIDINGDOWN);
		bool movingHorizontal = (_state == BoxState.GLIDINGLEFT) || (_state == BoxState.GLIDINGRIGHT);

		bool stationary = (_state == BoxState.STATIONARY);
		bool floating = (_state == BoxState.FLOATING);
	
		Vector2 _CP = coll.GetComponent<RectTransform> ().anchoredPosition;
		Vector2 myPos = GetComponent<RectTransform> ().anchoredPosition;
		float eps = GetComponent<RectTransform>().sizeDelta.x/2;

		// LINGERING COLLISION CHECKS-----------------------
		// distanceBetween check is to make sure a corner collision is not being mistaken
		// for a head on collision.
		// If FLOATING and with a lingering collision below, set occupiedBelow to true
		if ((floating || stationary) && isBelow(myPos, _CP, eps) && distanceBetween(coll.gameObject))
		{
			occupiedBelow = true;
			belowOccupant = coll.gameObject.GetComponent <BoxController> ();

			/* ADDITIONAL COLLISION RESLOUTION CHECK FOR IF THIS INSTANCE IS THE ONE IN MOTION*/
			if (movingVertical) 
			{
				myPos.y = _CP.y + coll.GetComponent<RectTransform> ().sizeDelta.y;
				GetComponent<RectTransform> ().anchoredPosition = myPos;
			}


			_state = BoxState.FREEZE;
			_ButtonLock = false;
		}

		// If FLOATING and with a lingering collision above, set occupiedAbove to true
		if ((floating || stationary) && isAbove(myPos, _CP, eps) &&  distanceBetween(coll.gameObject))
		{
			occupiedAbove = true;
			aboveOccupant = coll.gameObject.GetComponent <BoxController> ();

			/* ADDITIONAL COLLISION RESOLUTION CHECK FOR IF THIS INSTANCE IS THE ONE IN MOTION*/
			if (movingVertical) 
			{
				myPos.y = _CP.y - coll.GetComponent<RectTransform> ().sizeDelta.y;
				GetComponent<RectTransform> ().anchoredPosition = myPos;
			}


			_state = BoxState.FREEZE;
			_ButtonLock = false;
		}


		// If FLOATING and with a lingering collision to the left, set occupiedLeft to true
		if ((floating || stationary) && isLeft(myPos, _CP, eps) && distanceBetween(coll.gameObject) )
		{
			occupiedLeft = true;
			leftOccupant = coll.gameObject.GetComponent <BoxController> ();

			/* ADDITIONAL COLLISION RESOLUTION CHECK FOR IF THIS INSTANCE IS IN MOTION*/
			if (movingHorizontal) 
			{
				myPos.x = _CP.x + coll.GetComponent<RectTransform> ().sizeDelta.x;
				GetComponent<RectTransform> ().anchoredPosition = myPos;
			}

			_state = BoxState.FREEZE;
			_ButtonLock = false;
		}

		// If FLOATING and with a lingering collision to the right, set occupiedRight to true
		if ((floating || stationary) && isRight(myPos, _CP, eps) && distanceBetween(coll.gameObject))
		{
			occupiedRight = true;
			rightOccupant = coll.gameObject.GetComponent<BoxController> ();

			/* ADDITIONAL COLLISION RESOLUTION CHECK FOR IF THIS INSTANCE IS IN MOTION*/
			if (movingHorizontal) 
			{
				myPos.x = _CP.x - coll.GetComponent<RectTransform> ().sizeDelta.x;
				GetComponent<RectTransform> ().anchoredPosition = myPos;
			}


			_state = BoxState.FREEZE;
			_ButtonLock = false;
		}

		//-----------------------------------
	}


	public void OnTriggerExit2D(Collider2D coll)
	{
		
		Vector2 _CP = coll.GetComponent<RectTransform> ().anchoredPosition;
		Vector2 myPos = GetComponent<RectTransform> ().anchoredPosition;
		float eps = GetComponent<RectTransform>().sizeDelta.x/2;

		// If Collision Point of higher vertical value exits,
		// assume that this instance no longer has another box immediately above it
		if ( _CP.y > myPos.y ) 
		{
			occupiedAbove = false;
			aboveOccupant = null;

		}

		// If Collision Point of lower vertical value exits,
		// assume that this instance no longer has another box immediately below it
		if ( _CP.y < myPos.y ) 
		{
			occupiedBelow = false;
			belowOccupant = null;

		}

		// If Collision Point of lower horizontal value exits,
		// assume that this instance no longer has another box immediately left of it
		if ( _CP.x < myPos.x ) 
		{
			occupiedLeft = false;
			leftOccupant = null;

		}

		// If Collision Point of higher horizontal value exits,
		// assume that this instance no longer has another box immediately right of it
		if ( _CP.x > myPos.x ) 
		{
			occupiedRight = false;
			rightOccupant = null;
		}
			
	}


	#endregion

	#region Trigger Proximity Checks
	/// <summary>
	/// Checks if paramter pCollPos is directly above this instance up to some epsilon
	/// </summary>
	/// <returns><c>true</c>, if above was ised, <c>false</c> otherwise.</returns>
	/// <param name="p_MyPos">P my position.</param>
	/// <param name="p_CollPos">P coll position.</param>
	/// <param name="eps">Eps.</param>
	private bool isAbove(Vector2 p_MyPos, Vector2 p_CollPos, float eps)
	{

		if ((p_CollPos.y - p_MyPos.y >= eps) && (Mathf.Abs (p_MyPos.x - p_CollPos.x) < eps)) {
			return true;
		} else
			return false;
	}

	/// <summary>
	/// Checks if pCollPos is close enough to this instance to be considered beneath it
	/// </summary>
	/// <returns><c>true</c>, if below was ised, <c>false</c> otherwise.</returns>
	/// <param name="p_MyPos">P my position.</param>
	/// <param name="p_CollPos">P coll position.</param>
	/// <param name="eps">Eps.</param>
	private bool isBelow(Vector2 p_MyPos, Vector2 p_CollPos, float eps)
	{

		if ((p_CollPos.y - p_MyPos.y < eps) && (Mathf.Abs (p_MyPos.x - p_CollPos.x) < eps)) {
			return true;
		} else
			return false;
	}

	/// <summary>
	/// Check if pCollPos is close enough to this instance to be considered to the right of it
	/// </summary>
	/// <returns><c>true</c>, if right was ised, <c>false</c> otherwise.</returns>
	/// <param name="p_MyPos">P my position.</param>
	/// <param name="p_CollPos">P coll position.</param>
	/// <param name="eps">Eps.</param>
	private bool isRight(Vector2 p_MyPos, Vector2 p_CollPos, float eps)
	{

		if ((p_CollPos.x - p_MyPos.x >= eps) && (Mathf.Abs (p_MyPos.y - p_CollPos.y) < eps)) {
			return true;
		} else
			return false;
	}

	/// <summary>
	/// Check if pColl pos is close enough to this instance to be considered to the left of it
	/// </summary>
	/// <returns><c>true</c>, if left was ised, <c>false</c> otherwise.</returns>
	/// <param name="p_MyPos">P my position.</param>
	/// <param name="p_CollPos">P coll position.</param>
	/// <param name="eps">Eps.</param>
	private bool isLeft(Vector2 p_MyPos, Vector2 p_CollPos, float eps)
	{

		if ((p_CollPos.x - p_MyPos.x < eps) && (Mathf.Abs (p_MyPos.y - p_CollPos.y) < eps)) {
			return true;
		} else
			return false;
	}
		

	#endregion
	// Update is called once per frame
	void Update () {

		handleState ();
		holdPosition ();
	}
		

}
