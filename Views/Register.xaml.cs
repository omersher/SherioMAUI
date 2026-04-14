using ApiInterface;
using Model;
using System.Text.RegularExpressions;

namespace SherioMAUI.Views
{
    public partial class Register : ContentPage
    {
        private readonly ApiService apiClient = new ApiService();

        public Register()
        {
            InitializeComponent();
        }

        private async void CreateAccount_Click(object sender, EventArgs e)
        {
            string fullName = FullNameEntry.Text?.Trim() ?? "";
            string guestId = IdEntry.Text?.Trim() ?? "";
            string email = EmailEntry.Text?.Trim() ?? "";
            string password = PasswordEntry.Text ?? "";

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(guestId) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("שגיאה", "נא למלא את כל השדות", "אישור");
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                await DisplayAlert("שגיאה", "כתובת האימייל אינה תקינה", "אישור");
                return;
            }

            try
            {
                User newUser = new User
                {
                    FullName = fullName,
                    GuestID = guestId,
                    Email = email,
                    Phone = "",
                    PassHash = password
                };

                int result = await apiClient.InsertUserAsync(newUser);

                if (result > 0)
                {
                    await DisplayAlert("הצלחה", "נרשמת בהצלחה", "אישור");
                    await Navigation.PushAsync(new Login());
                }
                else
                {
                    await DisplayAlert("שגיאה", "המשתמש כבר קיים במערכת", "אישור");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("שגיאה", ex.Message, "אישור");
            }
        }

        private async void Login_Click(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Login());
        }
    }
}