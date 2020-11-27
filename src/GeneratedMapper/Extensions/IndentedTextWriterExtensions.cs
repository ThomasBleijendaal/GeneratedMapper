using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace GeneratedMapper.Extensions
{
    internal static class IndentedTextWriterExtensions
    {
        public static void WriteLines(this IndentedTextWriter writer, IEnumerable<string> lines, bool insertEmptyLinesBetweenEntries = false)
        {
            foreach (var line in lines)
            {
                writer.WriteLine(line);

                if (insertEmptyLinesBetweenEntries)
                {
                    writer.WriteLine();
                }
            }
        }
    }
}
