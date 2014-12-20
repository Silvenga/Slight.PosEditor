namespace POS_Editor {

    public enum PosCommands {

        Version = 0x02,
        HideCursor = 0x0E,
        ShowCursor = 0x0F,
        OverwriteMode = 0x12,
        Clear = 0x15,
        Return = 0x0D,
        Feed = 0x0A

    }

    public static class PosCommandsHelper {

        public static string Stringify(this PosCommands command) {

            var temp = (char) command;
            return "" + temp;
        }

    }

}