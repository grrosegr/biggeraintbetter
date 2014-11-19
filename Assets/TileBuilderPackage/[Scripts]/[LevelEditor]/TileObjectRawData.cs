using UnityEngine;

[ExecuteInEditMode]
public class TileObjectRawData : MonoBehaviour {
	//Class that holds data of this object.
	[HideInInspector]
	public int collisionIndex;
	[HideInInspector]
	public int layerDepth;
	[HideInInspector]
	public int selectedTileInteger;
	[HideInInspector]
	public string selectedTileName;
	[HideInInspector]
	public bool stackableObject;
	
	private bool updated = false;
	void Update() {	
		if (updated)		return;
		updated = true;
		
		Collider2D[] x = GetComponents<Collider2D>();
		foreach (var c in x)
			DestroyImmediate(c);
		
		var b = gameObject.AddComponent<BoxCollider2D>();
		
		SpriteRenderer t = GetComponent<SpriteRenderer>();
		if (t.sprite.name == "Donuts_0" || t.sprite.name == "Donuts_1" || t.sprite.name == "Donuts_2") {
			b.size = new Vector2(1, 0.9f);
			b.center = new Vector2(0, -0.05f);
		}
	}
}
