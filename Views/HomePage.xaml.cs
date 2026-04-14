using ApiInterface;
using Model;

namespace SherioMAUI.Views
{
    public partial class HomePage : ContentPage
    {
        private readonly ApiService _apiService = new ApiService();
        private int adults = 2;
        private int children = 0;
        private List<City> _cities = new();

        public HomePage()
        {
            InitializeComponent();

            CheckInPicker.Date = DateTime.Today;
            CheckOutPicker.Date = DateTime.Today.AddDays(1);
            CheckInPicker.MinimumDate = DateTime.Today;
            CheckOutPicker.MinimumDate = DateTime.Today.AddDays(1);

            UpdateGuestsUI();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (App.CurrentUser != null)
                WelcomeLabel.Text = $"שלום {App.CurrentUser.FullName}";

            await LoadCitiesAsync();
        }

        private async Task LoadCitiesAsync()
        {
            try
            {
                var cities = await _apiService.GetAllCitiesAsync();
                _cities = cities.ToList();
                CityPicker.ItemsSource = _cities;
            }
            catch (Exception ex)
            {
                await DisplayAlert("שגיאה", $"בעיה בטעינת ערים: {ex.Message}", "אישור");
            }
        }

        private void UpdateGuestsUI()
        {
            AdultsCountLabel.Text = adults.ToString();
            ChildrenCountLabel.Text = children.ToString();
            GuestsSummaryLabel.Text = $"{adults} מבוגרים, {children} ילדים";

            App.Adults = adults;
            App.Children = children;
        }

        private void PlusAdults_Click(object sender, EventArgs e)
        {
            adults++;
            UpdateGuestsUI();
        }

        private void MinusAdults_Click(object sender, EventArgs e)
        {
            if (adults > 1)
                adults--;

            UpdateGuestsUI();
        }

        private void PlusChildren_Click(object sender, EventArgs e)
        {
            children++;
            UpdateGuestsUI();
        }

        private void MinusChildren_Click(object sender, EventArgs e)
        {
            if (children > 0)
                children--;

            UpdateGuestsUI();
        }

        private async void Search_Click(object sender, EventArgs e)
        {
            if (App.CurrentUser == null)
            {
                await DisplayAlert("שגיאה", "עליך להתחבר כדי לבצע חיפוש", "אישור");
                Application.Current!.Windows[0].Page = new NavigationPage(new Login());
                return;
            }

            if (CityPicker.SelectedItem == null)
            {
                await DisplayAlert("שגיאה", "נא לבחור עיר", "אישור");
                return;
            }

            if (CheckInPicker.Date >= CheckOutPicker.Date)
            {
                await DisplayAlert("שגיאה", "תאריך יציאה חייב להיות אחרי תאריך כניסה", "אישור");
                return;
            }

            try
            {
                var selectedCity = (City)CityPicker.SelectedItem;

                App.CheckInDate = CheckInPicker.Date;
                App.CheckOutDate = CheckOutPicker.Date;

                var hotels = await _apiService.GetAllHotelsAsync();

                var filteredHotels = hotels
                    .Where(h => h.City != null && h.City.Id == selectedCity.Id)
                    .ToList();

                if (filteredHotels.Count == 0)
                {
                    await DisplayAlert("לא נמצאו תוצאות", "לא נמצאו מלונות בעיר שנבחרה", "אישור");
                    return;
                }

                await Navigation.PushAsync(new ShowResultsPage(filteredHotels, CheckInPicker.Date, CheckOutPicker.Date));
            }
            catch (Exception ex)
            {
                await DisplayAlert("שגיאה", ex.Message, "אישור");
            }
        }

        private async void MyOrders_Click(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyOrdersPage());
        }

        private void Logout_Click(object sender, EventArgs e)
        {
            App.CurrentUser = null;
            Application.Current!.Windows[0].Page = new NavigationPage(new Login());
        }
    }
}