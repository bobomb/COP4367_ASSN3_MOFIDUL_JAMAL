//*********************************************************
//
// (c) Copyright 2014 Dr. Thomas Fernandez
// 
// All rights reserved.
//
//*********************************************************

using System;
using System.Collections.Generic;
using System.IO;
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

namespace WpfTransducer
{
    /// <summary>
    /// Interaction logic for ControlWindow.xaml
    /// </summary>

    
    
    public partial class ControlWindow : Window
    {
        public MainWindow mainWindow;

        private List<string> cmdHistory = new List<string>();
        int currentCmdHistory = -1;
        public int maxNumberOfButtons = 8;
        List<Button> buttonList = new List<Button>();
        List<ButtonActions> buttonActionList = new List<ButtonActions>();


        public ControlWindow()
        {
            InitializeComponent();
            double buttonSize=GridButton.Width/maxNumberOfButtons;
            for(int i=0;i<maxNumberOfButtons;i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width =new GridLength(buttonSize);
                GridButton.ColumnDefinitions.Add(cd);
            }
            for (int i = 0; i < maxNumberOfButtons; i++)
            {
                Button but = new Button();
                but.Height=25;
                but.Width=buttonSize;
                but.Content=i.ToString();
                Grid.SetColumn(but,i);
                GridButton.Children.Add(but);
                buttonList.Add(but);
                but.Click += but_Click;
                but.Visibility = Visibility.Hidden;
                ButtonActions ba = new ButtonActions();
                ba.ButtonContents = i.ToString();
                ba.ButtonCommand = "RANDBACK";
                buttonActionList.Add(ba);
                //but.IsEnabled=false;
            }
        }

        void but_Click(object sender, RoutedEventArgs e)
        {
            Button sendButton = (Button)sender;
            foreach(ButtonActions ba in buttonActionList)
            {
                if (ba.ButtonContents == (string)(sendButton.Content))
                {
                    string result = transducer.transduce(ba.ButtonCommand);
                    LabelResult.Content = result;
                }
            }
        }

        private void TextBoxCommand_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                string command = TextBoxCommand.Text;
                TextBoxCommand.Text = "";
                if (TextBoxHistory.Text.Length > 0)
                {
                    TextBoxHistory.AppendText("\n");
                }
                TextBoxHistory.AppendText(command);
                TextBoxHistory.SelectionStart = TextBoxHistory.Text.Length;
                TextBoxHistory.ScrollToEnd();
                cmdHistory.Add(command);
                currentCmdHistory = cmdHistory.Count;
                string resultString=transducer.transduce(command);
                LabelResult.Content = resultString;
            }
        }


        Transducer transducer;

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            transducer = new Transducer();
            transducer.setup(mainWindow, this);
            if (File.Exists(transducer.batchDir + "AUTO.txt"))
            {
                transducer.transduce("AUTO");
            }
            TextBoxCommand.Focus();
        }

        private void TextBoxCommand_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                currentCmdHistory--;
                if (currentCmdHistory < 0) currentCmdHistory = 0;
                TextBoxCommand.Text = cmdHistory[currentCmdHistory];
            }
            else if (e.Key == Key.Down)
            {
                currentCmdHistory++;
                if (currentCmdHistory > cmdHistory.Count-1) currentCmdHistory = cmdHistory.Count-1;
                TextBoxCommand.Text = cmdHistory[currentCmdHistory];
            }

        }

        bool commandLineVisable = true;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            commandLineVisable = !commandLineVisable;
            if (commandLineVisable)
            {
                ShowCommandLine();
            }
            else 
            {
                HideCommandLine();
            }

        }

        public void HideCommandLine()
        {
            Height = 102;
            commandLineVisable = false;
        }

        public void ShowCommandLine()
        {
            Height = 300;
            commandLineVisable = true;
            TextBoxCommand.Focus();
        }


        internal void setButton(int p1, string p2, string p3)
        {
            buttonActionList[p1].ButtonContents = p2;
            buttonActionList[p1].ButtonCommand = p3;
            buttonList[p1].Content = p2;
            buttonList[p1].Visibility = Visibility.Visible;
        }

        public bool isNowClosing = false;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isNowClosing = true;
            if (!mainWindow.isNowClosing) mainWindow.Close();
        }

        internal void clearButton(int i)
        {
            buttonList[i].Visibility = Visibility.Hidden;
        }
    }
}
