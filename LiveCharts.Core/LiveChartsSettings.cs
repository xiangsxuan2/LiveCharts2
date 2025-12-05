using LiveChartsCore.Context;
using System;
using System.Collections.Generic;

namespace LiveChartsCore
{
    /// <summary>
    /// LiveCharts global settings
    /// LiveCharts 全局设置类
    /// </summary>
    /// <remarks>
    /// 这个类用于存储和管理 LiveCharts 的全局配置，
    /// 包括数据类型映射、默认动画效果等
    /// </remarks>
    public class LiveChartsSettings
    {
        /// <summary>
        /// 存储数据类型映射的字典
        /// </summary>
        /// <remarks>
        /// Key: 数据类型 Type
        /// Value: 映射函数，将数据对象转换为图表坐标
        /// </remarks>
        private readonly Dictionary<Type, object> _mappers = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveChartsSettings"/> class.
        /// 初始化 <see cref="LiveChartsSettings"/> 类的新实例
        /// </summary>
        /// <remarks>
        /// 构造函数会自动添加默认的数据类型映射和全局动画设置
        /// </remarks>
        public LiveChartsSettings()
        {
            AddDefaultMappers()
                .AddGlobalEasing(EasingFunctions.Lineal, TimeSpan.FromMilliseconds(500));
        }

        /// <summary>
        /// Adds or replaces a mapping for a given type, the mapper defines how a type is mapped to a <see cref="ChartPoint"/> instance,
        /// then the <see cref="ChartPoint"/> will be drawn as a point in our chart.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="predicate">The mapper</param>
        /// <returns></returns>
        /// <summary>
        /// 添加或替换指定类型的映射函数
        /// </summary>
        /// <typeparam name="TModel">要映射的数据类型</typeparam>
        /// <param name="predicate">映射函数，接收数据对象和索引，返回图表坐标</param>
        /// <returns>当前设置实例，支持方法链式调用</returns>
        /// <remarks>
        /// 映射函数定义了如何将数据对象转换为图表上的点坐标
        /// 对于自定义数据类型，必须提供映射函数才能正常显示
        /// </remarks>
        public LiveChartsSettings SetMapping<TModel>(Func<TModel, int, ICartesianCoordinate> predicate)
        {
            _mappers[typeof(TModel)] = predicate;
            return this;
        }

        /// <summary>
        /// Gets the current mapping for a given type.
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <returns>The current mapper</returns>
        /// <summary>
        /// 获取指定类型的当前映射函数
        /// </summary>
        /// <typeparam name="TModel">数据类型</typeparam>
        /// <returns>映射函数</returns>
        /// <exception cref="NotImplementedException">当指定类型没有映射函数时抛出</exception>
        /// <remarks>
        /// 如果尝试获取未注册类型的映射函数，会抛出异常并提示用户如何注册
        /// </remarks>
        public Func<TModel, int, ICartesianCoordinate> GetMapping<TModel>()
        {
            if (!_mappers.TryGetValue(typeof(TModel), out var mapper))
                throw new NotImplementedException(
                    $"A mapper for type {typeof(TModel)} is not implemented yet, consider using {nameof(LiveCharts)}.{nameof(LiveCharts.Configure)}() " +
                    $"method to call {nameof(SetMapping)}() with the type you are trying to plot.");

            return (Func<TModel, int, ICartesianCoordinate>)mapper;
        }

        /// <summary>
        /// Enables LiveCharts to be able to plot short, int, long, float, double, decimal and <see cref="ChartPoint"/>.
        /// 启用 LiveCharts 对常见数值类型的默认映射支持
        /// </summary>
        /// <returns>当前设置实例，支持方法链式调用</returns>
        /// <remarks>
        /// 默认支持的数值类型包括：short, int, long, float, double, decimal
        /// 这些类型的数据会自动映射为图表点，X坐标为索引，Y坐标为数值
        /// </remarks>
        public LiveChartsSettings AddDefaultMappers()
        {
            SetMapping<short>((value, index) => new ChartPoint<short>(index, value, index, value));
            SetMapping<int>((value, index) => new ChartPoint<int>(index, value, index, value));
            SetMapping<long>((value, index) => new ChartPoint<long>(index, value, index, value));
            SetMapping<float>((value, index) => new ChartPoint<float>(index, value, index, value));
            SetMapping<double>((value, index) => new ChartPoint<double>(index, value, index, value));
            SetMapping<decimal>((value, index) => new ChartPoint<decimal>(index, (double)value, index, value));

            return this;
        }

        /// <summary>>        
        /// Configures <see cref="NaturalGeometries"/> class to use LiveCharts settings transitions globally.
        /// 配置全局动画效果
        /// </summary>
        /// <param name="easingFunction">缓动函数，控制动画的运动曲线</param>
        /// <param name="duration">动画持续时间</param>
        /// <returns>当前设置实例，支持方法链式调用</returns>
        /// <remarks>
        /// 这个设置会影响所有图表元素的动画效果
        /// 默认使用线性缓动函数，持续500毫秒
        /// </remarks>
        public LiveChartsSettings AddGlobalEasing(Func<float, float> easingFunction, TimeSpan duration)
        {
            //Visual.AddTransition(Visual.AllShapesAllProperties, new Animation(easingFunction, duration));
            return this;
        }
    }
}