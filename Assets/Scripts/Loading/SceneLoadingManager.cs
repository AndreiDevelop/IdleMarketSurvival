using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace RentTycoon
{
    public class SceneLoadingManager : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        public static SceneLoadingManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //LoadProjectContext();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void LoadProjectContext()
        {
            // Ensure that ProjectContext is loaded from resources
            if (!ProjectContext.HasInstance)
            {
                var prefab = Resources.Load<GameObject>(ProjectContext.ProjectContextResourcePath);
                if (prefab != null)
                {
                    Instantiate(prefab);
                }
                else
                {
                    Debug.LogError("ProjectContext prefab not found in Resources.");
                }
            }
        }
        
        private void Start()
        {
            Debug.LogError(Application.persistentDataPath);
            
            StartLoading();
        }

        private async UniTask StartLoading()
        {
            await UniTask.WaitForSeconds(1);
            
            //TODO refactoring scene names
            LoadSceneAdditive(Constants.SceneMain);
            LoadSceneAdditive(Constants.SceneLevel_1);
            
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _camera.gameObject.SetActive(false);
            SceneManager.sceneLoaded -= OnSceneLoaded;
            
            //SceneManager.UnloadSceneAsync(Constants.SceneLoading);
        }

        public void LoadSceneAdditive(string sceneName)
        {
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
        
        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}