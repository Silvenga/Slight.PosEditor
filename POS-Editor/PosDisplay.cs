using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace POS_Editor {
    public class PosDisplay {

        public int Rows {
            get;
            set;
        }

        public int Columns {
            get;
            set;
        }

        public static bool TryParse(string message, out PosDisplay display, int rows, int columns) {

            display = default(PosDisplay);

            string[] lines;

            bool success = TryParse(message, out lines, rows, columns);

            if(success) {

                display = new PosDisplay(rows, columns);

                for(int i = 0; i < lines.Length; i++) {

                    display[i] = lines[i].Trim();
                }
            }

            return success;
        }

        private static bool TryParse(string message, out string[] wrapped, int rows, int columns) {

            wrapped = ToLines(Wrap(message, columns));

            return wrapped.Length <= rows && !wrapped.Any(x => x.Length > columns);
        }

        private PosDisplay(int rows, int columns) {

            Rows = rows;
            Columns = columns;

            Display = new char[Rows][];

            for(int i = 0; i < Display.Length; i++) {

                Display[i] = new char[Columns];
            }
        }

        private char[][] Display {
            get;
            set;
        }

        public string this[int index] {
            get {
                if(index >= Rows || index < 0)
                    throw new ArgumentException("Index out of row range.");

                return new string(Display[index]);
            }
            set {

                if(index >= Rows || index < 0)
                    throw new ArgumentException("Index out of row range.");

                if(value.Length > Columns)
                    throw new ArgumentException("String length out of column range.");

                string line = value;
                line = line.Trim();


                int length = line.Length;
                int whiteSpace = ((Columns - length) + 1) / 2;

                string white = "";
                for(int i = 0; i < whiteSpace; i++) {
                    white += " ";
                }

                line = white + line;


                for(int i = 0; i < line.Length; i++) {

                    Display[index][i] = line.ToCharArray()[i];
                }
            }
        }

        private static readonly char[] SplitChars = { ' ', '-', '\t' };

        private static string Wrap(string str, int columns) {

            string[] words = Explode(str, SplitChars);

            int curLineLength = 0;
            StringBuilder strBuilder = new StringBuilder();

            for(int i = 0; i < words.Length; i += 1) {

                string word = words[i];

                if(curLineLength + word.Length > columns) {

                    if(curLineLength > 0) {
                        strBuilder.Append(Environment.NewLine);
                        curLineLength = 0;
                    }

                    while(word.Length > columns) {
                        strBuilder.Append(word.Substring(0, columns - 1) + "-");
                        word = word.Substring(columns - 1);

                        strBuilder.Append(Environment.NewLine);
                    }

                    word = word.TrimStart();
                }
                strBuilder.Append(word);
                curLineLength += word.Length;
            }

            return strBuilder.ToString();
        }

        private static string[] Explode(string str, char[] splitChars) {

            List<string> parts = new List<string>();
            int startIndex = 0;

            while(true) {

                int index = str.IndexOfAny(splitChars, startIndex);

                if(index == -1) {

                    parts.Add(str.Substring(startIndex));
                    return parts.ToArray();
                }

                string word = str.Substring(startIndex, index - startIndex);
                char nextChar = str.Substring(index, 1)[0];

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

            return text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
