
namespace Homework5.Navigation
{
    internal enum ViewType
    {
     Main
    }

    interface INavigationModel
    {
        void Navigate(ViewType viewType);
    }
}
