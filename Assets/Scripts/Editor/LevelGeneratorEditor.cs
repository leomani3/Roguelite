using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(LevelGenerator))]
public class LevelGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelGenerator levelGenerator = (LevelGenerator)target;
        if (GUILayout.Button("Generate"))
        {
            levelGenerator.GenerateLayout();
            UnityEditor.SceneView.RepaintAll();
        }

        if (GUILayout.Button("Dijkstra"))
        {
            levelGenerator.Dijkstra();
            UnityEditor.SceneView.RepaintAll();
        }

        if (GUILayout.Button("PutRoomOfInterest"))
        {
            levelGenerator.PutRoomOfInterest();
            UnityEditor.SceneView.RepaintAll();
        }

        if (GUILayout.Button("Place Rooms"))
        {
            levelGenerator.PlaceRoomPrefabs();
            UnityEditor.SceneView.RepaintAll();
        }

        if (GUILayout.Button("Clear"))
        {
            levelGenerator.ClearLayout();
            UnityEditor.SceneView.RepaintAll();
        }
    }
}
