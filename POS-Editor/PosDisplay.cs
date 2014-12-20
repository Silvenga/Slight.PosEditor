#region Usings

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

#endregion

namespace POS_Editor {

    public class PosDisplay {

        private static readonly char[] _splitChars = {
            ' ',
            '-',
            '\t'
        };

        private PosDisplay(int rows, int columns) {

            Rows = rows;
            Columns = columns;

            Display = new char[Rows][];

            for(var i = 0; i < Display.Length; i++) {

                Display[i] = new char[Columns];

                for(var j = 0; j < Columns; j++) {

                    Display[i][j] = ' ';
                }
            }
        }

        public int Rows {
            get;
            set;
        }

        public int Columns {
            get;
            set;
        }

        private char[][] Display {
            get;
            set;
        }

        public string this[int index] {
            get {
                if(index >= Rows || index < 0) {
                    throw new ArgumentException("Index out of row range.");
                }

                return new string(Display[index]);
            }
            set {

                if(index >= Rows || index < 0) {
                    throw new ArgumentException("Index out of row range.");
                }

                if(value.Length > Columns) {
                    throw new ArgumentException("String length out of column range.");
                }

                var line = value;
                line = line.Trim();

                var length = line.Length;
                var whiteSpace = ((Columns - length) + 1) / 2;

                var white = "";
                for(var i = 0; i < whiteSpace; i++) {
                    white += " ";
                }

                line = white + line;

                for(var i = Columns - line.Length; i > 0; i--) {
                    line += " ";
                }

                for(var i = 0; i < line.Length; i++) {

                    Display[index][i] = line.ToCharArray()[i];
                }
            }
        }

        public static bool TryParse(string message, out PosDisplay display, int rows, int columns) {

            display = default(PosDisplay);

            string[] lines;

            var success = TryParse(message, out lines, rows, columns);

            if(success) {

                display = new PosDisplay(rows, columns);

                for(var i = 0; i < lines.Length; i++) {

                    display[i] = lines[i].Trim();
                }
            }

            return success;
        }

        private static bool TryParse(string message, out string[] wrapped, int rows, int columns) {

            wrapped = ToLines(Wrap(message, columns));

            return wrapped.Length <= rows && !wrapped.Any(x => x.Length > columns);
        }

        private static string Wrap(string str, int columns) {

            var words = Explode(str, _splitChars);

            var currentLineLength = 0;
            var builder = new StringBuilder();

            for(var i = 0; i < words.Length; i += 1) {

                var word = words[i];

                if(currentLineLength + word.Length > columns) {

                    if(currentLineLength > 0) {
                        builder.Append(Environment.NewLine);
                        currentLineLength = 0;
                    }

                    while(word.Length > columns) {
                        builder.Append(word.Substring(0, columns - 1) + "-");
                        word = word.Substring(columns - 1);

                        builder.Append(Environment.NewLine);
                    }

                    word = word.TrimStart();
                }
                builder.Append(word);
                currentLineLength += word.Length;
            }

            return builder.ToString();
        }

        private static string[] Explode(string str, char[] splitChars) {

            var parts = new List<string>();
            var startIndex = 0;

            while(true) {

                var index = str.IndexOfAny(splitChars, startIndex);

                if(index == -1) {

                    parts.Add(str.Substring(startIndex));
                    return parts.ToArray();
                }

                var word = str.Substring(startIndex, index - startIndex);
                var nextChar = str.Substring(index, 1)[0];

                if(char.IsWhiteSpace(nextChar)) {

                    parts.Add(word);
                    parts.Add(nextChar.ToString(CultureInfo.InvariantCulture));
                } else {

                    parts.Add(word + nextChar);
                }

                startIndex = index + 1;
            }
        }

        private static string[] ToLines(string text) {

            return text.Split(
                new[] {
                    Environment.NewLine
                },
                StringSplitOptions.RemoveEmptyEntries);
        }

    }

}