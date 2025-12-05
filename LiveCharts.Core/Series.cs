using LiveChartsCore.Context;
using LiveChartsCore.Drawing;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace LiveChartsCore
{
    /// <summary>
    /// Defines data to plot in a chart.
    /// 数据系列抽象基类，定义图表中数据系列的基本行为
    /// 这是一个泛型类，支持不同类型的数据模型和视觉元素
    /// </summary>
    /// <typeparam name="TModel">数据模型类型</typeparam>
    /// <typeparam name="TVisual">视觉元素类型</typeparam>
    /// <typeparam name="TDrawingContext">绘图上下文类型</typeparam>
    public abstract class Series<TModel, TVisual, TDrawingContext> : IDisposable, ISeries<TDrawingContext>
        where TDrawingContext : DrawingContext
        where TVisual : ISizedGeometry<TDrawingContext>, IHighlightableGeometry<TDrawingContext>, new()
    {
        /// <summary>
        /// 已订阅的图表核心集合，用于在数据变化时通知图表更新
        /// </summary>
        private readonly HashSet<ChartCore<TDrawingContext>> subscribedTo = new HashSet<ChartCore<TDrawingContext>>();

        /// <summary>
        /// 之前的数据集合（用于实现 INotifyCollectionChanged）
        /// </summary>
        private INotifyCollectionChanged previousValuesNCCInstance;

        /// <summary>
        /// 数据值集合
        /// </summary>
        private IEnumerable<TModel> values;

        /// <summary>
        /// 是否实现了 INotifyCollectionChanged 接口
        /// </summary>
        protected bool implementsINCC = false;

        /// <summary>
        /// 绘制上下文，用于图例和工具提示
        /// </summary>
        protected PaintContext<TDrawingContext> paintContext;

        /// <summary>
        /// 当前边界缓存，提高性能
        /// </summary>
        private CartesianBounds _currentBounds = null;

        /// <summary>
        /// 是否为值类型
        /// </summary>
        protected readonly bool isValueType;

        /// <summary>
        /// 是否实现了 INotifyPropertyChanged 接口
        /// </summary>
        protected readonly bool implementsINPC;

        /// <summary>
        /// 是否实现了 ICartesianCoordinate 接口
        /// </summary>
        protected readonly bool implementsICC;

        /// <summary>
        /// 按索引映射的坐标字典（用于值类型）
        /// </summary>
        protected Dictionary<int, ICartesianCoordinate> byValueVisualMap = new Dictionary<int, ICartesianCoordinate>();

        /// <summary>
        /// 按引用映射的坐标字典（用于引用类型）
        /// </summary>
        protected Dictionary<TModel, ICartesianCoordinate> byReferenceVisualMap = new Dictionary<TModel, ICartesianCoordinate>();

        /// <summary>
        /// 描边画笔
        /// </summary>
        private IDrawableTask<TDrawingContext> stroke;

        /// <summary>
        /// 填充画笔
        /// </summary>
        private IDrawableTask<TDrawingContext> fill;

        /// <summary>
        /// 高亮描边画笔
        /// </summary>
        private IDrawableTask<TDrawingContext> highlightStroke;

        /// <summary>
        /// 高亮填充画笔
        /// </summary>
        private IDrawableTask<TDrawingContext> highlightFill;

        /// <summary>
        /// 图例形状大小
        /// </summary>
        private double legendShapeSize = 15;

        /// <summary>
        /// Initializes a new instance of the <see cref="Series{T}"/> class.
        /// 初始化新的数据系列实例
        /// 检查数据模型的类型特性，以优化性能
        /// </summary>
        public Series()
        {
            var t = typeof(TModel);
            implementsINPC = typeof(INotifyPropertyChanged).IsAssignableFrom(t);
            implementsICC = typeof(ICartesianCoordinate).IsAssignableFrom(t);
            isValueType = t.IsValueType;
        }

        /// <summary>
        /// Gets or sets the series to draw in the chart.
        /// 获取或设置要在图表中绘制的数据系列
        /// 支持集合变化通知，自动处理数据变化
        /// </summary>
        public IEnumerable<TModel> Values
        {
            get => values;
            set
            {
                // 如果设置了新的集合实例
                if (value != previousValuesNCCInstance)
                {
                    // 取消对旧集合的订阅
                    if (previousValuesNCCInstance != null) previousValuesNCCInstance.CollectionChanged -= OnValuesCollectionChanged;

                    // 如果新集合支持集合变化通知，则订阅
                    if (value is INotifyCollectionChanged incc)
                    {
                        incc.CollectionChanged += OnValuesCollectionChanged;
                        implementsINCC = true;
                    }

                    previousValuesNCCInstance = values as INotifyCollectionChanged;
                    _currentBounds = null;  // 重置边界缓存
                }
                values = value;
            }
        }

        /// <inheritdoc/>
        /// <summary>
        /// 获取或设置系列使用的X轴索引
        /// </summary>
        public int ScalesXAt { get; set; }

        /// <inheritdoc/>
        /// <summary>
        /// 获取或设置系列使用的Y轴索引
        /// </summary>
        public int ScalesYAt { get; set; }

        /// <summary>
        /// 获取或设置系列的描边画笔
        /// </summary>
        public IDrawableTask<TDrawingContext> Stroke
        {
            get => stroke;
            set
            {
                stroke = value;
                if (stroke != null)
                {
                    stroke.IsStroke = true;  // 描边任务
                }

                OnPaintContextChanged();  // 画笔变化时更新绘制上下文
            }
        }

        /// <summary>
        /// 获取或设置系列的填充画笔
        /// </summary>
        public IDrawableTask<TDrawingContext> Fill
        {
            get => fill;
            set
            {
                fill = value;
                if (fill != null)
                {
                    fill.IsStroke = false;    // 填充任务
                    fill.StrokeWidth = 0;     // 填充不需要描边宽度
                }
                OnPaintContextChanged();  // 画笔变化时更新绘制上下文
            }
        }

        /// <summary>
        /// 获取或设置系列的高亮描边画笔
        /// 用于鼠标悬停或选择时的边框
        /// </summary>
        public IDrawableTask<TDrawingContext> HighlightStroke
        {
            get => highlightStroke;
            set
            {
                highlightStroke = value;
                if (highlightStroke != null)
                {
                    highlightStroke.IsStroke = true;   // 描边任务
                    highlightStroke.ZIndex = 1;        // 确保高亮显示在顶部
                }
                OnPaintContextChanged();  // 画笔变化时更新绘制上下文
            }
        }

        /// <summary>
        /// 获取或设置系列的高亮填充画笔
        /// 用于鼠标悬停或选择时的填充
        /// </summary>
        public IDrawableTask<TDrawingContext> HighlightFill
        {
            get => highlightFill;
            set
            {
                highlightFill = value;
                if (highlightFill != null)
                {
                    highlightFill.IsStroke = false;    // 填充任务
                    highlightFill.StrokeWidth = 0;     // 填充不需要描边宽度
                    highlightFill.ZIndex = 1;          // 确保高亮显示在顶部
                }
                OnPaintContextChanged();  // 画笔变化时更新绘制上下文
            }
        }

        /// <summary>
        /// 获取默认绘制上下文
        /// 用于图例和工具提示中的系列表示
        /// </summary>
        public PaintContext<TDrawingContext> DefaultPaintContext => paintContext;

        /// <summary>
        /// 获取或设置系列名称
        /// 用于图例和工具提示显示
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置图例形状大小
        /// </summary>
        public double LegendShapeSize { get => legendShapeSize; set => legendShapeSize = value; }

        /// <summary>
        /// Gets or sets the mapping that defines how a type is mapped to a <see cref="ChartPoint"/> instance,
        /// then the <see cref="ChartPoint"/> will be drawn as a point in our chart.
        /// </summary>
        /// <summary>
        /// 获取或设置映射函数，定义如何将类型映射到 <see cref="ChartPoint"/> 实例
        /// 然后 <see cref="ChartPoint"/> 将被绘制为图表中的一个点
        /// </summary>
        public Func<TModel, int, ICartesianCoordinate> Mapping { get; set; }

        /// <inheritdoc/>
        /// <summary>
        /// 从图表核心获取数据点
        /// 同时订阅图表更新通知
        /// </summary>
        /// <param name="chart">图表核心对象</param>
        /// <returns>数据点集合</returns>
        public virtual IEnumerable<ICartesianCoordinate> Fetch(ChartCore<TDrawingContext> chart)
        {
            subscribedTo.Add(chart);  // 订阅图表更新
            return GetPonts();        // 返回所有数据点
        }

        /// <inheritdoc/>
        /// <summary>
        /// 获取数据系列的边界
        /// 使用缓存提高性能，当数据变化时重新计算
        /// </summary>
        /// <param name="controlSize">控件大小</param>
        /// <param name="x">X轴</param>
        /// <param name="y">Y轴</param>
        /// <returns>数据系列的边界信息</returns>
        public virtual CartesianBounds GetBounds(SizeF controlSize, IAxis<TDrawingContext> x, IAxis<TDrawingContext> y)
        {
            // 如果满足缓存条件，使用缓存的边界
            if (_currentBounds != null && implementsICC && implementsINCC && implementsINPC) return _currentBounds;

            // when we implement INotifyCollectionChanged, INotifyPropertyChanged and ICartesianCoordinate
            // then we could skip this the next code.
            // 当实现 INotifyCollectionChanged, INotifyPropertyChanged 和 ICartesianCoordinate 时
            // 我们可以跳过以下代码，因为可以通过事件跟踪边界变化

            var bounds = new CartesianBounds();

            // 遍历所有数据点，计算边界
            foreach (var coordinate in GetPonts())
            {
                var isXLimit = coordinate.X == bounds.XAxisBounds.max || coordinate.X == bounds.XAxisBounds.min;
                var isYLimit = coordinate.Y == bounds.YAxisBounds.Max || coordinate.Y == bounds.YAxisBounds.min;

                // 添加X坐标到边界
                var abx = bounds.XAxisBounds.AppendValue(coordinate.X);
                // 添加Y坐标到边界
                var aby = bounds.YAxisBounds.AppendValue(coordinate.Y);

                // 如果影响了X轴边界
                if (abx > 0)
                {
                    if (!isXLimit) bounds.XCoordinatesBounds = new HashSet<ICartesianCoordinate>();
                    bounds.XCoordinatesBounds.Add(coordinate);
                }
                ;

                // 如果影响了Y轴边界
                if (aby > 0)
                {
                    if (!isYLimit) bounds.YCoordinatesBounds = new HashSet<ICartesianCoordinate>();
                    bounds.YCoordinatesBounds.Add(coordinate);
                }
            }

            _currentBounds = bounds;  // 缓存边界
            return bounds;
        }

        /// <inheritdoc/>
        /// <summary>
        /// 测量数据系列的抽象方法
        /// 子类必须实现此方法以计算数据点的位置和大小
        /// </summary>
        /// <param name="view">图表视图</param>
        /// <param name="xAxis">X轴</param>
        /// <param name="yAxis">Y轴</param>
        /// <param name="drawBucket">绘制桶，用于收集要绘制的几何图形</param>
        public abstract void Measure(
            IChartView<TDrawingContext> view,
            IAxis<TDrawingContext> xAxis,
            IAxis<TDrawingContext> yAxis,
            HashSet<IGeometry<TDrawingContext>> drawBucket);

        /// <summary>
        /// Gets the
        /// 获取所有数据点
        /// 根据数据类型选择不同的获取策略
        /// </summary>
        /// <returns>数据点集合</returns>
        public IEnumerable<ICartesianCoordinate> GetPonts() => implementsICC ? GetPointsFromICC() : GetMappedPoints();

        /// <inheritdoc/>
        /// <summary>
        /// 释放资源，取消事件订阅
        /// </summary>
        public void Dispose()
        {
            if (previousValuesNCCInstance != null)
                previousValuesNCCInstance.CollectionChanged -= OnValuesCollectionChanged;
            byReferenceVisualMap = null;
            byValueVisualMap = null;
        }

        /// <summary>
        /// 当数据点测量完成时调用的虚方法
        /// 子类可以重写此方法以执行自定义逻辑
        /// </summary>
        /// <param name="coordinate">数据点坐标</param>
        /// <param name="visual">视觉元素</param>
        protected virtual void OnPointMeasured(ICartesianCoordinate coordinate, TVisual visual)
        {
        }

        /// <summary>
        /// 从实现了 ICartesianCoordinate 接口的数据中获取点
        /// </summary>
        /// <returns>数据点集合</returns>
        private IEnumerable<ICartesianCoordinate> GetPointsFromICC()
        {
            var i = 0;
            foreach (var item in Values.Cast<ICartesianCoordinate>())
            {
                item.Index = i++;
                item.DataSource = item;

                // 订阅属性变化事件
                item.PropertyChanged -= OnValuesElementPropertyChanged;
                item.PropertyChanged += OnValuesElementPropertyChanged;

                yield return item;
            }
        }

        /// <summary>
        /// 通过映射函数获取点
        /// 将原始数据映射为图表坐标
        /// </summary>
        /// <returns>数据点集合</returns>
        private IEnumerable<ICartesianCoordinate> GetMappedPoints()
        {
            // 获取映射函数：优先使用自定义映射，否则使用全局映射
            var mapper = Mapping ?? LiveCharts.CurrentSettings.GetMapping<TModel>();
            var index = 0;

            foreach (var item in Values)
            {
                // 如果实现了 INotifyPropertyChanged，订阅属性变化事件
                if (implementsINPC)
                {
                    var inpc = (INotifyPropertyChanged)item;
                    inpc.PropertyChanged -= OnValuesElementPropertyChanged;
                    inpc.PropertyChanged += OnValuesElementPropertyChanged;
                }

                ICartesianCoordinate icc;

                // 根据类型选择映射策略
                if (isValueType)
                {
                    // 值类型：按索引缓存
                    if (!byValueVisualMap.TryGetValue(index, out icc))
                        byValueVisualMap[index] = (icc = mapper(item, index));
                }
                else
                {
                    // 引用类型：按对象引用缓存
                    if (!byReferenceVisualMap.TryGetValue(item, out icc))
                        byReferenceVisualMap[item] = (icc = mapper(item, index));
                }

                // 设置坐标点属性
                icc.Index = index;
                icc.DataSource = item;
                index++;

                yield return icc;
            }
        }

        /// <summary>
        /// 当数据元素属性变化时调用的方法
        /// 更新边界缓存并通知图表更新
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">属性变化事件参数</param>
        private void OnValuesElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // 如果当前有边界缓存且数据实现了 ICartesianCoordinate
            if (_currentBounds != null && implementsICC)
            {
                var icc = (ICartesianCoordinate)sender;
                // if any limit was modified, then we clear the limits, that means they will be calculate again.
                // 如果任何边界点被修改，则清除边界缓存，这意味着它们将重新计算
                if (_currentBounds.XCoordinatesBounds.Contains(icc) || _currentBounds.YCoordinatesBounds.Contains(icc))
                    _currentBounds = null;
            }
            NotifySubscribers();  // 通知所有订阅的图表更新
        }

        /// <summary>
        /// 当数据集合变化时调用的方法
        /// 根据集合变化类型更新边界缓存
        /// </summary>
        /// <param name="sender">发送者</param>
        /// <param name="e">集合变化事件参数</param>
        private void OnValuesCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // 如果当前有边界缓存
            if (_currentBounds != null)
            {
                // 如果数据实现了 ICartesianCoordinate
                if (implementsICC)
                {
                    switch (e.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                            // 添加新项：扩展边界
                            foreach (var item in e.NewItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                _currentBounds.XAxisBounds.AppendValue(coordinate.X);
                                _currentBounds.YAxisBounds.AppendValue(coordinate.Y);
                            }
                            break;

                        case NotifyCollectionChangedAction.Remove:
                            // 移除项：如果移除的是边界点，则清除缓存
                            foreach (var item in e.OldItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                if (coordinate.X < _currentBounds.XAxisBounds.min || coordinate.X > _currentBounds.XAxisBounds.max ||
                                    coordinate.Y < _currentBounds.YAxisBounds.min || coordinate.Y > _currentBounds.YAxisBounds.max)
                                {
                                    _currentBounds = null;
                                    break;
                                }
                            }
                            break;

                        case NotifyCollectionChangedAction.Replace:
                            // 替换项：更新新项，检查旧项是否为边界点
                            foreach (var item in e.NewItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                _currentBounds.XAxisBounds.AppendValue(coordinate.X);
                                _currentBounds.YAxisBounds.AppendValue(coordinate.Y);
                            }
                            foreach (var item in e.OldItems)
                            {
                                var coordinate = (ICartesianCoordinate)item;
                                if (coordinate.X < _currentBounds.XAxisBounds.min || coordinate.X > _currentBounds.XAxisBounds.max ||
                                    coordinate.Y < _currentBounds.YAxisBounds.min || coordinate.Y > _currentBounds.YAxisBounds.max)
                                {
                                    _currentBounds = null;
                                    break;
                                }
                            }
                            break;

                        case NotifyCollectionChangedAction.Move:
                            /// ignored.
                            // 移动项：忽略，不影响边界
                            break;

                        case NotifyCollectionChangedAction.Reset:
                            // 重置集合：清除缓存
                            _currentBounds = null;
                            break;
                    }
                }
            }
            NotifySubscribers();  // 通知所有订阅的图表更新
        }

        /// <summary>
        /// 通知所有订阅的图表更新
        /// </summary>
        private void NotifySubscribers()
        {
            foreach (var chart in subscribedTo) chart.Update();
        }

        /// <summary>
        /// 当绘制上下文变化时调用的虚方法
        /// 更新图例和工具提示中的系列表示
        /// </summary>
        protected virtual void OnPaintContextChanged()
        {
            var context = new PaintContext<TDrawingContext>();

            // 如果有填充画笔，创建图例形状
            if (Fill != null)
            {
                var fillClone = Fill.CloneTask();
                var visual = new TVisual { X = 0, Y = 0, Height = (float)legendShapeSize, Width = (float)legendShapeSize };
                visual.CompleteTransitions();
                fillClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(fillClone);
            }

            var w = LegendShapeSize;

            // 如果有描边画笔，创建图例形状
            if (Stroke != null)
            {
                var strokeClone = Stroke.CloneTask();
                var visual = new TVisual
                {
                    X = strokeClone.StrokeWidth,
                    Y = strokeClone.StrokeWidth,
                    Height = (float)legendShapeSize,
                    Width = (float)legendShapeSize
                };
                visual.CompleteTransitions();
                w += 2 * strokeClone.StrokeWidth;  // 考虑描边宽度
                strokeClone.AddGeometyToPaintTask(visual);
                context.PaintTasks.Add(strokeClone);
            }

            // 设置绘制上下文的尺寸
            context.Width = w;
            context.Height = w;

            paintContext = context;  // 更新绘制上下文
        }
    }
}