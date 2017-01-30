using UnityEngine;
using System.Collections;
using UnityEditor;

public class PerlinTexture : EditorWindow {
    
    string path = "Assets/Default.ptex";


    [MenuItem("Window/PerlinTexture")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(PerlinTexture));
    }

    void OnGUI()
    {
        // The actual window code goes here
        if(GUILayout.Button("Save as"))
        {
            path = EditorUtility.SaveFilePanel("Save as", "Assets", "", ".ptex");
        }
        if(GUILayout.Button("Load"))
            path = EditorUtility.OpenFilePanel("Load a texture", "Assets", ".ptex");
    }
}
