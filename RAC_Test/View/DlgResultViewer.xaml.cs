using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RAC_Test.View
{
    /// <summary>
    /// DlgResultViewer.xaml 的交互逻辑
    /// </summary>
    public partial class DlgResultViewer : Window
    {
        string fileNameAndPath = "";
        public DlgResultViewer(string filePath)
        {
            InitializeComponent();
            fileNameAndPath = filePath;
        }

        private void excelViewerForWpf_Loaded(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(fileNameAndPath) || !System.IO.File.Exists(fileNameAndPath))
            {
                MessageBox.Show("未发现测试数据!", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                this.excelViewerForWpf.CloseFile();
                this.excelViewerForWpf.OpenFile(fileNameAndPath);
                return;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.excelViewerForWpf.CloseFile();
        }
    }
}
