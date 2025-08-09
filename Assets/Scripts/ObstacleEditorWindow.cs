using UnityEditor;
using UnityEngine;

public class ObstacleEditorWindow : EditorWindow
{
    private ObstacleData obstacleData;

    [MenuItem("Tools/Obstacle Editor")]
    public static void OpenWindow()
    {
        GetWindow<ObstacleEditorWindow>("Obstacle Editor");
    }

    private void OnGUI()
    {
        obstacleData = (ObstacleData)EditorGUILayout.ObjectField("Obstacle Data", obstacleData, typeof(ObstacleData), false); // false means no scene objects are allowed

        if (obstacleData == null)
        {
            EditorGUILayout.HelpBox(" Assign an ObstacleData asset to edit.", MessageType.Info); // help box
            return;
        }

        EditorGUILayout.Space();

        for (int y = obstacleData.height - 1; y >= 0; y--) // draws the grid
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < obstacleData.width; x++)
            {
                bool current = obstacleData.GetBlocked(x, y); // gets the current blocked state
                bool newVal = GUILayout.Toggle(current, "", GUILayout.Width(20), GUILayout.Height(20)); // creates a toggle button

                if (newVal != current)
                {
                    Undo.RecordObject(obstacleData, "Toggle Obstacle"); // records the change
                    obstacleData.SetBlocked(x, y, newVal);
                    EditorUtility.SetDirty(obstacleData);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Clear All"))
        {
            Undo.RecordObject(obstacleData, "Clear Obstacles");
            for (int i = 0; i < obstacleData.blocked.Length; i++)
                obstacleData.blocked[i] = false;

            EditorUtility.SetDirty(obstacleData);
        }
    }
}
