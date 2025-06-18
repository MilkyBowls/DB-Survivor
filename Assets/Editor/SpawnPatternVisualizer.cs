#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class SpawnPatternVisualizer : EditorWindow
{
    public SpawnPatternSO patternToPreview;
    public Vector2 previewCenter = Vector2.zero;

    [MenuItem("Tools/Spawn Pattern Previewer")]
    public static void ShowWindow()
    {
        GetWindow<SpawnPatternVisualizer>("Spawn Pattern Previewer");
    }

    private void OnGUI()
    {
        patternToPreview = (SpawnPatternSO)EditorGUILayout.ObjectField("Spawn Pattern", patternToPreview, typeof(SpawnPatternSO), false);
        previewCenter = EditorGUILayout.Vector2Field("Preview Center", previewCenter);

        if (patternToPreview == null)
        {
            EditorGUILayout.HelpBox("Assign a SpawnPatternSO to preview.", MessageType.Info);
        }
    }

    private void OnFocus()
    {
        SceneView.duringSceneGui += DrawPatternInScene;
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= DrawPatternInScene;
    }

    private void DrawPatternInScene(SceneView sceneView)
    {
        if (patternToPreview == null) return;

        Handles.color = Color.cyan;
        foreach (var offset in patternToPreview.spawnOffsets)
        {
            Vector3 pos = new Vector3(previewCenter.x + offset.x, previewCenter.y + offset.y, 0);
            Handles.DrawSolidDisc(pos, Vector3.back, 0.25f);
            Handles.Label(pos + Vector3.up * 0.5f, $"({offset.x}, {offset.y})");
        }

        SceneView.RepaintAll();
    }
}
#endif
