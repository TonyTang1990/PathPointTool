/*
 * Description:             TSegment.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/12
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TSegment.cs
    /// 路段抽象
    /// </summary>
    public class TSegment :IRecycle
    {
        /// <summary>
        /// 顶点起始索引
        /// </summary>
        public int PointStartIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// 分段长度
        /// </summary>
        public float Length
        {
            get;
            private set;
        }

        /// <summary>
        /// 路线类型
        /// </summary>
        public TPathwayType PathwayType
        {
            get;
            private set;
        }

        public TSegment()
        {
            PointStartIndex = 0;
            Length = 0;
            PathwayType = TPathwayType.Line;
        }

        public void OnCreate()
        {
            Reset();
        }

        public void OnDispose()
        {
            Reset();
        }

        private void Reset()
        {
            PointStartIndex = 0;
            Length = 0;
            PathwayType = TPathwayType.Line;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="pointStartIndex"></param>
        /// <param name="length"></param>
        /// <param name="pathwayType"></param>
        public void Init(int pointStartIndex, float length, TPathwayType pathwayType)
        {
            PointStartIndex = pointStartIndex;
            Length = length;
            PathwayType = pathwayType;
        }
    }
}