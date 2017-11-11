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
		public int Current {get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="OptimizationMethod"> Method of optimization.</param>
        /// <param name="Start">An initial value of progress.</param>
        /// <param name="End">An end value of progress.</param>
        /// <param name="Current">A current value of progress.</param>
		public Progress(object OptimizationMethod, int Start, int End, int Current)
		{
			this.OptimizationMethod = OptimizationMethod;
			this.Start = Start;
			this.End = End;
			this.Current = Current;
		} 
	}
}