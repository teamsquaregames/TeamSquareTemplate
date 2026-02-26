using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstraper : MonoBehaviour
{
    // [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    // private static void Init()
    // {
    //     Scene _currentScene = SceneManager.GetActiveScene();
    //
    //     if (_currentScene.name != "InitScene")
    //     {
    //         foreach (GameObject obj in FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None))
    //             obj.SetActive(false);
    //             
    //         SceneManager.LoadScene("InitScene");
    //
    //         if (GameConfig.Instance.CheatSettings.noMenu)
    //         {
    //             LoadMainSceneAsync();
    //         }
    //         else if (_currentScene.IsValid())
    //         {
    //             SceneManager.LoadSceneAsync(_currentScene.name, LoadSceneMode.Single);
    //         }
    //     }
    //     else
    //     {
    //         SceneManager.LoadSceneAsync("MenuScene", LoadSceneMode.Single);
    //     }
    // }
    //
    // private static async void LoadMainSceneAsync()
    // {
    //     AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");
    //
    //     while (!asyncLoad.isDone)
    //         await Task.Yield();
    //
    //     if (GameData.Instance.mapGenerated)
    //     {
    //         GameHandler.Instance.StartRun();
    //     }
    //     else
    //     {
    //         if (GameData.Instance.firstGameLaunch)
    //             GameHandler.Instance.StartRun();
    //     }
    // }
}