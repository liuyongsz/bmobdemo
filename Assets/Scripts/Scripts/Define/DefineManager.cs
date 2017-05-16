using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary></summary>
public class DefineManager : MonoBehaviour
{
    public static DefineManager Ins;

    public bool DrawBallLine;
    public bool LogSynInfo;
    public bool OneEndSameDebug; //第一轮 和 最后一轮是同一个人 
    public Transform BallEndObject;
    public Vector3 TestMoveToPostion = Vector3.zero;
    public string Debug_PlayRun;

    public bool ShootTestResult;

    public MatchHalfType RoundType;
    public bool TestStep;
    public E_StepResult FstResult = E_StepResult.Null;
    public E_StepResult SecResult = E_StepResult.Null;
    public E_StepResult ThrResult = E_StepResult.Null;
    public E_StepResult FstSelResult = E_StepResult.Null;
    public E_StepResult SecSelResult = E_StepResult.Null;
    public E_StepResult ThrSelResult = E_StepResult.Null;

    public List<int> TestPoint;
    public List<int> i_ids;
    public List<int> i_fstList;
    public List<int> i_secList;
    public List<int> i_thrList;
    public int i_keeperId;

    public bool AtkIsLeft = true;

    //是否使用补差时间
    public bool UseOffsetTimer;

    public TD_PassOrShootItem ToSupplyItem;
    public TD_PassOrShootItem SupplyBackItem;
    public void Start()
    {
        Ins = this;

        ToSupplyItem = new TD_PassOrShootItem();
        ToSupplyItem.distance = 10;
        ToSupplyItem.force = 69;
        ToSupplyItem.drag = 0.3f;
        ToSupplyItem.airDrag = 0.5f;
        ToSupplyItem.addFore = "0,14,0";
        ToSupplyItem.passType = "3";

        SupplyBackItem = new TD_PassOrShootItem();
        SupplyBackItem.distance = 13;
        SupplyBackItem.force = 85;
        SupplyBackItem.drag = 0.3f;
        SupplyBackItem.airDrag = 0.5f;
        SupplyBackItem.addFore = "0,14,0";
        SupplyBackItem.passType = "3";
    }

    public void Update()
    {
       
    }
}
