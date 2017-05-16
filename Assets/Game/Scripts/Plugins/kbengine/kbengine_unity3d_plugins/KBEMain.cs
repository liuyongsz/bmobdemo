using UnityEngine;
using System;
using System.Collections;
using KBEngine;

/*
	可以理解为插件的入口模块
	在这个入口中安装了需要监听的事件(installEvents)，同时初始化KBEngine(initKBEngine)
*/
	
public class KBEMain : MonoBehaviour 
{
	public static KBEngineApp gameapp = null;
	
	// 在unity3d界面中可见选项
	public static DEBUGLEVEL debugLevel = DEBUGLEVEL.DEBUG;
	public static bool isMultiThreads = true;
    public static string ip = string.Empty;
    //外网配置
    public static int port;
	public static KBEngineApp.CLIENT_TYPE clientType = KBEngineApp.CLIENT_TYPE.CLIENT_TYPE_MINI;
	public static string persistentDataPath = "Application.persistentDataPath";
	public static bool syncPlayer = true;
	public static int threadUpdateHZ = 10;
	public static int SEND_BUFFER_MAX = (int)KBEngine.NetworkInterface.TCP_PACKET_MAX;
	public static int RECV_BUFFER_MAX = (int)KBEngine.NetworkInterface.TCP_PACKET_MAX;
	public static bool useAliasEntityID = true;
	public static bool isOnInitCallPropertysSetMethods = true;

	void Awake() 
	 {
		DontDestroyOnLoad(transform.gameObject);

       if(Define.WaiWangDebug)
            port = 17574;
       else
            port = 20013;
}

// Use this for initialization
public static void StartClientap(string serverIp)
    {
        if (Define.WaiWangDebug)
            ip = "1667vz2198.iok.la";
        else
            ip = serverIp;

        Debug.Log("clientapp::start()");
        initKBEngine();
    }

    public static void initKBEngine()
    {
        // 如果此处发生错误，请查看 Assets\Scripts\kbe_scripts\if_Entity_error_use______git_submodule_update_____kbengine_plugins_______open_this_file_and_I_will_tell_you.cs

        Dbg.debugLevel = debugLevel;

        KBEngineArgs args = new KBEngineArgs();

        args.ip = ip;
        args.port = port;
        args.clientType = clientType;

        if (persistentDataPath == "Application.persistentDataPath")
            args.persistentDataPath = Application.persistentDataPath;
        else
            args.persistentDataPath = persistentDataPath;

        args.syncPlayer = syncPlayer;
        args.threadUpdateHZ = threadUpdateHZ;
        args.useAliasEntityID = useAliasEntityID;
        args.isOnInitCallPropertysSetMethods = isOnInitCallPropertysSetMethods;

        args.SEND_BUFFER_MAX = (UInt32)SEND_BUFFER_MAX;
        args.RECV_BUFFER_MAX = (UInt32)RECV_BUFFER_MAX;

        args.isMultiThreads = isMultiThreads;

        if (isMultiThreads)
            gameapp = new KBEngineAppThread(args);
        else
            gameapp = new KBEngineApp(args);
    }
	
	void OnDestroy()
	{
		MonoBehaviour.print("clientapp::OnDestroy(): begin");
        if (KBEngineApp.app != null)
        {
            KBEngineApp.app.destroy();
            KBEngineApp.app = null;
        }
		MonoBehaviour.print("clientapp::OnDestroy(): end");
	}
	
	void FixedUpdate () 
	{
		KBEUpdate();
	}

	public virtual void KBEUpdate()
	{
		// 单线程模式必须自己调用
		if(!isMultiThreads)
			gameapp.process();
		
		KBEngine.Event.processOutEvents();
	}
}
