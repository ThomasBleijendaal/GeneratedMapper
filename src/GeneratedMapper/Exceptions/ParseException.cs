using System;
using Microsoft.CodeAnalysis;

namespace GeneratedMapper.Exceptions
{
    internal class ParseException : Exception
    {
        public ParseException(Diagnostic issue)
        {
            Issue = issue;
        }

        public Diagnostic Issue { get; }
    }
}
