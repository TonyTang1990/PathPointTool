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
        /// 路线对象
        /// </summary>
        private TPath mPath;

        /// <summary>
        /// 经历时长
        /// </summary>
        private float mTimePassed;

        /// <summary>
        /// 构造函数
        /// </summary>
        public TPathTweener()
        {
            UID = TPathUtilities.GetNextPathTweenerUID();
            IsLoop = false;
            Duration = 0f;
            UpdateForward = false;
            mTimePassed = 0f;
            IsPaused = false;

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
            IsLoop = false;
            Duration = 0f;
            UpdateForward = false;
            mTimePassed = 0f;
            IsPaused = false;
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
        /// <param name="updateForward"></param>
        /// <param name="pathType"></param>
        /// <param name="segment"></param>
        public void InitByPoints(Transform target, IEnumerable<Vector3> points, float duration,
                                    TPathwayType pathwayType = TPathwayType.Line, bool isLoop = false,
                                        Action completeCB = null, bool updateForward = false, int segment = 10)
        {
            Target = target;
            mPath = ObjectPool.Singleton.pop<TPath>();
            mPath.InitByPoints(points, pathwayType, segment);
            Duration = duration;
            IsLoop = isLoop;
            mCompleteCB = completeCB;
            UpdateForward = updateForward;
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
                                        TPathwayType pathwayType = TPathwayType.Line, bool isLoop = false,
                                        bool updateForward = false, Action completeCB = null, int segment = 10)
        {
            Target = target;
            mPath = ObjectPool.Singleton.pop<TPath>();
            mPath.InitByTransforms(transforms, pathwayType, segment);
            Duration = duration;
            IsLoop = isLoop;
            mCompleteCB = completeCB;
            UpdateForward = updateForward;
        }

        /// <summary>
        /// 初始化指定顶点列表的路线
        /// </summary>
        /// <param name="path"></param>
        /// <param name="duration"></param>
        /// <param name="isLoop"></param>
        /// <param name="segment"></param>
        public void InitByPath(TPath pathbool, float duration, bool isLoop = false,
                               Action completeCB = null, bool updateForward = false)
        {
            mPath = pathbool;
            Duration = duration;
            IsLoop = isLoop;
            mCompleteCB = completeCB;
            UpdateForward = updateForward;
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
            mTimePassed += deltaTime;
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
            UpdateTargetByPercent(progress);
            if (!IsLoop && Mathf.Approximately(progress, 1))
            {
                OnPathTweenComplete();
            }
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
            var newForward = newPosition - oldPosition;
            Target.forward = newForward;
        }

        /// <summary>
        /// 路线缓动完成
        /// </summary>
        private void OnPathTweenComplete()
        {
            if(mCompleteCB != null)
            {
                mCompleteCB = null;
            }
            TPathTweenerManager.Singleton.RemovePathTween(this);
        }
    }
}