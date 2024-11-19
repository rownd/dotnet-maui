using Rownd.Maui;
using RowndMauiExample.Services;

namespace RowndMauiExample
{
    public partial class App : Application
    {
        public RowndInstance Rownd { get; private set; }

        public App()
        {
            var config = new global::Rownd.Maui.Core.Config
            {
                ApiUrl = "https://api.dev.rownd.io",
                HubUrl = "https://hub.dev.rownd.io"
            };
            Rownd = RowndInstance.GetInstance(this, config);
 
            Rownd.OnAuthenticated += (sender, state) =>
            {
                Console.WriteLine("Authenticated!");
            };

            Rownd.Configure("YOUR_APP_KEY");

            DependencyService.RegisterSingleton<IRowndInstance>(Rownd);

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
