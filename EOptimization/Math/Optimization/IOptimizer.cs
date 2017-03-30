namespace EOpt.Math.Optimization
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for optimization methods.
    /// </summary>
    public interface IOptimizer
    {
        /// <summary>
        /// The solution of the constrained optimization problem.
        /// </summary>
        PointND Solution { get; }

        /// <summary>
        /// Initializing parameters of methods.
        /// </summary>
        /// <param name="parametrs"></param>
        void InitializeParameters(object parametrs);

        

        /// <summary>
        /// Finding solution of the constrained optimization problem.
        /// </summary>
        /// <param name="parametrs">General parameters <see cref="GeneralParams"/>.</param>
        void Optimize(GeneralParams parametrs);

        /// <summary>
        /// Finding solution of the constrained optimization problem.
        /// </summary>
        /// <param name="parametrs">General parameters <see cref="GeneralParams"/>.</param>
        /// <param name="cancelToken"><see cref="CancellationToken"/></param>
        void Optimize(GeneralParams parametrs, CancellationToken cancelToken);

        /// <summary>
        /// Finding solution of the constrained optimization problem. If you want see progress, then you need set <paramref name="reporter"/>.
        /// </summary>
        /// <param name="parametrs">General parameters <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface IProgress&lt;Tuple&lt;object, int, int, int&gt;&gt;, 
        /// where first item in tuple is the self object, second item initial value, third item is the end value, fourth item is the current progress value.</param>
        void Optimize(GeneralParams parametrs, IProgress<Tuple<object, int, int, int>> reporter);

        /// <summary>
        /// Finding solution of the constrained optimization problem. If you want see progress, then you need set <paramref name="reporter"/>.
        /// </summary>
        /// <param name="parametrs">General parameters <see cref="GeneralParams"/>.</param>
        /// <param name="reporter">Object which implement interface IProgress&lt;Tuple&lt;object, int, int, int&gt;&gt;, 
        /// where first item in tuple is the self object, second item initial value, third item is the end value, fourth item is the current progress value.</param>
        /// <param name="cancelToken"><see cref="CancellationToken"/></param>
        void Optimize(GeneralParams parametrs, IProgress<Tuple<object, int, int, int>> reporter, CancellationToken cancelToken);
    }

    
}
