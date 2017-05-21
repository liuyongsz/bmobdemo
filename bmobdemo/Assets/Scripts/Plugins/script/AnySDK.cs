using UnityEngine;
using System.Collections;
using System;
namespace anysdk
{
	/**
	 * PluginX 插件
	 * 提供PluginX环境初始化，退出等功能等
	 */
	public class AnySDK : MonoBehaviour{
		
		/*
		 * 填写在在创建游戏时生成的AppKey
		 */
		private static string appKey="F91C0C5B-F4CB-8C05-9DDB-C354D9AC04BB";
		/*
		 * 填写在在创建游戏时生成的AppSecret
		 */
		private static string appSecret="e611c3468640484d2077046a57bd8f27";
		/*
		 * 填写在在创建游戏时生成的PrivateKey
		 */
		private static string privateKey="E8BADD0CA12D3A1D2A450A213CA0D3EB";
		/**
		 * 游戏登录验证服务器验证地址
		 */
		private static string authLoginServer="http://oauth.anysdk.com/api/OauthLoginDemo/Login.php";
		
		void Awake()
		{
			GameObject.DontDestroyOnLoad(gameObject);
			//绑定脚本，默认绑定所有功能脚本，也可以只绑定游戏需要的功能脚本
			this.gameObject.AddComponent<AnySDKUser>(  );
			this.gameObject.AddComponent<AnySDKIAP>(  );
			this.gameObject.AddComponent<AnySDKAds>(  );
			this.gameObject.AddComponent<AnySDKShare>(  );
			this.gameObject.AddComponent<AnySDKSocial>(  );
			this.gameObject.AddComponent<AnySDKAnalytics>(  );
			
			AnySDK.initPluginSystem();
		}
	
		void onDestory() {
			AnySDK.release();
		}
		
		/**
		 * 初始化系统环境，在调用插件任何函数之前必须先调用此函数
		 * 这函数只需要调用一次
		 */
		static void initPluginSystem() {
			if( null == privateKey || privateKey.Trim().Equals( "" ) ) {
				throw new Exception( "AnySDK privateKey not inited..." );
				Application.Quit();
			}
#if UNITY_ANDROID
			AndroidJavaClass jcUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject joActivity = jcUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			AndroidJavaClass jcPluginX = new AndroidJavaClass( "com.anysdk.framework.unity.PluginX" );
			jcPluginX.CallStatic( "initPluginSystem", new object[]{joActivity, appKey, appSecret, privateKey, authLoginServer });
#endif
		}
		
		/**
		 * 系统退出时需调用此函数来释放插件资源
		 */
		static void release() {
#if UNITY_ANDROID
			AndroidJavaClass jcPluginX = new AndroidJavaClass( "com.anysdk.framework.unity.PluginX" );
			jcPluginX.CallStatic( "release", new object[]{});
#endif
		}
		
		public static void log( string msg ) {
#if UNITY_ANDROID
			AndroidJavaClass jcPluginXUser = new AndroidJavaClass( "com.anysdk.framework.LogUtils" );
			jcPluginXUser.CallStatic( "dPrintln", new object[]{msg});
#endif
		}
	}
}
