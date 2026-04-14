using ApiInterface;
using Model;
using System.Collections.ObjectModel;

namespace SherioMAUI.Views;

public partial class MyOrdersPage : ContentPage
{
    private readonly ApiService _api = new ApiService();
    private ObservableCollection<Booking> _myBookings = new();

    public MyOrdersPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadMyBookingsAsync();
    }

    private async Task LoadMyBookingsAsync()
    {
        if (App.CurrentUser == null)
        {
            await DisplayAlert("שגיאה", "יש להתחבר תחילה.", "אישור");
            await Shell.Current.GoToAsync("..");
            return;
        }

        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;
            OrdersCollection.IsVisible = false;
            EmptyLabel.IsVisible = false;

            var allBookings = await _api.GetAllBookingsAsync();

            var myBookings = allBookings
                .Where(b => b.UserID == App.CurrentUser.Id)
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            _myBookings = new ObservableCollection<Booking>(myBookings);
            OrdersCollection.ItemsSource = _myBookings;

            OrdersCollection.IsVisible = _myBookings.Count > 0;
            EmptyLabel.IsVisible = _myBookings.Count == 0;
        }
        catch (Exception ex)
        {
            await DisplayAlert("שגיאה", "שגיאה בטעינת ההזמנות:\n" + ex.Message, "אישור");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private async void CancelBooking_Click(object sender, EventArgs e)
    {
        if (sender is not Button button || button.CommandParameter is not Booking booking)
            return;

        bool confirm = await DisplayAlert(
            "אישור ביטול",
            "האם אתה בטוח שברצונך לבטל את ההזמנה?",
            "כן",
            "לא");

        if (!confirm)
            return;

        try
        {
            var dto = new BookingUpdateDto
            {
                Id = booking.Id,
                AdultCount = booking.AdultCount,
                ChildCount = booking.ChildCount,
                Status = BookingStatus.Cancelled
            };

            await _api.UpdateBookingAsync(dto);

            booking.Status = BookingStatus.Cancelled;

            OrdersCollection.ItemsSource = null;
            OrdersCollection.ItemsSource = _myBookings;

            await DisplayAlert("הצלחה", "ההזמנה בוטלה בהצלחה.", "אישור");
        }
        catch (Exception ex)
        {
            await DisplayAlert("שגיאה", "שגיאה בתהליך הביטול: " + ex.Message, "אישור");
        }
    }

    private async void Back_Click(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}