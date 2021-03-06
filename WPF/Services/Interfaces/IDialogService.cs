﻿namespace WPF.Services.Interfaces
{
    internal interface IDialogService
    {
        void ShowDialog<TViewModel>();

        void ShowDialog<TViewModel>(TViewModel instance);
    }
}