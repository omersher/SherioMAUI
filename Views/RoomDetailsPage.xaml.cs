using Model;

namespace SherioMAUI.Views
{
    public partial class RoomDetailsPage : ContentPage
    {
        private readonly Room _room;

        public RoomDetailsPage(Room room)
        {
            InitializeComponent();
            _room = room;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            RoomNameLabel.Text = _room.RoomName;
            RoomHotelLabel.Text = App.CurrentHotel?.Name ?? "";
            AdultRateLabel.Text = $"₪{_room.AdultRate:N0}";
            ChildRateLabel.Text = $"₪{_room.ChildRate:N0}";
            RoomInfoLabel.Text = $"חדרי שינה: {_room.Bedrooms} | חדרי רחצה: {_room.Bathrooms}";
            RoomFeaturesLabel.Text = BuildFeaturesText();
            StayDatesLabel.Text = $"תאריכים: {App.CheckInDate:dd/MM/yyyy} - {App.CheckOutDate:dd/MM/yyyy}";
            GuestsLabel.Text = $"אורחים: {App.Adults} מבוגרים, {App.Children} ילדים";
            TotalEstimateLabel.Text = $"סה״כ משוער: ₪{CalculateTotal():N0}";
            RoomImage.Source = App.CurrentHotel?.MainHotelImageLink;
        }

        private string BuildFeaturesText()
        {
            var features = new List<string>();

            if (_room.HasKitchen)
                features.Add("מטבחון");

            if (_room.HasParking)
                features.Add("חניה");

            if (_room.HasBalcony)
                features.Add("מרפסת");

            if (HasLivingRoom())
                features.Add("סלון");

            if (features.Count == 0)
                return "ללא מאפיינים מיוחדים";

            return "מאפיינים: " + string.Join(", ", features);
        }

        private bool HasLivingRoom()
        {
            var prop1 = _room.GetType().GetProperty("HasLivingRoom");
            if (prop1 != null && prop1.PropertyType == typeof(bool))
                return (bool)(prop1.GetValue(_room) ?? false);

            var prop2 = _room.GetType().GetProperty("HasLivingR");
            if (prop2 != null && prop2.PropertyType == typeof(bool))
                return (bool)(prop2.GetValue(_room) ?? false);

            return false;
        }

        private decimal CalculateTotal()
        {
            int nights = (App.CheckOutDate.Date - App.CheckInDate.Date).Days;
            if (nights < 1)
                nights = 1;

            decimal pricePerNight = (_room.AdultRate * App.Adults) + (_room.ChildRate * App.Children);
            return pricePerNight * nights;
        }

        private async void ContinueToCheckout_Click(object sender, EventArgs e)
        {
            App.CurrentRoom = _room;
            await Navigation.PushAsync(new CheckOutPage());
        }

        private async void Back_Click(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}