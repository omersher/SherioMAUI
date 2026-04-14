using Model;

namespace SherioMAUI.Views
{
    public partial class ShowResultsPage : ContentPage
    {
        private readonly List<Hotel> _hotels;
        private readonly DateTime _checkIn;
        private readonly DateTime _checkOut;

        public ShowResultsPage(List<Hotel> hotels, DateTime checkIn, DateTime checkOut)
        {
            InitializeComponent();

            _hotels = hotels ?? new List<Hotel>();
            _checkIn = checkIn;
            _checkOut = checkOut;

            HotelsCollectionView.ItemsSource = _hotels;
            ResultsInfoLabel.Text = $"נמצאו {_hotels.Count} מלונות · {_checkIn:dd/MM} - {_checkOut:dd/MM}";
        }

        private async void Back_Click(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void Details_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Hotel selectedHotel)
            {
                App.CurrentHotel = selectedHotel;
                await Navigation.PushAsync(new HotelDetailsPage(selectedHotel));
            }
        }
    }
}