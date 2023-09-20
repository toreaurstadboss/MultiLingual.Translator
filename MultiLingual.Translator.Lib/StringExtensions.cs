using System.Text;

namespace MultiLingual.Translator.Lib
{
    public static class StringExtensions
    {

        /// <summary>
        /// Merges a collection of lines into a flattened string separating each line by a specified line separator.
        /// Newline is deafult
        /// </summary>
        /// <param name="inputLines"></param>
        /// <param name="lineSeparator"></param>
        /// <returns></returns>
        public static string? FlattenString(this IEnumerable<string>? inputLines, string lineSeparator = "\n")
        {
            if (inputLines == null || !inputLines.Any())
            {
                return null;
            }
            var flattenedString = inputLines?.Aggregate(new StringBuilder(),
                (sb, l) => sb.AppendLine(l + lineSeparator),
                sb => sb.ToString().Trim());

            return flattenedString;
        }

    }
}
