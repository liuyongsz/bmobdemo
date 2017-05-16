using UnityEngine;
using UnityEditor;
using ProxyInstance;
using System.Collections.Generic;

public class ShootTestEdit : EditorWindow
{
    private static ShootTestManager shootMen;
    private static Camera camera;
    private static ShootTestEdit m_window;
    private static PlayerManager m_cfg;
    private static MyCamera m_myCamera;

    private TD_PushShootData pushData;

    private bool m_ballFoldout = false;
    private bool m_groundFoldout = false;

    private int m_idsLen = 0;

    [SerializeField]//必须要加
    protected List<int> _assetLst = new List<int>();

    //序列化对象
    protected static SerializedObject _serializedObject;

    //序列化属性
    protected static SerializedProperty _assetLstPropertyI_ids;
    protected static SerializedProperty _assetLstPropertyI_fstLst;
    protected static SerializedProperty _assetLstPropertyI_secLst;
    protected static SerializedProperty _assetLstPropertyI_thrLst;
    protected static SerializedProperty _assetLstPropertyI_PointLst;

    private TD_PassOrShootItem m_editShootItem;
    private TD_PassOrShootItem m_editShootItem2;
    private int m_shootId;

    private int m_skillIdIndex = 0;

    private int skill_useId;

    [MenuItem("UGame/Shoot Test")]
    public static void CreateWizard()
    {
        if(null == m_window)
        {
            m_window = (ShootTestEdit)EditorWindow.GetWindow(typeof(ShootTestEdit));
            m_window.Show();

            camera = Camera.main;
            shootMen = ShootTestManager.Instace;
            m_myCamera = camera.GetComponent<MyCamera>();
            
            m_cfg = ProxyInstance.InstanceProxy.Get<PlayerManager>();

            //使用当前类初始化
            _serializedObject = new SerializedObject(DefineManager.Ins);
            //获取当前类中可序列话的属性
            _assetLstPropertyI_ids = _serializedObject.FindProperty("i_ids");
            _assetLstPropertyI_fstLst = _serializedObject.FindProperty("i_fstList");
            _assetLstPropertyI_secLst = _serializedObject.FindProperty("i_secList");
            _assetLstPropertyI_thrLst = _serializedObject.FindProperty("i_thrList");
            _assetLstPropertyI_PointLst = _serializedObject.FindProperty("TestPoint");

            DefineManager.Ins.FstResult = E_StepResult.con_result_be_steal;
            DefineManager.Ins.SecResult = E_StepResult.con_result_pass_succ;
            DefineManager.Ins.ThrResult = E_StepResult.con_result_shoot_succ;
        }
    }

    public void OnGUI()
    {
        if (null == shootMen)
        {
            if (null != m_window)
            {
                camera = null;
                shootMen = null;
                m_window.Close();
            }

            return;
        }

        AllRef.EditerType = (EditerType)EditorGUILayout.EnumPopup("编辑", AllRef.EditerType);
        EditorGUILayout.Space();

        if (AllRef.EditerType == EditerType.Shoot)
        {
            OnGUI_Shoot();
        }
        else if(AllRef.EditerType == EditerType.Match)
        {
            OnGUI_Match();
        }
    }

    private void OnGUI_Skill()
    {
        skill_useId = EditorGUILayout.IntField("技能ID", skill_useId);

        if (GUILayout.Button("确定"))
        {
            SkillProxy.Instance.onGmSetSkill(skill_useId);
        }
    }

    private void OnGUI_Match()
    {
        ShootTestManager.Instace.LeftMesh.gameObject.SetActive(true);
        ShootTestManager.Instace.RightMesh.gameObject.SetActive(true);

        DefineManager deMen = DefineManager.Ins;

        //更新
        _serializedObject.Update();

        //开始检查是否有修改
        EditorGUI.BeginChangeCheck();

        //显示属性
        //第二个参数必须为true，否则无法显示子节点即List内容
        EditorGUILayout.PropertyField(_assetLstPropertyI_ids, true);
        EditorGUILayout.PropertyField(_assetLstPropertyI_fstLst, true);
        EditorGUILayout.PropertyField(_assetLstPropertyI_secLst, true);
        EditorGUILayout.PropertyField(_assetLstPropertyI_thrLst, true);
        EditorGUILayout.PropertyField(_assetLstPropertyI_PointLst, true);
        deMen.i_keeperId = EditorGUILayout.IntField("守门员ID:", deMen.i_keeperId);

        //结束检查是否有修改
        if (EditorGUI.EndChangeCheck())
        {//提交修改
            _serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Create"))
        {
            ShootTestManager.Instace.CreateVarturalMatch();
        }

        if (GUILayout.Button("Start"))
        {
            AllRef.Ball.owner = MatchManager.AtkPlayer.playerBody;
            CommonFun.BallToPlayerFoot(AllRef.Ball.owner.transform);
            MatchManager.Step_One();
        }

        EditorGUILayout.LabelField("我的球员:");
        EditorGUILayout.BeginHorizontal();
        if(null != MatchManager.MyCards)
        {
            int idex;
            int cnt = MatchManager.MyCards.Count;
            for(int i = 0; i < cnt;i++)
            {
                idex = MatchManager.MyCards[i];
                if (GUILayout.Button(idex.ToString()))
                {
                    MatchPlayerItem item = MatchManager.GetItemByDid(idex);
                    Selection.activeObject = item.playerBody;
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        //EditorGUILayout.LabelField("对手球员");

        DefineManager dman = DefineManager.Ins;
        dman.FstResult = (E_StepResult)EditorGUILayout.EnumPopup("首轮结果:", dman.FstResult);
        if(dman.FstResult == E_StepResult.select)
            dman.FstSelResult = (E_StepResult)EditorGUILayout.EnumPopup("首轮选择结果:", dman.FstSelResult);

        dman.SecResult = (E_StepResult)EditorGUILayout.EnumPopup("次轮结果:", dman.SecResult);
        if (dman.SecResult == E_StepResult.select)
            dman.SecSelResult = (E_StepResult)EditorGUILayout.EnumPopup("次轮选择结果:", dman.SecSelResult);

        dman.ThrResult = (E_StepResult)EditorGUILayout.EnumPopup("终轮结果:", dman.ThrResult);
        if (dman.ThrResult == E_StepResult.select)
            dman.ThrSelResult = (E_StepResult)EditorGUILayout.EnumPopup("终轮选择结果:", dman.ThrSelResult);

        if( dman.FstResult == E_StepResult.con_result_shoot_succ)
            MatchManager.ShootCalResult = true;
        else if(dman.ThrResult == E_StepResult.con_result_shoot_succ)
            MatchManager.ShootCalResult = true;
        else if (dman.SecResult == E_StepResult.con_result_shoot_succ)
            MatchManager.ShootCalResult = true;
        else
            MatchManager.ShootCalResult = false;

        OnGUI_FieldView();
    }

    private void OnGUI_FieldView()
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("视野:");
        camera.fieldOfView = EditorGUILayout.Slider(camera.fieldOfView, 0, 100f);
        EditorGUILayout.EndHorizontal();
    }

    private void OnGUI_Shoot()
    {
        shootMen.m_shootType = (E_PassOrShootType)EditorGUILayout.EnumPopup("类型", shootMen.m_shootType);
        string shootName = "";
        switch (shootMen.m_shootType)
        {
            case E_PassOrShootType.PointHeight: shootName = "贝塞尔"; break;
            case E_PassOrShootType.Ground: shootName = "踢球"; break;
            case E_PassOrShootType.PushShoot: shootName = "推射"; break;
            case E_PassOrShootType.Normal: shootName = "普通射门"; break;
        }

        if (GUILayout.Button(shootName))
        {
            shootMen.ResetState();

            if (shootMen.m_shootType == E_PassOrShootType.Ground)
                shootMen.player.Do_TestPass(shootMen.EndPostion);
            else if (shootMen.m_shootType == E_PassOrShootType.PushShoot)
                shootMen.player.Do_TestPushShoot(shootMen.EndPostion);
        }

        if (shootMen.m_shootType == E_PassOrShootType.Ground)
        {
            m_ballFoldout = EditorGUILayout.Foldout(m_ballFoldout, "球的参数");
            if (m_ballFoldout)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("  动摩擦力:");
                m_cfg.BallPhysis.DynamicFriction = EditorGUILayout.FloatField(m_cfg.BallPhysis.DynamicFriction);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("  静摩擦力:");
                m_cfg.BallPhysis.StaticFriction = EditorGUILayout.FloatField(m_cfg.BallPhysis.StaticFriction);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("  弹性系数:");
                m_cfg.BallPhysis.Bounciness = EditorGUILayout.FloatField(m_cfg.BallPhysis.Bounciness);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            m_groundFoldout = EditorGUILayout.Foldout(m_groundFoldout, "地面参数");
            if (m_groundFoldout)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("  动摩擦力:");
                m_cfg.GroundPhysis.DynamicFriction = EditorGUILayout.FloatField(m_cfg.GroundPhysis.DynamicFriction);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("  静摩擦力:");
                m_cfg.GroundPhysis.StaticFriction = EditorGUILayout.FloatField(m_cfg.GroundPhysis.StaticFriction);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("  弹性系数:");
                m_cfg.GroundPhysis.Bounciness = EditorGUILayout.FloatField(m_cfg.GroundPhysis.Bounciness);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("距离:");
            float oldz = shootMen.EndPostion.z;
            shootMen.EndPostion.z = EditorGUILayout.FloatField(shootMen.EndPostion.z);

            if (oldz != shootMen.EndPostion.z)
            {
                shootMen.UpdateInfo(shootMen.EndPostion.z);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("方向力:");
            shootMen.force = EditorGUILayout.FloatField(shootMen.force);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("向上力:");
            shootMen.Yforce = EditorGUILayout.FloatField(shootMen.Yforce);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("空气阻力:");
            shootMen.airDrag = EditorGUILayout.FloatField(shootMen.airDrag);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("地面阻力:");
            shootMen.groundDrag = EditorGUILayout.FloatField(shootMen.groundDrag);
            EditorGUILayout.EndHorizontal();


        }
        else if (shootMen.m_shootType == E_PassOrShootType.PushShoot)
        {
            if (null == pushData)
            {
                pushData = m_cfg.GetPushShootData(0);
                shootMen.EndPostion.z = pushData.distance;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("距离:");
            float oldz = shootMen.EndPostion.z;
            shootMen.EndPostion.z = EditorGUILayout.FloatField(shootMen.EndPostion.z);
            if (oldz != pushData.distance)
            {
                pushData = m_cfg.GetPushShootData(shootMen.EndPostion.z);
                shootMen.EndPostion.z = pushData.distance;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("方向力:");
            pushData.force = EditorGUILayout.FloatField(pushData.force);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("向上力:");
            pushData.addFore = EditorGUILayout.TextArea(pushData.addFore);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("地面阻力:");
            pushData.drag = EditorGUILayout.FloatField(pushData.drag);
            EditorGUILayout.EndHorizontal();
        }
        else if (shootMen.m_shootType == E_PassOrShootType.NormalShoot)
        {
            OnGui_NormalBezier();
            Save();
            OnGUI_FieldView();
            return;
        }
        else if (shootMen.m_shootType == E_PassOrShootType.SkillShoot)
        {
            OnGUI_FieldView();
            OnGui_SkillBezier();
            Save();
            return;
        }

        Save();

        GUILayout.Label(" ");
        if (GUILayout.Button("保存传球时间"))
        {
            shootMen.Save_Timer();
            BinaryXmlConvertor.SaveSngXML("PlayerDefine");
        }

        m_cfg.GuoRenRange = EditorGUILayout.FloatField("过人曲线幅度:", m_cfg.GuoRenRange);
        GameManager.Instance.PlayerRunCure = EditorGUILayout.CurveField("过人曲线: ", GameManager.Instance.PlayerRunCure);

        if (GUILayout.Button("过人测试"))
        {
            shootMen.ResetState();
            shootMen.player.Do_TestGuoRen();
        }

        m_cfg.AddPostion = EditorGUILayout.Vector3Field("焦点最终位置:", m_cfg.AddPostion);
        m_cfg.AddRotation = EditorGUILayout.Vector3Field("焦点最终旋转:", m_cfg.AddRotation);

        OnGUI_FieldView();
    }

    public void Save()
    {
        if (GUILayout.Button("保存传球数据"))
        {
            if (m_editShootItem != null && m_editShootItem.speed == 0)
            {
                m_cfg.m_skillShootData.Remove(m_editShootItem);
                m_editShootItem = null;
            }

            CoreGame.Instance.save();
            BinaryXmlConvertor.SaveSngXML("PlayerDefine");
        }
    }

    private void OnGui_NormalBezier()
    {
        ShootTestManager.Instace.player.Skill_Control.ShootSkillID = 0;
        DefineManager.Ins.DrawBallLine = EditorGUILayout.Toggle("DrawLine", DefineManager.Ins.DrawBallLine);

        m_shootId = EditorGUILayout.IntField("射门配置ID:", m_shootId);
        ShootTestManager.Instace.ShootPoint = EditorGUILayout.IntField("测试点:", ShootTestManager.Instace.ShootPoint);

        if (m_oldSKillID2 != m_shootId)
        {
            m_oldSKillID2 = m_shootId;

            m_editShootItem2 = m_cfg.GetShootSpeedByCfgId(m_shootId);

            if (null != m_editShootItem2)
                m_editShootItem2.GetOperType();
        }

        if (null == m_editShootItem2 && m_shootId > 0)
        {
            if (GUILayout.Button("----创建----"))
            {
                m_editShootItem2 = new TD_PassOrShootItem();
                m_editShootItem2.id = m_shootId;
                m_editShootItem2.passPer = "1";
                m_editShootItem2.point = "";
                m_editShootItem2.passType = "2";
                m_editShootItem2.radHeight = "";
                m_editShootItem2.P1_RelativeDistacnePers = "";
                m_editShootItem2.P1_OffsetAngles = "";
                m_cfg.m_shootData.Add(m_editShootItem2);
            }
        }

        if (null != m_editShootItem2)
        {
            m_editShootItem2.speed = EditorGUILayout.FloatField("速度:", m_editShootItem2.speed);
            m_editShootItem2.point = EditorGUILayout.TextField("攻击点:", m_editShootItem2.point);
            m_editShootItem2.typeRadHeight = EditorGUILayout.FloatField("控制点高度:", m_editShootItem2.typeRadHeight);
            m_editShootItem2.relativeDistacnePer = EditorGUILayout.FloatField("控制点位置:", m_editShootItem2.relativeDistacnePer);
            m_editShootItem2.P1_OffsetAngle = EditorGUILayout.IntField("控制点偏角:", m_editShootItem2.P1_OffsetAngle);

            m_editShootItem2.radHeight = m_editShootItem2.typeRadHeight.ToString();
            m_editShootItem2.P1_RelativeDistacnePers = m_editShootItem2.relativeDistacnePer.ToString();
            m_editShootItem2.P1_OffsetAngles = m_editShootItem2.P1_OffsetAngle.ToString();

            if (GUILayout.Button("----删除----"))
            {
                m_cfg.m_shootData.Remove(m_editShootItem2);
                m_editShootItem2 = null;
            }
        }

        if (GUILayout.Button("射门"))
        {
            shootMen.ResetState();
            shootMen.player.Do_TestShoot();
        }

        CoreGame.Instance.goalKeeper_left.state = (GoalKeeper_State)EditorGUILayout.EnumPopup("Left Kepper State:", CoreGame.Instance.goalKeeper_left.state);
        CoreGame.Instance.goalkeeper_right.state = (GoalKeeper_State)EditorGUILayout.EnumPopup("Reft Kepper State:", CoreGame.Instance.goalkeeper_right.state);
    }

    private int m_oldSKillID;
    private int m_oldSKillID2;
    private void OnGui_SkillBezier()
    {
        ShootTestManager.Instace.ShootPoint = 0;
        DefineManager.Ins.DrawBallLine = EditorGUILayout.Toggle("DrawLine", DefineManager.Ins.DrawBallLine);

        int shootId = ShootTestManager.Instace.player.Skill_Control.ShootSkillID;
        if (shootId == 0 && m_oldSKillID > 0)
            shootId = m_oldSKillID;

        //ShootTestManager.Instace.player.Skill_Control.ShootSkillID = EditorGUILayout.IntField("技能ID:",shootId);

        int i = 0;
        string[] keys = new string[m_cfg.m_skillShootData.Count];
        int cnt = keys.Length;
        for (i = 0; i < cnt; i++)
            keys[i] = m_cfg.m_skillShootData[i].id.ToString();

        if (m_skillIdIndex >= cnt)
            m_skillIdIndex = cnt - 1;

        m_skillIdIndex = EditorGUILayout.Popup("技能射门ID:", m_skillIdIndex, keys);
        ShootTestManager.Instace.player.Skill_Control.ShootSkillID = m_cfg.m_skillShootData[m_skillIdIndex].id;

        ShootTestManager.Instace.PSItem = this.m_editShootItem;

        if (m_oldSKillID != shootId)
        {
            if (m_editShootItem != null && m_editShootItem.speed == 0)
                m_cfg.m_skillShootData.Remove(m_editShootItem);

            m_editShootItem = m_cfg.GetSkillShootSpeed(shootId);

            if(null != m_editShootItem)
                m_editShootItem.GetOperType();
        }

        // if (null == m_editShootItem && shootId > 0)
        //  {
        if (GUILayout.Button("----创建----"))
        {
            int lastIndex = m_cfg.m_skillShootData.Count - 1;

            m_editShootItem = new TD_PassOrShootItem();
            m_editShootItem.id = m_cfg.m_skillShootData[lastIndex].id + 1;
            m_editShootItem.speed = 28f;
            m_editShootItem.passPer = "1";
            m_editShootItem.passType = "2";
            m_editShootItem.radHeight = "";
            m_editShootItem.P1_RelativeDistacnePers = "";
            m_editShootItem.P1_OffsetAngles = "";
            m_cfg.m_skillShootData.Add(m_editShootItem);

            m_skillIdIndex = cnt;
            shootId = m_editShootItem.id;
            return;
        }
        // }

        m_oldSKillID = shootId;

        if (null != m_editShootItem)
        {
            m_editShootItem.speed = EditorGUILayout.FloatField("速度:", m_editShootItem.speed);
            m_editShootItem.typeRadHeight = EditorGUILayout.FloatField("控制点高度:", m_editShootItem.typeRadHeight);
            m_editShootItem.relativeDistacnePer = EditorGUILayout.FloatField("控制点位置:", m_editShootItem.relativeDistacnePer);
            m_editShootItem.P1_OffsetAngle = EditorGUILayout.IntField("控制点偏角:", m_editShootItem.P1_OffsetAngle);

            m_editShootItem.radHeight = m_editShootItem.typeRadHeight.ToString();
            m_editShootItem.P1_RelativeDistacnePers = m_editShootItem.relativeDistacnePer.ToString();
            m_editShootItem.P1_OffsetAngles = m_editShootItem.P1_OffsetAngle.ToString();

            if (GUILayout.Button("----删除----") && m_cfg.m_skillShootData.Count > 1)
            {
                m_cfg.m_skillShootData.Remove(m_editShootItem);

                m_editShootItem = m_cfg.m_skillShootData[0];
                m_skillIdIndex = 0;
                ShootTestManager.Instace.player.Skill_Control.ShootSkillID = m_editShootItem.id;
                return;
            }

            if (GUILayout.Button("射门"))
            {
                shootMen.ResetState();
                shootMen.player.Do_TestShoot();
            }
        }

        CoreGame.Instance.goalKeeper_left.state = (GoalKeeper_State)EditorGUILayout.EnumPopup("Left Kepper State:", CoreGame.Instance.goalKeeper_left.state);
        CoreGame.Instance.goalkeeper_right.state = (GoalKeeper_State)EditorGUILayout.EnumPopup("Reft Kepper State:", CoreGame.Instance.goalkeeper_right.state);
    }
}
