using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldMap))]
[CanEditMultipleObjects]
public class WorldMapEditor : Editor {
    public Vector2 scrollPosition;
    int tx = 0;
    int ty = 0;

    public int brush_size;
    public float brush_opacity;

    enum Tool
    {
        Brush,
        Heightbrush
    };
    Tool tool = Tool.Brush;

    Material material;

    int getInvertedY(int ty)
    {
        var t = (target as WorldMap);
        return (t.texture.height / t.tileHeight - 1) - ty;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "map", "tileSizeX", "tileSizeY");
        serializedObject.ApplyModifiedProperties();
        var t = (target as WorldMap);
        Rect texRect = new Rect(0, 0, t.texture.width, t.texture.height);
        Material mat = (Material)EditorGUILayout.ObjectField("material", material, typeof(Material));
        if (mat != material)
        {
            t.setMaterial(mat);
            material = mat;
        }

        //Tool Selection
        if (GUILayout.Toggle((tool == Tool.Brush) ? true:false,"Brush", "Button"))
            tool = Tool.Brush;
        if (GUILayout.Toggle((tool == Tool.Heightbrush) ? true : false, "HeightBrush", "Button"))
            tool = Tool.Heightbrush;

        EditorGUILayout.LabelField("Brush Size:");
        brush_size = EditorGUILayout.IntSlider(brush_size, 1, 150);
        EditorGUILayout.LabelField("Opacity:");
        if(tool == Tool.Brush)
            brush_opacity = EditorGUILayout.Slider(brush_opacity, 0, 1);
        if(tool == Tool.Heightbrush)
            brush_opacity = EditorGUILayout.Slider(brush_opacity, -1, 1);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUILayout.Height(200));

        //Events
        if (Event.current.type == EventType.MouseDown && texRect.Contains(Event.current.mousePosition))
        {
            tx = (int)Event.current.mousePosition.x / t.tileWidth;
            ty = (int)Event.current.mousePosition.y / t.tileHeight;
            this.Repaint();
        }

        GUILayout.Label(t.texture, GUIStyle.none, GUILayout.Width(t.texture.width), GUILayout.Height(t.texture.height));
        EditorGUI.DrawRect(new Rect(tx * t.tileWidth, ty * t.tileHeight, t.tileWidth, t.tileHeight)
            , new Color(1, 1, 1, 0.2f));

        //EditorGUI.DrawPreviewTexture(new Rect(0, 0, t.texture.width, t.texture.height), t.texture);
        EditorGUILayout.EndScrollView();
        
    }

    public void OnSceneGUI()
    {
        int ControlID = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(ControlID);

        switch (Event.current.type)
        {
            case EventType.MouseDrag:
            case EventType.MouseDown:
                if (Event.current.button == 0 && !Event.current.alt)
                {
                    GUIUtility.hotControl = ControlID;
                    switch (tool)
                    {
                        case Tool.Brush:
                            draw();
                            break;

                        case Tool.Heightbrush:
                            drawHeight();
                            break;

                        default:
                            draw();
                            break;
                    }
                }
                break;
            case EventType.MouseUp:
                if (Event.current.button == 0 && !Event.current.alt)
                    GUIUtility.hotControl = 0;
                break;
            default:
                break;
        }



    }

    Vector3 getMousePoint()
    {
        WorldMap map = target as WorldMap;
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Plane plane = new Plane(map.getNormal(), map.transform.position);
        float distance;
        RaycastHit hit;
        Vector3 point;
        if (Physics.Raycast(ray, out hit))
         point = hit.point - map.transform.position;
        else
        {
            plane.Raycast(ray, out distance);
            point = ray.GetPoint(distance) - map.transform.position;
        }
        return Quaternion.Inverse(map.transform.rotation) * point;
    }

    void draw()
    {
        WorldMap map = target as WorldMap;
        Vector3 point = getMousePoint();
        float circle_size = brush_size * 1.31544f;
        for (int i = (int)(-circle_size / 2.0f); i < circle_size / 2.0f; i++)
            for (int j = (int)(-circle_size / 2.0f); j < circle_size / 2.0f; j++)
                if (Vector2.Distance(Vector2.zero, new Vector2(i, j)) < circle_size / 2.0f
                   && Random.Range(0.0f, 1.0f) < brush_opacity)
                    map.SetTile((int)point.x + i, (int)point.y + j, tx, getInvertedY(ty));
    }

    void drawHeight()
    {
        WorldMap map = target as WorldMap;
        Vector3 point = getMousePoint();
        float circle_size = brush_size * 1.31544f;
        map.modifyHeight(point, circle_size, brush_opacity);
    }
}
