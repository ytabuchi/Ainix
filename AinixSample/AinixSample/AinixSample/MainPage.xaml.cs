using Newtonsoft.Json;
using Sockets.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AinixSample
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            picker.ItemsSource = new string[] { "Localhost", "Azure" };
        }

        async void SocketButton_Clicked(object sender, EventArgs e)
        {
            var address = "10.0.2.2";
            var port = 2001;
            var sendString = $"{Name.Text}," +
                $"{Product.Text}," +
                $"{Lot.Text}," +
                $"{Date.Date.ToString("yyyy/MM/dd")} {Time.Time}\n";

            using (var client = new TcpSocketClient())
            {
                try
                {
                    await client.ConnectAsync(address, port);

                    var enc = Encoding.UTF8;
                    var sendBytes = enc.GetBytes(sendString);
                    foreach (var byteData in sendBytes)
                    {
                        client.WriteStream.WriteByte(byteData);
                        await client.WriteStream.FlushAsync();
                    }

                    await client.DisconnectAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.InnerException);
                }
            }
        }

        async void HttpPostButton_Clicked(object sender, EventArgs e)
        {
            using (var client = new HttpClient())
            {
                if (picker.SelectedItem.ToString() == "Localhost")
                {
                    client.BaseAddress = Secrets.LocalUri;
                    client.DefaultRequestHeaders.Add("x-cdata-authtoken", Secrets.LocalToken);
                }
                else
                {
                    client.BaseAddress = Secrets.AzureUri;
                    client.DefaultRequestHeaders.Add("x-cdata-authtoken", Secrets.AzureToken);
                }

                try
                {
                    var data = new Value()
                    {
                        Name = Name.Text,
                        Product = Product.Text,
                        Lot = Lot.Text,
                        StockDate = Date.Date.ToString()
                    };

                    var content = new StringContent(JsonConvert.SerializeObject(data));
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    var response = await client.PostAsync("api.rsc/dbo_StockTable/", content);
                    response.EnsureSuccessStatusCode();

                    await DisplayAlert("Success", "Data is added to API", "OK");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.InnerException);
                }
            }
        }

        async void HttpGetButton_Clicked(object sender, EventArgs e)
        {
            using (var client = new HttpClient())
            {
                if (picker.SelectedItem.ToString() == "Localhost")
                {
                    client.BaseAddress = Secrets.LocalUri;
                    client.DefaultRequestHeaders.Add("x-cdata-authtoken", Secrets.LocalToken);
                }
                else
                {
                    client.BaseAddress = Secrets.AzureUri;
                    client.DefaultRequestHeaders.Add("x-cdata-authtoken", Secrets.AzureToken);
                }

                try
                {
                    var response = await client.GetAsync("api.rsc/dbo_StockTable/");
                    response.EnsureSuccessStatusCode();

                    var json = await response.Content.ReadAsStringAsync();
                    var values = JsonConvert.DeserializeObject<CDataJson>(json);

                    var sb = new StringBuilder();
                    foreach (var value in values.Values)
                    {
                        sb.Append($"Product:{value.Product} Name:{value.Name}...\n");
                    }
                    await DisplayAlert("Data", sb.ToString(), "OK");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.InnerException);
                }
            }
        }
    }
}
