using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {

	/// <summary>
	/// The movement amount by which all displacement is scaled
	/// </summary>
	private float movementAmount;


	// Use this for initialization
	void Start () {
		movementAmount = 0.66f;

	}


	#region Standard Displacement Methods
	private void verticalInput(BoxController p_Box)
	{

		// UP : Tier 1
		if (Input.GetKeyDown (KeyCode.W)) 
		{
			p_Box.setTextTo ("w");
			p_Box.setDisplacementAmount (1* movementAmount);
			p_Box.setState (BoxController.BoxState.POSITIVE);

		}


		// UP : Tier 2
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown (KeyCode.W)) 
		{
			p_Box.setTextTo ("W");
			p_Box.setDisplacementAmount (2 * movementAmount);
			p_Box.setState (BoxController.BoxState.POSITIVE);

		}

		// DOWN : Tier 1
		if (Input.GetKeyDown (KeyCode.S)) 
		{

			p_Box.setTextTo ("s");
			p_Box.setDisplacementAmount (-1 * movementAmount);
			p_Box.setState (BoxController.BoxState.NEGATIVE);

		}

		// DOWN : Tier 2
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown (KeyCode.S)) 
		{

			p_Box.setTextTo ("S");
			p_Box.setDisplacementAmount (-2 * movementAmount);
			p_Box.setState (BoxController.BoxState.NEGATIVE);

		}

	}

	private void horizontalInput(BoxController p_Box)
	{

		// LEFT : Tier 1
		if (Input.GetKeyDown (KeyCode.Q)) 
		{

			if (p_Box._state == BoxController.BoxState.STATIONARY) 
			{
				p_Box.setTextTo ("q");
				p_Box.setDisplacementAmount (-1 * movementAmount);
				p_Box.setState (BoxController.BoxState.SIDELEFT);
			}

		}

		// LEFT : Tier 2
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown (KeyCode.Q)) 
		{

			if (p_Box._state == BoxController.BoxState.STATIONARY) 
			{
				p_Box.setTextTo ("Q");
				p_Box.setDisplacementAmount (-2 * movementAmount);
				p_Box.setState (BoxController.BoxState.SIDELEFT);
			}

		}

		// RIGHT : Tier 1
		if (Input.GetKeyDown (KeyCode.E)) 
		{

			if (p_Box._state == BoxController.BoxState.STATIONARY) 
			{
				p_Box.setTextTo ("e");
				p_Box.setDisplacementAmount (1 * movementAmount);
				p_Box.setState (BoxController.BoxState.SIDERIGHT);
			}

		}

		// RIGHT : Tier 2
		if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown (KeyCode.E)) 
		{

			if (p_Box._state == BoxController.BoxState.STATIONARY) 
			{
				p_Box.setTextTo ("E");
				p_Box.setDisplacementAmount (-2 * movementAmount);
				p_Box.setState (BoxController.BoxState.SIDERIGHT);
			}

		}
	}
	#endregion


	private void glideInput(BoxController p_Box)
	{
		//GLIDES -------------------------
		// Keypress to make currentlyHighlighted box return to its startposition
		if (Input.GetKeyDown (KeyCode.P)) 
		{
			if (p_Box._state == BoxController.BoxState.STATIONARY) 
			{
				p_Box.setTextTo ("p");
			}
			else 
			{
				p_Box.setTextTo ("");
				p_Box.setState (BoxController.BoxState.RETURNING);
			}

		}

		if (Input.GetKeyDown (KeyCode.O)) 
		{
			if (p_Box._state == BoxController.BoxState.STATIONARY) 
			{
				p_Box.setTextTo ("o");
			}
			else 
			{
				p_Box.setTextTo ("");
				p_Box.setState (BoxController.BoxState.RETURNING);
			}
		}

		if (Input.GetKeyDown (KeyCode.K)) 
		{
			if (p_Box._state == BoxController.BoxState.STATIONARY) 
			{
				p_Box.setTextTo ("k");
			}
			else 
			{
				p_Box.setTextTo ("");
				p_Box.setState (BoxController.BoxState.RETURNING);
			}
		}

		if (Input.GetKeyDown (KeyCode.D)) 
		{
			if (p_Box._state == BoxController.BoxState.STATIONARY) 
			{
				p_Box.setTextTo ("d");
			}
			else 
			{
				p_Box.setTextTo ("");
				p_Box.setState (BoxController.BoxState.RETURNING);
			}
		}

		if (Input.GetKeyDown (KeyCode.B)) 
		{
			if (p_Box._state == BoxController.BoxState.STATIONARY) 
			{
				p_Box.setTextTo ("b");
			}
			else 
			{
				p_Box.setTextTo ("");
				p_Box.setState (BoxController.BoxState.RETURNING);
			}
		}

		if (Input.GetKeyDown (KeyCode.V)) 
		{
			if (p_Box._state == BoxController.BoxState.STATIONARY) 
			{
				p_Box.setTextTo ("v");
			}
			else 
			{
				p_Box.setTextTo ("");
				p_Box.setState (BoxController.BoxState.RETURNING);
			}
		}
	}

	/// <summary>
	/// Handles the input of player
	/// </summary>
	public void handleInput(BoxController _CurrentlyHighlighted)
	{
		// NOT CURRENTLY IMPLEMENTED
		// If _CurrentlyHighlighted is in the process of having a force applied,
		// no other input will be registered until that force is done being applied
		if (_CurrentlyHighlighted.getButtonLock ())
			return;
		//-------------------

		verticalInput (_CurrentlyHighlighted);
		glideInput (_CurrentlyHighlighted);
		//horizontalInput (_CurrentlyHighlighted);


		// Keypress to make currentlyHighlighted box return to its startposition
		if (Input.GetKeyDown (KeyCode.Backspace)) 
		{
			_CurrentlyHighlighted.setTextTo ("");
			_CurrentlyHighlighted.setState (BoxController.BoxState.RETURNING);

		}
		//----------------------------------

	
	}
		
}
