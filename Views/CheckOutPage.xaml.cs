using ApiInterface;
using Model;

namespace SherioMAUI.Views
{
    public partial class CheckOutPage : ContentPage
    {
        private readonly ApiService _apiService = new ApiService();

        public CheckOutPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (App.CurrentHotel == null || App.CurrentRoom == null)
                return;

            HotelSummaryLabel.Text = $"מלון: {App.CurrentHotel.Name}";
            RoomSummaryLabel.Text = $"חדר: {App.CurrentRoom.RoomName}";
            DatesSummaryLabel.Text = $"תאריכים: {App.CheckInDate:dd/MM/yyyy} - {App.CheckOutDate:dd/MM/yyyy}";
            GuestsSummaryLabel.Text = $"אורחים: {App.Adults} מבוגרים, {App.Children} ילדים";
            PriceSummaryLabel.Text = $"סה״כ לתשלום: ₪{CalculateTotal():N0}";
        }

        private decimal CalculateTotal()
        {
            if (App.CurrentRoom == null)
                return 0;

            int nights = (App.CheckOutDate.Date - App.CheckInDate.Date).Days;
            if (nights < 1)
                nights = 1;

            decimal pricePerNight = (App.CurrentRoom.AdultRate * App.Adults) + (App.CurrentRoom.ChildRate * App.Children);
            return pricePerNight * nights;
        }

        private async void ConfirmOrder_Click(object sender, EventArgs e)
        {
            if (App.CurrentUser == null || App.CurrentRoom == null)
            {
                await DisplayAlert("שגיאה", "חסרים פרטי משתמש או חדר", "אישור");
                return;
            }

            if (string.IsNullOrWhiteSpace(CardNameEntry.Text) ||
                string.IsNullOrWhiteSpace(CardNumberEntry.Text) ||
                string.IsNullOrWhiteSpace(ExpiryEntry.Text) ||
                string.IsNullOrWhiteSpace(CvvEntry.Text))
            {
                await DisplayAlert("שגיאה", "נא למלא את כל פרטי התשלום", "אישור");
                return;
            }

            try
            {
                var booking = new Booking
                {
                    UserID = App.CurrentUser.Id,
                    RoomID = App.CurrentRoom.Id,
                    CreatedAt = DateTime.Now,
                    StartDate = App.CheckInDate,
                    EndDate = App.CheckOutDate,
                    AdultCount = App.Adults,
                    ChildCount = App.Children,
                    Status = BookingStatus.Pending
                };

                int bookingId = await _apiService.InsertBookingAsync(booking);

                await Navigation.PushAsync(new Success(bookingId));
            }
            catch (Exception ex)
            {
                await DisplayAlert("שגיאה", ex.Message, "אישור");
            }
        }

        private async void Back_Click(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}