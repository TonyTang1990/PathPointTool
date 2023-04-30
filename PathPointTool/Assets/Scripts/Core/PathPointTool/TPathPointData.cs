/*
 * Description:             TPathPointData.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/09
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TPathPointData.cs
    /// 路点数据组件
    /// </summary>
    [Serializable]
    public class TPathPointData
    {
        /// <summary>
        /// 坐标位置
        /// </summary>
        [Header("坐标位置")]
        public Vector3 Position;

        /// <summary>
        /// 路点类型
        /// </summary>
        [Header("路点类型")]
        public TPathPointType PPType = TPathPointType.Normal;

        public TPathPointData()
        {
            Position = Vector3.zero;
            PPType = TPathPointType.Normal;
        }

        public TPathPointData(Vector3 position, TPathPointType ppType = TPathPointType.Normal)
        {
            Position = position;
            PPType = ppType;
        }
    }
}