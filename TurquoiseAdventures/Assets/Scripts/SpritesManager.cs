﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesManager : MonoBehaviour {

	/// <summary>
	/// List of Sprites for Boxes to access
	/// </summary>
	public static Sprite[] Sprites;

	[SerializeField]
	private Sprite _Sprite0;
	[SerializeField]
	private Sprite _Sprite1;
	[SerializeField]
	private Sprite _Sprite2;

	/// <summary>
	/// Sets the sprites for SpritesManager to use inGame
	/// </summary>
	public void setSprites()
	{
		Sprites = new Sprite[3];
		Sprites [0] = _Sprite0;
		Sprites [1] = _Sprite1;
		Sprites [2] = _Sprite2;
	}

}
