using System;
using System.Windows;
using CSharpRegexTester.ViewModels;
using CSharpRegexTester.Views;
using Microsoft.Extensions.DependencyInjection;
using R3;

namespace CSharpRegexTester;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
	public IServiceProvider Services { get; }

	public App() {
		this.Services = ConfigureServices();
	}

	private static IServiceProvider ConfigureServices() {
		var services = new ServiceCollection();

		// ViewModels
		services.AddTransient<MainWindowViewModel>();

		// Views
		services.AddTransient<MainWindow>();

		return services.BuildServiceProvider();
	}

	protected override void OnStartup(StartupEventArgs e) {
		WpfProviderInitializer.SetDefaultObservableSystem(ex => {
			Console.WriteLine("R3 Unhandled Exception: " + ex);
		});

		AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;

		var mainWindow = this.Services.GetRequiredService<MainWindow>();
		mainWindow.Show();

		base.OnStartup(e);
	}

	private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e) {
		if (e.ExceptionObject is Exception ex) {
			Console.WriteLine("集約エラーハンドラ");
			Console.WriteLine(ex);
		} else {
			Console.WriteLine(e);
		}

		MessageBox.Show(
			"不明なエラーが発生しました。アプリケーションを終了します。",
			"エラー",
			MessageBoxButton.OK,
			MessageBoxImage.Error);

		Environment.Exit(1);
	}
}
