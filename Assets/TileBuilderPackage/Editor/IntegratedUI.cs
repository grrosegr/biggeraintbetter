using UnityEngine;
using UnityEditor;

static class IntegratedUI {
	[MenuItem("Tile Builder/Editor Window")]
	static void Menu_Open_TileBuilder_Window() {
		GridWindow window = (GridWindow)EditorWindow.GetWindow(typeof(GridWindow));
		window.minSize = new Vector2(750, 460);
	}
}
