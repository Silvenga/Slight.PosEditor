using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace POS_Editor {
    public class PosDisplay {

        public const int Rows = 4;
        public const int Columns = 20;

        public static bool TryParse(string message, out PosDisplay display) {

            display = default(PosDisplay);

            string[] lines;

            bool success = TryParse(message, out lines);

            if(success) {

                display = new PosDisplay();

                for(int i = 0; i < lines.Length; i++) {

                    display[i] = lines[i].Trim();
                }
            }

            return success;
        }

        private static bool TryParse(string message, out string[] wrapped) {

            wrapped = ToLines(Wrap(message));

            return wrapped.Length <= Rows && !wrapped.Any(x => x.Length > Columns);
        }

        private PosDisplay() {

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

        //        private static string Wrap(string text) {
        //
        //            return string.Join(string.Empty, Wrap(text.Split(new char[0], StringSplitOptions.RemoveEmptyEntries)));
        //        }
        //
        //        private static IEnumerable<string> Wrap(IEnumerable<string> words) {
        //
        //            var currentWidth = 0;
        //            foreach(var word in words) {
        //
        //                if(currentWidth != 0) {
        //                    if(currentWidth + word.Length < Columns) {
        //                        currentWidth++;
        //                        yield return " ";
        //
        //                    } else {
        //
        //                        currentWidth = 0;
        //                        yield return Environment.NewLine;
        //                    }
        //                }
        //                currentWidth += word.Length;
        //                yield return word;
        //            }
        //        }

        private static readonly char[] SplitChars = { ' ', '-', '\t' };

        private static string Wrap(string str) {

            string[] words = Explode(str, SplitChars);

            int curLineLength = 0;
            StringBuilder strBuilder = new StringBuilder();

            for(int i = 0; i < words.Length; i += 1) {

                string word = words[i];

                if(curLineLength + word.Length > Columns) {

                    if(curLineLength > 0) {
                        strBuilder.Append(Environment.NewLine);
                        curLineLength = 0;
                    }

                    while(word.Length > Columns) {
                        strBuilder.Append(word.Substring(0, Columns - 1) + "-");
                        word = word.Substring(Columns - 1);

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
