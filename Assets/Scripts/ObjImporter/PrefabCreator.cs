#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace ObjImporter{
    public class PrefabCreator : MonoBehaviour
    {
        public GameObject objectToPrefab;
        public string prefabName = "MyPrefab";

        // Call this method to create the prefab
        public void CreatePrefab()
        {
            if (objectToPrefab == null)
            {
                Debug.LogError("No GameObject assigned to create a prefab from.");
                return;
            }

            // Create the directory if it doesn't exist
            string localPath = "Assets/Prefabs";
            if (!AssetDatabase.IsValidFolder(localPath))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            // Create a unique path for the prefab
            localPath = AssetDatabase.GenerateUniqueAssetPath($"{localPath}/{prefabName}.prefab");

            // Create the prefab
            PrefabUtility.SaveAsPrefabAssetAndConnect(objectToPrefab, localPath, InteractionMode.UserAction);

            Debug.Log($"Prefab created at: {localPath}");
        }
    }
}
#endif