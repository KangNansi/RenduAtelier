using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(Road))]
[CanEditMultipleObjects]
public class RoadEditor : Editor {

    List<Vector3> handles=new List<Vector3>();
    Vector3 origin;
    Vector3 destination;

    public void OnSceneGUI()
    {
        int ControlID = GUIUtility.GetControlID(FocusType.Passive);
        HandleUtility.AddDefaultControl(ControlID);

        for (int i = 0; i < handles.Count; i++)
        {
            handles[i] = Handles.PositionHandle(handles[i], Quaternion.identity);
        }
        switch (Event.current.type)
        {
            case EventType.MouseDrag:
                break;
            case EventType.MouseDown:
                if (Event.current.button == 0 && !Event.current.alt)
                {
                    GUIUtility.hotControl = ControlID;
                    Vector3 point = getMousePoint();
                    for (int i = 0; i < handles.Count; i++)
                        if (Vector3.Distance(handles[i], point) < 0.5)
                            point = handles[i];
                        
                    if(point!=Vector3.zero)
                        handles.Add(getMousePoint());
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
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        Vector3 point;
        if (Physics.Raycast(ray, out hit))
            point = hit.point;
        else
            return Vector3.zero;
        return point;
    }
}
