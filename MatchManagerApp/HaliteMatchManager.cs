using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace Halite3MatchManager
{
    public partial class HaliteMatchManager : Form
    {

        public const string BOTNEW = @"dotnet D:\Projetos\Halite3\Working\Halite3\bin\Release\netcoreapp2.0\MyBot.dll";
        public const string BOTOLD = @"dotnet D:\Projetos\Halite3\Working\Halite3v40\bin\Release\netcoreapp2.0\MyBot.dll";

        public HaliteMatchManager()
        {
            InitializeComponent();

            cBot0.Text = BOTNEW;
            cBot1.Text = BOTOLD;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var games = 0;

            if (cGame1.Checked) games = 1;
            if (cGame10.Checked) games = 10;
            if (cGame100.Checked) games = 100;
            if (cGame1000.Checked) games = 1000;

            PlayMatch(games, cBot0.Text, cBot1.Text);
        }

        private void PlayMatch(int quantity, string bot0, string bot1)
        {
            var win0 = 0;
            var win1 = 0;
            var map_size = 0;

            if (cMS32.Checked) map_size = 32;
            if (cMS40.Checked) map_size = 40;
            if (cMS48.Checked) map_size = 48;
            if (cMS56.Checked) map_size = 56;
            if (cMS64.Checked) map_size = 64;
            if (cMSMix.Checked) map_size = 32;

            for (int i = 1; i <= quantity; i++) {
                int result = PlayGame(quantity, i, bot0, bot1, map_size);
                if (result == 0) win0++;
                if (result == 1) win1++;
                cResults.AppendText("   ---> Games: " + i + "   NEW: " + win0 + "   OLD: " + win1 + "   " + (win0 * 100 / i) + " %" + Environment.NewLine);
                Application.DoEvents();
                if (cMSMix.Checked) {
                    map_size += 8;
                    if (map_size > 64) map_size = 32;
                }
            }
        }

        private int PlayGame(int quantity, int number, string bot0, string bot1, int map_size)
        {
            //halite.exe --replay-directory replays/ -vvv --width 32 --height 32 "dotnet %cd%\Halite3\bin\Debug\netcoreapp2.0\MyBot.dll" "dotnet %cd%\Halite3v04\bin\Debug\netcoreapp2.0\MyBot.dll"

            cResults.AppendText("Game " + number + " of " + quantity + " NEW vs OLD (" + map_size + "x" + map_size + ") -> ");

            var filename = @"D:\Projetos\Halite3\Working\halite.exe";
            var map_param = "--width " + map_size + " --height " + map_size;
            var arguments = "--replay-directory D:/Projetos/Halite3/Working/replays_batch/ -vvv " + map_param + " \"" + bot0 + "\" \"" + bot1 + "\"";

            Process halite_process = new Process();
            halite_process.StartInfo.FileName = filename;
            halite_process.StartInfo.Arguments = arguments;
            halite_process.StartInfo.UseShellExecute = false;
            halite_process.StartInfo.RedirectStandardError = true;
            halite_process.StartInfo.CreateNoWindow = true;
            halite_process.Start();

            int winbot = -1;

            while (true) {

                var line = halite_process.StandardError.ReadLine();
                if (line == null) break;
                if (line.StartsWith("[info] Player initialization")) continue;
                if (!line.StartsWith("[info] Player")) continue;

                if (line.StartsWith("[info] Player 0") && line.Contains("was rank 1")) {
                    winbot = 0;
                    cResults.AppendText(" *** NEW wins *** " + line + Environment.NewLine);
                }
                if (line.StartsWith("[info] Player 1") && line.Contains("was rank 1")) {
                    winbot = 1;
                    cResults.AppendText(" *** OLD wins *** " + line + Environment.NewLine);
                }
            }

            halite_process.WaitForExit();

            return winbot;
        }

    }


}
