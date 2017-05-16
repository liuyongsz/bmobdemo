using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[Serializable]
public class PhysicMaterialVO
{
    /// <summary>动摩擦因数</summary>
    public float DynamicFriction;
    /// <summary>静摩擦因数</summary>
    public float StaticFriction;
    /// <summary>弹性系数</summary>
    public float Bounciness;
}
