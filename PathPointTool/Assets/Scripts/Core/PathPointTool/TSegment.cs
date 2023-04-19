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
        public int StartPointIndex
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
        /// 路段第一个点占总路程比例
        /// </summary>
        public float FirstPointPathRatio
        {
            get;
            private set;
        }

        /// <summary>
        /// 路段最后一个点占总路程比例
        /// </summary>
        public float LastPointPathRatio
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
            StartPointIndex = 0;
            Length = 0;
            FirstPointPathRatio = 0f;
            LastPointPathRatio = 0f;
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
            StartPointIndex = 0;
            Length = 0;
            FirstPointPathRatio = 0f;
            LastPointPathRatio = 0f;
            PathwayType = TPathwayType.Line;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="startPointIndex"></param>
        /// <param name="length"></param>
        /// <param name="firstPointPathRatio"></param>
        /// <param name="lastPointPathRatio"></param>
        /// <param name="pathwayType"></param>
        public void Init(int startPointIndex, float length, float firstPointPathRatio, float lastPointPathRatio, TPathwayType pathwayType)
        {
            StartPointIndex = startPointIndex;
            Length = length;
            FirstPointPathRatio = firstPointPathRatio;
            LastPointPathRatio = lastPointPathRatio;
            PathwayType = pathwayType;
        }
    }
}