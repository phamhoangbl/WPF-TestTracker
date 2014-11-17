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

namespace TestTracker.Controls.Messagebox
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : UserControl
    {
        public MessageBox()
        {
            InitializeComponent();
        }

        public void ShowMessage(MessageType messageType, string message)
        {
            _messageGroupBox.Visibility = Visibility.Visible;
            _messageLabel.Content = message;
            switch (messageType)
            {
                case MessageType.Error:
                    {
                        _messageLabel.Foreground = Brushes.Red;
                        break;
                    }
                case MessageType.Warning:
                    {
                        _messageLabel.Foreground = Brushes.Yellow;
                        break;
                    }
                case MessageType.Success:
                    {
                        _messageLabel.Foreground = Brushes.Green;
                        break;
                    }
            }
        }

    }

    public enum MessageType
    {
        Info = 1,
        Warning = 2,
        Success = 3,
        Error = 4
    };
}