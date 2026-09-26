using System;
using CommunityToolkit.Mvvm.ComponentModel;
using R3;

namespace CSharpRegexTester.ViewModels;

/// <summary>
/// ViewModel基底クラス
/// </summary>
public abstract class ViewModelBase : ObservableObject, IDisposable {
	private CompositeDisposable? _compositeDisposable;

	/// <summary>
	/// まとめて破棄するための CompositeDisposable
	/// </summary>
	public CompositeDisposable CompositeDisposable => this._compositeDisposable ??= [];

	public void Dispose() {
		this.Dispose(true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing) {
		if (disposing) {
			this._compositeDisposable?.Dispose();
		}
	}
}
