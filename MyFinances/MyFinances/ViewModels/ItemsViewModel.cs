using MyFinances.Core.Dtos;
using MyFinances.Models;
using MyFinances.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyFinances.ViewModels
{
    public class ItemsViewModel : BaseViewModel
    {
        private int _actualPage = 1;
        private int _records = 3;
       

        public ObservableCollection<OperationDto> Operations { get; }
        public Command LoadItemsCommand { get; }
        public Command AddItemCommand { get; }
        public Command DeleteItemCommand { get; }
        public Command PreviousPageCommand { get;}
        public Command NextPageCommand { get; }
        public Command<OperationDto> ItemTapped { get; }

        public ItemsViewModel()
        {
            Title = "Operacje";

            Operations = new ObservableCollection<OperationDto>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<OperationDto>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);

            NextPageCommand = new Command(async () => await NextPage());

            PreviousPageCommand = new Command(async () => await PreviousPage());

            DeleteItemCommand = new Command<OperationDto>(async (x) => await OnDeleteItem(x));

            VisibleNextPage = true;
           
        }

        private bool _visibleNextPage;

        public bool VisibleNextPage
        {
            get { return _visibleNextPage; }
            set { 
                _visibleNextPage = value;
                OnPropertyChanged(nameof(VisibleNextPage));
            }
        }


        private bool _visiblePreviousPage;

        public bool VisiblePreviousPage
        {
            get { return _visiblePreviousPage; }
            set { 
                _visiblePreviousPage = value;
                OnPropertyChanged(nameof(VisiblePreviousPage));
            }
        }


        private async Task PreviousPage()
        {

            _actualPage--;

            if (_actualPage <= 1)
            {
                VisiblePreviousPage = false;
                VisibleNextPage = true;
            }
            else
                VisibleNextPage = true;

            await ExecuteLoadItemsCommand();
        }

        private async Task NextPage()
        {
            _actualPage++;

            var operation = await OperationService.GetAsync();

            int totalRecords = operation.Data.Count();

            var totalPage = (int)Math.Ceiling(totalRecords / 3.0);

            if (_actualPage >= totalPage)
            {
                VisibleNextPage = false;
                VisiblePreviousPage = true;
            }
            VisiblePreviousPage = true;

            await ExecuteLoadItemsCommand(); 
        }

        private async Task OnDeleteItem(OperationDto operation)
        {
            if (operation == null)
                return;

            var dialog = await Shell.Current.DisplayAlert("Usuwanie!", 
                $"Czy na pewno chcesz usunąć operacje: {operation.Name}?", "Tak", "Nie");

            if (!dialog)
                return;

            var response = await OperationService.DeleteAsync(operation.Id);

            if (!response.IsSuccess)
                await ShowErrorAlert(response);

            await ExecuteLoadItemsCommand();  
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                var response = await OperationService.GetAsync(_records,_actualPage);

                if (!response.IsSuccess)
                    await ShowErrorAlert(response);

                Operations.Clear();

                
                foreach (var item in response.Data)
                {
                    Operations.Add(item);
                }
            }
            catch (Exception exception)
            {
                await Shell.Current.DisplayAlert("Wystąpił Błąd!", exception.Message, "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        async void OnItemSelected(OperationDto operation)
        {
            if (operation == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack
            await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={operation.Id}");
        }
    }
}