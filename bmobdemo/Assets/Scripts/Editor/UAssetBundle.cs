using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using AssetBundles;

public class UAssetBundle
{
   public enum SetNameType
    {
        Null,
        Normal,
        Force,
    }


    [MenuItem("UGame/FindCamera")]
    public static void FindCamera()
    {
        AudioListener[] cameras = GameObject.FindObjectsOfType<AudioListener>();
        
        int cnt = cameras.Length;
        for (int i = 0; i < cnt;i++)
        {
            Debug.LogError(cameras[i].name);
        }
    }

    [MenuItem("UGame/Load UI Text")]
    public static void Loaded_Text()
    {
        TextManager.Init(null);
    }

    [MenuItem("UGame/AssetBundles_Build 打包")]
    public static void AssetBundles_Build()
    {
        UBuildScript.BuildAssetBundles();

        Packager.BuildFileIndex();
        Debug.Log("Alert:Build assetbundles over.");
    }

    [MenuItem("UGame/AssetBundle_SetNameForce 强制设置所有")]
    public static void AssetBundle_SetNameForce()
    {
        AssetBundle_SetName(SetNameType.Force);
        //AssetBundle_SetName_XML();
    }

    private static string versionDir = "UGame";
	// Use this for initialization
	[MenuItem("UGame/AssetBundle_SetName 设置没有命名的")]
	public static void AssetBundle_SetName(SetNameType type,string rootDir = "Res")
	{
		//string fullPath = Application.dataPath + "/" + versionDir + "/";
        string fullPath = Application.dataPath + "/" + rootDir + "/";
        int relativeLen = versionDir.Length + 6; // Assets 长度
        if (Directory.Exists(fullPath))
        {
			DirectoryInfo dir = new DirectoryInfo(fullPath);
			FileInfo[] files = dir.GetFiles("*", SearchOption.AllDirectories);

            int lastLine = 0;
            for (int i = 0; i < files.Length; ++i)
            {
                FileInfo fileInfo = files[i];
               // if (fileInfo.Name.EndsWith(".prefab") || fileInfo.Name.EndsWith(".unity"))
                {
                    string basePath = fileInfo.FullName.Substring(fullPath.Length - relativeLen).Replace('\\', '/');
                    basePath = basePath.Replace("nt/", "");
                    AssetImporter importer = AssetImporter.GetAtPath(basePath);

                   // importer.assetBundleName = "None";
                    if (importer != null && importer.assetBundleName != versionDir)
                    {
                       //basePath = basePath.Replace("Assets/", "");
                        //basePath = basePath.Replace(".prefab", "");
                        //basePath = basePath.Replace(".unity", "");
                        //basePath = basePath.Replace("/", "-");
                        basePath = basePath.ToLower();

                        lastLine = basePath.LastIndexOf("/");
                        basePath = basePath.Substring(lastLine);
                        basePath = basePath.Replace("/", "");
                        basePath = basePath.Replace(".prefab", "");
                        basePath = basePath.Replace(".unity", "");

                        if (type == SetNameType.Force)
                        {
                            EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 1f * i / files.Length);
                            importer.assetBundleName = basePath;
                        }
                        else if(type == SetNameType.Normal)
                        {
                            if (importer.assetBundleName.IndexOf(basePath) < 0)
                            {
                                EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 1f * i / files.Length);
                                importer.assetBundleName = basePath;
                            }
                        }
                        else if(type == SetNameType.Null)
                        {
                            EditorUtility.DisplayProgressBar("设置AssetName名称", "正在设置AssetName名称中...", 1f * i / files.Length);
                            importer.assetBundleName = null;
                        }
                    }
                }
            }

            EditorUtility.ClearProgressBar();

            Debug.Log("Alert:Set assetbundles all name over.");
        }
    }

    // 资源目录
    private static string RES_SRC_PATH = "Assets/Res/Config/";
    // AssetBundle打包后缀
    private static string ASSET_BUNDLE_SUFFIX = ".bundle";
    private static void AssetBundle_SetName_XML()
    {
        List<string> resList = GetAllResDirs(RES_SRC_PATH);
        foreach (string dir in resList)
        {
            setXmlAssetBundleName(dir);
        }
    }
    /// <summary>
    /// 设置AssetBundleName
    /// </summary>
    /// <param name="fullpath">Fullpath.</param>
    public static void setXmlAssetBundleName(string fullPath)
    {
        string[] files = System.IO.Directory.GetFiles(fullPath);
        if (files == null || files.Length == 0)
        {
            return;
        }

        Debug.Log("Set AssetBundleName Start......");
        string dirBundleName = fullPath.Substring(RES_SRC_PATH.Length);
        dirBundleName = dirBundleName.Replace("/", "@") + ASSET_BUNDLE_SUFFIX;

        foreach (string file in files)
        {
            if (file.EndsWith(".meta"))
            {
                continue;
            }
            AssetImporter importer = AssetImporter.GetAtPath(file);
            if (importer != null)
            {
                string ext = System.IO.Path.GetExtension(file);
                string bundleName = dirBundleName;

                if (null!=ext&&(ext.Equals(".xml")))
                {
                    //// prefab单个文件打包
                    //bundleName = file.Substring(RES_SRC_PATH.Length);
                    //bundleName = bundleName.Replace("/", "@");
                    //if (null != ext)
                    //{
                    //    bundleName = bundleName.Replace(ext, ASSET_BUNDLE_SUFFIX);
                    //}
                    bundleName = bundleName.ToLower();
                    Debug.LogFormat("Set AssetName Succ, File:{0}, AssetName:{1}", file, bundleName);
                    importer.assetBundleName = bundleName;
                    EditorUtility.UnloadUnusedAssetsImmediate();
                }

            }
            else
            {
                Debug.LogFormat("Set AssetName Fail, File:{0}, Msg:Importer is null", file);
            }
        }
        Debug.Log("Set AssetBundleName End......");


    }
    /// <summary>
	/// 获取所有资源目录
	/// </summary>
	/// <returns>The res all dir path.</returns>
	public static List<string> GetAllResDirs(string fullPath)
    {
        List<string> dirList = new List<string>();

        // 获取所有子文件
        GetAllSubResDirs(fullPath, dirList);

        return dirList;
    }

    /// <summary>
	/// 递归获取所有子目录文件夹
	/// </summary>
	/// <param name="fullPath">当前路径</param>
	/// <param name="dirList">文件夹列表</param>
	public static void GetAllSubResDirs(string fullPath, List<string> dirList)
    {
        if ((dirList == null) || (string.IsNullOrEmpty(fullPath)))
            return;

        string[] dirs = System.IO.Directory.GetDirectories(fullPath);
        if (dirs != null && dirs.Length > 0)
        {
            for (int i = 0; i < dirs.Length; ++i)
            {
                GetAllSubResDirs(dirs[i], dirList);
            }
        }
        else
        {
            dirList.Add(fullPath);
        }
    }
}
