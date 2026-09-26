using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;
using R3;

namespace CSharpRegexTester.ViewModels;

/// <summary>
/// メインウィンドウViewModel
/// </summary>
public class MainWindowViewModel : ViewModelBase {
	/// <summary>
	/// 対象文字列
	/// </summary>
	public BindableReactiveProperty<string> Text { get; } = new("");

	/// <summary>
	/// 正規表現パターン
	/// </summary>
	public BindableReactiveProperty<string> Format { get; } = new("");

	/// <summary>
	/// 置換文字列
	/// </summary>
	public BindableReactiveProperty<string> Replacement { get; } = new("");

	/// <summary>
	/// マッチ結果
	/// </summary>
	public BindableReactiveProperty<IReadOnlyList<Match>> MatchResult { get; } = new([]);

	/// <summary>
	/// 置換後文字列
	/// </summary>
	public BindableReactiveProperty<string> ReplacedText { get; } = new("");

	/// <summary>
	/// オプション候補
	/// </summary>
	public IReadOnlyList<RegexOptionWithEnabledFlag> CandidateRegexOptions { get; }

	public MainWindowViewModel() {
		this.Text.AddTo(this.CompositeDisposable);
		this.Format.AddTo(this.CompositeDisposable);
		this.Replacement.AddTo(this.CompositeDisposable);
		this.MatchResult.AddTo(this.CompositeDisposable);
		this.ReplacedText.AddTo(this.CompositeDisposable);

		this.Format.EnableValidation(x => {
			if (string.IsNullOrEmpty(x)) {
				return null;
			}
			try {
				_ = new Regex(x);
				return null;
			} catch (Exception ex) {
				return ex;
			}
		});

		this.CandidateRegexOptions = [
			.. Enum.GetValues<RegexOptions>()
				.Select(ro => new RegexOptionWithEnabledFlag(ro).AddTo(this.CompositeDisposable))
		];

		Observable.Merge(
			this.Text.AsUnitObservable(),
			this.Format.AsUnitObservable(),
			this.Replacement.AsUnitObservable(),
			this.CandidateRegexOptions.Select(x => x.Enabled.AsUnitObservable()).Merge()
		)
		.Subscribe(_ => this.Update())
		.AddTo(this.CompositeDisposable);

		this.Update();
	}

	private void Update() {
		if (this.Format.HasErrors) {
			return;
		}

		if (string.IsNullOrEmpty(this.Format.Value)) {
			this.MatchResult.Value = [];
			this.ReplacedText.Value = this.Text.Value;
			return;
		}

		var options = RegexOptions.None;
		foreach (var op in this.CandidateRegexOptions) {
			if (op.Enabled.Value) {
				options |= op.RegexOption;
			}
		}

		try {
			var regex = new Regex(this.Format.Value, options, TimeSpan.FromSeconds(2));
			this.ReplacedText.Value = regex.Replace(this.Text.Value, this.Replacement.Value);
			this.MatchResult.Value = [.. regex.Matches(this.Text.Value).Cast<Match>()];
		} catch (Exception ex) {
			this.ReplacedText.Value = $"[エラー: {ex.Message}]";
			this.MatchResult.Value = [];
		}
	}
}

/// <summary>
/// 正規表現オプションと有効フラグのペア
/// </summary>
public class RegexOptionWithEnabledFlag(RegexOptions regexOption, bool enabled = false) : ObservableObject, IDisposable {
	public RegexOptions RegexOption { get; } = regexOption;

	public string DisplayName => Enum.GetName(this.RegexOption) ?? this.RegexOption.ToString();

	public BindableReactiveProperty<bool> Enabled { get; } = new(enabled);

	public void Dispose() {
		this.Enabled.Dispose();
		GC.SuppressFinalize(this);
	}
}
