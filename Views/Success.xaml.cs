namespace SherioMAUI.Views
{
    public partial class Success : ContentPage
    {
        private readonly int _bookingId;

        public Success(int bookingId)
        {
            InitializeComponent();
            _bookingId = bookingId;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BookingIdLabel.Text = _bookingId.ToString();
        }

        private void BackHome_Click(object sender, EventArgs e)
        {
            Application.Current!.Windows[0].Page = new NavigationPage(new HomePage());
        }
    }
}