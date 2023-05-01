/*
 * Description:             TPathData.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/09
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TPathData.cs
    /// 路线数据编辑组件
    /// </summary>
    [ExecuteInEditMode]
    public class TPathData : MonoBehaviour
    {
        /// <summary>
        /// 绘制总开关
        /// </summary>
        [Header("绘制总开关")]
        public bool DrawSwitch = true;

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
        public float Duration = 10f;

        /// <summary>
        /// 路点起始位置
        /// </summary>
        [Header("路点起始位置")]
        public Vector3 PathPointStartPos = Vector3.zero;

        /// <summary>
        /// 路点间隔
        /// </summary>
        [Header("路点间隔")]
        public float PathPointGap = 1;

        /// <summary>
        /// 每段顶点细分数量
        /// </summary>
        [Header("每段顶点细分数量")]
        [Range(1, 100)]
        public int Segment = 15;

        /// <summary>
        /// 路点球体大小
        /// </summary>
        [Header("路点球体大小")]
        [Range(0.1f, 10f)]
        public float PathPointSphereSize = 0.5f;

        /// <summary>
        /// LineRenderer绘制组件
        /// </summary>
        [Header("LineRenderer绘制组件")]
        public LineRenderer DrawLineRenderer;

        /// <summary>
        /// 路点球体颜色
        /// </summary>
        [Header("路点球体颜色")]
        public Color PathPointSphereColor = Color.green;

        /// <summary>
        /// 路线绘制颜色
        /// </summary>
        [Header("路线绘制颜色")]
        public Color PathDrawColor = Color.yellow;

        /// <summary>
        /// 细分路点绘制颜色
        /// </summary>
        [Header("细分路点绘制颜色")]
        public Color SubPathPointDrawColor = Color.blue;

        /// <summary>
        /// 路点数据列表
        /// </summary>
        [Header("路点数据列表")]
        [SerializeReference]
        public List<TPathPointData> PathPointDataList = new List<TPathPointData>();

        /// <summary>
        /// 模拟移动对象
        /// </summary>
        [Header("模拟移动对象")]
        public GameObject SimulationMoveGO;

        /// <summary>
        /// 路点父节点
        /// </summary>
        private Transform mPathPointParentNode;

        private void Awake()
        {

        }

        /// <summary>
        /// 获取所有路点位置列表(新列表)
        /// </summary>
        /// <returns></returns>
        public List<Vector3> GetPathPointPosList()
        {
            var newPathPointList = new List<Vector3>();
            foreach(var pathPointData in PathPointDataList)
            {
                newPathPointList.Add(pathPointData.Position);
            }
            return newPathPointList;
        }

        /// <summary>
        /// 指定位置索引添加路点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool AddPathPointDataByIndex(int index)
        {
            var pathPointNum = PathPointDataList.Count;
            if (index < 0 || index > pathPointNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:{0}-{pathPointNum}，添加路点失败！");
                return false;
            }
            var newPathPointData = ConstructNewPathPointData(index);
            PathPointDataList.Insert(index, newPathPointData);
            return true;
        }

        /// <summary>
        /// 移除指定索引的路点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemovePathPointDataByIndex(int index)
        {
            var pathPointNum = PathPointDataList.Count;
            if (index < 0 || index >= pathPointNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:{0}-{pathPointNum - 1}，移除路点数据失败！");
                return false;
            }
            PathPointDataList.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 构建一个新的路点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TPathPointData ConstructNewPathPointData(int index)
        {
            // 路点初始位置根据路点索引是否有有效路点决定
            // 没有任何路点则用路点起始位置
            // 有有效路点则用当前构建位置的路点位置作为初始化位置
            var pathPointPosition = PathPointStartPos;
            var positionReferenceIndex = Mathf.Clamp(index, 0, PathPointDataList.Count - 1);
            var isExistReferencePathPoint = IsExistPathPointDataByIndex(positionReferenceIndex);
            if (isExistReferencePathPoint && PathPointDataList[positionReferenceIndex] != null)
            {
                pathPointPosition = PathPointDataList[positionReferenceIndex].Position;
            }
            var pathPoint = new TPathPointData(pathPointPosition);
            return pathPoint;
        }

        /// <summary>
        /// 获取指定路点索引的节点名
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetPathPointNameByIndex(int index)
        {
            return $"P({index})";
        }

        /// <summary>
        /// 是否有路点
        /// </summary>
        /// <returns></returns>
        private bool HasPathPoint()
        {
            return PathPointDataList.Count > 0;
        }

        /// <summary>
        /// 指定索引是否存在路点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsExistPathPointDataByIndex(int index)
        {
            return index >= 0 && index < PathPointDataList.Count;
        }

#if UNITY_EDITOR
        /// <summary>
        /// 场景Gizmos绘制
        /// </summary>
        private void OnDrawGizmos()
        {
            if(DrawSwitch)
            {
                DrawPathPointSpheres();
                DrawPathPointIcons();
            }
        }

        /// <summary>
        /// 绘制路点球体
        /// </summary>
        private void DrawPathPointSpheres()
        {
            var preGimozColor = Gizmos.color;
            Gizmos.color = PathPointSphereColor;
            for(int i = 0, length = PathPointDataList.Count; i < length; i++)
            {
                var pathPointData = PathPointDataList[i];
                if(pathPointData != null)
                {
                    Gizmos.DrawWireSphere(pathPointData.Position, PathPointSphereSize);
                }
            }
            Gizmos.color = preGimozColor;
        }

        /// <summary>
        /// 绘制路点图标
        /// </summary>
        private void DrawPathPointIcons()
        {
            for (int i = 0, length = PathPointDataList.Count; i < length; i++)
            {
                var pathPointData = PathPointDataList[i];
                if (pathPointData != null)
                {
                    var drawIcon = TPathUtilities.GetDrawIconByPathPointType(pathPointData.PPType);
                    Gizmos.DrawIcon(pathPointData.Position, drawIcon);
                }
            }
        }
#endif
    }
}