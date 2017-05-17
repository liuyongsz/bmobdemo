using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using AssetBundles;
using UnityEngine.EventSystems;
using PureMVC.Patterns;
using UnityEngine.Events;

public enum EUIDepth
{
    FIRST,
    SECOND,
}

class Main : MonoBehaviour 
{
    public bool GameDebug = false;
    /// <summary>是否使用滚地球 test</summary>
    public bool UsePassGroundBall;
    public static Main Ins;

    public static Sprite m_defaultsprite;
    public static Texture2D m_defaultTexture;
    private GameObject loading;
    public static UnityEngine.Events.UnityAction SoketCallBack = null;

    public Camera Camera3D;
    public static Camera CameraUI;
    public static Transform GetUIRoot()
    {
        if (m_UIRoot == null)
        {
            m_UIRoot = GameObject.Find("UIRoot").transform;
            if (m_UIRoot != null)
            {
                int ManualWidth = 1334;
                int ManualHeight = 750;
                UIRoot uiRoot = m_UIRoot.GetComponent<UIRoot>();
                if (uiRoot != null)
                {
                    if (System.Convert.ToSingle(Screen.height) / Screen.width > System.Convert.ToSingle(ManualHeight) / ManualWidth)
                        uiRoot.manualHeight = Mathf.RoundToInt(System.Convert.ToSingle(ManualWidth) / Screen.width * Screen.height);
                    else
                        uiRoot.manualHeight = ManualHeight;
                }
            }

            CameraUI = m_UIRoot.GetComponent<Camera>();

            DontDestroyOnLoad(m_UIRoot);
        }
        var eventSystem = GameObject.Find("EventSystem");
        if (eventSystem != null)
            DontDestroyOnLoad(eventSystem);

        return m_UIRoot;
    }

    public static Transform UIRoot3D
    {
        get
        {
            if(null == m_UIRoot3D)
            {
                m_UIRoot3D = GameObject.Find("UIRoot3D").transform;

                DontDestroyOnLoad(m_UIRoot3D.gameObject);
            }

            return m_UIRoot3D;
        }
    }

    public static Transform GetBattleNode()
    {
         if(null==m_DestoryNode)
        {
            m_DestoryNode = GameObject.Find("BattleNode").transform;
            DontDestroyOnLoad(m_DestoryNode.gameObject);
        }
        return m_DestoryNode;
    }

    private static Transform m_DestoryNode;
    private static Transform m_UIRoot;
    private static Transform m_UIRoot3D;

    void Awake()
    {
        //时间控制管理器
        AddComponent("TimerManager");
       

    }
    // Use this for initialization
    void Start () {

        Screen.orientation = ScreenOrientation.AutoRotation;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Time.timeScale = 1f;

        DontDestroyOnLoad(gameObject);

        Ins = this;
        //启动MVC
        GameFacade.Instance().Startup();

        GetUIRoot();
        Transform root3 = UIRoot3D;
        Camera3D = root3.GetComponent<Camera>();

        //初始化资源管理器
        ResourceManager.Instance.Init(ResourceManagerInitComplete);
        //初始化各种SDK

        //设置全局对象池
        PoolManager.Init();
        m_defaultsprite = Resources.Load<Sprite>("notfount");

        Application.targetFrameRate = Define.GameFrameRate;
    }
    ///<summary>
    /// 添加管理器
    /// </summary>
    void AddComponent(string component)
    {
        if (string.IsNullOrEmpty(component))
        {
            return;
        }

        GameObject go = GameObject.Find(component);
        if (go == null)
        {
            go = new GameObject(component);
        }

        go.AddComponent(System.Type.GetType(component));
        go.transform.parent = transform;
    }
    public static long heartTime = 0;
    public static bool isHeart = false; // 先设置false 如果请求到玩家数据进行开启

    private static NetHelper m_netHelp;
    void Update()
    {
        if (null == m_netHelp)
            m_netHelp = Instance.Get<NetHelper>();
        m_netHelp.UpdateNet();

        if (Input.GetKey(KeyCode.F1))
        {
            PureMVC.Patterns.Facade.Instance.SendNotification(NotificationID.Hide_Main);
            PureMVC.Patterns.Facade.Instance.SendNotification(NotificationID.CHANGE_SCENE, new SceneVO("s_match1", "s_match1", false, EScene.PVE));
        }
        else if(Input.GetKey(KeyCode.F2))
        {
            GUIManager.CloseAllUI();
            UnityEngine.SceneManagement.SceneManager.LoadScene("S_ShootTest");
        }

        //当网络不可用时
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            //Do sth.
        }

        //当用户使用WiFi时
        else if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
        {
            //Do sth.
          
        }

        //当用户使用移动网络时
        else if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork)
        {
            //Do sth.               
        }
    }


    void ResourceManagerInitComplete()
    {
        GUIManager.HideLoadingUI();
        PureMVC.Patterns.Facade.Instance.SendNotification(NotificationID.START_GAME);
    }

    void FixedUpdate()
    {
        
    }

    void OnApplicationQuit()
    {
        Destroy();
    }
    void Destroy()
    {
       
    }

    public void OnGUI()
    {
        
    }

    public void OnDrawGizmos()
    {

    }
}
