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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Reyna;

namespace TestHarness
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ReynaService reyna;

        public MainWindow()
        {
            InitializeComponent();
            this.reyna = new ReynaService(null);
        }

        private void startReynaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.reyna.Start();
                MessageBox.Show("Reyna service has started");
                this.reynaStatusLabel.Content = "Service Running";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void stopReynaButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.reyna.Stop();
                MessageBox.Show("Reyna service has stopped");
                this.reynaStatusLabel.Content = "Service Stopped";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void sendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.reyna.Put(new Message(new Uri("http://localhost:62559/post.aspx"), getMessage()));
                MessageBox.Show("Message Sent");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string getMessage()
        {
         return  "<soapenv:Envelope xmlns:soapenv='http://schemas.xmlsoap.org/soap/envelope/' xmlns:tem='http://tempuri.org/'><soapenv:Header/><soapenv:Body><tem:HandleMessage><!--Optional:--><tem:message>7</tem:message></tem:HandleMessage></soapenv:Body></soapenv:Envelope>";
        }
    }
}
