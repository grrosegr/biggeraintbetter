using UnityEngine;

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
}
