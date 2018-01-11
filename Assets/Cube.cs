using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cube : MonoBehaviour {
	private Color color = Color.white;

	public Color Color {
		get {
			return color;
		}

		set {
			color = value;
		}
	}



}
