using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using findmeadrink_mobile.Services;
using findmeadrink_mobile.Views;

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
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
