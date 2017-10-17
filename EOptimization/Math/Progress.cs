namespace EOpt.Math
{
	using System;

	/// <summary>
    /// Information about progress of computation.
    /// </summary>
	public class Progress
	{
		public object OptimizationMethod { get; }

		public int Start { get; }

		public int End { get; }

		public int Current {get; set; }

		public Progress(object OptimizationMethod, int Start, int End, int Current)
		{
			this.OptimizationMethod = OptimizationMethod;
			this.Start = Start;
			this.End = End;
			this.Current = Current;
		} 
	}
}