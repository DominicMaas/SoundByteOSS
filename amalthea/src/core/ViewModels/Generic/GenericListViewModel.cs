using MvvmCross.ViewModels;
using SoundByte.Core.Models.Core;
using SoundByte.Core.Sources;
using System.Threading;
using System.Threading.Tasks;
using MvvmCross.Commands;
using SoundByte.Core.Models.Media;
using static SoundByte.Core.ViewModels.Generic.GenericListViewModel;

namespace SoundByte.Core.ViewModels.Generic
{
    /// <summary>
    ///     View model used on all list view items.
    /// </summary>
    public class GenericListViewModel : MvxViewModel<Holder>
    {
        #region Getters and Setters

        /// <summary>
        ///     The title to display on the list
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private string _title;

        /// <summary>
        ///     The collection of items
        /// </summary>
        public SoundByteCollection<ISource> Model
        {
            get => _model;
            set => SetProperty(ref _model, value);
        }

        private SoundByteCollection<ISource> _model;

        #endregion Getters and Setters

        public IMvxAsyncCommand RefreshCommand { get; }
        public IMvxAsyncCommand<Media> InvokeCommand { get; }

        public GenericListViewModel()
        {
            RefreshCommand = new MvxAsyncCommand(async () => await Model.RefreshItemsAsync());
            InvokeCommand = new MvxAsyncCommand<Media>(async media => await Helpers.ItemInvokeHelper.InvokeItem(Model, media));
        }

        public override void Prepare(Holder parameter)
        {
            Title = parameter.Title;
            Model = parameter.Model;
        }

        public override async Task Initialize()
        {
            await Model.LoadMoreItemsAsync(25, CancellationToken.None);
        }

        public class Holder
        {
            /// <summary>
            ///     Item source
            /// </summary>
            public SoundByteCollection<ISource> Model { get; }

            /// <summary>
            ///     Title
            /// </summary>
            public string Title { get; }

            /// <summary>
            ///     Create a new view model
            /// </summary>
            /// <param name="model">The item source</param>
            /// <param name="title">Title to display</param>
            public Holder(SoundByteCollection<ISource> model, string title)
            {
                Model = model;
                Title = title;
            }
        }
    }
}