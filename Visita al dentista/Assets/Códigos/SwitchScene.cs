using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void switchScenes(string sceneName)
     {
        SceneManager.LoadScene(sceneName);
     }

    
}
