using UnityEngine;
using System.Collections;
using System.Globalization;

[ExecuteInEditMode]
public class TileAligner : MonoBehaviour {
	private Vector2 grid_WidthHeight = new Vector2(1.0f, 1.0f);

	public void SetSnapWidthHeigth(Vector2 snapWidthHeigth) {
		grid_WidthHeight = snapWidthHeigth;
	}

	private void Update() {
//		bool addRectOffsetX;// = grid_WidthHeight.x % 2 > 0;
//		bool addRectOffsetY;// = grid_WidthHeight.y % 2 > 0;
		
		float offsetX;
		float offsetY;
		
		if (Mathf.Approximately(transform.localScale.x, 1)) {
			offsetX = .5f;
			offsetY = .5f;
		} else {
			offsetX = 0;
			offsetY = 0;
		}
				
		float snapX = Mathf.Floor(this.gameObject.transform.position.x) + offsetX;
		float snapY = Mathf.Floor(this.gameObject.transform.position.y) + offsetY;
		float snapZ = Mathf.Floor(this.gameObject.transform.position.z);
        
		this.gameObject.transform.position = new Vector3(snapX, snapY, snapZ);
	}
}