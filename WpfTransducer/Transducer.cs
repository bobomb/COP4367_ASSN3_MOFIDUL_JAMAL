//*********************************************************
//
// (c) Copyright 2014 Dr. Thomas Fernandez
// 
// All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Media;

namespace WpfTransducer
{
    class Transducer
    {
        public MainWindow mainWindow;
        public ControlWindow controlWindow;
        Functionality functionality;
        SoundPlayer soundPlayer = new SoundPlayer(@"..\..\..\..\clang.wav");



        public void setup(MainWindow mw,ControlWindow cw)
        {
            mainWindow = mw;
            controlWindow = cw;
            functionality = new Functionality();
            functionality.mainWindow = mw;
            functionality.setup();
        }


        static public string stripBlanks(string s)
        {
            if (s.Length > 0)
            {
                while (s[0] == ' ') s = s.Substring(1);
                while (s[s.Length - 1] == ' ') s = s.Substring(0, s.Length - 1);
            }
            return s;
        }


        static List<string> tokenize(string command)
        {
            command += " ";
            List<string> result = new List<string>();
            int found = command.IndexOf(' ');
            result.Add(stripBlanks(command.Substring(0, found)));
            command = command.Substring(found + 1);
            if (command.Length > 0)
            {
                while ((command.Length > 0) && (command[0] == ' ')) command = command.Substring(1);
                if (command.Length > 0)
                {
                    command.Remove(command.Length - 1);
                    command += ";";
                }
            }
            while (command.Length > 0)
            {
                found = command.IndexOf(';');
                result.Add(stripBlanks(command.Substring(0, found)));
                command = command.Substring(found + 1);
            }

            return result;
        }

        public string batchDir = @"..\..\..\..\";

        public string transduce(string cmd)
        {
            string result="";
            List<string> token = tokenize(cmd);
            if (token[0] == "GA")
            {
                result = functionality.runGA(Convert.ToInt32(token[1]));
            }
            else if (token[0] == "GAP")
            {
                result = functionality.runGA(Convert.ToInt32(token[1]));
                functionality.plotGA();
                soundPlayer.Play();
            }
            else if (token[0] == "PLOTGA")
            {
                result = functionality.plotGA();
            }
            else if (token[0] == "CLEARGA")
            {
                functionality.setup();
                result += " "+functionality.plotGA();
            }
            else if (token[0] == "CLEARBUTTONS")
            {
                for (int i = 0; i < controlWindow.maxNumberOfButtons; i++)
                {
                    controlWindow.clearButton(i);
                }
                result = "Buttons cleared.";
            }
            else if (token[0] == "CLEARBUTTON")
            {
                controlWindow.clearButton(Convert.ToInt32(token[1]));
                result = "Button "+token[1]+" cleared.";
            }
            else if (token[0] == "SETBUTTON")
            {
                // In the case of SETBUTTON all tokens after token[3] are appended to token[3] with semicolens between them because
                // token[3] is a command that may have many parameters.
                for (int i = 4; i < token.Count(); i++)
                {
                    token[3] += ";" + token[i];
                }
                controlWindow.setButton(Convert.ToInt32(token[1]), token[2], token[3]);
                result = "Button set.";
            }
            else if (token[0] == "COMMANDLINE")
            {
                if (token[1] == "HIDE")
                {
                    controlWindow.HideCommandLine();
                    result = "Command Line Hidden.";
                }
                else if (token[1] == "SHOW")
                {
                    controlWindow.ShowCommandLine();
                    result = "Command Line Shown.";
                }
            }
            else if (token[0] == "SLEEP")
            {
                Thread.Sleep(Convert.ToInt32(token[1]));
            }
            else
            {
                string filename = batchDir + token[0] + ".txt";
                if (File.Exists(filename))
                {
                    string line;
                    StreamReader sr = new StreamReader(filename);
                    while ((line = sr.ReadLine()) != null)
                    {
                        for (int i = 1; i < token.Count(); i++)
                        {
                            line = line.Replace("%"+i.ToString(), token[i]);
                        }
                        if (line.Length > 0)
                        {
                            transduce(line);
                            mainWindow.InvalidateVisual();
                        }
                    }
                    result = token[0] + ".txt executed.";
                }
                else
                {
                    result = "Error: Command not Recognized";
                }
            }
            return result;
        }
    }
}
