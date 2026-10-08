using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using DeepSeaPatrol;

namespace DeepSeaPatrol.Editor
{
    public static class DeepSeaPatrolMenu
    {
        [MenuItem("Deep Sea Patrol/Build Full Game Scene", false, 1)]
        public static void BuildSceneFromMenu()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying)
            {
                Debug.LogWarning("[Deep Sea Patrol] Cannot build or save scene while in Play Mode.");
                return;
            }

            // Ensure SampleScene is active
            var activeScene = EditorSceneManager.GetActiveScene();
            Debug.Log($"[Deep Sea Patrol] Building game in scene: {activeScene.name}");

            GameObject builderObj = GameObject.Find("DeepSeaSceneBuilder");
            if (builderObj == null)
            {
                builderObj = new GameObject("DeepSeaSceneBuilder");
            }

            DeepSeaSceneBuilder builder = builderObj.GetComponent<DeepSeaSceneBuilder>();
            if (builder == null)
            {
                builder = builderObj.AddComponent<DeepSeaSceneBuilder>();
            }

            builder.BuildCompleteGameScene();

            if (!EditorApplication.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorSceneManager.MarkSceneDirty(activeScene);
                EditorSceneManager.SaveScene(activeScene);
                Debug.Log("[Deep Sea Patrol] Scene built and saved successfully!");
            }
        }
    }
}
