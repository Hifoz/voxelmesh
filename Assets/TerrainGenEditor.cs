using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(TerrainGen))]
public class TerrainGenEditor : Editor
{
    TerrainGen tg;


    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        tg = (TerrainGen)target;

        if (GUILayout.Button("Generate Cube")) {
            tg.testGenerator();
        }


    }

}

