// This is an open source non-commercial project. Dear PVS-Studio, please check it. PVS-Studio Static
// Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace EOpt.Exceptions
{
    using System;

    using Math;

    /// <summary>
    /// The exception presents an invalid value of the function. 
    /// </summary>
    public class InvalidValueFunctionException : ArithmeticException
    {
        private PointND _point;

        /// <summary>
        /// Get a point, where function has an invalid value. 
        /// </summary>
        public PointND AtPoint => _point;


        /// <summary>
        /// The exception that is thrown for errors in a calculation of function. 
        /// </summary>
        /// <param name="Message">       A message of exception. </param>
        /// <param name="Point">         A point, where function has an invalid value. </param>
        /// <exception cref="ArgumentNullException"> If <paramref name="Point"/> is null. </exception>
        public InvalidValueFunctionException(string Message, PointND Point) : base(Message)
        {
            if (Point == null)
            {
                throw new ArgumentNullException(nameof(Point));
            }
            this._point = Point;
        }
    }
}