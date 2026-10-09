using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using DeepSeaPatrol;

namespace DeepSeaPatrol.Editor
{
    [InitializeOnLoad]
    public static class BuildSceneOnEditorLoad
    {
        static BuildSceneOnEditorLoad()
        {
            // Automatic rebuild disabled to preserve user's manual scene changes
        }
    }
}
