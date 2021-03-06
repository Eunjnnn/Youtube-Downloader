﻿namespace WPF.Services
{
    using System.Collections.Generic;
    using System.Windows;

    using Caliburn.Micro;

    using WPF.Services.Interfaces;

    internal class DialogService : IDialogService
    {
        private static readonly Dictionary<string, object> DefaultSettings = new Dictionary<string, object>
        {
            ["ResizeMode"] = ResizeMode.CanMinimize,
            ["ShowInTaskbar"] = false,
            ["WindowStartupLocation"] = WindowStartupLocation.CenterScreen
        };

        private readonly IWindowManager _windowManager;

        public DialogService(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public void ShowDialog<TViewModel>()
        {
            TViewModel viewModel = IoC.Get<TViewModel>();
            ShowDialog(viewModel);
        }

        public void ShowDialog<TViewModel>(TViewModel viewModel)
        {
            _windowManager.ShowDialog(viewModel, default, DefaultSettings);
        }
    }
}