  
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HMSXRFoundation
{
    public class XvLoadScenesManager : MonoBehaviour
    {

        public static XvLoadScenesManager Instance;
        public List<GameObject> DontDestroyOnLoadGameobjec;

        public string currentSceneName = "MainMenu";



        private void Awake()
        {
            Instance = this;

            LoadScenes(currentSceneName);
        }

        public void LoadScenes(string sceneName)
        {
            if (!string.IsNullOrEmpty(currentSceneName) && currentSceneName != sceneName)
            {
                MyDebugTool.Log("unload " + currentSceneName);
                SceneManager.UnloadSceneAsync(currentSceneName);

                SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

                currentSceneName = sceneName;
#if PLATFORM_ANDROID && !UNITY_EDITOR

            // API.xslam_reset_slam();
#endif

            }


        }

     
     
    }
}
