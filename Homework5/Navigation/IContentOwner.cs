using System.Windows.Controls;

namespace Homework5.Navigation
{
    internal interface IContentOwner
    {
        ContentControl ContentControl { get; }
    }
}