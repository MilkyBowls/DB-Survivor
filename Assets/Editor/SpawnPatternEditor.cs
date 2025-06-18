#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnPatternSO))]
public class SpawnPatternEditor : Editor
{
    void OnSceneGUI()
    {
        var pattern = (SpawnPatternSO)target;
        Handles.color = Color.cyan;

        for (int i = 0; i < pattern.spawnOffsets.Count; i++)
        {
            Vector3 worldPos = SceneView.lastActiveSceneView.camera.transform.position + new Vector3(pattern.spawnOffsets[i].x, pattern.spawnOffsets[i].y, 0);
            Vector3 newPos = Handles.PositionHandle(worldPos, Quaternion.identity);
            pattern.spawnOffsets[i] = new Vector2(newPos.x - SceneView.lastActiveSceneView.camera.transform.position.x, newPos.y - SceneView.lastActiveSceneView.camera.transform.position.y);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(pattern);
        }
    }
}
#endif
