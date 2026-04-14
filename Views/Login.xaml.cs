using ApiInterface;
using Model;

namespace SherioMAUI.Views
{
    public partial class Login : ContentPage
    {
        private readonly ApiService apiClient = new ApiService();

        public Login()
        {
            InitializeComponent();
        }

        private async void Login_Click(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim() ?? "";
            string password = PasswordEntry.Text ?? "";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("שגיאה", "נא למלא אימייל וסיסמה", "אישור");
                return;
            }

            try
            {
                UserList users = await apiClient.GetAllUsersAsync();

                User? foundUser = users.FirstOrDefault(u =>
                    !string.IsNullOrWhiteSpace(u.Email) &&
                    u.Email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
                    u.PassHash == password);

                if (foundUser == null)
                {
                    await DisplayAlert("שגיאה", "אימייל או סיסמה שגויים", "אישור");
                    return;
                }

                App.CurrentUser = foundUser;

                Application.Current!.Windows[0].Page = new NavigationPage(new HomePage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("שגיאה", ex.Message, "אישור");
            }
        }

        private async void Register_Click(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new Register());
        }
    }
}