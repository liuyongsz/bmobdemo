using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

namespace anysdk {
	/**
	 * 统计分析
	 */
	public class AnySDKAnalytics : AnySDKProtocol
	{
		private static AnySDKAnalytics _instance;
#if UNITY_ANDROID		
		private AndroidJavaClass mAndroidJavaClass;
#endif
		
		private static AnySDKAnalytics instance() {
			if( null == _instance ) {
				_instance = new AnySDKAnalytics();
			}
			return _instance;
		}
#if UNITY_ANDROID			
		protected override  AndroidJavaClass getAndroidJavaClass() {
			checkAndCreatePluginXAnalyticsAndroidClass();
			return mAndroidJavaClass;
		}
#endif
		
		/**
		 * start analytics session
		 */
		public static void startSession() {
			instance()._startSession();
		}
		
		/**
		 * stop analytics session
		 */
		public static void stopSession() {
			instance()._stopSession();
		}
		
		/**
		 * set session
		 */
		public static void setSessionContinueMillis( long millis ) {
			instance()._setSessionContinueMillis( millis );
		}
		
		/**
		 * log error
		 */
		public static void logError( string errorId, string message ) {
			instance()._logError( errorId, message );
		}
		
		/**
		 * log event
		 */
		public static void logEvent( string eventId, Dictionary<string,string> maps ) {
			instance()._logEvent( eventId, maps );
		}
		
		/**
		 * log time event begin
		 */
		public static void logTimedEventBegin( string eventId ) {
			instance()._logTimedEventBegin( eventId );
		}
		
		/**
		 * log time event end
		 */
		public static void logTimedEventEnd( string eventId ) {
			instance()._logTimedEventBegin( eventId );
		}
		
		/**
		 * set catch exception switch
		 */
		public static void setCaptureUncaughtException( bool enabled ) {
			instance()._setCaptureUncaughtException( enabled );
		}
		
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
		
		private void checkAndCreatePluginXAnalyticsAndroidClass() {
#if UNITY_ANDROID	
			if( null == mAndroidJavaClass ) {
				mAndroidJavaClass = new AndroidJavaClass( "com.anysdk.framework.unity.PluginXAnalytics" );
			}
#endif
		}
		
		private void _startSession() {
#if UNITY_ANDROID	
			checkAndCreatePluginXAnalyticsAndroidClass();
			mAndroidJavaClass.CallStatic( "startSession", new object[]{}); 
#endif
		}
		
		private void _stopSession() {
#if UNITY_ANDROID	
			checkAndCreatePluginXAnalyticsAndroidClass();
			mAndroidJavaClass.CallStatic( "stopSession", new object[]{}); 
#endif
		}
		
		private void _setSessionContinueMillis( long millis ) {
#if UNITY_ANDROID
			checkAndCreatePluginXAnalyticsAndroidClass();
			mAndroidJavaClass.CallStatic( "setSessionContinueMillis", new object[]{millis}); 
#endif
		}
	
		private void _logError( string errorId, string message ) {
#if UNITY_ANDROID
			checkAndCreatePluginXAnalyticsAndroidClass();
			mAndroidJavaClass.CallStatic( "logError", new object[]{errorId,message}); 
#endif
		}
	
		private void _logEvent( string eventId, Dictionary<string,string> maps ) {
#if UNITY_ANDROID
			checkAndCreatePluginXAnalyticsAndroidClass();
			AndroidJavaObject mapParams = dictionary2JavaMap( maps );
			mAndroidJavaClass.CallStatic( "logEvent", new object[]{eventId,mapParams}); 
#endif
		}
		
		private void _logTimedEventBegin( string eventId ) {
#if UNITY_ANDROID
			checkAndCreatePluginXAnalyticsAndroidClass();
			mAndroidJavaClass.CallStatic( "logTimedEventBegin", new object[]{eventId}); 
#endif
		}
		
		private void _logTimedEventEnd( string eventId ) {
#if UNITY_ANDROID
			checkAndCreatePluginXAnalyticsAndroidClass();
			mAndroidJavaClass.CallStatic( "logTimedEventEnd", new object[]{eventId}); 
#endif
		}
		
		private void _setCaptureUncaughtException( bool enabled ) {
#if UNITY_ANDROID
			checkAndCreatePluginXAnalyticsAndroidClass();
			mAndroidJavaClass.CallStatic( "setCaptureUncaughtException", new object[]{enabled}); 
#endif
		}
	}
}

