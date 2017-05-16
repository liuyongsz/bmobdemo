using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Xml;
using UnityEditor.SceneManagement;

public class MatchPositionEdit : Editor {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private static void BallPosition()
    {
       
    }

    public static void ExportXML(string savePath)
    {
        // 所有的动态加载的物体都挂在ActiveObjectRoot下面
        GameObject[] objs = GameObject.FindGameObjectsWithTag("PlayerPosition");

        if (objs == null || objs.Length == 0)
        {
            Debug.LogError("No PlayerPosition!");
            return;
        }

        XmlDocument XmlDoc = new XmlDocument();
        XmlElement XmlRoot = XmlDoc.CreateElement("Root");
        XmlRoot.SetAttribute("SceneName", EditorSceneManager.GetActiveScene().name);
        XmlDoc.AppendChild(XmlRoot);

        int count = objs.Length;
        for (int i = 0;i < count;i++)
        {
            Transform tranGroup = objs[i].transform;
            XmlElement xmlGroupNode = XmlDoc.CreateElement("Group");

            xmlGroupNode.SetAttribute("name", tranGroup.name);
            xmlGroupNode.SetAttribute("posX", tranGroup.position.x.ToString());
            xmlGroupNode.SetAttribute("posY", tranGroup.position.y.ToString());
            xmlGroupNode.SetAttribute("posZ", tranGroup.position.z.ToString());

            XmlRoot.AppendChild(xmlGroupNode);

            //foreach (Transform tranNode in tranGroup.transform)
            //{
            //    XmlElement xmlNode = XmlDoc.CreateElement("Node");
            //    xmlGroupNode.AppendChild(xmlNode);

            //    CreateTransformNode(XmlDoc, xmlNode, tranNode);
            //    CreateMeshNode(XmlDoc, xmlNode, tranNode);
            //}
        }

        string scenenName = EditorSceneManager.GetActiveScene().name.ToLower();
        string path = savePath + "/";
        path += scenenName + ".xml";
        XmlDoc.Save(path);
        XmlDoc = null;

        Debug.Log("save success:" + path);
    }

    private static void CreateTransformNode(XmlDocument XmlDoc, XmlElement xmlNode, Transform tran)
    {
        if (XmlDoc == null || xmlNode == null || tran == null)
            return;

        XmlElement xmlProp = XmlDoc.CreateElement("Transform");
        xmlNode.AppendChild(xmlProp);

        xmlNode.SetAttribute("name", tran.name);
        xmlProp.SetAttribute("posX", tran.position.x.ToString());
        xmlProp.SetAttribute("posY", tran.position.y.ToString());
        xmlProp.SetAttribute("posZ", tran.position.z.ToString());
        //xmlProp.SetAttribute("rotX", tran.eulerAngles.x.ToString());
        //xmlProp.SetAttribute("rotY", tran.eulerAngles.y.ToString());
        //xmlProp.SetAttribute("rotZ", tran.eulerAngles.z.ToString());
        //xmlProp.SetAttribute("scaleX", tran.localScale.x.ToString());
        //xmlProp.SetAttribute("scaleY", tran.localScale.y.ToString());
        //xmlProp.SetAttribute("scaleZ", tran.localScale.z.ToString());
    }

    private static void CreateMeshNode(XmlDocument XmlDoc, XmlElement xmlNode, Transform tran)
    {
        if (XmlDoc == null || xmlNode == null || tran == null)
            return;

        XmlElement xmlProp = XmlDoc.CreateElement("MeshRenderer");
        xmlNode.AppendChild(xmlProp);

        foreach (MeshRenderer mr in tran.gameObject.GetComponentsInChildren<MeshRenderer>(true))
        {
            if (mr.material != null)
            {
                XmlElement xmlMesh = XmlDoc.CreateElement("Mesh");
                xmlProp.AppendChild(xmlMesh);

                // 记录Mesh名字和Shader
                xmlMesh.SetAttribute("Mesh", mr.name);
                xmlMesh.SetAttribute("Shader", mr.material.shader.name);

                // 记录主颜色
                XmlElement xmlColor = XmlDoc.CreateElement("Color");
                xmlMesh.AppendChild(xmlColor);
                bool hasColor = mr.material.HasProperty("_Color");
                xmlColor.SetAttribute("hasColor", hasColor.ToString());
                if (hasColor)
                {
                    xmlColor.SetAttribute("r", mr.material.color.r.ToString());
                    xmlColor.SetAttribute("g", mr.material.color.g.ToString());
                    xmlColor.SetAttribute("b", mr.material.color.b.ToString());
                    xmlColor.SetAttribute("a", mr.material.color.a.ToString());
                }

                // 光照贴图信息
                //XmlElement xmlLightmap = XmlDoc.CreateElement("Lightmap");
                //xmlMesh.AppendChild(xmlLightmap);
                // 是否为static，static的对象才有lightmap信息
                //xmlLightmap.SetAttribute("IsStatic", mr.gameObject.isStatic.ToString());
                //xmlLightmap.SetAttribute("LightmapIndex", mr.lightmapIndex.ToString());
                //xmlLightmap.SetAttribute("OffsetX", mr.lightmapTilingOffset.x.ToString());
                //xmlLightmap.SetAttribute("OffsetY", mr.lightmapTilingOffset.y.ToString());
                //xmlLightmap.SetAttribute("OffsetZ", mr.lightmapTilingOffset.z.ToString());
                //xmlLightmap.SetAttribute("OffsetW", mr.lightmapTilingOffset.w.ToString());
            }
        }
    }
}
