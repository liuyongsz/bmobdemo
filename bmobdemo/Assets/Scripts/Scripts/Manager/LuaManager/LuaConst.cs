using UnityEngine;
using System.IO;

public static class LuaConst
{
    public static string luaDir = Application.dataPath + "/Lua";                //lua逻辑代码目录
    public static string luaBytesDir = Application.dataPath + "/LuaBytes";  //luabytes 文件目录 用来打包的

#if UNITY_STANDALONE
    public static string osDir = "Windows";
#elif UNITY_ANDROID
    public static string osDir = "Android";
#elif UNITY_IPHONE
    public static string osDir = "iOS";        
#else
    public static string osDir = "";        
#endif

    public static string luaResDir = string.Format("{0}/{1}/Lua", Application.persistentDataPath, osDir);

    public static string zbsExe = @"D:\soft\ZeroBraneStudio-1.40\ZeroBraneStudio-1.40\zbstudio.exe";           //ZeroBraneStudio  exe目录

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
    public static string zbsDir = "D:/soft/ZeroBraneStudio-1.40/ZeroBraneStudio-1.40/lualibs/mobdebug";        //ZeroBraneStudio目录       
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
	public static string zbsDir = "/Applications/ZeroBraneStudio.app/Contents/ZeroBraneStudio/lualibs/mobdebug";
#else
    public static string zbsDir = luaResDir + "/mobdebug/";
#endif

    public static bool beBundle = true;                                                    //是否从assetbundle加载lua

    //-----------------------------------------------------------------------------------------

    public static string GetLuaAssetBundlePath(string bundleName)
    {
        string path = string.Format("{0}/{1}/{2}", Application.persistentDataPath, osDir, bundleName.ToLower());
        if (!File.Exists(path))
            path = string.Format("{0}/{1}/{2}", Application.streamingAssetsPath, osDir, bundleName.ToLower());
        return path;
    }
}