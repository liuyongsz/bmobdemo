using UnityEngine;
using UnityEditor;
using ProxyInstance;

public class PlayerXMLEdit : Editor {

    public int passCount = 0;
    public int shootCount;
	public void EditorTest()
	{

	}

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Save XML Data"))
        {
         
            BinaryXmlConvertor.SaveSngXML("PlayerDefine");
        }
    }
}
