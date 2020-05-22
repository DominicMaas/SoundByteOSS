using GalaSoft.MvvmLight.Ioc;
using SoundByte.Core;
using SoundByte.Core.Items.Generic;
using SoundByte.Core.Services;
using SoundByte.Core.Sources.SoundByte;
using SoundByte.App.Uwp.Helpers;
using SoundByte.App.Uwp.Services;
using SoundByte.App.Uwp.ServicesV2;
using SoundByte.App.Uwp.ViewModels;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SoundByte.App.Uwp.Views
{
    public sealed partial class HistoryView
    {
        public SoundByteCollection<SoundByteHistorySource> History { get; } = new SoundByteCollection<SoundByteHistorySource>();

        public HistoryView()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SimpleIoc.Default.GetInstance<ITelemetryService>().TrackPage("History View");
        }

        public async void PlayShuffleItems()
        {
            await BaseViewModel.ShufflePlayAllTracksAsync(History);
        }

        public async void PlayAllItems()
        {
            await BaseViewModel.PlayAllTracksAsync(History);
        }

        public async void ClearAll()
        {
            if (!SoundByteService.Current.IsSoundByteAccountConnected)
            {
                await NavigationService.Current.CallMessageDialogAsync("You must login with your SoundByte Account to do this.", "Delete History Error");
                return;
            }

            try
            {
                await SoundByteService.Current.PostAsync<object>(ServiceTypes.SoundByte, "/history/delete-all", string.Empty);
                History.RefreshItems();
            }
            catch (Exception ex)
            {
                await NavigationService.Current.CallMessageDialogAsync(ex.Message, "Delete History Error");
            }
        }

        public async void PlayItem(object sender, ItemClickEventArgs e)
        {
            await BaseViewModel.PlayAllTracksAsync(History, ((BaseSoundByteItem)e.ClickedItem).Track);
        }
    }
}