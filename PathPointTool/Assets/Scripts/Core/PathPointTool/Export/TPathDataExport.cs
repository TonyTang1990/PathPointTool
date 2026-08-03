/*
 * Description:             TPathDataExport.cs
 * Author:                  TONYTANG
 * Create Date:             2026/07/31
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PathPoint
{
    /// <summary>
    /// TPathDataExport.cs
    /// 统一导出数据结构
    /// </summary>
    [Serializable]
    public class TPathDataExport
    {
        /// <summary>
        /// 路线类型
        /// </summary>
        [Header("路线类型")]
        public TPathType PathType = TPathType.Normal;

        /// <summary>
        /// 路线绘制类型
        /// </summary>
        [Header("路线绘制类型")]
        public TPathwayType PathwayType = TPathwayType.Liner;

        /// <summary>
        /// 缓动类型
        /// </summary>
        [Header("缓动类型")]
        public EasingFunction.Ease Ease = EasingFunction.Ease.Linear;

        /// <summary>
        /// 是否循环
        /// </summary>
        [Header("是否循环")]
        public bool IsLoop = false;

        /// <summary>
        /// 是否更新朝向
        /// </summary>
        [Header("是否更新朝向")]
        public bool UpdateForward = false;

        /// <summary>
        /// 持续时长
        /// </summary>
        [Header("持续时长")]
        public float Duration = 0f;

        /// <summary>
        /// 每段顶点细分数量
        /// </summary>
        [Header("每段顶点细分数量")]
        [Range(1, 100)]
        public int Segment = 15;

        /// <summary>
        /// 路点数据列表
        /// </summary>
        [Header("路点数据列表")]
        public List<TPathPointData> PathPointDatas = new List<TPathPointData>();

        /// <summary>
        /// 构造函数
        /// </summary>
        public TPathDataExport()
        {
            PathPointDatas = new List<TPathPointData>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pathPointDatas"></param>
        public TPathDataExport(List<TPathPointData> pathPointDatas)
        {
            PathPointDatas = pathPointDatas;
        }

        /// <summary>
        /// 添加路点数据列表
        /// </summary>
        /// <param name="pathPointDatas"></param>
        /// <returns></returns>
        public bool AddPathPointDatas(List<TPathPointData> pathPointDatas)
        {
            if (pathPointDatas == null || pathPointDatas.Count == 0)
            {
                Debug.LogError("TPathDataExport.AddPathPointDatas: pathPointDatas is null or empty");
                return false;
            }

            PathPointDatas.AddRange(pathPointDatas);
            return true;
        }

        /// <summary>
        /// 添加路点数据
        /// </summary>
        /// <param name="pathPointData"></param>
        /// <returns></returns>
        public bool AddPathPointData(TPathPointData pathPointData)
        {
            if (pathPointData == null)
            {
                Debug.LogError("TPathDataExport.AddPathPointData: pathPointData is null");
                return false;
            }

            PathPointDatas.Add(pathPointData);
            return true;
        }

        /// <summary>
        /// 获取指定索引路点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TPathPointData GetPathPointData(int index)
        {
            var totalPathPointNum = PathPointDatas != null ? PathPointDatas.Count : 0;
            if (index < 0 || index >= totalPathPointNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:{0}-{totalPathPointNum - 1}，获取路点数据失败！");
                return null;
            }
            return PathPointDatas[index];
        }

        /// <summary>
        /// 获取路点数据列表
        /// </summary>
        public void ClearPathPointDatas()
        {
            PathPointDatas.Clear();
        }

        /// <summary>
        /// 获取所有路点位置列表(新列表)
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetPathPointPosList()
        {
            var posList = new List<Vector3>();
            foreach (var pathPointData in PathPointDatas)
            {
                posList.Add(pathPointData.Position);
            }
            return posList;
        }
    }
}