using ObjImporter;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    
    void Start()
    {
        // Mark the object as not destroyable on load
        DontDestroyOnLoad(DataHolder.objectToPass);
        //SceneManager.LoadScene("RaceScene", LoadSceneMode.Additive);
    }

    // You can also call this method from other scripts or UI buttons
    public void LoadSceneAdditively(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
    }
}
