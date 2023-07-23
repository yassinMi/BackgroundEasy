using Mi.UI;
using Mi.ViewModel;
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

namespace Mi.View
{
    /// <summary>
    /// Interaction logic for PromptWindow.xaml
    /// </summary>
    public partial class PromptWindow : Window
    {
        public PromptWindow()
        {
            InitializeComponent();
        }

        public static MessageBoxImage PromptTypeToMsgBoxIcon(PromptType t)
        {
            switch (t)
            {
                case PromptType.Error:
                    return MessageBoxImage.Error;
                case PromptType.Warning:
                    return MessageBoxImage.Warning;
                case PromptType.Information:
                    return MessageBoxImage.Information;
                case PromptType.Question:
                    return MessageBoxImage.Question;
                default:
                    return MessageBoxImage.None;
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == PromptWindow.DataContextProperty)
            {
                ((PromptWindowVM)e.NewValue).WindowObj = this;
            }
        }
    }
}
