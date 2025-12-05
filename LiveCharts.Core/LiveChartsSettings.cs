using LiveChartsCore.Context;
using System;
using System.Collections.Generic;

namespace LiveChartsCore
{
    /// <summary>
    /// LiveCharts global settings
    /// </summary>
    public class LiveChartsSettings
    {
        private readonly Dictionary<Type, object> _mappers = new Dictionary<Type, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveChartsSettings"/> class.
        /// </summary>
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
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Configures <see cref="NaturalGeometries"/> class to use LiveCharts settings transitions globally.
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="easingFunction"></param>
        /// <returns></returns>
        public LiveChartsSettings AddGlobalEasing(Func<float, float> easingFunction, TimeSpan duration)
        {
            //Visual.AddTransition(Visual.AllShapesAllProperties, new Animation(easingFunction, duration));
            return this;
        }
    }
}
