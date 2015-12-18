using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using InfloCommon.Models;

namespace RWProbeSimulator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private HttpClient _client;
		private RWProbeModel _probe;
		private DispatcherTimer _oneSecondTimer = new DispatcherTimer();
		private int _uploadCount;

		public MainWindow()
		{
			InitializeComponent();

			_probe = new RWProbeModel
			{
				NomadicDeviceId = "12321",
				DateGenerated = DateTime.Now,
				AirTemperature = 70,
				AtmosphericPressure = 50,
				Speed = 65.4,
				SteeringWheelAngle = 90.4f,
				WiperStatus = "On",
				RightFrontWheelSpeed = 20.1f,
				LeftFrontWheelSpeed = 20.4f,
				LeftRearWheelSpeed = 20.2f,
				RightRearWheelSpeed = 20.8f,
				GpsHeading = 14.0f,
				GpsLatitude = 33.9833f,
				GpsLongitude = -82.9833f,
				GpsElevation = 100.45f,
				GpsSpeed = 64.8f,
				CVQueuedStatus = false,
				LatAccel = 10.0f,
				LongAccel = 11.0f,
			};

			ProbePropertyGrid.SelectedObject = _probe;

			_oneSecondTimer.Tick += (sender, args) => Send();
			_oneSecondTimer.Interval = TimeSpan.FromSeconds(1.0);
		}

		private void LogLine(string text)
		{
			LogTextBox.Text += string.Format("[{0}] {1}\n", DateTime.Now.ToString("hh:mm:ss"), text);
			LogTextBox.ScrollToEnd();
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			var baseAddress = BaseAddressTextBox.Text.Trim();
			if (string.IsNullOrEmpty((baseAddress)))
			{
				LogLine("Invalid Base Address.");
				return;
			}
			if (!baseAddress.EndsWith("/"))
				baseAddress += "/";

			try
			{
				_client = new HttpClient();
				_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
				_client.BaseAddress = new Uri(baseAddress);

				LogLine("Created HTTP Client at: " + _client.BaseAddress);
			}
			catch (Exception ex)
			{
				LogLine("Error creating HTTP Client: " + ex.Message);
				return;
			}

			StartButton.IsEnabled = false;
			StopButton.IsEnabled = true;

			LogLine("Started Uploads.");
			_oneSecondTimer.Start();
		}

		private void StopButton_Click(object sender, RoutedEventArgs e)
		{
			StartButton.IsEnabled = true;
			StopButton.IsEnabled = false;

			LogLine("Stopped Uploads.");
			_oneSecondTimer.Stop();
		}

		private async void Send()
		{
			_probe.DateGenerated = DateTime.Now;

			var probeList = new List<RWProbeModel> { _probe };

			try
			{
				var response = await _client.PostAsJsonAsync("RWProbe", probeList);

				if (response.IsSuccessStatusCode)
				{
					_uploadCount++;
					UploadCountTextBlock.Text = _uploadCount.ToString();
				}
				else
					LogLine("Error sending probe data: " + response.ReasonPhrase);

				
			}
			catch (Exception ex)
			{
				LogLine("Error Sending: " + ex.Message);
			}
		}
	}
}
