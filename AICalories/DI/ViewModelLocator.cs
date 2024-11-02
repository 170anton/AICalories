﻿using System;
using AICalories.Interfaces;
using AICalories.Services;
using AICalories.ViewModels;
using CommunityToolkit.Maui.Views;

namespace AICalories.DI
{
	public class ViewModelLocator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IImageInfo _imageInfo;

        public ViewModelLocator(IServiceProvider serviceProvider, IImageInfo imageInfo)
        {
            _serviceProvider = serviceProvider;
            _imageInfo = imageInfo;
        }

        public MainVM GetMainViewModel() => _serviceProvider.GetRequiredService<MainVM>();
        public AppSettingsVM GetAppSettingsViewModel() => _serviceProvider.GetRequiredService<AppSettingsVM>();
        public HistoryVM GetHistoryViewModel() => _serviceProvider.GetRequiredService<HistoryVM>();

        public TakeImageVM GetTakeImageViewModel(CameraView cameraView)
        {
            var cameraService = new CameraService(cameraView);
            var viewModelService = _serviceProvider.GetService<IViewModelService>();
            var imageInfo = _serviceProvider.GetService<IImageInfo>();
            var navigationService = _serviceProvider.GetService<INavigationService>();
            var alertService = _serviceProvider.GetService<IAlertService>();
            var adService = _serviceProvider.GetService<IAdService>();

            return new TakeImageVM(viewModelService, imageInfo, cameraService, navigationService, alertService, adService);
        }
        public ContextVM GetContextViewModel() => _serviceProvider.GetRequiredService<ContextVM>();
        public ResultVM GetResultViewModel() => _serviceProvider.GetRequiredService<ResultVM>();
        public MealInfoVM GetMealInfoViewModel() => _serviceProvider.GetRequiredService<MealInfoVM>();
    }
}

