using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Text;
using Caliburn.Micro;
using Gemini.Framework;
using Gemini.Framework.Services;
using Gemini.Modules.Output.Views;
using Gemini.Modules.Output.Properties;

namespace Gemini.Modules.Output.ViewModels
{
	[Export(typeof(IOutput))]
	public class OutputViewModel : Tool, IOutput
	{
		private readonly StringBuilder _stringBuilder;
		private readonly object _syncRoot = new object();
		private readonly OutputWriter _writer;
		private IOutputView _view;
		private int _viewTextLength;
		private bool _flushPending;

        public bool AutoScrollEnd
        {
            get { return _view.AutoScrollEnd; }
            set
            {
                _view.AutoScrollEnd = value;
                NotifyOfPropertyChange(() => AutoScrollEnd);
            }
        }

        public override PaneLocation PreferredLocation
		{
			get { return PaneLocation.Bottom; }
		}

		public TextWriter Writer
		{
			get { return _writer; }
		}

		public OutputViewModel()
		{
		    DisplayName = Resources.OutputDisplayName;
			_stringBuilder = new StringBuilder();
			_writer = new OutputWriter(this);
		}

        public void ToggleAutoScrollEnd()
        {
            AutoScrollEnd = !AutoScrollEnd;
        }

        public void Clear()
		{
			IOutputView view;
			lock (_syncRoot)
			{
				_stringBuilder.Clear();
				view = _view;
			}

			if (view == null)
				return;

			Execute.OnUIThread(() =>
			{
				view.Clear();

				lock (_syncRoot)
					_viewTextLength = 0;

				// Text appended while the buffer was being cleared has to be flushed again.
				ScheduleFlush();
			});
		}

		public void AppendLine(string text)
		{
			Append(text);
			Append(Environment.NewLine);
		}

		// Appends are buffered here and pushed to the view as deltas, never by replacing the
		// whole view content: replacing the content made the cost of every log line grow with
		// the total size of the log (quadratic overall) and blocked the calling thread on the
		// UI thread. Flushes are coalesced, so a burst of appends costs a single view update.
		public void Append(string text)
		{
			lock (_syncRoot)
				_stringBuilder.Append(text);

			ScheduleFlush();
		}

		private void ScheduleFlush()
		{
			lock (_syncRoot)
			{
				if (_flushPending || _view == null)
					return;

				_flushPending = true;
			}

			Execute.BeginOnUIThread(FlushToView);
		}

		private void FlushToView()
		{
			IOutputView view;
			string pendingText;

			lock (_syncRoot)
			{
				_flushPending = false;

				view = _view;
				var totalLength = _stringBuilder.Length;
				if (view == null || totalLength <= _viewTextLength)
					return;

				pendingText = _stringBuilder.ToString(_viewTextLength, totalLength - _viewTextLength);
				_viewTextLength = totalLength;
			}

			view.AppendText(pendingText);
		}

		protected override void OnViewLoaded(object view)
		{
			var outputView = (IOutputView) view;

			lock (_syncRoot)
			{
				_view = outputView;
				_viewTextLength = _stringBuilder.Length;
			}

			outputView.SetText(_stringBuilder.ToString());
			outputView.ScrollToEnd();
		}
	}
}
