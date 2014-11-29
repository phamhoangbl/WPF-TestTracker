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
using TestTracker.HandleEvents;

namespace TestTracker.Controls.Editor
{
    /// <summary>
    /// Interaction logic for TestQueueEditor.xaml
    /// </summary>
    public partial class TestQueueEditor : UserControl
    {
        public event EventHandler<TextArgs> Feedback;

        public static readonly DependencyProperty SelectedItemProperty =
             DependencyProperty.Register("SelectedItem", typeof(string), typeof(TestQueueEditor));

        public string SelectedItem
        {
            get { return (string)GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public TestQueueEditor()
        {
            InitializeComponent();
        }

        protected void Save_Click(object sender, RoutedEventArgs e)
        {
            RaiseFeedback("Data process starting...");

            string a = SelectedItem;
        }

        private void RaiseFeedback(string p)
        {
            EventHandler<TextArgs> handler = Feedback;
            if (handler != null)
            {
                handler(null, new TextArgs(p));
            }
        }
    }
}
