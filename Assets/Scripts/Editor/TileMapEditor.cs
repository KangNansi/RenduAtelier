using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMap))]
[CanEditMultipleObjects]
public class TileMapEditor : Editor {
    public Vector2 scrollPosition;
    int tx=0;
    int ty=0;

	public int brush_size;
	public float brush_opacity;


    int getInvertedY(int ty)
    {
        var t = (target as TileMap);
        return (t.texture.height / t.tile_height - 1) - ty;
    }

    public override void OnInspectorGUI()
    {
		serializedObject.Update ();
		DrawPropertiesExcluding (serializedObject, "map", "tileSizeX", "tileSizeY");

        var t = (target as TileMap);
        Rect texRect = new Rect(0, 0, t.texture.width, t.texture.height);
        if (GUILayout.Button("Generate"))
        {
            (target as TileMap).CreateMap();
        }
        if (GUILayout.Button("Randomize"))
        {
            (target as TileMap).randomizeTile();
        }
        if (GUILayout.Button("Fill"))
        {
            (target as TileMap).fillTile(tx, (t.texture.height/t.tile_height-1)-ty);
        }


		EditorGUILayout.LabelField ("Brush Size:");
		brush_size = EditorGUILayout.IntSlider (brush_size, 1, 150);
		EditorGUILayout.LabelField ("Opacity:");
		brush_opacity = EditorGUILayout.Slider (brush_opacity, 0, 1);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUILayout.Height(200));

        //Events
        if (Event.current.type == EventType.MouseDown && texRect.Contains(Event.current.mousePosition))
        {
            tx = (int)Event.current.mousePosition.x/t.tile_width;
            ty = (int)Event.current.mousePosition.y/t.tile_height;
            this.Repaint();
        }

        GUILayout.Label(t.texture, GUIStyle.none, GUILayout.Width(t.texture.width), GUILayout.Height(t.texture.height));
        EditorGUI.DrawRect(new Rect(tx * t.tile_width, ty * t.tile_height, t.tile_width, t.tile_height)
            , new Color(1, 1, 1, 0.2f));
        
        //EditorGUI.DrawPreviewTexture(new Rect(0, 0, t.texture.width, t.texture.height), t.texture);
        EditorGUILayout.EndScrollView();
		serializedObject.ApplyModifiedProperties ();
    }

    public void OnSceneGUI()
    {
        int ControlID = GUIUtility.GetControlID(FocusType.Passive);
		HandleUtility.AddDefaultControl (ControlID);
		switch (Event.current.type) {
		case EventType.MouseDrag:
		case EventType.MouseDown:
			if (Event.current.button == 0) {
				GUIUtility.hotControl = ControlID;
				draw ();
			}
			break;
		case EventType.MouseUp:
			if(Event.current.button == 0)
				GUIUtility.hotControl = 0;
			break;
		default:
			break;
		}



    }

	void draw(){
		TileMap map = target as TileMap;
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		Plane plane = new Plane(map.getNormal(), map.transform.position);
		float distance;
		plane.Raycast(ray, out distance);
		Vector3 point = ray.GetPoint(distance)-map.transform.position;
		point = Quaternion.Inverse(map.transform.rotation)*point;
		float circle_size = brush_size * 1.31544f;
		for(int i=(int)(-circle_size/2.0f);i<circle_size/2.0f;i++)
			for(int j=(int)(-circle_size/2.0f);j<circle_size/2.0f;j++)
				if(Vector2.Distance(Vector2.zero,new Vector2(i,j))<circle_size/2.0f
				   && Random.Range(0.0f,1.0f)<brush_opacity)
						map.setTile((int)point.x+i, (int)point.y+j, tx, getInvertedY(ty));
	}
}
