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
        /// 路点位置列表(传入构建路线的点列表)
        /// </summary>
        public List<Vector3> PathPointList
        {
            get;
            private set;
        }

        /// <summary>
        /// 计算路线需要的路点位置列表
        /// Note:
        /// 1. Cutmull-Rom Spline类型下和PathPointList不一致，因为需要头尾加路点确保头尾两个路点被经过
        /// </summary>
        public List<Vector3> CaculatePathPointList
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
        /// 每一段细分顶点数组Map<段数索引(从0开始), 顶点数组>
        /// Note:
        /// 此数据采取跟随分段数据更新而清除
        /// 访问获取指定分段索引数据时采取实时计算并缓存的方式
        /// </summary>
        private Dictionary<int, Vector3[]> mSegmentPointsMap;

        public TPath()
        {
            PathPointList = new List<Vector3>();
            SegmentList = new List<TSegment>();
            PathwayType = TPathwayType.Liner;
            Ease = EasingFunction.Ease.Linear;
            Segment = 15;
            Length = 0f;
            mPointDistanceList = new List<float>();
            mSegmentPointsMap = new Dictionary<int, Vector3[]>();
            CaculatePathPointList = new List<Vector3>();
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
        public void Reset()
        {
            PathPointList.Clear();
            RecyleAllSegments();
            PathwayType = TPathwayType.Liner;
            Ease = EasingFunction.Ease.Linear;
            Segment = 15;
            Length = 0f;
            mPointDistanceList.Clear();
            mSegmentPointsMap.Clear();
            CaculatePathPointList.Clear();
        }

        /// <summary>
        /// 初始化指定顶点列表的路线
        /// </summary>
        /// <param name="points"></param>
        /// <param name="pathwayType"></param>
        /// <param name="ease"></param>
        /// <param name="segment"></param>
        public void InitByPoints(IEnumerable<Vector3> points, TPathwayType pathwayType = TPathwayType.Liner,
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
        public void InitByTransforms(IEnumerable<Transform> transforms, TPathwayType pathwayType = TPathwayType.Liner,
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
        /// 更新路线类型
        /// </summary>
        /// <param name="pathwayType"></param>
        public void UpdatePathwayType(TPathwayType pathwayType = TPathwayType.Liner)
        {
            PathwayType = pathwayType;
            UpdatePathDatas();
        }

        /// <summary>
        /// 更新路线缓动类型
        /// </summary>
        /// <param name="pathwayType"></param>
        public void UpdateEaseType(EasingFunction.Ease ease = EasingFunction.Ease.Linear)
        {
            Ease = ease;
        }

        /// <summary>
        /// 更新路线每段细分顶点数量
        /// </summary>
        /// <param name="segment"></param>
        public void UpdateSetgmentNum(int segment = 15)
        {
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
                Debug.LogError($"添加的路点索引:{pointIndex}不在有效路点索引:0-{pointNum}范围内，添加指定索引路点失败！");
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
                Debug.LogError($"删除的路点索引:{pointIndex}不在有效路点索引:0-{pointNum - 1}范围内，删除指定索引路点失败！");
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
            UpdateCaculatePathPointDatas();
            UpdatePathLengthDatas();
            UpdateSegmentDatas();
        }

        /// <summary>
        /// 更新参与计算的路点数据
        /// </summary>
        private void UpdateCaculatePathPointDatas()
        {
            CaculatePathPointList.Clear();
            CaculatePathPointList.AddRange(PathPointList);
            if (PathwayType == TPathwayType.CRSpline)
            {
                // Cutmull-Rom Spline需要构建头尾额外一个点，确保路线经过首尾两个点
                var maxPointIndex = Mathf.Clamp(PathPointList.Count - 1, 0, Int32.MaxValue);
                // 只有一个点或0个点不成线段
                if(maxPointIndex <= 1)
                {
                    return;
                }
                // pMinusOne = 2 * p0 - p1
                var pMinusOne = 2 * PathPointList[0] - PathPointList[1];
                // pN = 2 * pNMinusOne - pNMinusTwo
                var pN = 2 * PathPointList[maxPointIndex] - PathPointList[maxPointIndex - 1];
                CaculatePathPointList.Insert(0, pMinusOne);
                CaculatePathPointList.Add(pN);
            }
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
            mSegmentPointsMap.Clear();
            var caculatePointNum = CaculatePathPointList.Count;
            if(caculatePointNum == 0)
            {
                return;
            }
            else if(caculatePointNum == 1)
            {
                var segment = ObjectPool.Singleton.pop<TSegment>();
                segment.Init(0, 0, 1, 1, PathwayType);
                SegmentList.Add(segment);
                return;
            }
            var segmentPointNum = TPathUtilities.GetSegmentPointNumByType(PathwayType);
            var segmentStepNum = TPathUtilities.GetSegmentStepNumByType(PathwayType);
            var pointStep = Mathf.Clamp(segmentStepNum, 1, Int32.MaxValue);
            var segmentLength = 0f;
            var maxPointNum = Mathf.Clamp(caculatePointNum - 1, 0, Int32.MaxValue);
            var distanceAccumulation = 0f;
            var segmentNum = GetExpectedSegmentNum();
            for (int i = 0; i < segmentNum; i++)
            {
                var pointStartIndex = i * pointStep;
                segmentLength = 0f;
                var firstPointPathRatio = distanceAccumulation / Length;
                for (int j = pointStartIndex, length2 = pointStartIndex + pointStep; j < length2; j++)
                {
                    var pointDistanceIndex = Mathf.Clamp(j, 0, maxPointNum);
                    var pointDistance = GetPointDistanceByIndex(pointDistanceIndex);
                    segmentLength += pointDistance;
                    distanceAccumulation += pointDistance;
                }
                var lastPointPathRatio = distanceAccumulation / Length;
                var segment = ObjectPool.Singleton.pop<TSegment>();
                segment.Init(pointStartIndex, segmentLength, firstPointPathRatio, lastPointPathRatio, PathwayType);
                SegmentList.Add(segment);
            }
        }

        /// <summary>
        /// 获取当前分段数量
        /// </summary>
        /// <returns></returns>
        public int GetSegmentNum()
        {
            return SegmentList.Count;
        }

        /// <summary>
        /// 获取预期的分段数量
        /// </summary>
        /// <returns></returns>
        private int GetExpectedSegmentNum()
        {
            var caculatePointNum = CaculatePathPointList.Count;
            if (caculatePointNum == 0)
            {
                return 0;
            }
            else if (caculatePointNum == 1)
            {
                return 1;
            }
            var segmentPointNum = TPathUtilities.GetSegmentPointNumByType(PathwayType);
            var segmentStepNum = TPathUtilities.GetSegmentStepNumByType(PathwayType);
            var segmentNum = Mathf.CeilToInt((caculatePointNum - segmentPointNum + 1) * 1.0f / segmentStepNum);
            // 多出来的点Bezier曲线需要额外构成一个
            if(PathwayType == TPathwayType.Bezier || PathwayType == TPathwayType.CubicBezier)
            {
                var leftNum = (caculatePointNum - 1) % segmentStepNum;
                if(leftNum != 0)
                {
                    segmentNum++;
                }
            }
            return segmentNum;
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
            var pointNum = PathPointList.Count;
            if(pointNum == 0)
            {
                Debug.LogError($"没有有效路点数据，获取不到路线指定比例位置！");
                return Vector3.zero;
            }
            else if(pointNum == 1)
            {
                return PathPointList[0];
            }
            if(PathwayType == TPathwayType.Liner)
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
            else if(PathwayType == TPathwayType.CRSpline)
            {
                return GetCRSplinePointAt(t);
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
                var maxPointIndex = CaculatePathPointList.Count - 1;
                var firstPointIndex = SegmentList[segmentIndex].StartPointIndex;
                var secondPointIndex = Mathf.Clamp(firstPointIndex + 1, 0, maxPointIndex);
                if(PathwayType == TPathwayType.Liner)
                {
                    subPoints = BezierUtilities.GetLinerList(CaculatePathPointList[firstPointIndex], CaculatePathPointList[secondPointIndex], Segment);
                }
                else if(PathwayType == TPathwayType.Bezier)
                {
                    var thirdPointIndex = Mathf.Clamp(firstPointIndex + 2, 0, maxPointIndex);
                    subPoints = BezierUtilities.GetBeizerList(CaculatePathPointList[firstPointIndex], CaculatePathPointList[secondPointIndex],
                                                               CaculatePathPointList[thirdPointIndex], Segment);
                }
                else if(PathwayType == TPathwayType.CubicBezier)
                {
                    var thirdPointIndex = Mathf.Clamp(firstPointIndex + 2, 0, maxPointIndex);
                    var fourthPointIndex = Mathf.Clamp(firstPointIndex + 3, 0, maxPointIndex);
                    subPoints = BezierUtilities.GetCubicBeizerList(CaculatePathPointList[firstPointIndex], CaculatePathPointList[secondPointIndex],
                                                                    CaculatePathPointList[thirdPointIndex], CaculatePathPointList[fourthPointIndex], Segment);
                }
                else if(PathwayType == TPathwayType.CRSpline)
                {
                    var thirdPointIndex = Mathf.Clamp(firstPointIndex + 2, 0, maxPointIndex);
                    var fourthPointIndex = Mathf.Clamp(firstPointIndex + 3, 0, maxPointIndex);
                    subPoints = BezierUtilities.GetCRSplineList(CaculatePathPointList[firstPointIndex], CaculatePathPointList[secondPointIndex],
                                                                    CaculatePathPointList[thirdPointIndex], CaculatePathPointList[fourthPointIndex], Segment);
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
        /// 获取线性路线指定比例路点位置
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 GetLinerPoinAt(float t)
        {
            var pointNum = CaculatePathPointList.Count;
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
            return BezierUtilities.CaculateLinerPoint(CaculatePathPointList[firstPointIndex], CaculatePathPointList[secondPointIndex], currentUnderSegmentPercent);
        }

        /// <summary>
        /// 获取二阶贝塞尔路线指定比例路点位置
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 GetBezierPointAt(float t)
        {
            var pointNum = CaculatePathPointList.Count;
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
            return BezierUtilities.CaculateBezierPoint(CaculatePathPointList[firstPointIndex],
                                                       CaculatePathPointList[secondPointIndex],
                                                       CaculatePathPointList[thirdPointIndex],
                                                       currentUnderSegmentPercent);
        }

        /// <summary>
        /// 获取三阶贝塞尔路线指定比例路点位置
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 GetCubicBezierPointAt(float t)
        {
            var pointNum = CaculatePathPointList.Count;
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
            return BezierUtilities.CaculateCubicBezierPoint(CaculatePathPointList[firstPointIndex],
                                                            CaculatePathPointList[secondPointIndex],
                                                            CaculatePathPointList[thirdPointIndex],
                                                            CaculatePathPointList[fourthPointIndex],
                                                            currentUnderSegmentPercent);
        }

        /// <summary>
        /// 获取Cutmull-Roll Spline路线指定比例路点位置
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private Vector3 GetCRSplinePointAt(float t)
        {
            var pointNum = CaculatePathPointList.Count;
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
            return BezierUtilities.CaculateCRSplinePoint(CaculatePathPointList[firstPointIndex],
                                                            CaculatePathPointList[secondPointIndex],
                                                            CaculatePathPointList[thirdPointIndex],
                                                            CaculatePathPointList[fourthPointIndex],
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
            for (int i = 0, length = SegmentList.Count; i < length; i++)
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