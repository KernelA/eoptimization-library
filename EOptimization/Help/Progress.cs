// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Help
{
	using System;

	/// <summary>
    /// Information about progress of computation.
    /// </summary>
	public class Progress
	{
        private int current;

        /// <summary>
        /// Method of optimization.
        /// </summary>
		public object OptimizationMethod { get; }

        /// <summary>
        /// An initial value of progress.
        /// </summary>
		public int Start { get; }

        /// <summary>
        /// An end value of progress.
        /// </summary>
		public int End { get; }

        /// <summary>
        /// A current value of progress.
        /// </summary>
		public int Current
        {
            get => current;
            set
            {
                if (value < Start || value > End)
                    throw new ArgumentException($"{nameof(Current)} (actual value is {value}) must be >= {nameof(Start)} " +
                        $"(actual value is {Start}) and <= {nameof(End)} (actual value is {End}).", nameof(Current));

                current = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OptimizationMethod"> Method of optimization.</param>
        /// <param name="Start">An initial value of progress.</param>
        /// <param name="End">An end value of progress.</param>
        /// <param name="Current">A current value of progress.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="OptimizationMethod"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="Current"/> &lt; <paramref name="Start"/> or gt; <paramref name="End"/>.</exception>
		public Progress(object OptimizationMethod, int Start, int End, int Current)
		{

            if (OptimizationMethod == null)
            {
                throw new ArgumentNullException(nameof(OptimizationMethod));
            }

            if (Current < Start || Current > End)
                throw new ArgumentException($"{nameof(Current)} (actual value is {Current}) must be >= {nameof(Start)} " +
                    $"(actual value is {Start}) and <= {nameof(End)} (actual value is {End}).", nameof(Current));


			this.OptimizationMethod = OptimizationMethod;
			this.Start = Start;
			this.End = End;
			this.Current = Current;
		} 
	}
}