using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace anysdk {
	/**
	 * 广告插件
	 */
	public class AnySDKAds : AnySDKProtocol
	{
		private static AnySDKAds _instance;
#if UNITY_ANDROID		
		private AndroidJavaClass mAndroidJavaClass;
#endif
		
		private static AnySDKAds instance() {
			if( null == _instance ) {
				_instance = new AnySDKAds();
			}
			return _instance;
		}
	
		void Awake()
		{
			GameObject.DontDestroyOnLoad(gameObject);
			AnySDKAds.registerPluginXActionCallback( this );
		}
		
		void onDestory() {
			AnySDKAds.unRegisterPluginXActionCallback( this );
		}
		
		/**
		 * 广告接收的回调
		 */
		public void onAdsReceived( string msg ) {
			//Todo
		}
		
		/**
		 * 广告显示后的回调
		 */
		public void onAdsShown( string msg ) {
			//Todo
		}
	
		/**
		 * 广告消失的回调
		 */
		public void onAdsDismissed( string msg ) {
			//Todo
		}
	
		/**
		 * 广告积分消费成功的回调
		 */
		public void onAdsPointsSpendSuccess( string msg ) {
			//Todo
		}
	
		/**
		 * 广告积分消费失败的回调
		 */
		public void onAdsPointsSpendFailed( string msg ) {
			//Todo
		}
		
		/**
		 * 网络错误的回调
		 */
		public void onAdsNetworkError( string msg ) {
			//Todo
		}
		
		/**
		 * 未知错误的回调
		 */
		public void onAdsUnkownError( string msg ) {
			//Todo
		}
		
		/**
		 * 积分墙改变的回调
		 */
		public void onAdsOfferWallOnPointsChanged( string msg ) {
			//Todo
		}
		
		/**
		 * 未知请求类型的回调
		 */
		public void onAdsActionResult( string msg ) {
			//Todo
		}
#if UNITY_ANDROID	
		protected override  AndroidJavaClass getAndroidJavaClass() {
			checkAndCreatePluginXAdsAndroidClass();
			return mAndroidJavaClass;
		}
#endif	
		/**
		 * 获取插件名称
		 */
		public static string getPluginName() {
			return instance()._getPluginName();
		}
		
		/**
		 * 获取插件版本号
		 */
		public static string getPluginVersion() {
			return instance()._getPluginVersion();
		}
		
		/**
		 * 获取Sdk 版本号
		 */
		public static string getSDKVersion() {
			return instance()._getSDKVersion();
		}
		
		/**
		 * 显示广告
		 * @param type 广告类型
		 */
		public static void showAds( AnySDKAdsTypeEnum type ) {
			instance()._showAds( type );
		}
		
		/**
		 * 隐藏广告
		 * @param type 广告类型
		 */
		public static void hideAds( AnySDKAdsTypeEnum type ) {
			instance()._hideAds( type );
		}
		
		/**
		 * 查询积分
		 */
		public static float queryPoints() {
			return instance()._queryPoints();	
		}
		
		/**
		 * 消费积分
		 */
		public static void spendPoints( int points ) {
			instance()._spendPoints( points );
		}
				
		/**
		 * 确定是否支持某功能
		 * @param functionName
		 * @return true 支持  false 不支持
		 */
		public static bool isSupportFunction( string functionName ) {
			return instance()._isSupportFunction( functionName );
		}
		
		/**
		 * 调用sdk中的函数
		 * @param functionName 函数名称
		 */
		public static void callFunction( string functionName ) {
			instance()._callFunction( functionName );
		}
		
		/**
		 * 调用SDK中的函数
		 * @param functionName 函数名称
		 * @param paramList	参数值列表
		 */
		public static void callFunction( string functionName, ArrayList paramList ) {
			instance()._callFunction( functionName, paramList );
		}
		
		/**
		 * 注册Action回调接口
		 * @param callback 
		 */
		static void registerPluginXActionCallback( MonoBehaviour callback ) {
			instance()._registerPluginXActionCallback( callback );
		}
		
		/**
		 * 去掉注册的action回调接口
		 */
		static void unRegisterPluginXActionCallback( MonoBehaviour callback ) {
			instance()._unRegisterPluginXActionCallback( callback );
		}
		
		private void _showAds( AnySDKAdsTypeEnum type ) {
#if UNITY_ANDROID	
			checkAndCreatePluginXAdsAndroidClass();

			mAndroidJavaClass.CallStatic( "showAds", new object[]{(int)type}); 
#endif
		}
		
		private void _hideAds( AnySDKAdsTypeEnum type ) {
#if UNITY_ANDROID	
			checkAndCreatePluginXAdsAndroidClass();
			mAndroidJavaClass.CallStatic( "hideAds", new object[]{(int)type}); 
#endif
		}
		
		private float _queryPoints() {
#if UNITY_ANDROID
			checkAndCreatePluginXAdsAndroidClass();	
			return mAndroidJavaClass.CallStatic<float>( "queryPoints", new object[]{}); 
#else
			return 0f;
#endif
		}
		
		private void _spendPoints( int points ) {
#if UNITY_ANDROID
			checkAndCreatePluginXAdsAndroidClass();	
			mAndroidJavaClass.CallStatic( "spendPoints", new object[]{points}); 
#endif
		}
		
		private void checkAndCreatePluginXAdsAndroidClass() {
#if UNITY_ANDROID	
			if( null == mAndroidJavaClass ) {
				mAndroidJavaClass = new AndroidJavaClass( "com.anysdk.framework.unity.PluginXAds" );
			}
#endif
		}
	}
}

