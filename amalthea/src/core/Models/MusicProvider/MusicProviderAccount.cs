using MvvmCross.ViewModels;
using System;

namespace SoundByte.Core.Models.MusicProvider
{
    public class MusicProviderAccount : MvxNotifyPropertyChanged
    {
        public MusicProviderAccount(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; }

        /// <summary>
        ///     The name of the music provider
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _name;

        /// <summary>
        ///     What the UI says about this account
        /// </summary>
        public string ConnectedStatus
        {
            get => _connectedStatus;
            set => SetProperty(ref _connectedStatus, value);
        }

        private string _connectedStatus;

        /// <summary>
        ///     Is the account connected
        /// </summary>
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private bool _isConnected;
    }
}
