using System;

namespace Rownd.Xamarin.Utils
{
    public class RowndException : Exception
    {
        public RowndException()
        {
        }

        public RowndException(string message)
            : base(message)
        {
        }

        public RowndException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}