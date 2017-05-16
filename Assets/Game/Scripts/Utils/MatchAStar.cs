using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MatchAStarNode
{
    /// <summary>父节点</summary>
    public MatchAStarNode parentNode;

    /// <summary>X坐标</summary>
    public int x;
    /// <summary>Y坐标</summary>
    public int z;
    /// <summary>估价值</summary>
    public int cost;
}

public class MatchAStar
{
    /// <summary>父节点</summary>
    private Dictionary<string, MatchAStarNode> m_dic;
    /// <summary>开放列表</summary>
    private List<MatchAStarNode> m_openList;
    /// <summary>关闭列表</summary>
    private List<MatchAStarNode> m_closeList;
    /// <summary>路径</summary>
    public List<MatchAStarNode> Path;

    /// <summary>x宽度</summary>
    public float CellX;
    /// <summary>z宽度</summary>
    public float CellZ;
    /// <summary>最大坐标</summary>
    public Vector3 MaxPosition;
    /// <summary>最小坐标</summary>
    public Vector3 MinPosition;
    /// <summary>长度</summary>
    private float CellLength;

    /// <summary>终点X</summary>
    private int m_endX;
    /// <summary>终点Z</summary>
    private int m_endZ;
    /// <summary>开始X</summary>
    private int m_startX;
    /// <summary>开始Z</summary>
    private int m_startZ;

    /// <summary>估价最小的项目</summary>
    private MatchAStarNode m_minCostItem;

    private bool m_over = false;

    public Transform m_endTransform;
    private Transform m_startTransform;

    private float m_startTimer = 0f;

    private List<Vector3> m_blockList;

    public void Start(Vector3 start, Vector3 end, Transform startObj, Transform endObj)
    {
        float xf = start.x / CellX;
        float zf = start.z / CellZ;
        float eXf = end.x / CellX;
        float eZf = end.z / CellZ;
        int x = Mathf.FloorToInt(xf);
        int z = Mathf.FloorToInt(zf);
        int ex = Mathf.FloorToInt(eXf);
        int ez = Mathf.FloorToInt(eZf);

        CellLength = Mathf.Sqrt(CellX * CellX + CellZ * CellZ);

        m_endTransform = endObj;
        m_startTransform = startObj;

        Start(x, z, ex, ez);
    }
    public void Start(Transform start, Transform end)
    {
        float xf = start.position.x / CellX;
        float zf = start.position.z / CellZ;
        float eXf = end.position.x / CellX;
        float eZf = end.position.z / CellZ;
        int x =  Mathf.FloorToInt(xf);
        int z =  Mathf.FloorToInt(zf);
        int ex = Mathf.FloorToInt(eXf);
        int ez = Mathf.FloorToInt(eZf);

        CellLength = Mathf.Sqrt(CellX * CellX + CellZ * CellZ);

        m_endTransform = end;
        m_startTransform = start;

        Start(x, z, ex, ez);
    }

    public void Start(int x, int z, int toX, int toZ)
    {
        if (null == m_dic) m_dic = new Dictionary<string, MatchAStarNode>();

        Path = new List<MatchAStarNode>();
        m_openList = new List<MatchAStarNode>();
        m_closeList = new List<MatchAStarNode>();

        m_over = false;
        m_endX = toX;
        m_endZ = toZ;
        m_startX = x;
        m_startZ = z;
        m_minCostItem = null;
        m_startTimer = Time.time;
        m_findCount = 0;

        m_blockList = new List<Vector3>();
        List<GameObject> objs = CoreGame.Instance.players.Concat(CoreGame.Instance.oponents).ToList<GameObject>();
        GameObject obj;
        int count = objs.Count;
        Vector3 pos;
        for (int i = 0; i < count; i++)
        {
            obj = objs[i];
            if (obj.gameObject.activeSelf && obj != m_startTransform.gameObject && obj != m_endTransform.gameObject)
            {
                pos = objs[i].transform.position.Clone();
                m_blockList.Add(pos);
            }
        }

        if(null != m_dic)
            foreach(var node in m_dic)
            {
                node.Value.cost = 0;
                node.Value.parentNode = null;
            }

        MatchAStarNode item = GetItem(x, z);
        FindNearest(x, z);
        FindPath();
    }

    /// <summary>重置</summary>
    public void Reset()
    {
        m_dic = new Dictionary<string, MatchAStarNode>();
        m_endTransform = null;
        m_startTransform = null;
        m_openList = new List<MatchAStarNode>();
        m_closeList = new List<MatchAStarNode>();
    }

    /// <summary>
    /// 获取路径
    /// </summary>
    /// <param name="playerY"></param>
    /// <returns></returns>
    public List<Vector3> GetPath(float playerY = 0)
    {
        int count = Path.Count;
        Vector3 newPos;
        MatchAStarNode node;
        List<Vector3> list = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            node = Path[i];
            newPos = new Vector3(node.x * CellX, playerY, node.z * CellZ);
            list.Add(newPos);
        }

        return list;
    }

    private int m_findCount = 0;
    /// <summary>寻找路径</summary>
    private void FindPath()
    {
        //防止在手机上卡顿 次数限制在400以内
        if (m_findCount > 400)
        {
            m_over = true;
            Path = new List<MatchAStarNode>();
            Debug.LogError("Astar Error Forced Stop!!! : Find Too Long");
            return;
        }

        m_findCount++;
        m_minCostItem = null;

        MatchAStarNode item;
        int count = m_openList.Count;
        int disx;
        int disz;
        int distance;
        int index = -1;

        for (int i = count - 1; i >= 0; i--)
        {
            if (m_over) return;

            item = m_openList[i];

            disx = m_endX - item.x;
            disz = m_endZ - item.z;

            distance = disx * disx + disz * disz;
            item.cost = distance;

            if (item.x == m_endX && item.z == m_endZ)
            {
                GetPath(item);
                return;
            }

            if (null == m_minCostItem)
            {
                index = i;
                m_minCostItem = item;
            }
            else if (item.cost > 0 && item.cost < m_minCostItem.cost)
            {
                index = i;
                m_minCostItem = item;
            }
        }

        if (index > -1)
        {
            item = m_minCostItem;
            m_closeList.Add(item);
            m_openList.RemoveAt(index);
            FindNearest(item.x, item.z, item);
            FindPath();
        }
    }

    /// <summary>得到列表</summary>
    private void GetPath(MatchAStarNode item)
    {
        m_over = true;
        BackPathList(item);
        Path.Reverse();
    }

    /// <summary>回溯列表</summary>
    private void BackPathList(MatchAStarNode item)
    {
        Path.Add(item);

        if (null != item.parentNode) BackPathList(item.parentNode);
    }

    /// <summary>寻找附近列表</summary>
    public void FindNearest(int x, int z, MatchAStarNode parent = null)
    {
        int nX = x; int nZ = z - 1;
        GetItem(nX, nZ, parent);

        nX = x; nZ = z + 1;
        GetItem(nX, nZ, parent);

        nX = x - 1; nZ = z - 1;
        GetItem(nX, nZ, parent);

        nX = x - 1; nZ = z;
        GetItem(nX, nZ, parent);

        nX = x - 1; nZ = z + 1;
        GetItem(nX, nZ, parent);

        nX = x + 1; nZ = z - 1;
        GetItem(nX, nZ, parent);

        nX = x + 1; nZ = z;
        GetItem(nX, nZ, parent);

        nX = x + 1; nZ = z + 1;
        GetItem(nX, nZ, parent);
    }

    /// <summary>获取项</summary>
    private MatchAStarNode GetItem(int x, int z, MatchAStarNode parent = null)
    {
        if (x > MaxPosition.x || z > MaxPosition.z || x < MinPosition.x || z < MinPosition.z) return null;

        MatchAStarNode item;

        string key = x + "_" + z;
        if (!m_dic.TryGetValue(key, out item))
        {
            item = new MatchAStarNode();
            m_dic[key] = item;
        }

        if (m_over) return item;
        if (m_closeList.IndexOf(item) >= 0) return item;

        item.x = x;
        item.z = z;
        item.cost = 0;
        item.parentNode = parent;

        if (null != m_startTransform && null != m_endTransform)
        {
            float nx = x * CellX;
            float nz = z * CellZ;
            float ny = 0;
            Vector3 newP = new Vector3(nx,ny,nz);

            bool add = true;
            float disLen;
            float compLen;
            compLen = CellX > CellZ ? CellX : CellZ;
            compLen /= 2.0f;
            compLen -= 0.0001f;
            //compLen = CellLength / 2f;
            int count;

            Vector3 blockPos;
            count = m_blockList.Count;
            for (int i = 0; i < count; i++)
            {
                blockPos = m_blockList[i];

                disLen = (blockPos - newP).magnitude;

                float xf = blockPos.x / CellX;
                float zf = blockPos.z / CellZ;
                int ex = Mathf.FloorToInt(xf);
                int ez = Mathf.FloorToInt(zf);

                if (ex == m_startX && ez == m_startZ) continue;

                //球员在compLen范围内 加入关闭列表
                //if (disLen < compLen
                //    || (x == ex && z == ez)
                //    || (Mathf.Abs(x - ex) == 1 && Math.Abs(z- ez) <= 1)
                //    || (Mathf.Abs(x - ex) <= 1 && Math.Abs(z - ez) == 1)
                //    )
                //{
                //    add = false;
                //    m_closeList.Add(item);
                //    break;
                //}
                //else 
                if (null != item.parentNode)
                {
                    //球员到 两点的摄影距离小于0.8米 并且球员在CellLength范围内 加入关闭列表
                    Vector3 oldv = new Vector3(item.parentNode.x * CellX, newP.y, item.parentNode.z * CellZ);
                    //if (GeomUtil.InSector(newP, blockPos, 30f, 1f) && disLen < CellLength && disLen > 0.8f)
                    if (GeomUtil.DistanceOfPointToVector(oldv, newP, blockPos) < 1f && disLen < CellLength)
                    {
                        add = false;
                        m_closeList.Add(item);
                        break;
                    }
                }
                else
                {
                    add = false;
                    m_closeList.Add(item);
                    break;
                }
            }

            if (add)
            {
                AddToOpenList(item);
            }   

        }

        return item;
    }

    /// <summary>加入到关闭列表</summary>
    public bool AddToCloseList(MatchAStarNode item)
    {
        if (m_closeList.IndexOf(item) == -1)
        {
            m_closeList.Add(item);
            return true;
        }
        return false;
    }

    /// <summary>加入到开放列表</summary>
    public bool AddToOpenList(MatchAStarNode item)
    {
        if (m_openList.IndexOf(item) == -1 && m_closeList.IndexOf(item) == -1)
        {
            m_openList.Add(item);
            return true;
        } 
        return false;
    }

    public bool CheckInRange(Transform obj, Vector3 endObj)
    {
        float xf = obj.position.x / CellX;
        float zf = obj.position.z / CellZ;

        int x = (int)xf;
        int z = (int)zf;

        return CheckInRange(x,z,endObj);
    }

    private bool CheckInRange(int nX,int nZ,Vector3 endObj)
    {
        if (CheckInRange(nX, nZ, CoreGame.Instance.players,endObj)) return true;

        else if (CheckInRange(nX, nZ, CoreGame.Instance.oponents,endObj)) return true;

        return false;
    }

    public  bool CheckInRange(int nX, int nZ,List<GameObject> objs,Vector3 endPos)
    {
        if (nX == m_endX && nZ == m_endZ) return false;

        //float range = 4f;
        //float range2 = 0.8f;
        Vector3 itemPos;
        GameObject obj;
        float disLen;
       // float disPlayer;
        int count = objs.Count;
        Vector3 objPos;
        //Vector3 dir;
        Vector3 orDir = new Vector3(1,0,0);

        for (int i = 0; i < count; i++)
        {
            obj = objs[i];

            if (!obj.activeSelf) continue;

            objPos = obj.transform.position;
            itemPos = new Vector3(nX * CellX, obj.transform.position.y, nZ * CellZ);

            disLen = (itemPos - obj.transform.position).magnitude;

            //disPlayer = GeomUtil.DistanceOfPointToVector(itemPos, m_endTransform.transform.position, obj.transform.position);

            if (
               //disPlayer < range
               //disPlayer > 1.2f
               //disLen < 1.2f &&
               // && deg <= 60
               obj.name.CompareTo(m_startTransform.gameObject.name) != 0
               && obj.name.CompareTo(m_endTransform.gameObject.name) != 0
               && CheckInRectRange(itemPos, endPos, objPos)
                )
            {
                return true;
            }
        }

        return false;
    }

    public bool CheckInRange(Vector3 startPos, Vector3 endObj)
    {
        if (CheckInRange(startPos, CoreGame.Instance.players, endObj)) return true;

        else if (CheckInRange(startPos, CoreGame.Instance.oponents, endObj)) return true;

        return false;
    }

    public bool CheckInRange(Vector3 startPos, List<GameObject> objs, Vector3 endPos)
    {
        float xf = startPos.x / CellX;
        float zf = startPos.z / CellZ;

        int x = (int)xf;
        int z = (int)zf;

        if (x == m_endX && z == m_endZ) return false;

        Vector3 itemPos;
        GameObject obj;
        float disLen;
        int count = objs.Count;
        Vector3 objPos;
        Vector3 orDir = new Vector3(1, 0, 0);
        MatchPlayerItem item;

        for (int i = 0; i < count; i++)
        {
            obj = objs[i];

            if (!obj.activeSelf) continue;

            objPos = obj.transform.position;
            itemPos = startPos;

            disLen = (itemPos - obj.transform.position).magnitude;

            item = MatchManager.GetPlayerItem(obj);

            if (
               obj.name.CompareTo(m_startTransform.gameObject.name) != 0
               && obj.name.CompareTo(m_endTransform.gameObject.name) != 0
               && obj.activeSelf
               && null != item.playerControl.capsuleCollider
               && !item.playerControl.capsuleCollider.isTrigger
               && CheckInRectRange(itemPos, endPos, objPos)
                )
            {
                return true;
            }
        }

        return false;
    }

    public static bool CheckInRectRange(Vector3 startPos, Vector3 endPos, Vector3 targetPos)
    {
        float range2 = 1.5f;
        Vector3 dir = endPos - startPos;
        Vector3 orDir = new Vector3(-dir.x, 0, 0);

        dir.y = 0;
        dir = dir.normalized;
        Vector3.OrthoNormalize(ref dir, ref orDir);

        if (dir.x == 0) orDir = new Vector3(1, 0, 0);
        else if (dir.z == 0) orDir = new Vector3(0, 0, 1);
        else if (dir.y > 0) orDir = new Vector3(0,0,1);

        Vector3 v0 = startPos + orDir * range2;
        Vector3 v1 = startPos - orDir * range2;
        Vector3 v2 = endPos + orDir * range2;
        Vector3 v3 = endPos - orDir * range2;
        v0.y = v1.y = v2.y = v3.y = 0.01f;
        AllRef.v0 = v0;
        AllRef.v1 = v1;
        AllRef.v2 = v2;
        AllRef.v3 = v3;

        return GeomUtil.IsPointInMatrix(v0, v1, v2, v3, targetPos);
    }
}
