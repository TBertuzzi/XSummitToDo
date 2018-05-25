using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Plugin.CurrentActivity;
using FFImageLoading.Forms.Droid;
using Acr.UserDialogs;
using Prism;
using Prism.Ioc;
using Plugin.Fingerprint;

namespace XSummitToDo.Droid
{
    [Activity(Label = "XSummitToDo", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);


            global::Xamarin.Forms.Forms.Init(this, bundle);

            FAB.Droid.FloatingActionButtonRenderer.InitControl();
            CrossCurrentActivity.Current.Init(this, bundle);
            CachedImageRenderer.Init(false);
            UserDialogs.Init(this);
            CrossFingerprint.SetCurrentActivityResolver(() => CrossCurrentActivity.Current.Activity);
            LoadApplication(new App(new AndroidInitializer()));
        }

        public class AndroidInitializer : IPlatformInitializer
        {
            public void RegisterTypes(IContainerRegistry container)
            {
                // Register any platform specific implementations
            }
        }
    }
}

