﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;
using TinyBinaryXml;

public class MatchEdit : EditorWindow
{

    private BuildTarget buildTarget = BuildTarget.iOS;
    private int skill_useId;

    [MenuItem("UGame/Match")]
    public static void CreateWizard()
    {
        MatchEdit window = (MatchEdit)EditorWindow.GetWindow(typeof(MatchEdit));
        window.Show();
    }

    public void OnGUI()
    {
        MatchManager.CloneGMType = (CloneGMFlag)EditorGUILayout.EnumPopup("sss", MatchManager.CloneGMType);

        if (GUILayout.Button("发送"))
            SkillProxy.Instance.onCloneGM(MatchManager.CloneGMType);

        EditorGUILayout.Space();

        //if (GUI.Button(new Rect(0,31,100,30),"Shoot"))   Shoot();

        //if (GUI.Button(new Rect(0, 62, 100, 30), "Pass"))   Pass();

        //if (GUI.Button(new Rect(0, 93, 100, 30), "Ball Owner Log"))
        //{
        //    if (Ball.Instance)
        //    {
        //        List<GameObject> list = Ball.Instance.m_logOwner;
        //        if (null != list)
        //        {
        //            int count = list.Count;
        //            for (int i = 0; i < count; i++)
        //            {
        //                if (list[i] != null)
        //                    Debug.Log(i + ":" + list[i].name);
        //                else
        //                    Debug.Log(i + ": null");
        //            }
        //        }
        //    }
        //}

        //if (GUI.Button(new Rect(0, 124, 100, 30), "Ball Owner Info"))
        //{
        //    if (Ball.Instance && Ball.Instance.owner)
        //    {
        //        Player py = Ball.Instance.owner.GetComponent<Player>();

        //        if (!py.gameObject.activeSelf)
        //        {
        //            Debug.Log(py.gameObject.name + " ActiveSelf is False");
        //            return;
        //        }

        //        if(null != py)
        //        {
        //            string str = py.gameObject.name + " : " + py.GetState().ToString();
        //            foreach (AnimationState anim in py.Ani)
        //            {
        //                if (py.Ani.IsPlaying(anim.name))
        //                {
        //                    str += " AniName : " + anim.name;
        //                    break;
        //                }
        //            }

        //            Debug.Log(str);
        //        }
        //    }
        //}


        //if (GUI.Button(new Rect(0, 155, 124, 30), "Look Info"))
        //{
        //    Player player = Ball.Instance.owner.GetComponent<Player>();

        //    Debug.Log("state:" + player.GetState());
        //}

        skill_useId = EditorGUILayout.IntField("技能ID", skill_useId);

        if (GUILayout.Button("确定"))
        {
            SkillProxy.Instance.onGmSetSkill(skill_useId);
        }

        if (GUILayout.Button("Test Skill Success"))
        {
            SkillProxy.Instance.onSkillSucc(new List<object>() { skill_useId });
        }
    }

    private void Shoot()
    {
        if (null != Ball.Instance && null != Ball.Instance.owner)
        {
            Player py = Ball.Instance.owner.GetComponent<Player>();

            if (null != py)
            {
                py.Do_Shoot();
            }
        }
        else
        {
            CommonFun.Debug("Ball or Owner is Null","#6699CC");
        }
    }

    private void Pass()
    {
        if (null != Ball.Instance && null != Ball.Instance.owner)
        {
            Player py = Ball.Instance.owner.GetComponent<Player>();

            if (null != py)
            {
                py.Do_Pass();
            }
        }
        else
        {
            CommonFun.Debug("Ball or Owner is Null", "#6699CC");
        }
    }
}
