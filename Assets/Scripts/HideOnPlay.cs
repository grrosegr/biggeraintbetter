using UnityEngine;
using System.Collections;

public class HideOnPlay : MonoBehaviour {

	void Start () {
		renderer.enabled = false;
	}
}
