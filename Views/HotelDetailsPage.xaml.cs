using ApiInterface;
using Model;

namespace SherioMAUI.Views
{
    public partial class HotelDetailsPage : ContentPage
    {
        private readonly ApiService _apiService = new ApiService();
        private readonly Hotel _hotel;

        public HotelDetailsPage(Hotel hotel)
        {
            InitializeComponent();
            _hotel = hotel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            HotelNameLabel.Text = _hotel.Name;
            HotelCityLabel.Text = _hotel.City?.CityName ?? _hotel.City?.CityName ?? "";
            HotelAddressLabel.Text = $"כתובת: {_hotel.StreetAddress}";
            HotelStarsLabel.Text = $"דירוג: {_hotel.StarRating} כוכבים";
            HotelDatesLabel.Text = $"{App.CheckInDate:dd/MM/yyyy} - {App.CheckOutDate:dd/MM/yyyy}";
            HotelImage.Source = _hotel.MainHotelImageLink;

            PoolBadge.IsVisible = _hotel.HasPool;
            GymBadge.IsVisible = _hotel.HasGym;
            RestaurantBadge.IsVisible = _hotel.HasRestaurant;

            await LoadRoomsAsync();
        }

        private async Task LoadRoomsAsync()
        {
            try
            {
                var rooms = await _apiService.GetRoomsByHotelIdAsync(_hotel.Id);
                RoomsCollectionView.ItemsSource = rooms.ToList();
            }
            catch (Exception ex)
            {
                await DisplayAlert("שגיאה", ex.Message, "אישור");
            }
        }

        private async void OpenRoom_Click(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Room room)
            {
                App.CurrentHotel = _hotel;
                App.CurrentRoom = room;
                await Navigation.PushAsync(new RoomDetailsPage(room));
            }
        }

        private async void Back_Click(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}