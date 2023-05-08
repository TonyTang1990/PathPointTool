# PathPointTool
此Github的目的是实现一个简易的路点编辑和缓动工具。

需求:

1. 纯Editor非运行时路点编辑器。
2. 路点编辑器需要生成可视化编辑对象和路点路线展示(支持纯Editor绘制和LineRenderer组件绘制两种方式)。
3. 路点编辑器要支持指定起始位置和固定位置偏移的路点编辑自动矫正(方便固定单位间隔的路点配置)。
4. 路点编辑器要支持指定路线移动时长，是否循环和是否自动更新朝向等路线缓动模拟设置。
5. 路点编辑器要支持自定义数据导出自定义格式数据。
6. 路点编辑器要支持多种路线类型(e.g. Line，Bezier，CubicBezier， Cutmull-Rom Spline等)。
7. 路线移动支持缓动曲线配置。
8. 路点编辑器要支持纯Editor模拟运行路点移动效果。
9. 路点编辑器编辑完成后的数据要支持运行时使用并模拟路线缓动，同时路线缓动要支持纯运行时构建使用。
10. 实现一个纯Editor的Tile可视化绘制脚本(方便路点编辑位置参考)。

实现思路：

1. 结合自定义Inspector面板(继承Editor)定义的方式实现纯Editor配置和操作
2. 利用Gizmos(Monobehaviour:OnDrawGizmos())，Handles(Editor.OnSceneGUI())和自定义Inspector(Editor)面板编辑操作实现可视化编辑对象生成和展示。LineRenderer通过挂在指定LinRenderer组件将路点细分的点通过LineRenderer:SetPositions()设置显示。
3. 利用自定义Inspector面板支持起始位置和路点间隔配置，然后通过配置数据进行路点位置矫正操作。
4. 自定义Inspecotr面板支持配置即可。
5. 同上，自定义Inspector面板支持操作分析路点数据进行导出即可。
6. 利用Bezier曲线知识，实现不同路线类型(e.g. 直线，Bezier，CubicBezier等)。
7. 利用InitializeOnLoad，ExecuteInEditMode和InitializeOnLoadMethod标签加EditorApplication.update实现纯Editor初始化和注入Update更新实现纯Editor模拟路点移动效果。
8. 利用缓动曲线去重新计算插值t(0-1)的值作为插值比例即可。
9. 实现一套超级简陋版DoTween支持运行时路线缓动模拟即可(见TPathTweener和TPathTweenerManager)。
10. 利用Gizmos的自定义Mesh绘制+自定义Inspector面板实现Tile网格自定义配置绘制。

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

## 博客

[路点编辑和缓动](http://tonytang1990.github.io/2023/04/09/PathPointTool/)