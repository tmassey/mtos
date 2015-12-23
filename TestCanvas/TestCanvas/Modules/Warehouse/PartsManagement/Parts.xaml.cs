﻿using System;
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

namespace aXon.Warehouse.Desktop.Modules.Warehouse.PartsManagement
{
    /// <summary>
    /// Interaction logic for Parts.xaml
    /// </summary>
    public partial class Parts : AxonScreen, IDisposable
    {
        public Parts()
        {
            InitializeComponent();
            ScreenName = "Parts Management";
            ModuleName = "Warehouse";
            Loaded += Parts_Loaded;
        }

        void Parts_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        private void MainGrid_OnSelectionChanged_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
