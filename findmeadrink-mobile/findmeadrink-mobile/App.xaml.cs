using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using findmeadrink_mobile.Services;
using findmeadrink_mobile.Views;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Analytics;

namespace findmeadrink_mobile
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            AppCenter.Start("6120d7d4-dc90-479c-8a19-fad2d704f219",
                               typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
