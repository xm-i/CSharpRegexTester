using CSharpRegexTester.ViewModels;

namespace CSharpRegexTester.Views;

/// <summary>
/// MainWindow.xaml の相互作用ロジック
/// </summary>
public partial class MainWindow {
	public MainWindow(MainWindowViewModel viewModel) {
		this.InitializeComponent();
		this.DataContext = viewModel;
	}
}
