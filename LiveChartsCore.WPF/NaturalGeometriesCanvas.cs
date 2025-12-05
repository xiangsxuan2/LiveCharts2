using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharp.Drawing;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LiveChartsCore.WPF
{
    /// <summary>
    /// 自然几何图形画布，用于在 WPF 中绘制 SkiaSharp 图形
    /// </summary>
    /// <remarks>
    /// 这个控件是 LiveCharts2 在 WPF 平台的核心绘制组件，使用 SkiaSharp 进行硬件加速渲染
    /// </remarks>
    public class NaturalGeometriesCanvas : Control
    {
        /// <summary>
        /// SkiaSharp 渲染元素，负责实际的图形绘制
        /// </summary>
        protected SKElement skiaElement;

        /// <summary>
        /// 标识绘制循环是否正在运行
        /// </summary>
        private bool isDrawingLoopRunning = false;

        /// <summary>
        /// 画布核心，管理绘制任务和图形元素
        /// </summary>
        private Canvas<SkiaDrawingContext> canvasCore = new Canvas<SkiaDrawingContext>();

        /// <summary>
        /// 目标帧率
        /// </summary>
        private double framesPerSecond = 90;

        /// <summary>
        /// 静态构造函数，注册控件的默认样式
        /// </summary>
        static NaturalGeometriesCanvas()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NaturalGeometriesCanvas), new FrameworkPropertyMetadata(typeof(NaturalGeometriesCanvas)));
        }

        /// <summary>
        /// 初始化 <see cref="NaturalGeometriesCanvas"/> 类的新实例
        /// </summary>
        public NaturalGeometriesCanvas()
        {
            canvasCore.Invalidated += OnCanvasCoreInvalidated;
            Unloaded += OnUnloaded;
        }

        /// <summary>
        /// 绘制任务依赖属性，用于绑定要绘制的图形任务集合
        /// </summary>
        public static readonly DependencyProperty PaintTasksProperty =
            DependencyProperty.Register(
                nameof(PaintTasks), typeof(HashSet<IDrawableTask<SkiaDrawingContext>>), typeof(NaturalGeometriesCanvas),
                new PropertyMetadata(new HashSet<IDrawableTask<SkiaDrawingContext>>(), new PropertyChangedCallback(OnPaintTaskChanged)));

        /// <summary>
        /// 获取或设置绘制任务集合
        /// </summary>
        /// <remarks>
        /// 每个绘制任务定义了一组具有相同样式的图形元素
        /// </remarks>
        public HashSet<IDrawableTask<SkiaDrawingContext>> PaintTasks
        {
            get { return (HashSet<IDrawableTask<SkiaDrawingContext>>)GetValue(PaintTasksProperty); }
            set { SetValue(PaintTasksProperty, value); }
        }

        /// <summary>
        /// 获取或设置目标帧率
        /// </summary>
        /// <remarks>
        /// 控制绘制循环的刷新频率，影响动画的流畅度
        /// </remarks>
        public double FramesPerSecond { get => framesPerSecond; set => framesPerSecond = value; }

        /// <summary>
        /// 获取画布核心实例
        /// </summary>
        public Canvas<SkiaDrawingContext> CanvasCore => canvasCore;

        /// <summary>
        /// 应用控件模板时调用，初始化 SkiaSharp 元素
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            skiaElement = Template.FindName("skiaElement", this) as SKElement;
            if (skiaElement == null)
                throw new Exception(
                    $"SkiaElement not found. This was probably caused because the control {nameof(NaturalGeometriesCanvas)} template was overridden, " +
                    $"If you override the template please add an {nameof(SKElement)} to the template and name it 'skiaElement'");

            skiaElement.PaintSurface += OnPaintSurface;
        }

        /// <summary>
        /// 设置绘制任务集合
        /// </summary>
        /// <param name="tasks">要绘制的任务集合</param>
        /// <remarks>
        /// 这个方法会替换当前的绘制任务，并触发重绘
        /// </remarks>
        public void SetPaintTasks(HashSet<IDrawableTask<SkiaDrawingContext>> tasks)
        {
            canvasCore.SetPaintTasks(tasks);
        }

        /// <summary>
        /// 使画布无效化，触发重绘
        /// </summary>
        public void Invalidate()
        {
            RunDrawingLoop();
        }

        /// <summary>
        /// 当 SkiaSharp 元素需要绘制表面时调用
        /// </summary>
        /// <param name="sender">事件源</param>
        /// <param name="args">绘制表面事件参数</param>
        /// <remarks>
        /// 这个方法是实际绘制图形的入口点
        /// </remarks>
        protected virtual void OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            canvasCore.DrawFrame(new SkiaDrawingContext(args.Info, args.Surface, args.Surface.Canvas));
        }

        /// <summary>
        /// 当画布核心无效化时调用，触发绘制循环
        /// </summary>
        private void OnCanvasCoreInvalidated(Canvas<SkiaDrawingContext> sender)
        {
            Invalidate();
        }

        /// <summary>
        /// 当控件卸载时调用，清理事件订阅
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            canvasCore.Invalidated -= OnCanvasCoreInvalidated;
        }

        /// <summary>
        /// 运行绘制循环，直到画布变为有效状态
        /// </summary>
        /// <remarks>
        /// 这个方法会以指定的帧率不断重绘画布，直到所有动画完成
        /// </remarks>
        private async void RunDrawingLoop()
        {
            if (isDrawingLoopRunning || skiaElement == null) return;
            isDrawingLoopRunning = true;

            var ts = TimeSpan.FromSeconds(1 / framesPerSecond);
            while (!canvasCore.IsValid)
            {
                skiaElement.InvalidateVisual();
                await Task.Delay(ts);
            }

            isDrawingLoopRunning = false;
        }

        /// <summary>
        /// 当绘制任务依赖属性改变时调用
        /// </summary>
        /// <param name="sender">依赖对象</param>
        /// <param name="e">依赖属性改变事件参数</param>
        private static void OnPaintTaskChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var naturalGeometries = (NaturalGeometriesCanvas)sender;
            naturalGeometries.canvasCore.SetPaintTasks(naturalGeometries.PaintTasks);
        }
    }
}