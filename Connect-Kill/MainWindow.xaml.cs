using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ConnectKill
{
    public partial class MainWindow : Window
    {
        string vpnAddress;
        string ipAddress;
        int appI;
        List<int> selectedKill = new List<int>();
        Process[] appList;
        bool ipCon = false;
        bool keepRunning = false;
        bool cancelRunning = false;
        private readonly BackgroundWorker IpCheck = new BackgroundWorker() { WorkerSupportsCancellation = true };

        public MainWindow()
        {
            InitializeComponent();

            appList = Process.GetProcesses();
            runBar.IsEnabled = true;
            runBar.IsIndeterminate = false;

            foreach (Process a in appList)
            {
                if (!String.IsNullOrEmpty(a.MainWindowTitle) && !a.MainWindowTitle.ToString().Contains("CN=") && !(a.MainWindowTitle.ToString().Replace("?", "").Split('/').Last() + " = ").Contains("Connect-Kill = "))
                {
                    appI = a.Id;
                    listBox.Items.Add(a.MainWindowTitle.ToString().Replace("?", "").Split('/').Last() + " = " + appI);
                }
            }

            Ip_Con();
//            if (ipCon)
//            {
//                runApp.Click += Run_Test;
                runApp.Click += Run_App;
//            }


            ipBlock.Text = "Current ip: " + ipAddress;

            IpCheck.DoWork += IpCheck_DoWork;
            IpCheck.RunWorkerCompleted += IpCheck_RunWorkerCompleted;

            ipSet.Click += Set_Ip;
            refreshButton.Click += Refresh_Ip;

            appRefresh.Click += App_Refresh;
            setApp.Click += Add_App;
            cancelApp.Click += Cancel_Click;
        }



        void Ip_Con()
        {
            try
            {
                ipAddress = new WebClient().DownloadString("http://icanhazip.com").Trim();
                ipCon = true;
            }
            catch
            {
                ipAddress = "Error, no ip connection";
                ipCon = false;
            }
        }

        void Set_Ip(object sender, RoutedEventArgs e)
        {
            if (ipCon)
            {
                vpnAddress = ipAddress;
            }
            else
            {
                vpnAddress = "Error, no ip connection";
            }
            vpnBlock.Text = "Current vpn: " + vpnAddress;
        }

        void Refresh_Ip(object sender, RoutedEventArgs e)
        {
            try
            {
                ipAddress = new WebClient().DownloadString("http://icanhazip.com").Trim();
                ipCon = true;
                ipBlock.Text = "Current ip: " + ipAddress;
                runBar.IsIndeterminate = true;
                runBar.Foreground = null;
            }
            catch
            {
                ipAddress = "Error, no ip connection";
                ipBlock.Text = "Current ip: " + ipAddress;
                ipCon = false;
            }
        }

        void Run_App(object sender, RoutedEventArgs e)
        {
            if (ipCon)
            {
                runApp.IsEnabled = false;
                appRefresh.IsEnabled = false;
                setApp.IsEnabled = false;
                ipSet.IsEnabled = false;
                refreshButton.IsEnabled = false;
                ipSet.Foreground = null;
                refreshButton.Foreground = null;
                runApp.Foreground = null;
                setApp.Foreground = null;
                appRefresh.Foreground = null;
                keepRunning = true;
                runBar.IsIndeterminate = true;
                runBar.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF29DC58");
                IpCheck.RunWorkerAsync();
            }
            else
            {
                Ip_Con();
            }
        }

        void Cancel_Click(object sender, RoutedEventArgs e)
        {
            keepRunning = false;
            cancelRunning = true;
            IpCheck.Dispose();
            appBlock.Text = null;
        }

        void Add_App(object sender, RoutedEventArgs e)
        {
            appBlock.Text += listBox.SelectedItem.ToString().Split(" = ")[0].Trim() + "\r\n";
            selectedKill.Add(int.Parse(listBox.SelectedItem.ToString().Split('=')[1].Split(' ')[1].Trim()));
            appList = Process.GetProcesses();
            listBox.Items.Clear();
            foreach (Process a in appList)
            {
                if (!String.IsNullOrEmpty(a.MainWindowTitle) && !a.MainWindowTitle.ToString().Contains("CN=") && !(a.MainWindowTitle.ToString().Replace("?", "").Split('/').Last() + " = ").Contains("Connect-Kill = "))
                {
                    appI = a.Id;
                    listBox.Items.Add(a.MainWindowTitle.ToString().Replace("?", "").Split('/').Last() + " = " + appI);
                }
            }
            runBar.IsIndeterminate = true;
            runBar.Foreground = null;
        }

        void App_Refresh(object sender, RoutedEventArgs e)
        {
            appList = Process.GetProcesses();
            listBox.Items.Clear();
            foreach (Process a in appList)
            {
                if (!String.IsNullOrEmpty(a.MainWindowTitle) && !a.MainWindowTitle.ToString().Contains("CN=") && !(a.MainWindowTitle.ToString().Replace("?", "").Split('/').Last() + " = ").Contains("Connect-Kill = "))
                {
                    appI = a.Id;
                    listBox.Items.Add(a.MainWindowTitle.ToString().Replace("?", "").Split('/').Last() + " = " + appI);
                }
            }
            runBar.IsIndeterminate = true;
            runBar.Foreground = null;
        }

        void IpCheck_DoWork(object sender, DoWorkEventArgs e)
        {
            while (keepRunning)
            {
                Thread.Sleep(2000);
                try
                {
                    ipAddress = new WebClient().DownloadString("http://icanhazip.com").Trim();
                }
                catch
                {
                    ipAddress = "Disconnected";
                }
                if (ipAddress != vpnAddress)
                {
                    appList = Process.GetProcesses();
                    foreach (Process p in appList)
                    {
                        if (selectedKill.Contains(p.Id))
                        {
                            p.Kill();
                        }
                    }
                    keepRunning = false;
                }
            }
        }

        private void IpCheck_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!cancelRunning)
            {
                vpnBlock.Text = "Vpn was disconnected";
                ipBlock.Text = "Current ip: " + ipAddress;
                runBar.Value = 100;
                runBar.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF890000");
            }
            else
            {
                runBar.Value = 0;
                runBar.Foreground = null;
                cancelRunning = false;
                keepRunning = true;
            }
            runBar.IsIndeterminate = false;
            appBlock.Text = null;
            runApp.IsEnabled = true;
            appRefresh.IsEnabled = true;
            setApp.IsEnabled = true;
            ipSet.IsEnabled = true;
            refreshButton.IsEnabled = true;
            ipSet.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF494949");
            refreshButton.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF494949");
            appRefresh.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF494949");
            setApp.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF494949");
            runApp.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#FF494949");
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}