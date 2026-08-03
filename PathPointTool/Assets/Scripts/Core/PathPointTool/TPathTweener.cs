/*
 * Description:             TPathTweener.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/16
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TPathTweener.cs
    /// 路线缓动抽象
    /// </summary>
    public class TPathTweener : IRecycle
    {
        /// <summary>
        /// TPathTweener UID
        /// </summary>
        public int UID
        {
            get;
            private set;
        }

        /// <summary>
        /// 目标移动对象
        /// </summary>
        public Transform Target
        {
            get;
            private set;
        }

        /// <summary>
        /// 路线类型
        /// </summary>
        public TPathwayType PathwayType
        {
            get
            {
                return mPath.PathwayType;
            }
        }

        /// <summary>
        /// 是否循环
        /// </summary>
        public bool IsLoop
        {
            get;
            private set;
        }

        /// <summary>
        /// 持续时长
        /// </summary>
        public float Duration
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否更新朝向
        /// </summary>
        public bool UpdateForward
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否暂停
        /// </summary>
        public bool IsPaused
        {
            get;
            private set;
        }

        /// <summary>
        /// 移动完成回调
        /// </summary>
        private Action mCompleteCB;

        /// <summary>
        /// 路点经过回调
        /// </summary>
        private Action<int> mPointPassCB;

        /// <summary>
        /// 循环开始回调
        /// </summary>
        private Action mLoopStartCB;

        /// <summary>
        /// 路线对象
        /// </summary>
        private TPath mPath;

        /// <summary>
        /// 经历时长
        /// </summary>
        private float mTimePassed;

        /// <summary>
        /// 下一个尚未触发行为的路点索引
        /// </summary>
        private int mNextPathPointIndex;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public TPathTweener()
        {
            UID = TPathUtilities.GetNextPathTweenerUID();
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
        /// 重置
        /// </summary>
        private void Reset()
        {
            Target = null;
            if(mPath != null)
            {
                ObjectPool.Singleton.push<TPath>(mPath);
            }
            mPath = null;
            IsLoop = false;
            Duration = 0f;
            UpdateForward = false;
            mTimePassed = 0f;
            mNextPathPointIndex = 0;
            IsPaused = false;
            mCompleteCB = null;
        }

        /// <summary>
        /// 初始化指定顶点列表的路线
        /// </summary>
        /// <param name="target"></param>
        /// <param name="points"></param>
        /// <param name="duration"></param>
        /// <param name="pathwayType"></param>
        /// <param name="isLoop"></param>
        /// <param name="completeCB"></param>
        /// <param name="pointPassCB"></param>
        /// <param name="loopStartCB"></param>
        /// <param name="updateForward"></param>
        /// <param name="ease"></param>
        /// <param name="segment"></param>
        public void InitByPoints(Transform target, IEnumerable<Vector3> points, float duration,
                                    TPathwayType pathwayType = TPathwayType.Liner, bool isLoop = false,
                                    bool updateForward = true, Action completeCB = null,
                                    Action<int> pointPassCB = null, Action loopStartCB = null,
                                    EasingFunction.Ease ease = EasingFunction.Ease.Linear, int segment = 10)
        {
            Target = target;
            mPath = ObjectPool.Singleton.pop<TPath>();
            mPath.InitByPoints(points, pathwayType, ease, segment);
            Duration = duration;
            IsLoop = isLoop;
            UpdateForward = updateForward;
            mCompleteCB = completeCB;
            mPointPassCB = pointPassCB;
            mLoopStartCB = loopStartCB;
            mTimePassed = 0f;
            mNextPathPointIndex = 0;
        }

        /// <summary>
        /// 初始化指定对象列表的路线
        /// </summary>
        /// <param name="target"></param>
        /// <param name="transforms"></param>
        /// <param name="duration"></param>
        /// <param name="pathwayType"></param>
        /// <param name="isLoop"></param>
        /// <param name="updateForward"></param>
        /// <param name="completeCB"></param>
        /// <param name="segment"></param>
        public void InitByTransforms(Transform target, IEnumerable<Transform> transforms, float duration,
                                        TPathwayType pathwayType = TPathwayType.Liner, bool isLoop = false,
                                        bool updateForward = true, Action completeCB = null,
                                        Action<int> pointPassCB = null, Action loopStartCB = null,
                                        EasingFunction.Ease ease = EasingFunction.Ease.Linear, int segment = 10)
        {
            Target = target;
            mPath = ObjectPool.Singleton.pop<TPath>();
            mPath.InitByTransforms(transforms, pathwayType, ease, segment);
            Duration = duration;
            IsLoop = isLoop;
            UpdateForward = updateForward;
            mCompleteCB = completeCB;
            mPointPassCB = pointPassCB;
            mLoopStartCB = loopStartCB;
            mTimePassed = 0f;
            mNextPathPointIndex = 0;
        }

        /// <summary>
        /// 初始化指定顶点列表的路线
        /// </summary>
        /// <param name="path"></param>
        /// <param name="duration"></param>
        /// <param name="isLoop"></param>
        /// <param name="completeCB"></param>
        /// <param name="pointPassCB"></param>
        /// <param name="loopStartCB"></param>
        /// <param name="updateForward"></param>
        public void InitByPath(TPath pathbool, float duration, bool isLoop = false,
                               Action completeCB = null, Action<int> pointPassCB = null,
                               Action loopStartCB = null, bool updateForward = true)
        {
            mPath = pathbool;
            Duration = duration;
            IsLoop = isLoop;
            mCompleteCB = completeCB;
            mPointPassCB = pointPassCB;
            mLoopStartCB = loopStartCB;
            UpdateForward = updateForward;
            mTimePassed = 0f;
            mNextPathPointIndex = 0;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            IsPaused = true;
        }

        public void Resume()
        {
            IsPaused = false;
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="deltaTime"></param>
        public void Update(float deltaTime)
        {
            if(IsPaused)
            {
                return;
            }
            var preProgress = GetTimeProgress(mTimePassed);
            mTimePassed += deltaTime;
            var newProgress = GetTimeProgress(mTimePassed);
            // 先修改位置，后触发循环开始和路点经过相关事件
            UpdateTargetByPercent(newProgress);
            if(IsLoop && newProgress < preProgress)
            {
                // 触发循环
                OnPathLoopStart();
            }
            if (CheckPathPointReached(newProgress, out int reachedIndex))
            {
                OnPassPathPoint(reachedIndex);
            }
            if (!IsLoop && Mathf.Approximately(newProgress, 1))
            {
                OnPathTweenComplete();
            }
        }
        
        /// <summary>
        /// 重置路线进度
        /// </summary>
        /// <param name="progress">路线进度，取值范围0-1</param>
        public void ResetProgress(float progress)
        {
            mTimePassed = progress * Duration;
            UpdateTargetByPercent(progress);
            UpdateNextPathPointIndex(progress);
            CorrectForward(progress);
        }

        /// <summary>
        /// 更新下一个路点索引
        /// </summary>
        /// <param name="progress"></param>
        private void UpdateNextPathPointIndex(float progress)
        {
            if(mPath == null)
            {
                return;
            }
            mNextPathPointIndex = mPath.GetNextPointIndexByRatio(progress);
        }

        /// <summary>
        /// 获取指定经历时长的路线进度
        /// </summary>
        /// <param name="timePassed"></param>
        /// <returns></returns>
        private float GetTimeProgress(float timePassed)
        {
            var progress = 0f;
            if(Duration != 0)
            {
                progress = mTimePassed / Duration;
                progress = IsLoop ? (progress % 1) : Mathf.Clamp01(progress);
            }
            else
            {
                progress = 1;
            }
            return progress;
        }

        /// <summary>
        /// 检查路点抵达的情况
        /// </summary>
        /// <param name="newProgress"></param>
        /// <param name="reachedIndex"></param>
        /// <returns></returns>
        private bool CheckPathPointReached(float newProgress, out int reachedIndex)
        {
            reachedIndex = -1;
            if(mPath == null)
            {
                return false;
            }
            var pointNum = mPath.GetPointNum();
            if(pointNum <= 0)
            {
                return false;
            }
            for(int i = mNextPathPointIndex; i < pointNum; i++)
            {
                var pointRatio = mPath.GetPointRatioByIndex(i);
                if(newProgress >= pointRatio)
                {
                    reachedIndex = i;
                    return true;
                }
                else
                {
                    break;
                }
            }
            return false;
        }

        /// <summary>
        /// 更新指定路线比例的目标对象数据
        /// </summary>
        /// <param name="t"></param>
        private void UpdateTargetByPercent(float t)
        {
            if(Target == null)
            {
                return;
            }
            var oldPosition = Target.position;
            var newPosition = mPath.GetPointAt(t);
            Target.position = newPosition;
            if(UpdateForward)
            {
                if(!Vector3.Equals(newPosition, oldPosition))
                {
                    var newForward = newPosition - oldPosition;
                    Target.forward = newForward;
                }
            }
        }

        /// <summary>
        /// 校正朝向(用于重置位置时朝向的矫正)
        /// </summary>
        /// <param name="t"></param>
        private void CorrectForward(float t)
        {
            if(Target == null || !UpdateForward)
            {
                return;
            }
            t = Mathf.Clamp01(t);
            var offsetT = 0.001f;
            var newT = t;
            var oldPosition = Target.position;
            if(t >= 1)
            {
                var oldT = t - offsetT;
                oldPosition = mPath.GetPointAt(oldT);;
            }
            else
            {
                newT = t + offsetT;
            }
            var newPosition = mPath.GetPointAt(newT);
            if(UpdateForward)
            {
                if(!Vector3.Equals(newPosition, oldPosition))
                {
                    var newForward = newPosition - oldPosition;
                    Target.forward = newForward;
                }
            }
        }

        /// <summary>
        /// 触发路线循环开始
        /// </summary>
        private void OnPathLoopStart()
        {
            UpdateNextPathPointIndex(0f);
            mLoopStartCB?.Invoke();
        }

        /// <summary>
        /// 触发路线通过路点
        /// </summary>
        /// <param name="pathPointIndex"></param>
        private void OnPassPathPoint(int pathPointIndex)
        {
            var totalPointNum = mPath != null ? mPath.GetPointNum() : 0;
            mNextPathPointIndex = Mathf.Clamp(pathPointIndex + 1, 0, totalPointNum);
            mPointPassCB?.Invoke(pathPointIndex);
        }

        /// <summary>
        /// 路线缓动完成
        /// </summary>
        private void OnPathTweenComplete()
        {
            if(mCompleteCB != null)
            {
                mCompleteCB();
            }
            TPathTweenerManager.Singleton.RemovePathTween(this);
        }
    }
}