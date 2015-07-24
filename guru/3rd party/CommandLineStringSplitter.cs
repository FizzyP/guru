﻿
/*
    This Code Found Here:
    http://stackoverflow.com/questions/298830/split-string-containing-command-line-parameters-into-string-in-c-sharp/298990#298990
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guru
{
    static class CommandLineStringSplitter
    {
        public static IEnumerable<string> SplitCommandLine(string commandLine)
        {
            bool inQuotes = false;

            return commandLine.Split(c =>
            {
                if (c == '\"')
                    inQuotes = !inQuotes;

                return !inQuotes && c == ' ';
            })
                              .Select(arg => arg.Trim().TrimMatchingQuotes('\"'))
                              .Where(arg => !string.IsNullOrEmpty(arg));
        }

        public static IEnumerable<string> Split(this string str,
                                                Func<char, bool> controller)
        {
            int nextPiece = 0;

            for (int c = 0; c < str.Length; c++)
            {
                if (controller(str[c]))
                {
                    yield return str.Substring(nextPiece, c - nextPiece);
                    nextPiece = c + 1;
                }
            }

            yield return str.Substring(nextPiece);
        }

        public static string TrimMatchingQuotes(this string input, char quote)
        {
            if ((input.Length >= 2) &&
                (input[0] == quote) && (input[input.Length - 1] == quote))
                return input.Substring(1, input.Length - 2);

            return input;
        }

    }
}
