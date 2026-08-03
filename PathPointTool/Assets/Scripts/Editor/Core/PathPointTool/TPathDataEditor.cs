/*
 * Description:             TPathDataEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/09
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace PathPoint
{
    /// <summary>
    /// TPathDataEditor.cs
    /// 路点系统组件Editor
    /// </summary>
    [CustomEditor(typeof(TPathData))]
    public class TPathDataEditor : Editor
    {
        /// <summary>
        /// 目标组件
        /// </summary>
        private TPathData mTarget;

        /// <summary>
        /// ExportFileName属性
        /// </summary>
        private SerializedProperty mExportFileNameProperty;

        /// <summary>
        /// ExportType属性
        /// </summary>
        private SerializedProperty mExportTypeProperty;

        /// <summary>
        /// DrawSwitch属性
        /// </summary>
        private SerializedProperty mDrawSwitchProperty;

        /// <summary>
        /// PathType属性
        /// </summary>
        private SerializedProperty mPathTypeProperty;

        /// <summary>
        /// PathwayType属性
        /// </summary>
        private SerializedProperty mPathwayTypeProperty;

        /// <summary>
        /// Ease属性
        /// </summary>
        private SerializedProperty mEaseProperty;

        /// <summary>
        /// IsLoop属性
        /// </summary>
        private SerializedProperty mIsLoopProperty;

        /// <summary>
        /// UpdateForward属性
        /// </summary>
        private SerializedProperty mUpdateForwardProperty;

        /// <summary>
        /// Duration属性
        /// </summary>
        private SerializedProperty mDurationProperty;

        /// <summary>
        /// PathPointStartPos属性
        /// </summary>
        private SerializedProperty mPathPointStartPosProperty;

        /// <summary>
        /// PathPointGap属性
        /// </summary>
        private SerializedProperty mPathPointGapProperty;

        /// <summary>
        /// Segment属性
        /// </summary>
        private SerializedProperty mSegmentProperty;

        /// <summary>
        /// PathPointSphereSize属性
        /// </summary>
        private SerializedProperty mPathPointSphereSizeProperty;

        /// <summary>
        /// DrawLineRenderer属性
        /// </summary>
        private SerializedProperty mDrawLineRendererProperty;

        /// <summary>
        /// PathPointSphereColor属性
        /// </summary>
        private SerializedProperty mPathPointSphereColorProperty;

        /// <summary>
        /// PathDrawColor属性
        /// </summary>
        private SerializedProperty mPathDrawColorProperty;

        /// <summary>
        /// SubPathPointDrawColor属性
        /// </summary>
        private SerializedProperty mSubPathPointDrawColorProperty;

        /// <summary>
        /// PathPointDataList属性
        /// </summary>
        private SerializedProperty mPathPointDataListProperty;

        /// <summary>
        /// SimulationMoveGO属性
        /// </summary>
        private SerializedProperty mSimulationMoveGOProperty;

        /// <summary>
        /// 路径相关属性变化
        /// </summary>
        private bool mPathPointRelativePropertyChange;

        /// <summary>
        /// 绘制路点所需的路线对象
        /// </summary>
        private TPath mDrawPath = new TPath();

        /// <summary>
        /// 绘制的路点列表
        /// </summary>
        private List<Vector3> mDrawPathPointList = new List<Vector3>();

        /// <summary>
        /// Lable显示GUIStyle
        /// </summary>
        private GUIStyle mLabelGUIStyle;

        /// <summary>
        /// 路点数据是否展开显示
        /// </summary>
        private bool mPathPointFoldOut = true;

        /// <summary>
        /// 模拟路线缓动Tweener
        /// </summary>
        private TPathTweener mSimulationPathTweener;

        private void OnEnable()
        {
            InitTarget();
            InitProperties();
            InitGUIStyles();
            UpdateDrawDatas();
        }

        /// <summary>
        /// 初始化目标组件
        /// </summary>
        private void InitTarget()
        {
            mTarget ??= (target as TPathData);
        }

        /// <summary>
        /// 初始化属性
        /// </summary>
        private void InitProperties()
        {
            mExportFileNameProperty ??= serializedObject.FindProperty("ExportFileName");
            mExportTypeProperty ??= serializedObject.FindProperty("ExportType");
            mDrawSwitchProperty ??= serializedObject.FindProperty("DrawSwitch");
            mPathTypeProperty ??= serializedObject.FindProperty("PathType");
            mPathwayTypeProperty ??= serializedObject.FindProperty("PathwayType");
            mEaseProperty ??= serializedObject.FindProperty("Ease");
            mIsLoopProperty ??= serializedObject.FindProperty("IsLoop");
            mUpdateForwardProperty ??= serializedObject.FindProperty("UpdateForward");
            mDurationProperty ??= serializedObject.FindProperty("Duration");
            mPathPointStartPosProperty ??= serializedObject.FindProperty("PathPointStartPos");
            mPathPointGapProperty ??= serializedObject.FindProperty("PathPointGap");
            mSegmentProperty ??= serializedObject.FindProperty("Segment");
            mPathPointSphereSizeProperty ??= serializedObject.FindProperty("PathPointSphereSize");
            mDrawLineRendererProperty ??= serializedObject.FindProperty("DrawLineRenderer");
            mPathPointSphereColorProperty ??= serializedObject.FindProperty("PathPointSphereColor");
            mPathDrawColorProperty ??= serializedObject.FindProperty("PathDrawColor");
            mSubPathPointDrawColorProperty ??= serializedObject.FindProperty("SubPathPointDrawColor");
            mPathPointDataListProperty ??= serializedObject.FindProperty("PathPointDataList");
            mSimulationMoveGOProperty ??= serializedObject.FindProperty("SimulationMoveGO");
        }

        /// <summary>
        /// 初始化GUIStyles
        /// </summary>
        private void InitGUIStyles()
        {
            if(mLabelGUIStyle == null)
            {
                mLabelGUIStyle = new GUIStyle();
                mLabelGUIStyle.fontSize = 15;
                mLabelGUIStyle.alignment = TextAnchor.MiddleCenter;
                mLabelGUIStyle.normal.textColor = Color.red;
            }
        }

        /// <summary>
        /// Inspector自定义显示
        /// </summary>
        public override void OnInspectorGUI()
        {
            InitTarget();
            InitProperties();
            InitGUIStyles();

            // 确保对SerializaedObject和SerializedProperty的数据修改每帧同步
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(mExportFileNameProperty);
            EditorGUILayout.PropertyField(mExportTypeProperty);
            EditorGUILayout.PropertyField(mDrawSwitchProperty);
            EditorGUILayout.PropertyField(mPathTypeProperty);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mPathwayTypeProperty);
            if (EditorGUI.EndChangeCheck())
            {
                mPathPointRelativePropertyChange = true;
            }

            EditorGUILayout.PropertyField(mEaseProperty);
            EditorGUILayout.PropertyField(mIsLoopProperty);
            EditorGUILayout.PropertyField(mUpdateForwardProperty);
            EditorGUILayout.PropertyField(mDurationProperty);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mPathPointStartPosProperty);
            EditorGUILayout.PropertyField(mPathPointGapProperty);
            if (EditorGUI.EndChangeCheck())
            {
                Debug.Log($"路点起始位置或者间距修改够记得通过矫正路点位置功能修复路点位置！");
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mSegmentProperty);
            if (EditorGUI.EndChangeCheck())
            {
                mPathPointRelativePropertyChange = true;
            }

            EditorGUILayout.PropertyField(mPathPointSphereSizeProperty);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mDrawLineRendererProperty);
            if(EditorGUI.EndChangeCheck())
            {
                OnDrawLineRendererChange();
            }

            EditorGUILayout.PropertyField(mPathPointSphereColorProperty);
            EditorGUILayout.PropertyField(mPathDrawColorProperty);
            EditorGUILayout.PropertyField(mSubPathPointDrawColorProperty);

            DrawPathMoveSimulationArea();
            DrawPositionCorrectArea();
            DrawPathPointDataListProperty();
            DrawExportDataArea();

            EditorGUILayout.EndVertical();

            // 确保对SerializedObject和SerializedProperty的数据修改写入生效
            serializedObject.ApplyModifiedProperties();

            if (mPathPointRelativePropertyChange)
            {
                //Debug.Log($"路点相关数据属性变化，更新相关数据！");
                UpdateDrawDatas();
                mPathPointRelativePropertyChange = false;
            }
        }
        /// <summary>
        /// 绘制模拟路点移动区域
        /// </summary>
        private void DrawPathMoveSimulationArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("模拟对象:", GUILayout.Width(80f));
            mSimulationMoveGOProperty.objectReferenceValue = EditorGUILayout.ObjectField(mSimulationMoveGOProperty.objectReferenceValue, TPathConst.GameObjectType, true, GUILayout.Width(200f));
            if (GUILayout.Button("模拟移动", GUILayout.ExpandWidth(true)))
            {
                DoSimulationMove();
            }
            if (GUILayout.Button("暂停移动", GUILayout.ExpandWidth(true)))
            {
                PauseSimulationMove();
            }
            if (GUILayout.Button("继续移动", GUILayout.ExpandWidth(true)))
            {
                ResumeSimulationMove();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制路点矫正区域
        /// </summary>
        private void DrawPositionCorrectArea()
        {
            if (GUILayout.Button("路点位置矫正", GUILayout.ExpandWidth(true)))
            {
                CorrectPathPointPositions();
            }
        }

        /// <summary>
        /// 绘制数据导出区域
        /// </summary>
        private void DrawExportDataArea()
        {
            if (GUILayout.Button("数据导出", GUILayout.ExpandWidth(true)))
            {
                ExportPathPointDatas();
            }
        }

        /// <summary>
        /// 绘制路点数据列表属性
        /// </summary>
        private void DrawPathPointDataListProperty()
        {
            EditorGUILayout.BeginVertical();
            mPathPointFoldOut = EditorGUILayout.Foldout(mPathPointFoldOut, "路点数据列表");
            if (mPathPointFoldOut)
            {
                for (int i =  0; i < mPathPointDataListProperty.arraySize; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField($"索引:{i}", GUILayout.Width(60f));
                    DrawOnePathPointDataPropertyByIndex(i);
                    if (GUILayout.Button("+", GUILayout.Width(40f)))
                    {
                        AddPathPointDataByIndex(i);
                        mPathPointRelativePropertyChange = true;
                    }
                    if (GUILayout.Button("-", GUILayout.Width(40f)))
                    {
                        RemovePathPointDataByIndex(i);
                        mPathPointRelativePropertyChange = true;
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (GUILayout.Button("+", GUILayout.ExpandWidth(true)))
            {
                AddPathPointDataByIndex(mPathPointDataListProperty.arraySize);
                mPathPointRelativePropertyChange = true;
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制单个PathPointData属性
        /// </summary>
        /// <param name="pathPointDataIndex"></param>
        private void DrawOnePathPointDataPropertyByIndex(int pathPointDataIndex)
        {
            var pathPointDataProperty = mPathPointDataListProperty.GetArrayElementAtIndex(pathPointDataIndex);
            DrawPathPointPositionProperty(pathPointDataIndex, pathPointDataProperty);
            DrawPathPointTypeProperty(pathPointDataIndex, pathPointDataProperty);
            DrawPathPointCustomProperties(pathPointDataIndex, pathPointDataProperty);
        }

        /// <summary>
        /// 绘制指定路点数据索引和路点数据属性的位置属性
        /// </summary>
        /// <param name="pathPointDataIndex"></param>
        /// <param name="pathPointDataProperty"></param>
        private void DrawPathPointPositionProperty(int pathPointDataIndex, SerializedProperty pathPointDataProperty)
        {
            if(pathPointDataProperty == null)
            {
                return;
            }
            var positionProperty = pathPointDataProperty.FindPropertyRelative("Position");
            var newVector3Value = positionProperty.vector3Value;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("X", GUILayout.Width(10f));
            newVector3Value.x = EditorGUILayout.FloatField(newVector3Value.x, GUILayout.Width(60f));
            EditorGUILayout.LabelField("Y", GUILayout.Width(10f));
            newVector3Value.y = EditorGUILayout.FloatField(newVector3Value.y, GUILayout.Width(60f));
            EditorGUILayout.LabelField("Z", GUILayout.Width(10f));
            newVector3Value.z = EditorGUILayout.FloatField(newVector3Value.z, GUILayout.Width(60f));
            if (EditorGUI.EndChangeCheck())
            {
                UpdatePathPointDataPosition(pathPointDataIndex, positionProperty, newVector3Value);
            }
        }

        /// <summary>
        /// 绘制指定路点数据索引和路点数据属性的路点类型属性
        /// </summary>
        /// <param name="pathPointDataIndex"></param>
        /// <param name="pathPointDataProperty"></param>
        private void DrawPathPointTypeProperty(int pathPointDataIndex, SerializedProperty pathPointDataProperty)
        {
            if (pathPointDataProperty == null)
            {
                return;
            }
            var pathPointTypeProperty = pathPointDataProperty.FindPropertyRelative("PathPointType");
            if(pathPointTypeProperty == null)
            {
                return;
            }
            EditorGUILayout.LabelField("路点类型:", GUILayout.Width(60f));
            pathPointTypeProperty.intValue = (int)(TPathPointType)EditorGUILayout.EnumPopup((TPathPointType)pathPointTypeProperty.intValue, GUILayout.Width(100f));
        }

        /// <summary>
        /// 绘制指定路点数据索引和路点数据属性的自定义属性
        /// </summary>
        /// <param name="pathPointDataIndex"></param>
        /// <param name="pathPointDataProperty"></param>
        private void DrawPathPointCustomProperties(int pathPointDataIndex, SerializedProperty pathPointDataProperty)
        {
            if (pathPointDataProperty == null)
            {
                return;
            }
            var pathPointTypeProperty = pathPointDataProperty.FindPropertyRelative("PathPointType");
            if(pathPointTypeProperty == null)
            {
                return;
            }
            var pathPointType = (TPathPointType)pathPointTypeProperty.intValue;
            switch (pathPointType)
            {
                case TPathPointType.Wait:
                    DrawWaitPathPointProperties(pathPointDataIndex, pathPointDataProperty);
                    break;
                case TPathPointType.Jump:
                    DrawJumpPathPointProperties(pathPointDataIndex, pathPointDataProperty);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 绘制指定路点数据索引和路点数据属性的等待路点属性
        /// </summary>
        /// <param name="pathPointDataIndex"></param>
        /// <param name="pathPointDataProperty"></param>
        private void DrawWaitPathPointProperties(int pathPointDataIndex, SerializedProperty pathPointDataProperty)
        {
            var waitTimeProperty = pathPointDataProperty.FindPropertyRelative("WaitTime");
            if(waitTimeProperty == null)
            {
                return;
            }
            EditorGUILayout.LabelField("等待时长:", GUILayout.Width(60f));
            waitTimeProperty.floatValue = EditorGUILayout.FloatField(waitTimeProperty.floatValue, GUILayout.Width(60f));
        }

        /// <summary>
        /// 绘制指定路点数据索引和路点数据属性的跳跃路点属性
        /// </summary>
        /// <param name="pathPointDataIndex"></param>
        /// <param name="pathPointDataProperty"></param>
        private void DrawJumpPathPointProperties(int pathPointDataIndex, SerializedProperty pathPointDataProperty)
        {
            var jumpAnimNameProperty = pathPointDataProperty.FindPropertyRelative("JumpAnimName");
            if (jumpAnimNameProperty == null)
            {
                return;
            }
            EditorGUILayout.LabelField("跳跃动画:", GUILayout.Width(60f));
            jumpAnimNameProperty.stringValue = EditorGUILayout.TextField(jumpAnimNameProperty.stringValue, GUILayout.Width(60f));
        }

        /// <summary>
        /// 更新指定索引路点数据的坐标位置
        /// </summary>
        /// <param name="pathPointDataIndex"></param>
        /// <param name="positionProperty"></param>
        /// <param name="newPos"></param>
        private void UpdatePathPointDataPosition(int pathPointDataIndex, SerializedProperty positionProperty, Vector3 newPos)
        {
            positionProperty.vector3Value = newPos;
            //Debug.Log($"更新路点索引:{pathPointDataIndex}的位置:{positionProperty.vector3Value.ToString()}");
            mPathPointRelativePropertyChange = true;
        }

        /// <summary>
        /// 指定位置索引添加路点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool AddPathPointDataByIndex(int index)
        {
            var pathPointNum = mPathPointDataListProperty.arraySize;
            if (index < 0 || index > pathPointNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:{0}-{pathPointNum}，添加路点失败！");
                return false;
            }
            var newPathPointData = mTarget.ConstructNewPathPointData(index);
            mPathPointDataListProperty.InsertArrayElementAtIndex(index);
            var newPathPointDataProperty = mPathPointDataListProperty.GetArrayElementAtIndex(index);
            newPathPointDataProperty.managedReferenceValue = newPathPointData;
            mPathPointRelativePropertyChange = true;
            return true;
        }

        /// <summary>
        /// 移除指定索引的路点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool RemovePathPointDataByIndex(int index)
        {
            var pathPointNum = mPathPointDataListProperty.arraySize;
            if (index < 0 || index >= pathPointNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:{0}-{pathPointNum - 1}，移除路点失败！");
                return false;
            }
            mPathPointDataListProperty.DeleteArrayElementAtIndex(index);
            mPathPointRelativePropertyChange = true;
            return true;
        }

        /// <summary>
        /// 更新绘制数据
        /// </summary>
        private void UpdateDrawDatas()
        {
            var pathWayType = (TPathwayType)mPathwayTypeProperty.intValue;
            if (pathWayType == TPathwayType.Liner ||
                pathWayType == TPathwayType.Bezier ||
                pathWayType == TPathwayType.CubicBezier ||
                pathWayType == TPathwayType.CRSpline)
            {
                mDrawPath.Reset();
                mDrawPath.UpdatePathwayType(pathWayType);
                mDrawPath.UpdateSetgmentNum(mSegmentProperty.intValue);
                for (int i = 0, length = mPathPointDataListProperty.arraySize; i < length; i++)
                {
                    var pathPointProperty = mPathPointDataListProperty.GetArrayElementAtIndex(i);
                    var positionProperty = pathPointProperty.FindPropertyRelative("Position");
                    mDrawPath.AddPoint(positionProperty.vector3Value, false);
                }
                mDrawPath.UpdatePathDatas();

                mDrawPathPointList.Clear();
                var segmentNum = mDrawPath.GetSegmentNum();
                for(int i = 0; i < segmentNum; i++)
                {
                    // 前一段和后一段的起始点是重复的，需要剔除
                    var segmentSubPoints = mDrawPath.GetSubPointsBySegmentIndex(i);
                    var segmentSubPointStartIndex = i != 0 ? 1 : 0;
                    for(int j = segmentSubPointStartIndex, length2 = segmentSubPoints.Length; j < length2; j++)
                    {
                        mDrawPathPointList.Add(segmentSubPoints[j]);
                    }
                }
                UpdateLineRendererDatas();
            }
            else
            {
                Debug.LogError($"不支持的路线类型:{pathWayType.ToString()}，更新绘制数据失败！");
            }
        }

        /// <summary>
        /// 选中场景绘制
        /// </summary>
        private void OnSceneGUI()
        {
            if(mDrawSwitchProperty.boolValue)
            {
                if(Event.current.type == EventType.Repaint)
                {
                    DrawPathPointLabels();
                    DrawPathPointLines();
                    DrawSubPathPointSpheres();
                }
                DrawPathPointPositionHandles();
            }
        }

        /// <summary>
        /// 绘制所有路点坐标操作PositionHandle
        /// </summary>
        private void DrawPathPointPositionHandles()
        {
            for (int i = 0, length = mPathPointDataListProperty.arraySize; i < length; i++)
            {
                EditorGUI.BeginChangeCheck();
                var pathPointDataProperty = mPathPointDataListProperty.GetArrayElementAtIndex(i);
                var positionProperty = pathPointDataProperty.FindPropertyRelative("Position");
                var newTargetPosition = Handles.PositionHandle(positionProperty.vector3Value, Quaternion.identity);
                if (EditorGUI.EndChangeCheck())
                {
                    UpdatePathPointDataPosition(i, positionProperty, newTargetPosition);
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// 绘制路点标签
        /// </summary>
        private void DrawPathPointLabels()
        {
            for (int i = 0, length = mPathPointDataListProperty.arraySize; i < length; i++)
            {
                var pathPointDataProperty = mPathPointDataListProperty.GetArrayElementAtIndex(i);
                var positionProperty = pathPointDataProperty.FindPropertyRelative("Position");
                var pathPointLabelName = mTarget.GetPathPointNameByIndex(i);
                Handles.Label(positionProperty.vector3Value, pathPointLabelName, mLabelGUIStyle);
            }
        }

        /// <summary>
        /// 绘制路点连线
        /// </summary>
        private void DrawPathPointLines()
        {
            var preHandlesColor = Handles.color;
            Handles.color = mPathDrawColorProperty.colorValue;
            for(int i = 0, length = mDrawPathPointList.Count - 1; i < length; i++)
            {
                var firstPathPoint = mDrawPathPointList[i];
                var secondPathPoint = mDrawPathPointList[i+1];
                Handles.DrawLine(firstPathPoint, secondPathPoint, 2f);
            }
            Handles.color = preHandlesColor;
        }

        /// <summary>
        /// 绘制子路点球体
        /// </summary>
        private void DrawSubPathPointSpheres()
        {
            var preHandlesColor = Handles.color;
            Handles.color = mSubPathPointDrawColorProperty.colorValue;
            for (int i = 0, length = mDrawPathPointList.Count; i < length; i++)
            {
                var firstDrawPathPoint = mDrawPathPointList[i];
                Handles.SphereHandleCap(i, firstDrawPathPoint, Quaternion.identity, 0.1f, EventType.Repaint);
            }
            Handles.color = preHandlesColor;
        }

        /// <summary>
        /// 矫正路点位置
        /// </summary>
        private void CorrectPathPointPositions()
        {
            var pathPointGap = mPathPointGapProperty.floatValue;
            if (Mathf.Approximately(pathPointGap, Mathf.Epsilon))
            {
                return;
            }
            var pathPointStartPos = mPathPointStartPosProperty.vector3Value;
            var halfGap = pathPointGap / 2f;
            for (int i = 0, length = mPathPointDataListProperty.arraySize; i < length; i++)
            {
                var pathPointDataProperty = mPathPointDataListProperty.GetArrayElementAtIndex(i);
                var positionProperty = pathPointDataProperty.FindPropertyRelative("Position");
                var position = positionProperty.vector3Value;
                var pathPointPosOffset = position - pathPointStartPos;
                var correctPosX = Mathf.FloorToInt((pathPointPosOffset.x + halfGap) / pathPointGap) * pathPointGap + pathPointStartPos.x;
                var correctPosY = Mathf.FloorToInt((pathPointPosOffset.y + halfGap) / pathPointGap) * pathPointGap + pathPointStartPos.y;
                var correctPosZ = Mathf.FloorToInt((pathPointPosOffset.z + halfGap) / pathPointGap) * pathPointGap + pathPointStartPos.z;
                var correctPosition = new Vector3(correctPosX, correctPosY, correctPosZ);
                if (!Vector3.Equals(position, correctPosition))
                {
                    UpdatePathPointDataPosition(i, positionProperty, correctPosition);
                }
            }
            Debug.Log($"路点位置矫正完成！");
        }

        /// <summary>
        /// 响应绘制LinerRenderer组件变化
        /// </summary>
        private void OnDrawLineRendererChange()
        {
            UpdateLineRendererDatas();
        }

        /// <summary>
        /// 更新LineRenderer组件数据
        /// </summary>
        private void UpdateLineRendererDatas()
        {
            //Debug.Log($"更新LineRenderer组件数据！");
            var drawLineRenderer = mDrawLineRendererProperty.objectReferenceValue as LineRenderer;
            if (drawLineRenderer != null)
            {
                drawLineRenderer.positionCount = mDrawPathPointList.Count;
                drawLineRenderer.SetPositions(mDrawPathPointList.ToArray());
                drawLineRenderer.textureMode = LineTextureMode.Tile;
            }
        }

        /// <summary>
        /// 导出路点数据
        /// </summary>
        private bool ExportPathPointDatas()
        {
            if(!CheckCanExportPathPointDatas())
            {
                Debug.LogError($"不满足导出条件，导出路点数据失败！");
                return false;
            }
            CorrectPathPointPositions();
            var exportFileName = mExportFileNameProperty.stringValue;
            if(string.IsNullOrEmpty(exportFileName))
            {
                // 导出文件名为空则用GameObject名
                exportFileName = mTarget.gameObject.name;
            }
            var exportType = (TPathExportType)mExportTypeProperty.intValue;
            var pathType = (TPathType)mPathTypeProperty.intValue;
            var pathDataExport = ConstructPathDataExport();

            var exportFileFullPath = TPathUtilities.GetExportFileFullPath(exportFileName, exportType, pathType);
            var exportFileFolderFullPath = Path.GetDirectoryName(exportFileFullPath);
            FolderUtilities.CheckAndCreateSpecificFolder(exportFileFolderFullPath);

            if(exportType == TPathExportType.Json)
            {
                return DoExportJsonPathPointDatas(exportFileFullPath, pathDataExport, pathType);
            }
            Debug.LogError($"不支持的导出类型:{exportType.ToString()}数据导出！");
            return false;
        }

        /// <summary>
        /// 检查是否可以导出数据
        /// </summary>
        /// <returns></returns>
        private bool CheckCanExportPathPointDatas()
        {
            // TODO:
            // 可以在开始导出前做一些自定义检查，不符合就不导出
            return true;
        }

        /// <summary>
        /// 构建导出统一数据结构
        /// </summary>
        /// <returns></returns>
        private TPathDataExport ConstructPathDataExport()
        {
            var pathDataExport = new TPathDataExport();
            pathDataExport.PathType = (TPathType)mPathTypeProperty.intValue;
            pathDataExport.PathwayType = (TPathwayType)mPathwayTypeProperty.intValue;
            pathDataExport.Ease = (EasingFunction.Ease)mEaseProperty.intValue;
            pathDataExport.IsLoop = mIsLoopProperty.boolValue;
            pathDataExport.UpdateForward = mUpdateForwardProperty.boolValue;
            pathDataExport.Duration = mDurationProperty.floatValue;
            pathDataExport.Segment = mSegmentProperty.intValue;
            for (int i = 0, length = mPathPointDataListProperty.arraySize; i < length; i++)
            {
                var pathPointDataProperty = mPathPointDataListProperty.GetArrayElementAtIndex(i);
                var pathPointData = pathPointDataProperty.managedReferenceValue as TPathPointData;
                pathDataExport.AddPathPointData(pathPointData);
            }
            return pathDataExport;
        }

        /// <summary>
        /// 导出Json格式路点数据
        /// </summary>
        /// <param name="exportFileFullPath"></param>
        /// <param name="pathDataExport"></param>
        /// <returns></returns>
        private bool DoExportJsonPathPointDatas(string exportFileFullPath, TPathDataExport pathDataExport, TPathType pathType)
        {
            if(pathDataExport == null)
            {
                Debug.LogError($"导出Json数据失败，导出数据结构为空！");
                return false;
            }
            var jsonStr = JsonUtility.ToJson(pathDataExport, true);
            File.WriteAllText(exportFileFullPath, jsonStr);
            Debug.Log($"导出Json数据文件全路径:{exportFileFullPath}成功！");
            return true;
        }

        /// <summary>
        /// 执行模拟移动
        /// </summary>
        private void DoSimulationMove()
        {
            if(mSimulationMoveGOProperty.objectReferenceValue == null)
            {
                Debug.LogError($"未设置任何有效模拟路点移动对象，模拟路线移动失败！");
                return;
            }
            var simulationMoveAsset = AssetDatabase.GetAssetPath(mSimulationMoveGOProperty.objectReferenceValue);
            if(!string.IsNullOrEmpty(simulationMoveAsset))
            {
                Debug.LogError($"不允许模拟本地Asset路点移动！");
                return;
            }
            if(mSimulationPathTweener != null)
            {
                TPathTweenerManager.Singleton.RemovePathTween(mSimulationPathTweener);
                mSimulationPathTweener = null;
            }
            var pathPointList = mTarget.GetPathPointPosList();
            var simulationMoveGo = mSimulationMoveGOProperty.objectReferenceValue as GameObject;
            var duration = mDurationProperty.floatValue;
            var isLoop = mIsLoopProperty.boolValue;
            var updateForward = mUpdateForwardProperty.boolValue;
            var pathwayType = (TPathwayType)mPathwayTypeProperty.intValue;
            var ease = (EasingFunction.Ease)mEaseProperty.intValue;
            mSimulationPathTweener = TPathTweenerManager.Singleton.DoPathTweenByPoints(simulationMoveGo.transform,
                                                                                        pathPointList, duration, isLoop,
                                                                                        updateForward,
                                                                                        () =>
                                                                                         {
                                                                                             mSimulationPathTweener = null;
                                                                                             Debug.Log($"路线模拟移动完成！");
                                                                                         },
                                                                                         (index) =>
                                                                                         {
                                                                                            Debug.Log($"路线通过路点索引:{index}");

                                                                                         },
                                                                                         () =>
                                                                                         {
                                                                                            Debug.Log($"路线缓动完成！");
                                                                                         },
                                                                                         pathwayType, ease);
        }

        /// <summary>
        /// 暂停模拟移动
        /// </summary>
        private void PauseSimulationMove()
        {
            if (mSimulationPathTweener == null)
            {
                Debug.LogError($"未处于模拟移动中，暂停模拟移动失败！");
                return;
            }
            mSimulationPathTweener.Pause();
        }

        /// <summary>
        /// 继续模拟移动
        /// </summary>
        private void ResumeSimulationMove()
        {
            if (mSimulationPathTweener == null)
            {
                Debug.LogError($"未处于模拟移动中，继续模拟移动失败！");
                return;
            }
            mSimulationPathTweener.Resume();
        }
    }
}