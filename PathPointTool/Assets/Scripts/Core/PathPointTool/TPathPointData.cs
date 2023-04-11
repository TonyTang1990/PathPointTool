/*
 * Description:             TPathPointData.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/09
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TPathPointData.cs
    /// 路点数据组件
    /// </summary>
    public class TPathPointData : MonoBehaviour
    {
        /// <summary>
        /// 路点类型
        /// </summary>
        [Header("路点类型")]
        public TPathPointType PPType = TPathPointType.Normal;
    }
}