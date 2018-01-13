using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cube : MonoBehaviour {
	private Color color = Color.white;

    private Vector2 position;

	public Color Color {
		get {
			return color;
		}

		set {
			color = value;
		}
	}

    Cube(int xpos, int ypos, Color? col = null) {
        position = new Vector2(xpos, ypos);

        color = col ?? Color.white;
    }



}
