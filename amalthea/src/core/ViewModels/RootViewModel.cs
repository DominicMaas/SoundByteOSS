using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using SoundByte.Core.Services.Definitions;
using System;

namespace SoundByte.Core.ViewModels
{
    /// <summary>
    ///     Contains the tabs host where the user can then navigate to
    ///     other pages throughout the app
    /// </summary>
    public class RootViewModel : MvxNavigationViewModel
    {
        private readonly IDialogService _dialogService;
        private readonly IPlaybackService _playbackService;

        public IMvxAsyncCommand<Type> NavigateCommand { get; private set; }

        public RootViewModel(IMvxLogProvider logProvider, IMvxNavigationService navigationService, IDialogService dialogService, IPlaybackService playbackService) : base(logProvider, navigationService)
        {
            _dialogService = dialogService;
            _playbackService = playbackService;
            NavigateCommand = new MvxAsyncCommand<Type>(x => NavigationService.Navigate(x));
        }

        private int _itemIndex;

        public int ItemIndex
        {
            get => _itemIndex;
            set => SetProperty(ref _itemIndex, value);
        }

        public IDialogService GetDialogService() => _dialogService;

        public IMvxNavigationService GetNavigationService() => NavigationService;

        public IPlaybackService GetPlaybackService() => _playbackService;
    }
}