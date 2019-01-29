using System.IO;
using System.Diagnostics;

namespace Halite3.hlt
{
    /// <summary>
    /// A class that can be used to log messages to a file.
    /// </summary>
    public class Log
    {
        private TextWriter file;
        private static Log instance;
        public static bool enabled = true;
        public static bool enabled_path_find = false;

        private static Stopwatch stopwatch = new Stopwatch();
        private static long total_elapsed_milliseconds = 0;

        private static int spaces = 0;

        private Log(TextWriter f)
        {
            file = f;
        }

        public static void Initialize(TextWriter f)
        {
            instance = new Log(f);
        }

        public static void StartTimeTrack()
        {
            total_elapsed_milliseconds = 0;
        }

        public static void StartSection(string section)
        {
            stopwatch.Restart();
            LogMessage(@"////////////////*********** " + section + @" ************\\\\\\\\\\\\\\");
            spaces += 4;
        }

        public static void EndSection()
        {
            stopwatch.Stop();
            long section_milliseconds = stopwatch.ElapsedMilliseconds;
            total_elapsed_milliseconds += section_milliseconds;
            spaces -= 4;
            LogMessage(@"\\\\\\\\\\\\\\\\*********** -------------------- ************//////////////  section elapsed milliseconds: " + section_milliseconds + " total elapsed milliseconds: " + total_elapsed_milliseconds);
        }

        public static void LogMessage(string message)
        {
            if (!Log.enabled) return;
            try
            {
                instance.file.WriteLine(new string(' ', spaces) + message);
                instance.file.Flush();
            }
            catch (IOException)
            {
            }
        }

        public static void LogPathFind(string message)
        {
            if (!Log.enabled_path_find) return;
            try {
                instance.file.WriteLine(message);
                instance.file.Flush();
            }
            catch (IOException) {
            }
        }
    }
}
