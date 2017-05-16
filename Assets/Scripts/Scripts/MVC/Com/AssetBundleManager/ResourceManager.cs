using UnityEngine;
using System.Collections;
using AssetBundles;
using System.IO;
using TinyBinaryXml;
public class ResourceManager:MonoBehaviour
{

    public delegate void AssetCallBack(GameObject go);
    private static ResourceManager m_instance = null;
    private static GameObject m_object;

    public void Init(System.Action Complete)
    {
       
        DontDestroyOnLoad(m_object);
        //初始化
        StartCoroutine(Initialize(Complete));
    }

    public void Update()
    {
        AsyncOperation sceneLoadOper = Scene_AsyncOperation;

        if(null != sceneLoadOper)
        {
            float progress = sceneLoadOper.progress;
            if (!sceneLoadOper.isDone && sceneLoadOper.progress >= 0.8f)
            {
                sceneLoadOper.allowSceneActivation = true;
                Scene_AsyncOperation = null;

                StartCoroutine(OnLoadComplete());

                progress = 1f;
            }

            GUIManager.SetLoadingProgress(progress);
        }
    }

    private IEnumerator OnLoadComplete()
    {
        yield return new WaitForSeconds(2f);

        if (MatchManager.Instace)
        {
            //MatchManager.LoadedInit();

            GUIManager.HideLoadingUI();
        }
    }

    private IEnumerator Initialize(System.Action Complete)
    {
        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project. 
        // 	Another approach would be to make this configurable in the standalone player.)
//#if DEVELOPMENT_BUILD || UNITY_EDITOR
//        AssetManager.SetDevelopmentAssetBundleServer();
//#else
//       AssetManager.SetSourceAssetBundleURL(ResourcePath.GetBaseURL());
//        // Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
//        AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
//        // Or customize the URL based on your deployment or configuration
//        //AssetBundleManager.SetSourceAssetBundleURL("http://www.MyWebsite/MyAssetBundles");
//#endif
    if(!Define.UseLoacalRes)
            AssetManager.SetDevelopmentAssetBundleServer();
    else
            AssetManager.SetSourceAssetBundleURL(ResourcePath.GetBaseURL());

        if (!Define.UpdateMode)
        {
            LuaManager.CreateInstance();
        }
        
        //Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetManager.Initialize(gameObject);
        if (request != null)
        {
            yield return StartCoroutine(request);
        }
        if(null!=Complete)
        {
            Complete();
        }

    }

    public void LoadPrefab(string prefabName, System.Action<string,GameObject> OnPackCompleted, System.Action<string> OnPackError=null)
    {
       StartCoroutine(InstantiateGameObjectAsync(prefabName, prefabName, OnPackCompleted, OnPackError));
    }
   
    public  void LoadScene(string sceneName, bool IsAdditive, System.Action<object> OnSceneLoadCompleted, object obj, System.Action<string> OnSceneLoadError = null)
    {
        StartCoroutine(InitializeLevelAsync(sceneName, sceneName, IsAdditive,OnSceneLoadCompleted,obj,OnSceneLoadError));
    }
     
    /// <summary>
    /// 读取XML二进制文件
    /// </summary>
    public void LoadBytes(string filename, EResType t, System.Action<NormalRes> onloaded = null, System.Action<string> errorload = null)
    {
        filename += ".bytes";
        StartCoroutine(LoadNormalResAsync(filename, t, onloaded, errorload));
    } /// <summary>
      /// 加载资源
      /// </summary>
      /// <param name="strFileName">资源全路径文件名</param>
      /// <param name="callback">完成后回调</param>
   
    public void LoadAsset(string filename, EResType t, System.Action<NormalRes> completeCallback)
    {
        filename += ".txt";
        StartCoroutine(LoadAssetResAsync(filename, t, completeCallback));
    }
    public void LoadTexture(string filename, System.Action<NormalRes> onloaded, System.Action<string> errorload)
    {
        StartCoroutine(LoadNormalResAsync(filename, EResType.E_TEXTURE, onloaded, errorload));
    }

    public void LoadAB(string filename, System.Action<AssetBundle> onloaded, System.Action<string> errorload)
    {
        StartCoroutine(LoadNormalResAsync(filename, EResType.E_AssetBundle, onloaded, errorload));
    }

    public static ResourceManager Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_object = new GameObject("ResourceManager");
                m_instance = (ResourceManager)m_object.AddComponent(typeof(ResourceManager));
            }
            return m_instance;
        }
    }

    protected IEnumerator LoadAssetResAsync(string name, EResType t, System.Action<NormalRes> completeCallback)
    {

        NoramlResOperation request = AssetManager.LoadNormalAssetAsync(name, t);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        if (null != request.m_loaded)
        {
            completeCallback(request.m_loaded.m_normalres);
        }
    }
    protected IEnumerator LoadNormalResAsync(string name, EResType t, System.Action<AssetBundle> onloaded, System.Action<string> errload)
    {

        NoramlResOperation request = AssetManager.LoadNormalAssetAsync(name, t);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        if (null != request.m_loaded)
        {
            onloaded(request.m_loaded.m_AssetBundle);
        }
        else
        {
            if (null != errload)
            {
                errload(name);
            }
        }
    }

    protected IEnumerator LoadNormalResAsync(string name, EResType t, System.Action<NormalRes> onloaded, System.Action<string> errload)
    {

        NoramlResOperation request = AssetManager.LoadNormalAssetAsync(name, t);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        if (null != request.m_loaded)
        {
            onloaded(request.m_loaded.m_normalres);
        }
        else
        {
            if(null!=errload)
            {
                errload(name);
            }
        }
    }

    protected IEnumerator LoadSpriteAsync(string assetBundleName,string spritename,System.Action<Sprite> OnLoad,System.Action OnPackError)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperationFull request = AssetManager.LoadAssetAsync(assetBundleName, spritename, typeof(Sprite),true);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        Sprite  s = request.GetAsset<Sprite>(spritename);

        if (s != null)
        {
            if (null != OnLoad)
            {
                OnLoad(s);
            }
        }
        else
        {
            if (null != OnPackError)
            {
                OnPackError();
            }
        }

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(spritename + (s == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
    }
   
    protected  IEnumerator InstantiateGameObjectAsync(string assetBundleName, string assetName, System.Action<string,GameObject> OnPackCompleted, System.Action<string> OnPackError)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.
        AssetBundleLoadAssetOperation request = AssetManager.LoadAssetAsync(assetBundleName, assetName, typeof(GameObject),false);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        GameObject prefab = request.GetAsset<GameObject>();

        if (prefab != null)
        {
            GameObject go= GameObject.Instantiate(prefab);
           // AssetManager.UnloadAssetBundle(assetBundleName);
            if(null!=OnPackCompleted)
            {
                OnPackCompleted(assetName, go);
            }
        }
        else
        {
            if(null!=OnPackError)
            {
                OnPackError(assetName);
            }
        }

        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");
    }


    protected IEnumerator InitializeLevelAsync(string sceneAssetBundle, string levelName, bool isAdditive, System.Action<object> OnSceneLoadCompleted, object obj, System.Action<string> OnSceneLoadError)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load level from assetBundle.
        AssetLoadOperation request = AssetManager.LoadLevelAsync(sceneAssetBundle, levelName, isAdditive);
        if (request == null)
        {
            if(null!= OnSceneLoadError)
            {
                OnSceneLoadError(levelName); 
            }
            yield break;
        }
        yield return StartCoroutine(request);

        Scene_AsyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(levelName, UnityEngine.SceneManagement.LoadSceneMode.Single);
        Scene_AsyncOperation.allowSceneActivation = false;
        yield return Scene_AsyncOperation;


        if (null!=OnSceneLoadCompleted)
        {
            OnSceneLoadCompleted(obj);
        }
        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log("Finished loading scene " + levelName + " in " + elapsedTime + " seconds");
    }

    public static AsyncOperation Scene_AsyncOperation;
}
