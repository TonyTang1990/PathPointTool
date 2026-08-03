# PathPointTool
此Github的目的是实现一个简易的路点编辑和缓动工具。

## 需求

1. 纯Editor非运行时路点编辑器。
2. 路点编辑器需要生成可视化编辑对象和路点路线展示(支持纯Editor绘制和LineRenderer组件绘制两种方式)。
3. 路点编辑器要支持指定起始位置和固定位置偏移的路点编辑自动矫正(方便固定单位间隔的路点配置)。
4. 路点编辑器要支持指定路线移动时长，是否循环和是否自动更新朝向等路线缓动模拟设置。
5. 路点编辑器要支持自定义数据导出自定义格式(比如Json，自定义CSV)数据。
6. 路点编辑器要支持多种路线类型(e.g. Line，Bezier，CubicBezier， Cutmull-Rom Spline等)。
7. 路线移动支持缓动曲线配置。
8. 路点编辑器要支持纯Editor模拟运行路点移动效果。
9. 路点编辑器编辑完成后的数据要支持运行时使用并模拟路线缓动，同时路线缓动要支持纯运行时构建使用。
10. 实现一个纯Editor的Tile可视化绘制脚本(方便路点编辑位置参考)。
11. **不同路点类型要支持DIY数据定义加配置导出**。
12. **支持自定义导出的数据加载快速使用触发路点移动表现**。
13. 路点移动要支持**移动完成，经过路点，循环开始等回调**。

## 实现思路

1. 结合自定义Inspector面板(继承Editor)定义的方式实现纯Editor配置和操作
2. 利用Gizmos(Monobehaviour:OnDrawGizmos())，Handles(Editor.OnSceneGUI())和自定义Inspector(Editor)面板编辑操作实现可视化编辑对象生成和展示。LineRenderer通过挂在指定LinRenderer组件将路点细分的点通过LineRenderer:SetPositions()设置显示。
3. 利用自定义Inspector面板支持起始位置和路点间隔配置，然后通过配置数据进行路点位置矫正操作。
4. 自定义Inspecotr面板支持配置即可。
5. 同上，自定义Inspector面板支持导出文件名和导出类型即可，路点数据导出前统一封装然后根据导出类型导出即可。
6. 利用Bezier曲线知识，实现不同路线类型(e.g. 直线，Bezier，CubicBezier等)。
7. 利用InitializeOnLoad，ExecuteInEditMode和InitializeOnLoadMethod标签加EditorApplication.update实现纯Editor初始化和注入Update更新实现纯Editor模拟路点移动效果。
8. 利用缓动曲线去重新计算插值t(0-1)的值作为插值比例即可。
9. 实现一套超级简陋版DoTween支持运行时路线缓动模拟即可(见TPathTweener和TPathTweenerManager)。
10. 利用Gizmos的自定义Mesh绘制+自定义Inspector面板实现Tile网格自定义配置绘制。
11. 通过定义TPathPointType定义路点类型，然后统一在TPathPointData里定义自定义数据成员，在自定义绘制Inspector面板时根据路点类型去决定是否绘制特定自定义数据面板。
12. 通过构造统一导出数据结构，给TPathTweenerManager添加使用导出数据结构作为路点数据表现的接口即可。
13. **通过TPathTweener去根据进度计算对应回调实际触发对应回调即可**。

自定义路点数据编辑面板：

![CustomPathDataInspector](/img/Unity/PathPointTool/CustomPathDataInspector.PNG)

自定义Tile绘制配置面板:

![CustomTileInspector](/img/Unity/PathPointTool/CustomTileInspector.PNG)

可视化路点路线展示:

![CubicBezierDraw](/img/Unity/PathPointTool/CubicBezierDraw.PNG)

自定义路线数据导出：

![CustomPathDataExport](/img/Unity/PathPointTool/CustomPathDataExport.PNG)

LineRenderer可视化展示：

![CutmullRomSplineDraw](/img/Unity/PathPointTool/CutmullRomSplineDraw.PNG)

Ease插值类型：

![EaseLerpFunction](/img/Unity/Math/EaseLerpFunction.png)

M个点的N个3阶Bezier插值计算思路如下：

1. **N个3阶Bezier曲线的组合插值是通过将M个点分成N段3阶Bezier，计算出总长度且每段Bezier存储起始点索引和Bezier类型(影响当前Bezier的采样点数)和路段长度**
2. **当我们要计算一个插值比例t(0-1)进度插值计算时，首先根据总距离和进度映射计算出在哪一段Bezier路段**
3. **映射计算到对应3阶Bezier段后，再进行单个3阶Bezier曲线比例插值从而得到我们M个点的插值比例t(0-1)的最终插值位置**

Cutmull-Rom Spline曲线经过首尾两个控制点思路：

1. **利用Catmull-Rom Spline曲线会通过中间两个控制点且中间两个点经过时的切线与前后两个控制点连线平行，那么我可以可以通过模拟构造一个P(-1)=2P0-P1(确保P(-1)P1和P0切线平行从而确保从P0处切线平行)，利用P(-1)P0P1P2构造一个CatmullRomSpline曲线即可画出P0开始的P0P1的曲线。最后一段曲线同理，构造一个P(N+1)=2P(N)-P(N-1)，然后绘制P(N-2)P(N-1)P(N)P(N+1)即可绘制出P(N-1)P(N)的曲线。**

### 路点类型数据配置

路点类型的自定义数据配置是通过统一定义在TPathPointData结构里，然后结合Inspector绘制决定路点类型是否绘制特定字段决定的。

TPathPointType.cs

```csharp
/// <summary>
/// TPathPointType.cs
/// 路点类型
/// </summary>
public enum TPathPointType
{
    Invalide = 1,       // 无效路点
    Normal,             // 普通路点类型
    Wait,               // 等待路点类型
    Jump,               // 跳跃路点类型
}
```

TPathPointData.cs

```csharp
/// <summary>
/// TPathPointData.cs
/// 路点数据组件
/// </summary>
[Serializable]
public class TPathPointData
{
    /// <summary>
    /// 坐标位置
    /// </summary>
    [Header("坐标位置")]
    public Vector3 Position;

    /// <summary>
    /// 路点类型
    /// </summary>
    [Header("路点类型")]
    public TPathPointType PathPointType = TPathPointType.Normal;

    /// <summary>
    /// 等待路点的等待时长
    /// </summary>
    [Header("等待路点的等待时长")]
    public float WaitTime = 0f;

    /// <summary>
    /// 跳跃路点的跳跃动画名称
    /// </summary>
    [Header("跳跃路点的跳跃动画名称")]
    public string JumpAnimName = string.Empty;
    
    ******
}

```

TPathDataEditor.cs

```csharp
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

******

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
```

自定义路点类型数据绘制如下:

![CustomPointDataInspector](/img/Unity/PathPointTool/CustomPointDataInspector.PNG)

### 自定义数据导出

自定义数据导出是通过在TPathData的Inspector面板支持导出类型和自定义导出名字(默认不填取GameObject名，方便制作多个关卡的路点数据)。

![CustomExportInspector](/img/Unity/PathPointTool/CustomExportInspector.PNG)

在自定义导出数据之前，我通过统一导出结构为TPathDataExport类，然后在通过对应序列化方式实现自定义导出类型支持：

TPathDataExport.cs

```csharp
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

    ******
}
```

TPathDataEditor.cs

```csharp
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
```

目前导出的类型只支持了**Json**，导出后内容如下:

![CustomPathDataExport](/img/Unity/PathPointTool/CustomPathDataExport.PNG)

### 自定义数据导出使用

自定义数据统一导出结构并导出后，我们如何快速使用这个结构进行路点数据模拟移动了？

首先我们要反序列化加载对应导出路点数据：

TPathUtilities.cs

```csharp
/// <summary>
/// 加载指定导出文件名，导出类型和路点路线类型的TPathDataExport
/// </summary>
/// <param name="exportFileName"></param>
/// <param name="exportType"></param>
/// <param name="pathType"></param>
/// <returns></returns>
public static TPathDataExport LoadTPathDataExport(string exportFileName, TPathExportType exportType = TPathExportType.Json,
                                                  TPathType pathType = TPathType.Normal)
{
    // 不同的资源加载方式请自行封装
    var exportFileFullPath = GetExportFileFullPath(exportFileName, exportType, pathType);
    var exportAssetFileRelativePath = PathUtilities.GetAssetsRelativeFolderPath(exportFileFullPath);
    var pathDataExportAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(exportAssetFileRelativePath);
    var pathDataExport = JsonUtility.FromJson<TPathDataExport>(pathDataExportAsset.text);
    if(pathDataExport == null)
    {
        Debug.LogError($"加载导出文件:{exportAssetFileRelativePath}失败，无法获取有效的TPathDataExport！");
        return null;
    }
    return pathDataExport;
}
```

然后使用封装好的TPathTweenerManager.DoPathTweenByTPathDataExport()接口快速使用：

```csharp
/// <summary>
/// 相应开始路线移动按钮点击
/// </summary>
private void OnBtnStartPathMove()
{
    Debug.Log($"GameLaucher:OnBtnStartPathMove()");
    if(mPathTweener != null)
    {
        TPathTweenerManager.Singleton.RemovePathTween(mPathTweener);
        mPathTweener = null;
    }
    if(mLoadedPathDataExport == null)
    {
        Debug.LogError($"没有加载有效的路点数据，无法开始路线缓动！");
        return;
    }
    mPathTweener = TPathTweenerManager.Singleton.DoPathTweenByTPathDataExport(PathMoveGo.transform, mLoadedPathDataExport, ***)
}
```

### 路点运行回调

目前路点运行回调支持了**移动完成，经过路点，循环开始**三个回调时机。

**完成和循环开始主要是通过比较进度判定完成，经过路点的判定是通过一开始记录了所有的路点对应进度数据，然后结合当前进度去比较是否跨过下一个路点的进度来判定的。**

TPathTeewner.cs

```csharp
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
    if (CheckPathPointReached(preProgress, newProgress, out int reachedIndex))
    {
        OnPassPathPoint(reachedIndex);
    }
    if (!IsLoop && Mathf.Approximately(newProgress, 1))
    {
        OnPathTweenComplete();
    }
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
```

运行时加载导出路点数据进行路点移动播放就会得到如下结果：

GameLauncher.cs

```csharp
/// <summary>
/// 相应开始路线移动按钮点击
/// </summary>
private void OnBtnStartPathMove()
{
    Debug.Log($"GameLaucher:OnBtnStartPathMove()");
    if(mPathTweener != null)
    {
        TPathTweenerManager.Singleton.RemovePathTween(mPathTweener);
        mPathTweener = null;
    }
    if(mLoadedPathDataExport == null)
    {
        Debug.LogError($"没有加载有效的路点数据，无法开始路线缓动！");
        return;
    }
    mPathTweener = TPathTweenerManager.Singleton.DoPathTweenByTPathDataExport(PathMoveGo.transform, mLoadedPathDataExport,
                                                                              () =>
                                                                              {
                                                                                  Debug.Log($"运行时路线缓动完成！");
                                                                                  mPathTweener = null;
                                                                              },
                                                                              (index) =>
                                                                              {
                                                                                  Debug.Log($"经过路点:{index}");
                                                                                  var pathPointData = mLoadedPathDataExport.GetPathPointData(index);
                                                                                  if(pathPointData != null)
                                                                                  {
                                                                                      Debug.Log($"路点数据:{pathPointData.ToString()}");
                                                                                  }
                                                                              },
                                                                              () =>
                                                                              {
                                                                                  Debug.Log($"路线缓动循环开始！");
                                                                              });
}
```

![PassPointLogView](/img/Unity/PathPointTool/PassPointLogView.PNG)

可以看到通过对应回调我们访问对应路点数据已经可以拿到路点类型+自定义路点数据了，如果未来想根据路点类型进行DIY逻辑编写，则可以直接在回调里访问数据编写对应逻辑即可。

## 博客

[路点编辑和缓动](http://tonytang1990.github.io/2023/04/09/PathPointTool/)