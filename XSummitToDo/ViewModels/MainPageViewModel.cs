using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Realms;
using XSummitToDo.Models;
using System.Linq;
using Prism.Commands;
using Xamarin.Forms.MultiSelectListView;
using Plugin.Fingerprint;
using Acr.UserDialogs;

namespace XSummitToDo.ViewModels
{
    public class MainPageViewModel : BindableBase, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private readonly IPageDialogService _pageDialogService;
        public MultiSelectObservableCollection<Tarefa> Tarefas { get; set; }
        private readonly Realm _realm;

        private bool finalizarTarefa;
        public bool FinalizarTarefa
        {
            get => finalizarTarefa;
            set => SetProperty(ref finalizarTarefa, value);
        }

        private bool incluirTarefa;
        public bool IncluirTarefa
        {
            get => incluirTarefa;
            set => SetProperty(ref incluirTarefa, value);
        }

        private DelegateCommand adicionarTarefaCommand;
        public DelegateCommand AdicionarTarefaCommand => adicionarTarefaCommand ?? (adicionarTarefaCommand = new DelegateCommand(async () => await AdicionarTarefaCommandExecute()));

        private DelegateCommand finalizarTarefaCommand;
        public DelegateCommand FinalizarTarefaCommand => finalizarTarefaCommand ?? (finalizarTarefaCommand = new DelegateCommand(() =>  FinalizarTarefaCommandExecute()));

        private DelegateCommand<SelectableItem> alterarCommand;
        public DelegateCommand<SelectableItem> AlterarCommand => alterarCommand ?? (alterarCommand = new DelegateCommand<SelectableItem>(async (selectableItem) => await AlterarCommandExecute(selectableItem)));

        private DelegateCommand<SelectableItem> apagarCommand;
        public DelegateCommand<SelectableItem> ApagarCommand => apagarCommand ?? (apagarCommand = new DelegateCommand<SelectableItem>((selectableItem) =>  ApagarCommandExecute(selectableItem)));

        private DelegateCommand<SelectableItem> itemTappedCommand;
        public DelegateCommand<SelectableItem> ItemTappedCommand => itemTappedCommand ?? (itemTappedCommand = new DelegateCommand<SelectableItem>((selectableItem) => ItemTappedCommandExecute(selectableItem)));

       

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            Tarefas = new MultiSelectObservableCollection<Tarefa>();
            _navigationService = navigationService;
            _pageDialogService = pageDialogService;

            _realm = Realm.GetInstance(new RealmConfiguration
            {
                ShouldDeleteIfMigrationNeeded = true
            });

            FinalizarTarefa = false;
            IncluirTarefa = true;
        }



        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            CarregarTarefas();
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
            
        }

        private async Task AdicionarTarefaCommandExecute()
        {

            var navigationParams = new NavigationParameters();
            await _navigationService.NavigateAsync("NovaTarefaPage", navigationParams, null, false);
        }

        private void FinalizarTarefaCommandExecute()
        {
            if (Tarefas.SelectedItems.Count() > 0)
            {
                using (var transaction = _realm.BeginWrite())
                {
                    foreach (var tarefa in Tarefas.SelectedItems)
                    {
                        tarefa.Concluido = true;
                        _realm.Add(tarefa);
                    }
                    transaction.Commit();
                }

                UserDialogs.Instance.Toast("Tarefas concluidas", TimeSpan.FromSeconds(5));
                CarregarTarefas();
            }
        }


        private async Task AlterarCommandExecute(SelectableItem selectableItem)
        {
            var tarefa = selectableItem.Data as Tarefa;


            if(tarefa.Protegida)
            {
                var result = await CrossFingerprint.Current.AuthenticateAsync("Tarefa protegida");
                if (result.Authenticated)
                {
                    await NavegarParaEditar(tarefa.Id);
                }
                else
                {
                    UserDialogs.Instance.Toast("Você não pode editar esta tarefa", TimeSpan.FromSeconds(5));
                }
            }
            else
            {
                await NavegarParaEditar(tarefa.Id);
            }

           
        }

        private void ApagarCommandExecute(SelectableItem selectableItem)
        {
            var tarefa = selectableItem.Data as Tarefa;

            var navigationParams = new NavigationParameters();
            navigationParams.Add("IdTarefa", tarefa.Id);

            using (var transaction = _realm.BeginWrite())
            {
                _realm.Remove(tarefa);
                transaction.Commit();
            }

            CarregarTarefas();
        }

        private async Task NavegarParaEditar(string idTarefa)
        {
            var navigationParams = new NavigationParameters();
            navigationParams.Add("TarefaID", idTarefa);

            await _navigationService.NavigateAsync("EditarTarefaPage", navigationParams, null, false);
        }

        private void CarregarTarefas()
        {
            Tarefas = new MultiSelectObservableCollection<Tarefa>(_realm.All<Tarefa>().Where(t => !t.Concluido).OrderBy(t => t.Id));
            RaisePropertyChanged("Tarefas");
        }

        private void ItemTappedCommandExecute(SelectableItem selectableItem)
        {
            if(Tarefas.SelectedItems.Count() > 0)
            {
                IncluirTarefa = false;
                FinalizarTarefa = true;
            }
            else
            {
                IncluirTarefa = true;
                FinalizarTarefa = false;
            }
        }
    }
}
