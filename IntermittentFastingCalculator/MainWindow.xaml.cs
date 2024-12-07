using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IntermittentFastingCalculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Default the date to yesterday
            DatePicker.SelectedDate = DateTime.Now.Date.AddDays(-1);

            // Default the time to 5 PM in 12-hour format
            TimeTextBox.Text = "5:00 PM";
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (DatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Please select a valid date.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Retrieve and parse the selected date and time
                DateTime selectedDate = DatePicker.SelectedDate.Value;
                string timeInput = TimeTextBox.Text;

                if (!DateTime.TryParseExact(
                        timeInput,
                        "h:mm tt",
                        System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.None,
                        out DateTime selectedTime))
                {
                    MessageBox.Show("Please enter a valid time in hh:mm AM/PM format.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Combine the selected date and time
                DateTime selectedDateTime = selectedDate.Date.Add(selectedTime.TimeOfDay);

                // Calculate the difference between now and the selected date/time
                DateTime now = DateTime.Now;
                if (selectedDateTime > now)
                {
                    MessageBox.Show("The entered date and time must be in the past.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                TimeSpan timeDifference = now - selectedDateTime;

                // Display the main result
                ResultTextBlock.Text = $"Time passed: {Math.Floor(timeDifference.TotalHours)} hours and {timeDifference.Minutes} minutes.";

                // Calculate and display the next upcoming interval
                string nextInterval = GetNextUpcomingInterval(selectedDateTime, now);
                NextIntervalTextBlock.Text = nextInterval;

                // Display all intervals
                string allIntervals = GetAllIntervals(selectedDateTime);
                AllIntervalsTextBlock.Text = allIntervals;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private string GetNextUpcomingInterval(DateTime baseTime, DateTime now)
        {
            // Define key intervals
            int[] hours = { 16, 18, 20, 22, 24, 36 };

            foreach (int hour in hours)
            {
                DateTime intervalTime = baseTime.AddHours(hour);
                if (intervalTime > now)
                {
                    TimeSpan timeUntil = intervalTime - now;
                    return $"Next interval: {hour} hours\nTime: {intervalTime:MMM dd, yyyy h:mm tt}\nTime remaining: {timeUntil.Hours} hours and {timeUntil.Minutes} minutes.";
                }
            }

            return "All intervals have passed.";
        }

        private string GetAllIntervals(DateTime baseTime)
        {
            // Define key intervals
            int[] hours = { 16, 18, 20, 22, 24, 36 };
            string result = "All intervals:\n";

            foreach (int hour in hours)
            {
                DateTime intervalTime = baseTime.AddHours(hour);
                result += $"{hour} hours: {intervalTime:MMM dd, yyyy h:mm tt}\n";
            }

            return result.TrimEnd();
        }
    }
}