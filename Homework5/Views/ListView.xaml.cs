using Homework5.Navigation;
using System.Windows.Controls;



namespace Homework5.Views
{
    /// <summary>
    /// Логика взаимодействия для ListView.xaml
    /// </summary>
    public partial class ListView : UserControl, INavigatable
    {
        public ListView()
        {
            InitializeComponent();
            DataContext = new ListViewModel();
        }
    }
}
