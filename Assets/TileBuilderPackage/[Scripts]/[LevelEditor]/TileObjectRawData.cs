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
	
	static Sprite theSprite;
	
	void Update() {
//		SpriteRenderer s = (SpriteRenderer)renderer;
//		if (s.sprite)
//			theSprite = s.sprite;
//		else
//			s.sprite = theSprite;
//			
//		Debug.Log ("theSprite");
		
//		if (!s.sprite)
//			s.sprite = 
		
//		Collider2D[] x = GetComponents<Collider2D>();
//		foreach (var c in x)
//			DestroyImmediate(c);
//		
//		var b = gameObject.AddComponent<BoxCollider2D>();
//		
//		b.size = new Vector2(1, 1f);
//		b.center = new Vector2(0, 0);
	}
}
