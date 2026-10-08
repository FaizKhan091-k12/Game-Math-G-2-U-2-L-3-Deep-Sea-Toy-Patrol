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
            EditorApplication.delayCall += () =>
            {
                if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
                {
                    return;
                }

                var scene = EditorSceneManager.GetActiveScene();
                if (scene.isLoaded && GameObject.Find("QuestionBanner_Lvl1") == null)
                {
                    Debug.Log("[Deep Sea Patrol] Updating Level 1 scene layout with Question Banner and Drop Zone...");
                    DeepSeaPatrolMenu.BuildSceneFromMenu();
                }
            };
        }
    }
}
