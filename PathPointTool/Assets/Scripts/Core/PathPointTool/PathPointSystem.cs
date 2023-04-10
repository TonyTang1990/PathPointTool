/*
 * Description:             PathPointSystem.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/09
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// PathPointSystem.cs
    /// 路点系统组件
    /// </summary>
    public class PathPointSystem : MonoBehaviour
    {
        /// <summary>
        /// 路点父节点名
        /// </summary>
        private const string PathPointParentNodeName = "Path";

        /// <summary>
        /// 路线类型
        /// </summary>
        [Header("路线类型")]
        public PathType Type = PathType.Normal;

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
        /// 路点对象列表
        /// </summary>
        [Header("路点对象列表")]
        public List<Transform> PathPointList = new List<Transform>();

        /// <summary>
        /// 路点父节点
        /// </summary>
        [Header("路点父节点")]
        public Transform PathPointParentNode;

        private void Awake()
        {
            InitPathPointParentNode();
            CheckPathPointParentNode();
        }

        private void Update()
        {
            CheckPathPointParentNode();
            CheckPathPointsNode();
        }

        /// <summary>
        /// 初始化路点父节点
        /// </summary>
        private void InitPathPointParentNode()
        {
            if (PathPointParentNode == null)
            {
                PathPointParentNode = new GameObject(PathPointParentNodeName).transform;
                PathPointParentNode.SetParent(transform);
                PathPointParentNode.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 路点父节点检查
        /// </summary>
        private void CheckPathPointParentNode()
        {
            if (PathPointParentNode == null)
            {
                InitPathPointParentNode();
            }
            if (PathPointParentNode != null && PathPointParentNode.parent != transform)
            {
                PathPointParentNode.SetParent(transform);
                PathPointParentNode.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 检查路点子节点
        /// </summary>
        private void CheckPathPointsNode()
        {
            for(int i = PathPointList.Count - 1; i  >= 0; i--)
            {
                if(PathPointList[i] == null)
                {
                    DeletePathPointByIndex(i);
                }
            }
        }

        /// <summary>
        /// 指定位置索引添加路点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool AddPathPointByIndex(int index)
        {
            if(index < 0 || index > PathPointList.Count)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:{0}-{PathPointList.Count}，添加路点失败！");
                return false;
            }
            var newPathPoint = ConstructNewPathPoint(index);
            PathPointList.Add(newPathPoint);
            UpdatePathPointNames();
            return true;
        }

        /// <summary>
        /// 移除指定索引的路点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool DeletePathPointByIndex(int index)
        {
            if (index < 0 || index >= PathPointList.Count)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:{0}-{PathPointList.Count - 1}，移除路点失败！");
                return false;
            }
            PathPointList.RemoveAt(index);
            UpdatePathPointNames();
            return true;
        }

        /// <summary>
        /// 更新路点节点名字
        /// </summary>
        private void UpdatePathPointNames()
        {
            for(int i = 0, length = PathPointList.Count; i < length; i++)
            {
                var pathPoint = PathPointList[i];
                if (pathPoint != null)
                {
                    pathPoint.name = GetPathPointNameByIndex(i);
                }
            }
        }

        /// <summary>
        /// 构建一个新的路点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private Transform ConstructNewPathPoint(int index)
        {
            // 路点初始位置根据路点索引是否有有效路点决定
            // 没有任何路点则用路点起始位置
            // 有有效路点则用当前构建位置的路点位置作为初始化位置
            var pathPoint = new GameObject(GetPathPointNameByIndex(index)).transform;
            var isExistPathPoint = IsExistPathPointByIndex(index);
            var pathPointPosition = PathPointStartPos;
            if(isExistPathPoint && PathPointList[index] != null)
            {
                pathPointPosition = PathPointList[index].position;
            }
            pathPoint.position = pathPointPosition;
            pathPoint.gameObject.AddComponent<PathPointData>();
            pathPoint.SetParent(PathPointParentNode);
            return pathPoint;
        }

        /// <summary>
        /// 获取指定路点索引的节点名
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private string GetPathPointNameByIndex(int index)
        {
            return $"PathPoint({index})";
        }

        /// <summary>
        /// 是否有路点
        /// </summary>
        /// <returns></returns>
        private bool HasPathPoint()
        {
            return PathPointList.Count > 0;
        }

        /// <summary>
        /// 指定索引是否存在路点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool IsExistPathPointByIndex(int index)
        {
            return index >= 0 && index < PathPointList.Count;
        }
    }
}