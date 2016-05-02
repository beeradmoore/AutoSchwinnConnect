using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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

namespace AutoSchwinnConnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ManagementEventWatcher _insertWatcher = null;
        ManagementEventWatcher _removeWatcher = null;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _insertWatcher.Stop();
            _insertWatcher.Dispose();
            _insertWatcher = null;

            _removeWatcher.Stop();
            _removeWatcher.Dispose();
            _removeWatcher = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WqlEventQuery insertQuery = new WqlEventQuery("SELECT * FROM __InstanceCreationEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            _insertWatcher = new ManagementEventWatcher(insertQuery);
            _insertWatcher.EventArrived += new EventArrivedEventHandler(DeviceInsertedEvent);
            _insertWatcher.Start();

            WqlEventQuery removeQuery = new WqlEventQuery("SELECT * FROM __InstanceDeletionEvent WITHIN 2 WHERE TargetInstance ISA 'Win32_USBHub'");
            _removeWatcher = new ManagementEventWatcher(removeQuery);
            _removeWatcher.EventArrived += new EventArrivedEventHandler(DeviceRemovedEvent);
            _removeWatcher.Start();
        }

        private void DeviceInsertedEvent(object sender, EventArrivedEventArgs e)
        {
            Debug.WriteLine("DeviceInsertedEvent");

            // Needs to be updated 
            string knownFile = "BRAD1.DAT";
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType == DriveType.Removable)
                {
                    try
                    {
                        if (drive.IsReady == false)
                        {

                            int sleepCount = 5;
                            do
                            {
                                if (drive.IsReady == false)
                                {
                                    Thread.Sleep(2000);
                                }
                            } while (sleepCount > 0 && drive.IsReady == false);

                            if (drive.IsReady == false)
                            {
                                Debug.WriteLine("Device never become available.");
                                break;
                            }
                        }

                        string dataFile = System.IO.Path.Combine(drive.Name, knownFile);
                        if (File.Exists(dataFile))
                        {
                            string data = File.ReadAllText(dataFile);
                            Debug.WriteLine(data);
                        }
                    }
                    catch (Exception err)
                    {
                        Debug.WriteLine($"Error: {err.Message}");
                    }
                }
            }
            // this.backgroundWorker1.RunWorkerAsync();
        }
        void DeviceRemovedEvent(object sender, EventArrivedEventArgs e)
        {
            Debug.WriteLine("DeviceRemovedEvent");
            //  this.backgroundWorker1.RunWorkerAsync();
        }
    }
}
