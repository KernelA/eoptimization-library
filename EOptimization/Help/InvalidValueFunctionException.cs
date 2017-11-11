// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Help
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Math;

    class InvalidValueFunctionException : ArithmeticException
    {
        protected PointND point;

        protected double value;

        /// <summary>
        /// Get a point, where function has an invalid value.
        /// </summary>
        public PointND AtPoint => point;

        /// <summary>
        /// An invalid value of function.
        /// </summary>
        public double InvalidValue => value;

        /// <summary>
        /// The exception that is thrown for errors in a calculation of function.
        /// </summary>
        /// <param name="Message">A message of exception.</param>
        /// <param name="point">A point, where function has an invalid value. </param>
        /// <param name="ValueFunction">An invalid value of function.</param>
        public InvalidValueFunctionException(string Message, PointND point, double ValueFunction) : base(Message)
        {
            this.point = point;
            this.value = ValueFunction;
        }
    }
}
