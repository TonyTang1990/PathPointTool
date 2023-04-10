/*
 * Description:             TileVisualizationEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/09
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// TileVisualizationEditor.cs
/// Tile可视化组件Editor
/// </summary>
[CustomEditor(typeof(TileVisualization))]
public class TileVisualizationEditor : Editor
{
    /// <summary>
    /// 目标组件
    /// </summary>
    private TileVisualization mTarget;

    /// <summary>
    /// TileRow属性
    /// </summary>
    private SerializedProperty mTileRowProperty;

    /// <summary>
    /// TileColumn属性
    /// </summary>
    private SerializedProperty mTileColumnProperty;

    /// <summary>
    /// TileLength属性
    /// </summary>
    private SerializedProperty mTileLengthProperty;

    /// <summary>
    /// TileStartPos属性
    /// </summary>
    private SerializedProperty mTileStartPosProperty;

    private void OnEnable()
    {
        InitTarget();
        InitProperties();
    }

    /// <summary>
    /// 初始化目标组件
    /// </summary>
    private void InitTarget()
    {
        mTarget ??= (target as TileVisualization);
    }

    /// <summary>
    /// 初始化属性
    /// </summary>
    private void InitProperties()
    {
        mTileRowProperty ??= serializedObject.FindProperty("TileRow");
        mTileColumnProperty ??= serializedObject.FindProperty("TileColumn");
        mTileLengthProperty ??= serializedObject.FindProperty("TileLength");
        mTileStartPosProperty ??= serializedObject.FindProperty("TileStartPos");
    }

    /// <summary>
    /// Inspector自定义显示
    /// </summary>
    public override void OnInspectorGUI()
    {
        // 确保对SerializaedObject和SerializedProperty的数据修改每帧同步
        serializedObject.Update();

        EditorGUILayout.BeginVertical();
        var tileMeshRelativePropertyChange = false;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(mTileRowProperty);
        EditorGUILayout.PropertyField(mTileColumnProperty);
        EditorGUILayout.PropertyField(mTileLengthProperty);
        if (EditorGUI.EndChangeCheck())
        {
            tileMeshRelativePropertyChange = true;
        }
        EditorGUILayout.PropertyField(mTileStartPosProperty);
        EditorGUILayout.EndVertical();

        // 确保对SerializedObject和SerializedProperty的数据修改写入生效
        serializedObject.ApplyModifiedProperties();

        if(tileMeshRelativePropertyChange)
        {
            mTarget?.UpdateTileDatas();
        }
    }
}