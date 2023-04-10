/*
 * Description:             TileVisualization.cs
 * Author:                  TONYTANG
 * Create Date:             2023/04/09
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TileVisualization.cs
/// Tile可视化组件
/// </summary>
[ExecuteInEditMode]
public partial class TileVisualization : MonoBehaviour
{
    /// <summary>
    /// Tile行数
    /// </summary>
    [Header("Tile行数")]
    [Range(0, 1000)]
    public int TileRow = 1;

    /// <summary>
    /// Tile列数
    /// </summary>
    [Header("Tile列数")]
    [Range(0, 1000)]
    public int TileColumn = 1;

    /// <summary>
    /// Tile长度
    /// </summary>
    [Header("Tile长度")]
    [Range(0, 50)]
    public int TileLength = 1;

    /// <summary>
    /// Tile起始位置
    /// </summary>
    [Header("Tile起始位置")]
    public Vector3 TileStartPos = Vector3.zero;

    private void Awake()
    {
#if UNITY_EDITOR
        mTileMesh = new Mesh();
        mTileMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        UpdateTileMeshData();
#endif
    }

#if UNITY_EDITOR
    /// <summary>
    /// Tile绘制Mesh
    /// </summary>
    private Mesh mTileMesh;

    /// <summary>
    /// 单行顶点数量
    /// </summary>
    private int mSingleRowVertexNum;

    /// <summary>
    /// 更新Tile数据
    /// </summary>
    public void UpdateTileDatas()
    {
        UpdateTileMeshData();
    }

    /// <summary>
    /// 更新Tile网格数据
    /// </summary>
    private void UpdateTileMeshData()
    {
        Debug.Log("更新Tile网格数据！");
        mTileMesh.Clear();
        mSingleRowVertexNum = TileColumn + 1;
        var tileNum = TileRow * TileColumn;
        var tileSquareMeshNum = tileNum;
        var triangleNum = tileSquareMeshNum * 2;
        var verticeNum = triangleNum * 3;
        var uvNum = verticeNum;
        var triangleIndiceNum = verticeNum;
        var verticeArray = new Vector3[verticeNum];
        var uvArray = new Vector2[uvNum];
        var triangleIndicesArray = new int[triangleIndiceNum];
        for(int i = 0; i < tileSquareMeshNum; i++)
        {
            UpdateTileSquareMeshData(i, verticeArray, uvArray, triangleIndicesArray);
        }
        mTileMesh.vertices = verticeArray;
        mTileMesh.uv = uvArray;
        mTileMesh.triangles = triangleIndicesArray;
        mTileMesh.RecalculateNormals();
        mTileMesh.RecalculateTangents();
        mTileMesh.RecalculateBounds();
    }

    /// <summary>
    /// 更新指定Tile索引的网格数据
    /// </summary>
    /// <param name="squareIndex"></param>
    /// <param name="vertices"></param>
    /// <param name="uvs"></param>
    /// <param name="triangleIndices"></param>
    private void UpdateTileSquareMeshData(int squareIndex, Vector3[] vertices, Vector2[] uvs, int[] triangleIndices)
    {
        // Note:
        // 1. 三角形顺序是按顺时针
        // 这里是按左下->左上->右下->右下顺序构建的三角形
        var squareRowIndex = squareIndex / TileColumn;
        var squareColumnIndex = squareIndex % TileColumn;
        var vertexStartIndex = squareRowIndex * mSingleRowVertexNum + squareColumnIndex;
        // Vertice
        // 左下
        var bottomLeftVertexIndex = vertexStartIndex;
        vertices[bottomLeftVertexIndex].x = squareColumnIndex * TileLength;
        vertices[bottomLeftVertexIndex].y = 0;
        vertices[bottomLeftVertexIndex].z = squareRowIndex * TileLength;
        // 左上
        var topLeftVertexIndex = vertexStartIndex + mSingleRowVertexNum;
        vertices[topLeftVertexIndex].x = vertices[bottomLeftVertexIndex].x;
        vertices[topLeftVertexIndex].y = 0;
        vertices[topLeftVertexIndex].z = vertices[bottomLeftVertexIndex].z + TileLength;
        // 右下
        var bottomRightVertexIndex = bottomLeftVertexIndex + 1;
        vertices[bottomRightVertexIndex].x = vertices[bottomLeftVertexIndex].x + TileLength;
        vertices[bottomRightVertexIndex].y = 0;
        vertices[bottomRightVertexIndex].z = vertices[bottomLeftVertexIndex].z;
        // 右上
        var topRightVertexIndex = topLeftVertexIndex + 1;
        vertices[topRightVertexIndex].x = vertices[bottomLeftVertexIndex].x + TileLength;
        vertices[topRightVertexIndex].y = 0;
        vertices[topRightVertexIndex].z = vertices[bottomLeftVertexIndex].z + TileLength;

        // UV
        uvs[bottomLeftVertexIndex] = UnityUtilities.BottomLeftUV;
        uvs[topLeftVertexIndex] = UnityUtilities.TopLeftUV;
        uvs[bottomRightVertexIndex] = UnityUtilities.BottomRightUV;
        uvs[topRightVertexIndex] = UnityUtilities.TopRightUV;

        // TriangleIndice
        var triangleIndiceStartIndex = squareIndex * 6;
        triangleIndices[triangleIndiceStartIndex] = bottomLeftVertexIndex;
        triangleIndices[triangleIndiceStartIndex + 1] = topLeftVertexIndex;
        triangleIndices[triangleIndiceStartIndex + 2] = bottomRightVertexIndex;
        triangleIndices[triangleIndiceStartIndex + 3] = bottomRightVertexIndex;
        triangleIndices[triangleIndiceStartIndex + 4] = topLeftVertexIndex;
        triangleIndices[triangleIndiceStartIndex + 5] = topRightVertexIndex;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireMesh(mTileMesh, TileStartPos);
    }
#endif
}