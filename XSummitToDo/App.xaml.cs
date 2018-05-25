using System;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XSummitToDo.ViewModels;
using XSummitToDo.Views;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace XSummitToDo
{
    public partial class App : PrismApplication
    {
        /* 
        * The Xamarin Forms XAML Previewer in Visual Studio uses System.Activator.CreateInstance.
        * This imposes a limitation in which the App class must have a default constructor. 
        * App(IPlatformInitializer initializer = null) cannot be handled by the Activator.
        */
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");

        }

        protected override void OnStart()
        {
            base.OnStart();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage,MainPageViewModel>();
            containerRegistry.RegisterForNavigation<NovaTarefaPage,NovaTarefaPageViewModel>();
            containerRegistry.RegisterForNavigation<EditarTarefaPage, EditarTarefaViewModel>();

        }
    }
}
