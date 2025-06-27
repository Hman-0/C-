using System.Windows;
using Day7.ViewModels;

namespace Day7;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private TodoViewModel _viewModel;
    
    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new TodoViewModel();
        DataContext = _viewModel;
    }
    
    protected override void OnClosed(EventArgs e)
    {
        _viewModel?.Dispose();
        base.OnClosed(e);
    }
}