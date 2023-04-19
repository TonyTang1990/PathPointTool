/*
 * Description:             TPath.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/12
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TPath.cs
    /// 路线抽象
    /// </summary>
    public class TPath : IRecycle
    {
        /// <summary>
        /// 路线类型
        /// </summary>
        public TPathwayType PathwayType
        {
            get;
            private set;
        }

        /// <summary>
        /// 缓动类型
        /// </summary>
        public EasingFunction.Ease Ease
        {
            get;
            private set;
        }

        /// <summary>
        /// 每段细分数量
        /// </summary>
        public int Segment
        {
            get;
            private set;
        }

        /// <summary>
        /// 路点位置列表
        /// </summary>
        public List<Vector3> PathPointList
        {
            get;
            private set;
        }

        /// <summary>
        /// 分段数据列表
        /// </summary>
        public List<TSegment> SegmentList
        {
            get;
            private set;
        }

        /// <summary>
        /// 路线总长度
        /// </summary>
        public float Length
        {
            get;
            private set;
        }

        /// <summary>
        /// 每两个路点间的距离列表(数量 = 顶点数量)
        /// Note:
        /// 最后一个路点距离下一个路点为自身距离0
        /// </summary>
        private List<float> mPointDistanceList;

        /// <summary>
        /// 每一段细分顶点数组Map<段数索引(从1开始), 顶点数组>
        /// Note:
        /// 此数据采取跟随分段数据更新而清除
        /// 访问获取指定分段索引数据时采取实时计算并缓存的方式
        /// </summary>
        private Dictionary<int, Vector3[]> mSegmentPointsMap;

        public TPath()
        {
            PathPointList = new List<Vector3>();
            SegmentList = new List<TSegment>();
            PathwayType = TPathwayType.Line;
            Ease = EasingFunction.Ease.Linear;
            Segment = 15;
            Length = 0f;
            mPointDistanceList = new List<float>();
            mSegmentPointsMap = new Dictionary<int, Vector3[]>();
        }

        public void OnCreate()
        {
            Reset();
        }

        public void OnDispose()
        {
            Reset();
        }

        /// <summary>
        /// 重置数据
        /// </summary>
        private void Reset()
        {
            PathPointList.Clear();
            RecyleAllSegments();
            PathwayType = TPathwayType.Line;
            Ease = EasingFunction.Ease.Linear;
            Segment = 15;
            Length = 0f;
            mPointDistanceList.Clear();
            mSegmentPointsMap.Clear();
        }

        /// <summary>
        /// 初始化指定顶点列表的路线
        /// </summary>
        /// <param name="points"></param>
        /// <param name="pathwayType"></param>
        /// <param name="ease"></param>
        /// <param name="segment"></param>
        public void InitByPoints(IEnumerable<Vector3> points, TPathwayType pathwayType = TPathwayType.Line,
                                    EasingFunction.Ease ease = EasingFunction.Ease.Linear, int segment = 10)
        {
            PathPointList.Clear();
            PathPointList.AddRange(points);
            PathwayType = pathwayType;
            Ease = ease;
            Segment = segment;
            UpdatePathDatas();
        }

        /// <summary>
        /// 初始化指定对象列表的路线
        /// </summary>
        /// <param name="transforms"></param>
        /// <param name="pathwayType"></param>
        /// <param name="ease"></param>
        /// <param name="segment"></param>
        public void InitByTransforms(IEnumerable<Transform> transforms, TPathwayType pathwayType = TPathwayType.Line,
                                        EasingFunction.Ease ease = EasingFunction.Ease.Linear, int segment = 10)
        {
            PathPointList.Clear();
            var transformIndex = -1;
            foreach(var transform in transforms)
            {
                transformIndex++;
                if (transform == null)
                {
                    Debug.LogWarning($"TPath:InitByTransforms()传入的对象列表里有空对象，跳过索引:{transformIndex}的对象！");
                    continue;
                }
                PathPointList.Add(transform.position);
            }
            PathwayType = pathwayType;
            Ease = ease;
            Segment = segment;
            UpdatePathDatas();
        }

        /// <summary>
        /// 添加路点到尾部
        /// </summary>
        /// <param name="point">路点位置</param>
        /// <param name="updatePathDatas">更新路线相关数据</param>
        /// <returns></returns>
        public bool AddPoint(Vector3 point, bool updatePathDatas = true)
        {
            PathPointList.Add(point);
            if(updatePathDatas)
            {
                UpdatePathDatas();
            }
            return true;
        }

        /// <summary>
        /// 指定索引位置添加路点
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <param name="updatePathDatas">更新路线相关数据</param>
        /// <returns></returns>
        public bool AddPointByIndex(Vector3 point, int pointIndex, bool updatePathDatas = true)
        {
            var pointNum = PathPointList.Count;
            if (pointIndex < 0 || pointIndex > pointNum)
            {
                Debug.LogError($"添加的路点索引:{pointIndex}不在有效路点索引:{0}-{pointNum}范围内，添加指定索引路点失败！");
                return false;
            }
            PathPointList.Insert(pointIndex, point);
            if (updatePathDatas)
            {
                UpdatePathDatas();
            }
            return true;
        }

        /// <summary>
        /// 移除指定索引路点
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <param name="updatePathDatas">更新路线相关数据</param>
        /// <returns></returns>
        public bool RemovePointByIndex(int pointIndex, bool updatePathDatas = true)
        {
            var pointNum = PathPointList.Count;
            if(pointIndex < 0 || pointIndex >= pointNum)
            {
                Debug.LogError($"删除的路点索引:{pointIndex}不在有效路点索引:{0}-{pointNum - 1}范围内，删除指定索引路点失败！");
                return false;
            }
            PathPointList.RemoveAt(pointIndex);
            if (updatePathDatas)
            {
                UpdatePathDatas();
            }
            return true;
        }

        /// <summary>
        /// 获取指定比例t(0-1)所在分段索引(从0开始)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int GetSegmentIndexByRatio(float t)
        {
            var segmentNum = SegmentList.Count;
            if(segmentNum == 0)
            {
                Debug.Log($"分段数量为0，获取不到指定比例t的所在分段索引！");
                return -1;
            }
            t = Mathf.Clamp01(t);
            for(int i = 0; i < segmentNum; i++)
            {
                if(t >= SegmentList[i].FirstPointPathRatio && t <= SegmentList[i].LastPointPathRatio)
                {
                    return i;
                }
            }
            Debug.LogError($"找不到比例:{t}对应的分段索引，理论上不可能！");
            return -1;
        }

        /// <summary>
        /// 获取指定顶点索引到下一个顶点索引的距离
        /// </summary>
        /// <param name="pointIndex"></param>
        /// <returns></returns>
        public float GetPointDistanceByIndex(int pointIndex)
        {
            var pointDistanceNum = mPointDistanceList.Count;
            if (pointIndex < 0 || pointIndex >= pointDistanceNum)
            {
                Debug.LogError($"获取顶点索引:{pointIndex}到下一个顶点的距离范围不在有效索引范围内:{0}-{pointDistanceNum - 1}，获取指定索引顶点到下一个顶点距离失败！");
                return 0f;
            }
            return mPointDistanceList[pointIndex];
        }

        /// <summary>
        /// 更新路线相关数据
        /// </summary>
        public void UpdatePathDatas()
        {
            UpdatePathLengthDatas();
            UpdateSegmentDatas();
        }

        /// <summary>
        /// 更新路线长度数据
        /// </summary>
        private void UpdatePathLengthDatas()
        {
            mPointDistanceList.Clear();
            var pointNum = PathPointList.Count;
            var maxPointIndex = pointNum - 1;
            for (int i = 0, length = pointNum; i < length; i++)
            {
                var firstPointIndex = Mathf.Clamp(i, 0, maxPointIndex);
                var secondPointIndex = Mathf.Clamp(i + 1, 0, maxPointIndex);
                var firstPoint = PathPointList[firstPointIndex];
                var secondPoint = PathPointList[secondPointIndex];
                var pointDistance = Vector3.Distance(firstPoint, secondPoint);
                mPointDistanceList.Add(pointDistance);
                Length += pointDistance;
            }
        }

        /// <summary>
        /// 更新分段数据
        /// </summary>
        private void UpdateSegmentDatas()
        {
            RecyleAllSegments();
            var pointNum = PathPointList.Count;
            if(pointNum == 0)
            {
                return;
            }
            else if(pointNum == 1)
            {
                var segment = ObjectPool.Singleton.pop<TSegment>();
                segment.Init(0, 0, 1, 1, PathwayType);
                SegmentList.Add(segment);
                return;
            }
            var segmentPointNum = TPathUtilities.GetSegmentPointNumByType(PathwayType);
            var pointStep = Mathf.Clamp(segmentPointNum - 1, 0, Int32.MaxValue);
            var segmentLength = 0f;
            var maxPointNum = Mathf.Clamp(PathPointList.Count - 1, 0, Int32.MaxValue);
            var distanceAccumulation = 0f;
            for (int i = 0, length = PathPointList.Count; i < length; i+= pointStep)
            {
                segmentLength = 0f;
                var firstPointPathRatio = distanceAccumulation / Length;
                for (int j = i, length2 = i + pointStep; j < length2; j++)
                {
                    var pointDistanceIndex = Mathf.Clamp(j, 0, maxPointNum);
                    var pointDistance = GetPointDistanceByIndex(pointDistanceIndex);
                    segmentLength += pointDistance;
                    distanceAccumulation += pointDistance;
                }
                var lastPointPathRatio = distanceAccumulation / Length;
                var segment = ObjectPool.Singleton.pop<TSegment>();
                segment.Init(i, segmentLength, firstPointPathRatio, lastPointPathRatio, PathwayType);
                SegmentList.Add(segment);
            }
            mSegmentPointsMap.Clear();
        }

        /// <summary>
        /// 回收所有分段数据
        /// </summary>
        private void RecyleAllSegments()
        {
            for (int i = 0, length = SegmentList.Count; i < length; i++)
            {
                ObjectPool.Singleton.push<TSegment>(SegmentList[i]);
            }
            SegmentList.Clear();
        }

        /// <summary>
        /// 获取指定比例的路点位置
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector3 GetPointAt(float t)
        {
            if(PathwayType == TPathwayType.Line)
            {
                return GetLinerPoinAt(t);
            }
            else if(PathwayType == TPathwayType.Bezier)
            {
                return GetBezierPointAt(t);
            }
            else if(PathwayType == TPathwayType.CubicBezier)
            {
                return GetCubicBezierPointAt(t);
            }
            else
            {
                Debug.LogError($"不支持的路线类型:{PathwayType.ToString()}，获取指定比例路点位置失败！");
                return Vector3.zero;
            }
        }

        /// <summary>
        /// 获取指定分段索引(从0开始)的细分顶点数组
        /// </summary>
        /// <param name="segmentIndex">分段索引(从0开始)</param>
        /// <returns></returns>
        public Vector3[] GetSubPointsBySegmentIndex(int segmentIndex)
        {
            var segmentNum = SegmentList.Count;
            if(segmentNum == 0)
            {
                Debug.LogError($"没有有效分段数据，获取指定分段索引:{segmentIndex}的细分顶点数组失败！");
                return null;
            }
            if (segmentIndex < 0 || segmentIndex >= segmentNum)
            {
                Debug.LogError($"分段索引:{segmentIndex}不在有效范围内:0-{segmentNum - 1}，获取指定分段索引的细分顶点数组失败！");
                return null;
            }
            Vector3[] subPoints;
            if(!mSegmentPointsMap.TryGetValue(segmentIndex, out subPoints))
            {
                var maxPointIndex = PathPointList.Count - 1;
                var firstPointIndex = SegmentList[segmentIndex].StartPointIndex;
                var secondPointIndex = Mathf.Clamp(firstPointIndex + 1, 0, maxPointIndex);
                if(PathwayType == TPathwayType.Line)
                {
                    // 直线没必要细分过多的点，这里强制直线每段细分数为1
                    subPoints = BezierUtilities.GetLinerList(PathPointList[firstPointIndex], PathPointList[secondPointIndex], 1);
                }
                else if(PathwayType == TPathwayType.Bezier)
                {
                    var thirdPointIndex = Mathf.Clamp(firstPointIndex + 2, 0, maxPointIndex);
                    subPoints = BezierUtilities.GetBeizerList(PathPointList[firstPointIndex], PathPointList[secondPointIndex],
                                                               PathPointList[thirdPointIndex], Segment);
                }
                else if(PathwayType == TPathwayType.CubicBezier)
                {
                    var thirdPointIndex = Mathf.Clamp(firstPointIndex + 2, 0, maxPointIndex);
                    var fourthPointIndex = Mathf.Clamp(firstPointIndex + 3, 0, maxPointIndex);
                    subPoints = BezierUtilities.GetCubicBeizerList(PathPointList[firstPointIndex], PathPointList[secondPointIndex],
                                                                    PathPointList[thirdPointIndex], PathPointList[fourthPointIndex], Segment);
                }
                else
                {
                    Debug.LogError($"不支持的路线类型:{PathwayType.ToString()},获取指定分段索引的细分顶点数组失败！");
                }
                if (subPoints != null)
                {
                    mSegmentPointsMap.Add(segmentIndex, subPoints);
                }
            }
            return subPoints;
        }

        /// <summary>
        /// 获取指定分段索引的细分顶点距离(目前用于获取计算总的LineRenderer绘制长度)
        /// </summary>
        /// <param name="segmentIndex"></param>
        /// <returns></returns>
        public float GetSegmentSubPointLengthByIndex(int segmentIndex)
        {
            var subPoints = GetSubPointsBySegmentIndex(segmentIndex);
            if(subPoints == null)
            {
                return 0f;
            }
            var subPointCount = subPoints.Length;
            if(subPointCount == 1)
            {
                return 0f;
            }
            var subPointLength = 0f;
            for(int i = 0, length = subPointCount - 1; i < length; i++)
            {
                subPointLength += Vector3.Distance(subPoints[i], subPoints[i + 1]);
            }
            return subPointLength;
        }

        /// <summary>
        /// 获取所有分段细分顶点的总长度
        /// </summary>
        /// <returns></returns>
        public float GetTotalSegmentSubPointLength()
        {
            var segmentNum = SegmentList.Count;
            if(segmentNum == 0)
            {
                return 0f;
            }
            var totalLength = 0f;
            for(int i = 0; i < segmentNum; i++)
            {
                var segmentSubPointLength = GetSegmentSubPointLengthByIndex(i);
                totalLength += segmentSubPointLength;
            }
            return totalLength;
        }

        /// <summary>
        /// 检查分段数据有效性
        /// </summary>
        /// <returns></returns>
        private bool CheckSegmentNumValidation()
        {
            // 路点数量为0，分段数量为0
            // 路点数量为1，分段数量为1
            // 路点数量>1，分段数量 = Mathf.CellToInt((路点数量 - 1) /  (N(Bezier曲线的顶点数) - 1))
            var pointNum = PathPointList.Count;
            var segmentNum = SegmentList.Count;
            if (pointNum == 0)
            {
                return segmentNum == 0;
            }
            var segmentPointNum = TPathUtilities.GetSegmentPointNumByType(PathwayType);
            if (pointNum <= segmentPointNum)
            {
                return segmentNum == 1;
            }
            var segmentExpectedNum = Mathf.CeilToInt((pointNum - 1) / (float)(segmentPointNum - 1));
            var result = segmentNum == segmentExpectedNum;
            if (!result)
            {
                Debug.LogWarning($"路径分段数量:{segmentNum}和顶点数量:{pointNum}对不上！");
            }
            return result;
        }

        /// <summary>
        /// 获取线性路线指定比例路点位置
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 GetLinerPoinAt(float t)
        {
            var pointNum = PathPointList.Count;
            var maxPointIndex = pointNum - 1;
            TSegment currentUnderSegment;
            float currentUnderSegmentPercent;
            GetRadioSegmentAndPercent(t, out currentUnderSegment, out currentUnderSegmentPercent);
            if(currentUnderSegment == null)
            {
                return Vector3.zero;
            }
            var firstPointIndex = currentUnderSegment.StartPointIndex;
            var secondPointIndex = Mathf.Clamp(currentUnderSegment.StartPointIndex + 1, 0, maxPointIndex);
            return BezierUtilities.CaculateLinerPoint(PathPointList[firstPointIndex], PathPointList[secondPointIndex], currentUnderSegmentPercent);
        }

        /// <summary>
        /// 获取二阶贝塞尔路线指定比例路点位置
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 GetBezierPointAt(float t)
        {
            var pointNum = PathPointList.Count;
            var maxPointIndex = pointNum - 1;
            TSegment currentUnderSegment;
            float currentUnderSegmentPercent;
            GetRadioSegmentAndPercent(t, out currentUnderSegment, out currentUnderSegmentPercent);
            if (currentUnderSegment == null)
            {
                return Vector3.zero;
            }
            var firstPointIndex = currentUnderSegment.StartPointIndex;
            var secondPointIndex = Mathf.Clamp(currentUnderSegment.StartPointIndex + 1, 0, maxPointIndex);
            var thirdPointIndex = Mathf.Clamp(currentUnderSegment.StartPointIndex + 2, 0, maxPointIndex);
            return BezierUtilities.CaculateBezierPoint(PathPointList[firstPointIndex], 
                                                       PathPointList[secondPointIndex],
                                                       PathPointList[thirdPointIndex],
                                                       currentUnderSegmentPercent);
        }

        /// <summary>
        /// 获取三阶贝塞尔路线指定比例路点位置
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 GetCubicBezierPointAt(float t)
        {
            var pointNum = PathPointList.Count;
            var maxPointIndex = pointNum - 1;
            TSegment currentUnderSegment;
            float currentUnderSegmentPercent;
            GetRadioSegmentAndPercent(t, out currentUnderSegment, out currentUnderSegmentPercent);
            if (currentUnderSegment == null)
            {
                return Vector3.zero;
            }
            var firstPointIndex = currentUnderSegment.StartPointIndex;
            var secondPointIndex = Mathf.Clamp(currentUnderSegment.StartPointIndex + 1, 0, maxPointIndex);
            var thirdPointIndex = Mathf.Clamp(currentUnderSegment.StartPointIndex + 2, 0, maxPointIndex);
            var fourthPointIndex = Mathf.Clamp(currentUnderSegment.StartPointIndex + 3, 0, maxPointIndex);
            return BezierUtilities.CaculateCubicBezierPoint(PathPointList[firstPointIndex],
                                                            PathPointList[secondPointIndex],
                                                            PathPointList[thirdPointIndex],
                                                            PathPointList[fourthPointIndex],
                                                            currentUnderSegmentPercent);
        }

        /// <summary>
        /// 获取指定比例路点所处分段和分段所占比例
        /// </summary>
        /// <param name="t"></param>
        /// <param name="segment"></param>
        /// <param name="segmentPercent"></param>
        private void GetRadioSegmentAndPercent(float t, out TSegment segment, out float segmentPercent)
        {
            var pointNum = PathPointList.Count;
            if (pointNum == 0)
            {
                segment = null;
                segmentPercent = 0f;
                return;
            }
            else if (pointNum == 1)
            {
                segment = SegmentList[0];
                segmentPercent = 1f;
                return;
            }
            t = Mathf.Clamp01(t);
            // 缓动公式对路线插值的影响
            var easeFunc = EasingFunction.GetEasingFunction(Ease);
            t = easeFunc(0, 1, t);
            var distance = t * Length;
            segment = SegmentList[0];
            for (int i = 0, length = SegmentList.Count - 1; i < length; i++)
            {
                distance -= SegmentList[i].Length;
                segment = SegmentList[i];
                if (distance < 0)
                {
                    break;
                }
            }
            segmentPercent = (segment.Length + distance) / segment.Length;
        }
    }
}