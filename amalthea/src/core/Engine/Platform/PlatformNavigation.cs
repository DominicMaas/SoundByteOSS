using MvvmCross.Navigation;
using SoundByte.Core.Helpers;
using System;

namespace SoundByte.Core.Engine.Platform
{
    public class PlatformNavigation
    {
        private readonly IMvxNavigationService _mvxNavigationService;

        public PlatformNavigation(IMvxNavigationService mvxNavigationService)
        {
            _mvxNavigationService = mvxNavigationService;
        }

        public bool NavigateTo(string viewModel)
        {
            var type = Type.GetType("SoundByte.Core.ViewModels." + viewModel);
            return AsyncHelper.RunSync(async () => await _mvxNavigationService.Navigate(type));
        }

        public bool NavigateTo(string viewModel, object param)
        {
            var type = Type.GetType("SoundByte.Core.ViewModels." + viewModel);
            return AsyncHelper.RunSync(async () => await _mvxNavigationService.Navigate(type, param));
        }
    }
}