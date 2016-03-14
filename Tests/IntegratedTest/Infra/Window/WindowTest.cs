﻿using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using HTML_WPF.Component;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace IntegratedTest.Infra.Window
{
    public class WindowTest : IDisposable
    {
        private readonly IWPFWindowWrapper _WPFThreadingHelper;
        private IDispatcher _Dispatcher;

        public WindowTest(IWindowTestEnvironment context, Action<System.Windows.Window> init)
        {
            _WPFThreadingHelper = context.GetWindowWrapper(() => CreateNewWindow(init));
        }

        private System.Windows.Window CreateNewWindow(Action<System.Windows.Window> init)
        {
            var window = new System.Windows.Window();
            NameScope.SetNameScope(window, new NameScope());
            init(window);
            _Dispatcher = new WPFUIDispatcher(window.Dispatcher);
            return window;
        }

        public System.Windows.Window Window { get { return _WPFThreadingHelper.MainWindow; } }

        public Dispatcher Dispatcher { get { return Window.Dispatcher; } }

        public async Task RunOnUIThread(Action Do) 
        {
            await _Dispatcher.RunAsync(Do);
        }

        public async Task RunOnUIThread(Func<Task> Do)
        {
            await await EvaluateOnUIThread(Do);
        }

        public async Task<T> EvaluateOnUIThread<T>(Func<T> Do) 
        {
            return await _Dispatcher.EvaluateAsync(Do);
        }

        public IDispatcher GetDispatcher() 
        {
            return _Dispatcher;
        }

        public void CloseWindow()
        {
            _WPFThreadingHelper.CloseWindow();
        }

        public void Dispose()
        {
            Action End = () => { _WPFThreadingHelper.CloseWindow(); };
            Dispatcher.Invoke(End);
            _WPFThreadingHelper.Dispose();
        }
    }
}