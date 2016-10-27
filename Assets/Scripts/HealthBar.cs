﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Image))]
public class HealthBar : MonoBehaviour {
	public Unit unit;
	public Color goodColor;
	public Color badColor;

	Image image;

	HealthBar() {
		goodColor = Color.Lerp(Color.green, new Color(0,255,0,0), 0.5f);
		badColor = Color.Lerp(Color.red, new Color(255,0,0,0), 0.5f);
	}

	void Start() {
		image = GetComponent<Image> ();

		// try finding the Unit
		if (unit == null) {
			// children
			unit = GetComponentInChildren<Unit> ();
		}
		if (unit == null && transform.parent != null) {
			// parent
			unit = GetComponentInParent<Unit> ();
			if (unit == null) {
				// siblings
				unit = transform.parent.GetComponentInChildren<Unit> ();
			}
		}
	}

	void Update() {
		var ratio = unit.Health / unit.MaxHealth;

		var color = Color.Lerp(badColor, goodColor, ratio);
		image.color = color;

		//image.fillAmount = ratio;
	}
}
