using System;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Realms;
using XSummitToDo.Models;

namespace XSummitToDo.ViewModels
{
    public class EditarTarefaViewModel: BindableBase, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private string _Lembrete { get; set; }
        private Transaction _transaction;
        private readonly Realm _realm;

        private DelegateCommand oKCommand;
        public DelegateCommand OKCommand => oKCommand ?? (oKCommand = new DelegateCommand(OKCommandExecute));


        private Tarefa tarefa;

        public Tarefa Tarefa
        {
            get => tarefa;
            set => SetProperty(ref tarefa, value);
        }
        public EditarTarefaViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            _realm = Realm.GetInstance(new RealmConfiguration
            {
                ShouldDeleteIfMigrationNeeded = true
            });
            _transaction = _realm.BeginWrite();
        }

        public void OnNavigatedFrom(NavigationParameters parameters)
        {
            var navigationMode = parameters.InternalParameters["__NavigationMode"].ToString();
            if (navigationMode == "Back" && _realm.IsInTransaction)
            {
                _transaction.Dispose();
            }
        }

        public void OnNavigatedTo(NavigationParameters parameters)
        {
            string tarefaID = string.Empty;
            if (parameters.ContainsKey("TarefaID"))
            {
                tarefaID = parameters["TarefaID"].ToString();
            }

            if (!string.IsNullOrEmpty(tarefaID))
            {
                Tarefa = _realm.Find<Tarefa>(tarefaID);
            }
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {
           
        }

        private void OKCommandExecute()
        {
            _transaction.Commit();

            _navigationService.GoBackAsync();
        }
    }
}
