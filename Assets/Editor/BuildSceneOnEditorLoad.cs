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
                if (scene.isLoaded && (GameObject.Find("QuestionBanner_Lvl2") == null || GameObject.Find("QuestionBanner_Lvl3") == null))
                {
                    Debug.Log("[Deep Sea Patrol] Updating Level 2 and Level 3 scene layout with Question Banners and larger fish...");
                    DeepSeaPatrolMenu.BuildSceneFromMenu();
                }
            };
        }
    }
}
