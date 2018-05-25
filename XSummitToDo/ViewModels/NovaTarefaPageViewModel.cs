using System;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using Realms;
using Xamarin.Forms;
using XSummitToDo.Models;
using Acr.UserDialogs;
using System.Collections.Generic;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System.IO;
using System.Linq;
using Plugin.LocalNotifications;

namespace XSummitToDo.ViewModels
{
    public class NovaTarefaPageViewModel : BindableBase, INavigationAware
    {
        private readonly INavigationService _navigationService;
        private string _Lembrete { get; set; }
        private Transaction _transaction;
        private readonly Realm _realm;

        private Tarefa tarefa;

        public Tarefa Tarefa
        {
            get => tarefa;
            set => SetProperty(ref tarefa, value);
        }

        private DelegateCommand lembrarCommand;
        public DelegateCommand LembrarCommand => lembrarCommand ?? (lembrarCommand = new DelegateCommand(async () => await LembrarCommandExecute()));

        private DelegateCommand adicionarFotoCommand;
        public DelegateCommand AdicionarFotoCommand => adicionarFotoCommand ?? (adicionarFotoCommand = new DelegateCommand(async () => await AdicionarFotoCommandExecute()));

        private DelegateCommand oKCommand;
        public DelegateCommand OKCommand => oKCommand ?? (oKCommand = new DelegateCommand(OKCommandExecute));



        public NovaTarefaPageViewModel(INavigationService navigationService)
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
            Tarefa = _realm.Add(new Tarefa());
        }

        public void OnNavigatingTo(NavigationParameters parameters)
        {

        }

        private async Task LembrarCommandExecute()
        {

            List<string> botoes = new List<string>() { "10 Segundos", "30 Minutos", "1 Hora", "Amanha" };
            _Lembrete = await UserDialogs.Instance.ActionSheetAsync("Lembrar-me em", "Cancelar", null, null, botoes.ToArray());

        }

        private async Task AdicionarFotoCommandExecute()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                UserDialogs.Instance.Toast("Acesso as fotos não está disponível", TimeSpan.FromSeconds(10));
            }

            try
            {
                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Small
                });

                if (file == null)
                {
                    UserDialogs.Instance.Toast("Acesso as fotos não está disponível", TimeSpan.FromSeconds(10));
                }

                var fileInfo = new FileInfo(file.Path);
                byte[] bytes = File.ReadAllBytes(fileInfo.FullName);

                Tarefa.Anexo = File.ReadAllBytes(fileInfo.FullName);

                RaisePropertyChanged("Imagem");
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.Toast($"Erro:{ex.Message}", TimeSpan.FromSeconds(40));
            }
        }

        private void OKCommandExecute()
        {
            string titulo = Tarefa.Titulo;

            bool lembrete = (!string.IsNullOrEmpty(_Lembrete) && _Lembrete != "Cancelar");

            Tarefa.Agendada = lembrete;
            _transaction.Commit();

            if (lembrete)
            {
                DateTime notificacao = DateTime.Now;
                switch (_Lembrete)
                {
                    case "10 Segundos":
                        notificacao = DateTime.Now.AddSeconds(10);
                        break;
                    case "30 Minutos":
                        notificacao = DateTime.Now.AddMinutes(30);
                        break;
                    case "1 Hora":
                        notificacao = DateTime.Now.AddHours(1);
                        break;
                    case "Amanha":
                        notificacao = DateTime.Now.AddDays(1);
                        break;
                    default:
                        break;
                }

                var id = _realm.All<Tarefa>().ToList().Count() + 1;
                CrossLocalNotifications.Current.Show("Lembrete", titulo, id, notificacao);
            }

            _navigationService.GoBackAsync();
        }
    }
}

