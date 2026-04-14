using Model;

namespace SherioMAUI
{
    public partial class App : Application
    {
        public static User? CurrentUser { get; set; }
        public static DateTime CheckInDate { get; set; }
        public static DateTime CheckOutDate { get; set; }
        public static int Adults { get; set; }
        public static int Children { get; set; }
        public static Hotel? CurrentHotel { get; set; }
        public static Room? CurrentRoom { get; set; }

        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new NavigationPage(new Views.Login()));
            window.Width = 400;
            window.Height = 800;
            return window;
        }
    }
}