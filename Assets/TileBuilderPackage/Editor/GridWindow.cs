using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

using System.IO;

public enum BrushOption {
    CREATE,
    DELETE,
    COPY,
    RESET
}

public class GridWindow : EditorWindow {
    #region Variables
    private Texture2D CreateTileBrush;
    private Texture2D DeleteTileBrush;
    private Texture2D CopyTileBrush;
    private Texture2D ResetTileBrush;

    private bool CreateTileBrushActive = true;
    private bool DeleteTileBrushActive = false;
    private bool CopyTileBrushActive = false;
    private bool ResetTileBrushActive = false;

    private BrushOption brushOption;

    private Object TilePrefab;
    private GameObject LevelParent;

    private List<GameObject> selectedGameObjects = new List<GameObject>();

    private string selectedWindowTab;
    private string savedRowTagDataLocation;
    private string savedEditorWindowDataLocation;

    private Vector2 grid_WidthHeight = new Vector2(1.0f, 1.0f);

    private int tile_Index = 0;
    private int tile_LayerDepth = 0;
    private int tile_CollisionIndex = 0;
    private bool tile_Stackable = false;
    private string tile_Name = "";
    private string[] tile_CollisionModels = new string[]
	{
		"Polygon",
		"Box",
		"Circle",
		"Edge",
		"None"
	};

    private List<RowData> rowDataList = new List<RowData>();
    private List<Sprite> temporaryKeywordSpriteList = new List<Sprite>();
    private List<Texture> drawTextureInWindowList = new List<Texture>();

    private KeyCode buildingKey = KeyCode.A;
    private KeyCode deletingKey = KeyCode.D;
    private KeyCode copyKey = KeyCode.C;
    private KeyCode resetKey = KeyCode.R;

    private string s_rowName = "";
    private string s_tagName = "";

    private int currentRowCount;
    private int maximumRowCount = 10;

    private int previewButtonWH = 70;
    private int tileWidthAndOffset;
    private int maxTilesPerRowEditorWindow;
    private Vector2 scrollPos;

    private Event _event;

    private Vector2 grid_RowsCollumns = new Vector2(1, 1);
    private Color grid_Color = new Color(1, 1, 1, 1);
    private bool grid_ShowPreviewCursor = true;

    private Vector3[] hoverPreviewVertices = new Vector3[4];

    #endregion

    #region Unity Methods
    private void Awake() {
        LoadResources();
    }

    private void OnEnable() {
        CreateLevelParent();
        FillEditorWindowSettings();
        EnableSceneDelegate();
    }

    private void OnDisable() {

    }

    private void OnDestroy() {
        DeleteTileAlignerComponents();
        UnloadResources();
        SaveEditorWindowSettings();
        ClearEditorWindowSettings();
        DisableSceneDelegate();

        AssetDatabase.Refresh();
    }

    private void OnGUI() {
        UpdateEditorWindowInterface();
    }

    private void Update() {
        CloseWindowOnEnteringPlayMode();
    }
    #endregion

    #region Init / Destroy Methods
    private void CreateLevelParent() {
        LevelParent = GameObject.Find("_LevelParent");

        if (LevelParent != null) {
            return;
        }

        LevelParent = new GameObject();
        LevelParent.name = "_LevelParent";
    }

    private void DeleteTileAlignerComponents() {
        foreach (Transform child in LevelParent.transform) {
            if (child.GetComponent<TileAligner>() != null) {
                DestroyImmediate(child.GetComponent<TileAligner>());
            }
        }
        selectedGameObjects.Clear();
    }

    private void LoadResources() {
        CreateTileBrush = (Texture2D)Resources.Load(@"EditorIcons/CreateTileIcon");
        DeleteTileBrush = (Texture2D)Resources.Load(@"EditorIcons/DeleteTileIcon");
        CopyTileBrush = (Texture2D)Resources.Load(@"EditorIcons/CopyTileIcon");
        ResetTileBrush = (Texture2D)Resources.Load(@"EditorIcons/ResetTileIcon");

        TilePrefab = Resources.Load(@"[Prefabs]/TilePrefab"); //The base prefab that is being used to build upon.

        savedEditorWindowDataLocation = AssetDatabase.GetAssetPath(Resources.Load("SaveData/EditorWindowSettings"));

        savedRowTagDataLocation = AssetDatabase.GetAssetPath(Resources.Load("SaveData/TagData"));
        ImageLibrary.GetSpriteAndTextureData();
    }

    private void UnloadResources() {
        ImageLibrary.DestroySpriteAndTextureData();
    }

    private void FillEditorWindowSettings() {
        ClearEditorWindowSettings();
        LoadEditorWindowSettings();
    }

    private void ClearEditorWindowSettings() {
        rowDataList.Clear();

        temporaryKeywordSpriteList.Clear();
        drawTextureInWindowList.Clear();
        selectedWindowTab = "";
    }

    private void SaveEditorWindowSettings() {
        //---------- Rows and Tags ----------//
        string rowTagData = "";

        File.Create(savedRowTagDataLocation).Close();
        TextWriter myTextWriter = new StreamWriter(savedRowTagDataLocation);

        for (int i = 0; i < rowDataList.Count; i++) {
            rowTagData += rowDataList[i].RowName + "|";

            for (int j = 0; j < rowDataList[i].TagDataList.Count; j++) {
                rowTagData += rowDataList[i].TagDataList[j].TagName;

                if (j < rowDataList[i].TagDataList.Count - 1) {
                    rowTagData += ",";
                }
            }

            myTextWriter.WriteLine(rowTagData);
            rowTagData = "";
        }

        myTextWriter.Close();

        //---------- Editor Settings ----------//
        File.Create(savedEditorWindowDataLocation).Close();
        string editorWindowSettingsData = grid_RowsCollumns.x + "," + grid_RowsCollumns.y + "," + grid_WidthHeight.x + "," + grid_WidthHeight.y + "," + grid_Color.r + "," + grid_Color.g + "," + grid_Color.b + "," + grid_Color.a;
        TextWriter myEditorWindowTextWriter = new StreamWriter(savedEditorWindowDataLocation);

        myEditorWindowTextWriter.Write(editorWindowSettingsData);
        myEditorWindowTextWriter.Close();
    }

    private void LoadEditorWindowSettings() {
        TextAsset rowTagFile = (TextAsset)Resources.Load("SaveData/TagData", typeof(TextAsset));
        char[] charRemoval = new char[] { '\r', '\n' };
        string[] rowTagFileLines = rowTagFile.text.Split(charRemoval, System.StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < rowTagFileLines.Length; i++) {
            string[] rowTagLine = rowTagFileLines[i].Split('|'); //Split the Row from the Tags.

            RowData rowData = new RowData();
            rowData.RowName = rowTagLine[0].ToString(); //First {0} integer is the Row string.
            rowDataList.Add(rowData);

            string[] tags = rowTagLine[1].Split(','); //Second {1} integer is the Tag string.
            for (int j = 0; j <= tags.Length - 1; j++) {
                TagData tagData = new TagData();
                string tag = tags[j].ToString();
                tagData.TagName = tag;
                rowDataList[i].TagDataList.Add(tagData);
            }
        }

        StreamReader myEditorSettingsStreamReader = new StreamReader(savedEditorWindowDataLocation);
        string editorWindowSettingsDataString = myEditorSettingsStreamReader.ReadToEnd();

        if (System.String.IsNullOrEmpty(editorWindowSettingsDataString)) {
            return;
        }

        string[] settings = editorWindowSettingsDataString.Split(',');
        grid_RowsCollumns.x = float.Parse(settings[0]);
        grid_RowsCollumns.y = float.Parse(settings[1]);

        grid_WidthHeight.x = float.Parse(settings[2]);
        grid_WidthHeight.y = float.Parse(settings[3]);

        grid_Color.r = float.Parse(settings[4]);
        grid_Color.g = float.Parse(settings[5]);
        grid_Color.b = float.Parse(settings[6]);
        grid_Color.a = float.Parse(settings[7]);
    }

    private void EnableSceneDelegate() {
        // Remove first so it doesnt stack
        SceneView.onSceneGUIDelegate -= GridUpdate;
        // Then add delegate 
        SceneView.onSceneGUIDelegate += GridUpdate;
    }

    private void DisableSceneDelegate() {
        SceneView.onSceneGUIDelegate -= GridUpdate;
    }
    #endregion

    #region Object Spawning
    private void InstantiatePrefabSettings(GameObject go) {
        RetrieveSpriteDataInteger(go);
        AttachCollisionToSprite(go);
        AttachLayerDepthToSprite(go);
        AttachSpriteObjectToParent(go);
        AttachNameToSprite(go);
        SaveTileCreationData(go);
    }

    private void RetrieveSpriteDataInteger(GameObject go) {
        foreach (KeyValuePair<int, Sprite> pair in ImageLibrary.spriteDictionary) {
            if (pair.Key == tile_Index) {
                go.GetComponent<SpriteRenderer>().sprite = pair.Value;
            }
        }
    }

    private void AttachCollisionToSprite(GameObject go) {
        switch (tile_CollisionIndex) {
            case 0://"Polygon":
                go.AddComponent<PolygonCollider2D>();
                break;
            case 1://"Box":
                go.AddComponent<BoxCollider2D>();
                break;
            case 2://"Cirle":
                go.AddComponent<CircleCollider2D>();
                break;
            case 3://"Edge":
                go.AddComponent<EdgeCollider2D>();
                break;
            case 4://"None":
                break;
            default:
                break;
        }
    }

    private void AttachLayerDepthToSprite(GameObject go) {
        go.GetComponent<SpriteRenderer>().sortingOrder = tile_LayerDepth;
    }

    private void AttachSpriteObjectToParent(GameObject go) {
        if (LevelParent == null) {
            return;
        }

        go.transform.parent = LevelParent.transform;
    }

    public void AttachNameToSprite(GameObject go) {
        go.name = ("[Tile] " + "[" + tile_Name + "] [" + go.transform.position.x + "X] " + "[" + go.transform.position.y + "Y] " + "[" + tile_LayerDepth + "L]").ToString();
    }

    private void SaveTileCreationData(GameObject go) {
        var c = go.AddComponent<TileObjectRawData>();

        c.collisionIndex = tile_CollisionIndex;
        c.layerDepth = tile_LayerDepth;
        c.selectedTileInteger = tile_Index;
        c.selectedTileName = tile_Name;
        c.stackableObject = tile_Stackable;
    }

    private void CopySelectedTile(GameObject go) {
        if (go.GetComponent<TileObjectRawData>() != null) {
            var c = go.GetComponent<TileObjectRawData>();

            tile_CollisionIndex = c.collisionIndex;
            tile_LayerDepth = c.layerDepth;
            tile_Index = c.selectedTileInteger;
            tile_Name = c.selectedTileName;
            tile_Stackable = c.stackableObject;
        }
    }

    private void ResetSelectedTileShape(GameObject go) {
        if (go.GetComponent<TileObjectRawData>() != null) {
            go.transform.localScale = new Vector2(grid_WidthHeight.x, grid_WidthHeight.y);
            go.transform.rotation = Quaternion.identity;
        }
    }
    #endregion

    #region EditorWindow Runtime
    private bool SetCategoryOption() {
        for (int i = 0; i < rowDataList.Count; i++) {
            rowDataList[i].RowActive = false;
        }
        return true;
    }

    private bool SetSubCategoryOption(List<TagData> _tagDataList) {
        for (int i = 0; i < _tagDataList.Count; i++) {
            _tagDataList[i].TagActive = false;
        }
        return true;
    }

    private void ExtractKeywordSpritesFromLibrary(string keyword) {
        temporaryKeywordSpriteList.Clear();

        foreach (Sprite sprite in ImageLibrary.spriteImages) {
            if (sprite.name.ToLower().Contains(keyword.ToLower())) {
                temporaryKeywordSpriteList.Add(sprite);
            }
        }

        PutExtractedKeywordsInDrawingList();
    }

    //Extract the Keywords once, put them in a seperate list, so that the same keywords arent being added every single update call.
    private void PutExtractedKeywordsInDrawingList() {
        drawTextureInWindowList.Clear();

        for (int i = 0; i < temporaryKeywordSpriteList.Count; i++) {
            for (int j = 0; j < ImageLibrary.textureImages.Count; j++) {
                if (temporaryKeywordSpriteList[i].name.ToLower() == ImageLibrary.textureImages[j].name.ToLower()) {
                    drawTextureInWindowList.Add(ImageLibrary.textureImages[j]);
                }
            }
        }
    }

    private void CloseWindowOnEnteringPlayMode() {
        EditorApplication.playmodeStateChanged = () => {
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying) {
                this.Close(); //Whenever Close() is called, OnDestroy() is automatically called.
            }
        };
    }

    private void SetPalletteBrushOption(BrushOption brushOption) {
        CreateTileBrushActive = DeleteTileBrushActive = CopyTileBrushActive = ResetTileBrushActive = false;

        switch (brushOption) {
            case BrushOption.CREATE:
                CreateTileBrushActive = true;
                break;
            case BrushOption.DELETE:
                DeleteTileBrushActive = true;
                break;
            case BrushOption.COPY:
                CopyTileBrushActive = true;
                break;
            case BrushOption.RESET:
                ResetTileBrushActive = true;
                break;
        }

        Repaint();
    }
    #endregion

    #region EditorWindow Updates
    //Draw the extracted sprite textures into the window.
    private void DrawTheExtractedSpritesInTheEditorWindow() {
        for (int i = 0; i < drawTextureInWindowList.Count; i++) {

            int xPos_StartPos = 10; //10 pixels from the left handside of the BeginArea() border.
            int yPos_StartPos = 10; //10 pixels from the top of the BeginArea() border.
            int xPos_Offset = 80;   //80 pixels total from left to right for the tileWidth (70) and offset between tiles (10).  
            int yPos_Offset = 80;   //You could also use: (int yPos_Offset = drawTextureInWindowList [i].height + 10) if you are sure that all your tiles are exactly the same size. This will scale the window nicely.
            int row = (int)(Mathf.Floor(i / maxTilesPerRowEditorWindow) * yPos_Offset);
            int collumn = (int)(maxTilesPerRowEditorWindow * row);

            tileWidthAndOffset = xPos_Offset + 1;

            if (GUI.Button(new Rect(xPos_StartPos + xPos_Offset * i - collumn, yPos_StartPos + row, previewButtonWH, previewButtonWH), drawTextureInWindowList[i])) {
                foreach (KeyValuePair<int, Sprite> pair in ImageLibrary.spriteDictionary) {
                    if (drawTextureInWindowList[i].name.ToLower() == pair.Value.name.ToLower()) {
                        tile_Index = pair.Key;
                        tile_Name = pair.Value.name;
                    }
                }

                SetPalletteBrushOption(BrushOption.CREATE);
            }
        }
    }

    private void UpdateEditorWindowInterface() {
        int buttonHeight = 18;
        currentRowCount = rowDataList.Count;

        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add Row" + " (" + currentRowCount.ToString() + "/" + maximumRowCount.ToString() + ")", EditorStyles.miniButtonLeft, GUILayout.Width(position.width * 0.14f), GUILayout.Height(buttonHeight))) {
            if (rowDataList.Count >= maximumRowCount) { //Cant add more rows if cap has been reached.
                return;
            }

            if (s_rowName != "") {
                RowData rowData = new RowData();
                rowData.RowName = s_rowName;
                rowDataList.Add(rowData);

                s_rowName = "";
            }
        }

        s_rowName = GUILayout.TextField(s_rowName, EditorStyles.textField, GUILayout.Width(position.width * 0.74f), GUILayout.Height(buttonHeight));

        if (GUILayout.Button("Delete All", EditorStyles.miniButtonRight, GUILayout.Width(position.width * 0.10f), GUILayout.Height(buttonHeight))) {
            if (EditorUtility.DisplayDialog("Delete all?", "Are you sure you want to delete all rows and tags? ", "Delete", "Cancel")) {
                ClearEditorWindowSettings();
            }
        }
        GUILayout.EndHorizontal();

        for (int i = 0; i < rowDataList.Count; i++) {
            if (rowDataList[i].RowActive = GUILayout.Toggle(rowDataList[i].RowActive, rowDataList[i].RowName, EditorStyles.miniButtonLeft, (rowDataList[i].RowActive ? GUILayout.Width(position.width * 0.885f) : GUILayout.Width(position.width)), GUILayout.Height(buttonHeight))) {
                rowDataList[i].RowActive = SetCategoryOption();

                GUILayout.BeginHorizontal();
                if (rowDataList[i].RowActive) {
                    if (GUILayout.Button("Add Tag", EditorStyles.miniButtonLeft, GUILayout.Width(position.width * 0.12f), GUILayout.Height(buttonHeight))) {

                        if (s_tagName != "") {
                            TagData tagData = new TagData();
                            tagData.TagName = s_tagName;
                            rowDataList[i].TagDataList.Add(tagData);

                            s_tagName = "";
                        }
                    }
                }
                s_tagName = GUILayout.TextField(s_tagName, EditorStyles.textField, GUILayout.Width(position.width * 0.15f), GUILayout.Height(buttonHeight));

                for (int j = rowDataList[i].TagDataList.Count - 1; j >= 0; j--) {
                    if (rowDataList[i].TagDataList[j].TagActive = GUILayout.Toggle(rowDataList[i].TagDataList[j].TagActive, rowDataList[i].TagDataList[j].TagName, EditorStyles.miniButtonMid, GUILayout.Height(buttonHeight))) {
                        rowDataList[i].TagDataList[j].TagActive = SetSubCategoryOption(rowDataList[i].TagDataList);

                        //Make sure that once a tab has been pressed, it only extracts the keyword once. Dont execute this line every update frame because the keyword cant change if not pressed manually on another tab.
                        if (selectedWindowTab != rowDataList[i].TagDataList[j].TagName.ToString()) {
                            ExtractKeywordSpritesFromLibrary(rowDataList[i].TagDataList[j].TagName.ToString());
                            selectedWindowTab = rowDataList[i].TagDataList[j].TagName.ToString();
                        }

                        if (GUILayout.Button("X", EditorStyles.miniButtonRight, GUILayout.Height(buttonHeight), GUILayout.Width(position.width * 0.05f))) {
                            if (EditorUtility.DisplayDialog("Delete Tag?", "Are you sure you want to delete this tag? ", "Delete", "Cancel")) {

                                rowDataList[i].TagDataList.RemoveAt(j);

                                temporaryKeywordSpriteList.Clear();
                                drawTextureInWindowList.Clear();
                            }
                        }
                    }
                }
                GUILayout.EndHorizontal();

                if (GUI.Button(new Rect(position.width * 0.889f, 20.5f + (buttonHeight * i + 2 * i), position.width * 0.10f, buttonHeight), "Delete Row", EditorStyles.miniButtonRight)) {

                    if (EditorUtility.DisplayDialog("Delete Row?", "Are you sure you want to delete this row? ", "Delete", "Cancel")) {
                        rowDataList.RemoveAt(i);
                    }
                }
            }
        }

        GUILayout.Space(20);

        Rect TileArea = new Rect(200, 60 + 20 * rowDataList.Count, position.width - 310, (position.height - 110) - (20 * rowDataList.Count));
        maxTilesPerRowEditorWindow = Mathf.FloorToInt(TileArea.width / (tileWidthAndOffset));

        GUILayout.BeginHorizontal();

        GUILayout.BeginArea(new Rect(10, 60 + 20 * rowDataList.Count, 180, TileArea.height), EditorStyles.textField);

        GUILayout.Space(10);
        GUILayout.Label(new GUIContent("How to use (Hover me)", "Consult the read-me file. \n\nKey '" + buildingKey + "': Spawn brush.\n\nKey '" + deletingKey + "': Delete brush.\n\nKey '" + copyKey + "': Copy brush.\n\nKey '" + resetKey + "': Reset brush.\n\nRight Mouse click to execute the brush functions."), EditorStyles.boldLabel);
        GUILayout.Space(5);

        GUILayout.Label("Tag Category: " + selectedWindowTab, EditorStyles.boldLabel);
        GUILayout.Label(new GUIContent("Amount of Tiles: " + temporaryKeywordSpriteList.Count.ToString(), "The amount of tiles currently in this category."));
        GUILayout.Label(new GUIContent("Tile Integer: " + tile_Index.ToString(), "The integer representing the selected tile. This integer is used in the code to find the correct spritesheet file."));
        GUILayout.Label(new GUIContent("Tile Name: " + tile_Name.ToString(), "The name of the current selected tile."));
        tile_LayerDepth = EditorGUILayout.IntField(new GUIContent("Tile Depth: ", "The depth the sprite will be placed at. Default depth is 0."), tile_LayerDepth, EditorStyles.numberField);
        tile_Stackable = EditorGUILayout.Toggle(new GUIContent("Stack this Tile?", "Check this button if you want to place your currently selected tile ontop of another tile. This is useful if you would like to build a miscellaneous item ontop of an environmental building block. (Limited to two tiles stacked up)"), tile_Stackable, EditorStyles.toggle);
        GUILayout.Space(5);
        GUILayout.Label(new GUIContent("Select collision model:", "Select the Collision Model you would like to have your sprite use."));
        tile_CollisionIndex = EditorGUILayout.Popup(tile_CollisionIndex, tile_CollisionModels);
        GUILayout.Space(5);

        #region GUI Layout grid properties
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        Rect scale = GUILayoutUtility.GetLastRect();
        EditorGUIUtility.LookLikeControls(scale.width / 2f);
        grid_RowsCollumns = EditorGUILayout.Vector2Field("Collumns and Rows", grid_RowsCollumns);
        grid_WidthHeight = EditorGUILayout.Vector2Field("Tile Width and Heigth", grid_WidthHeight);
        GUILayout.Space(15);
        grid_Color = EditorGUILayout.ColorField("Grid color", grid_Color);
        grid_ShowPreviewCursor = EditorGUILayout.Toggle(new GUIContent("Tile Preview", "Shows a rectangle at the current mouse position inside the grid."), grid_ShowPreviewCursor, EditorStyles.toggle);
        #endregion

        GUILayout.EndArea();

        GUILayout.BeginArea(TileArea, EditorStyles.textField);
        scrollPos = GUI.BeginScrollView(new Rect(0, 0, TileArea.width, TileArea.height), scrollPos, new Rect(0, 0, 400, (Mathf.Ceil(temporaryKeywordSpriteList.Count / maxTilesPerRowEditorWindow) + 1) * (tileWidthAndOffset)));
        DrawTheExtractedSpritesInTheEditorWindow();
        GUI.EndScrollView();
        GUILayout.EndArea();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginArea(new Rect(position.width - 100, 60 + 20 * rowDataList.Count, 84, TileArea.height), EditorStyles.textField);
        if (GUI.Button(new Rect(10, 10, 64, 64), CreateTileBrush, CreateTileBrushActive ? EditorStyles.objectFieldThumb : GUI.skin.button)) {
            SetPalletteBrushOption(BrushOption.CREATE);
        }

        if (GUI.Button(new Rect(10, 84, 64, 64), DeleteTileBrush, DeleteTileBrushActive ? EditorStyles.objectFieldThumb : GUI.skin.button)) {
            SetPalletteBrushOption(BrushOption.DELETE);
        }

        if (GUI.Button(new Rect(10, 158, 64, 64), CopyTileBrush, CopyTileBrushActive ? EditorStyles.objectFieldThumb : GUI.skin.button)) {
            SetPalletteBrushOption(BrushOption.COPY);
        }

        if (GUI.Button(new Rect(10, 232, 64, 64), ResetTileBrush, ResetTileBrushActive ? EditorStyles.objectFieldThumb : GUI.skin.button)) {
            SetPalletteBrushOption(BrushOption.RESET);
        }
        GUILayout.EndArea();
    }

    private void GridUpdate(SceneView sceneView) {
        if (TilePrefab == null) {
            return;
        }

        _event = Event.current;

        //Put these in a dictionary.
        if (_event.keyCode == buildingKey) {
            SetPalletteBrushOption(BrushOption.CREATE);
        } else if (_event.keyCode == deletingKey) {
            SetPalletteBrushOption(BrushOption.DELETE);
        } else if (_event.keyCode == copyKey) {
            SetPalletteBrushOption(BrushOption.COPY);
        } else if (_event.keyCode == resetKey) {
            SetPalletteBrushOption(BrushOption.RESET);
        }

        Handles.color = grid_Color;
        float size_x = grid_RowsCollumns.x;
        float size_y = grid_RowsCollumns.y;

        // Draw all X grid lines
        for (int i = 0; i <= size_x; i++) {
            // draw a line equal to x and the full height
            Handles.DrawLine(new Vector3(i * grid_WidthHeight.x, 0, 0), new Vector3(i * grid_WidthHeight.x, size_y * grid_WidthHeight.y, 0));
        }

        // Draw all Y grid lines
        for (int i = 0; i <= size_y; i++) {
            // draw a line equal to x and the full height
            Handles.DrawLine(new Vector3(0, i * grid_WidthHeight.y, 0), new Vector3(size_x * grid_WidthHeight.x, i * grid_WidthHeight.y, 0));
        }

        //TODO: Uncomment the line below if you want to only be able to create tiles inside your grid area. - ALSO Uncomment the bracket at line 728.
        //if ((mousePositionGUI.x > 0 && mousePositionGUI.x < size_x * grid_WidthHeight.x) && (mousePositionGUI.y > 0 && mousePositionGUI.y < size_y * grid_WidthHeight.y)) {
        // mouse is within a tile
        Ray ray = Camera.current.ScreenPointToRay(new Vector3(_event.mousePosition.x, -_event.mousePosition.y + Camera.current.pixelHeight));
        Vector3 mousePosition = ray.origin;
        Vector3 aligned = new Vector3(Mathf.Floor(mousePosition.x / grid_WidthHeight.x) * grid_WidthHeight.x + grid_WidthHeight.x / 2.0f, Mathf.Floor(mousePosition.y / grid_WidthHeight.y) * grid_WidthHeight.y + grid_WidthHeight.y / 2.0f, 0.0f);

        hoverPreviewVertices[0] = new Vector3(aligned.x - grid_WidthHeight.x / 2f, aligned.y + grid_WidthHeight.y / 2f, 0);
        hoverPreviewVertices[1] = new Vector3(aligned.x + grid_WidthHeight.x / 2f, aligned.y + grid_WidthHeight.y / 2f, 0);
        hoverPreviewVertices[2] = new Vector3(aligned.x + grid_WidthHeight.x / 2f, aligned.y - grid_WidthHeight.y / 2f, 0);
        hoverPreviewVertices[3] = new Vector3(aligned.x - grid_WidthHeight.x / 2f, aligned.y - grid_WidthHeight.y / 2f, 0);

        if (grid_ShowPreviewCursor) {
            Handles.DrawSolidRectangleWithOutline(hoverPreviewVertices, new Color(1, 0, 0, 0.2f), new Color(0, 0, 0, 1));
            SceneView.RepaintAll();
        }


        if (CreateTileBrushActive && _event.type == EventType.mouseDown && _event.button == 1 || CreateTileBrushActive && _event.isKey && _event.keyCode == KeyCode.Space) { //e.isKey && e.keyCode == buildingKey
            GameObject tile;

            if (!Physics2D.GetRayIntersection(ray)) { //If there is NO tile built already on the spot you want to build.
                if (!tile_Stackable) {
                    tile = (GameObject)PrefabUtility.InstantiatePrefab(TilePrefab);
                    tile.transform.position = aligned;
                    tile.transform.localScale = new Vector3(grid_WidthHeight.x, grid_WidthHeight.y, 1);

                    InstantiatePrefabSettings(tile);
                    Undo.RegisterCreatedObjectUndo(tile, "Create: " + tile.name);
                }
                //----------------------------- Pointing at a gridspace that is already filled -----------------------------//
                //NOTE: If you dont want to stack objects, simply delete this entire Else statement 
                //If you get into this coding block, that means that you are pointing at a Grid space that is already being used by another tile object.
            } else {
                //Check if the CURRENT selected tile object is able to stack.
                if (tile_Stackable) {
                    //Check if the item we want to build upon is in fact a tile, and has a class attached to it called TileObjectRawData.
                    if (Physics2D.GetRayIntersection(ray).collider.gameObject.GetComponent<TileObjectRawData>() != null) {
                        //Explanation: The object we want to stack with should be TRUE, the object we want to build upon should be FALSE.
                        //Check if the object that is being pointed at, its stackableObject bool is FALSE. If it is FALSE, that means that it is able to being build upon with a new TRUE stackable object. 
                        //The new TRUE object will return if you want to create another building block ontop of the newely created tile. 
                        //(This means that you are able to create up to two layers ontop of each other, for instance a Miscelanious item ontop of a Environmental Building Block).
                        if (!Physics2D.GetRayIntersection(ray).collider.gameObject.GetComponent<TileObjectRawData>().stackableObject) {
                            tile = (GameObject)PrefabUtility.InstantiatePrefab(TilePrefab);
                            tile.transform.position = aligned;

                            //Store original layerdepth.
                            int initialDepth = tile_LayerDepth;
                            //Apply +1 so that the object is moved ontop of the original object.
                            tile_LayerDepth = Physics2D.GetRayIntersection(ray).collider.gameObject.GetComponent<TileObjectRawData>().layerDepth + 1;
                            InstantiatePrefabSettings(tile);

                            //Restore its orignial layerdepth.
                            tile_LayerDepth = initialDepth;
                            Undo.RegisterCreatedObjectUndo(tile, "Create: " + tile.name);
                        }

                    }
                }
            }
            //------------------------------------------------------- END -------------------------------------------------------//

        } else if (DeleteTileBrushActive && _event.type == EventType.mouseDown && _event.button == 1) { //(e.isKey && e.keyCode == deletingKey)
            foreach (GameObject obj in Selection.gameObjects) {
                if (obj.GetComponent<TileObjectRawData>() != null) {
                    Undo.DestroyObjectImmediate(obj);
                    DestroyImmediate(obj);
                }
            }

        } else if (CopyTileBrushActive && _event.type == EventType.mouseDown && _event.button == 1) {
            if (Selection.activeGameObject == null) {
                return;
            }
            CopySelectedTile(Selection.activeGameObject);
            SetPalletteBrushOption(BrushOption.CREATE);
        } else if (ResetTileBrushActive && _event.type == EventType.mouseDown && _event.button == 1) {
            foreach (GameObject go in Selection.gameObjects) {
                ResetSelectedTileShape(go);
            }
        }

        //Create a Tile Aligner component when you select a tile prefab.
        if (Selection.gameObjects.Length > 0) {
            foreach (GameObject go in Selection.gameObjects) {
                if (go.GetComponent<TileObjectRawData>() != null) {
                    if (go.GetComponent<TileAligner>() == null) {
                        TileAligner tileAligner = go.AddComponent<TileAligner>();
                        tileAligner.SetSnapWidthHeigth(grid_WidthHeight);

                        selectedGameObjects.Add(go);
                    }
                    AttachNameToSprite(go);
                }
            }
        }
        //}
    }
    #endregion
}