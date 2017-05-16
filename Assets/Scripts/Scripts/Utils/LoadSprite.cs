using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class LoadSprite
{
    /// <summary>切换图集中的Sprite</summary>
    /// <param name="spriteName">名字</param>
    public static void Change(string spriteName, Image img)
    {
        Texture2D mainTxt = img.mainTexture as Texture2D;
        ResourceManager.Instance.LoadAB(mainTxt.name + ".png", delegate (AssetBundle ab)
        {
            if (img.IsDestroyed()) return;

            Sprite[] sp = ab.LoadAllAssets<Sprite>();
            int count = sp.Length;
            for (int i = 0; i < count; i++)
            {
                if (sp[i].name == spriteName)
                {
                    img.sprite = sp[i];
                    break;
                }
            }

            ab.Unload(false);
        },
         delegate (string errorString)
         {
             Debug.LogError(errorString);
         });
    }
    public static void LoaderBGTexture(Material target, string url, bool autoNativeSize = false)
    {
        ResourceManager.Instance.LoadTexture("bg/" + url + ".png", delegate (AssetBundles.NormalRes res)
        {

            AssetBundles.TextureRes tex = res as AssetBundles.TextureRes;

            target.mainTexture = tex.m_texture;
        },
         delegate (string errorString)
         { 
             Debug.LogError(errorString);
         });
    }

    public static void LoaderImage(UITexture target, string url, bool autoNativeSize = false)
    {
        ResourceManager.Instance.LoadTexture(url + ".png", delegate (AssetBundles.NormalRes res)
          {

              AssetBundles.TextureRes tex = res as AssetBundles.TextureRes;

              target.mainTexture = tex.m_texture;
          },
         delegate (string errorString)
         {
             Debug.LogError(errorString);
         });
    }

    public static void LoaderItem(UITexture target, string url, bool autoNativeSize = false)
    {
        LoaderImage(target,"item/" + url);
    }

    public static void LoaderHead(UITexture target, string url, bool autoNativeSize = false)
    {
        LoaderImage(target, "head/" + url);
    }
    public static void LoaderPlayerSkin(UITexture target, string url, bool autoNativeSize = false)
    {
        LoaderImage(target, "playerskin/" + url);
    }
    public static void LoaderBuild(UITexture target, string url, bool autoNativeSize = false)
    {
        LoaderImage(target, "build/" + url);
    }
    public static void LoaderSkill(UITexture target, string url, bool autoNativeSize = false)
    {
        LoaderImage(target, "skill/" + url);
    }
    public static void LoaderAdviser(UITexture target, string url, bool autoNativeSize = false)
    {
        LoaderImage(target, "adviser/" + url);
    }
}
