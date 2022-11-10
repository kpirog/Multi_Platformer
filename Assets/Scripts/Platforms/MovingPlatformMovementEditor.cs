using UnityEditor;
using UnityEngine;

namespace GDT.Platforms
{
    [CustomEditor(typeof(MovingPlatform))]
    public class MovingPlatformMovementEditor : Editor
    {
        private void OnSceneGUI()
        {
            var platform = target as MovingPlatform;
            if (!platform) return;
            
            EditorGUI.BeginChangeCheck();

            var destination = Handles.PositionHandle(platform.destination, Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(platform, "Change destination");
                platform.destination = destination;
            }
        }
    }
}