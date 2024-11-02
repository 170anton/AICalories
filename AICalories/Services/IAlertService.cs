using System;
namespace AICalories.Services
{
    public interface IAlertService
    {
        void ShowCustomAlert(string title, string message);
        void ShowError(string message);
        void ShowUnexpectedError();
    }
}

