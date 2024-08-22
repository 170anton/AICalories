using System;
namespace AICalories.Services
{
    public interface IAlertService
    {
        void ShowError(string message);
        void ShowUnexpectedError();
    }
}

