using Newtonsoft.Json.Linq;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;



namespace Tabs
{
	public partial class PCVision : ContentPage
	{
		public PCVision()
		{
			InitializeComponent();
            NoteLabel.Text = "Take a photo of any object. The app will determine if it's a PC peripherals or not (keyboard, mouse, monitor...)";
        }

		private async void loadCamera(object sender, EventArgs e)
		{
			await CrossMedia.Current.Initialize();
            NoteLabel.Text = "";

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
			{
				await DisplayAlert("No Camera", ":( No camera available.", "OK");
				return;
			}

			MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
			{
				PhotoSize = PhotoSize.Medium,
				Directory = "Sample",
				Name = $"{DateTime.UtcNow}.jpg"
			});

			if (file == null)
				return;

			image.Source = ImageSource.FromStream(() =>
			{
				return file.GetStream();
			});

           
			await requestCheck(file);
		}

        
			

            
        

        static byte[] GetImageAsByteArray(MediaFile file)
		{
			var stream = file.GetStream();
			BinaryReader binaryReader = new BinaryReader(stream);
			return binaryReader.ReadBytes((int)stream.Length);
		}

		async Task requestCheck(MediaFile file)
		{
            var client = new HttpClient();
			client.DefaultRequestHeaders.Add("Prediction-Key", "eaf9f18bb3bb43f0a15cc13a07d64708");
			string url = "https://southcentralus.api.cognitive.microsoft.com/customvision/v1.0/Prediction/f328e00f-fd4d-4032-866f-b3ea8e5b35b5/image?iterationId=0ac130be-1529-450a-a019-7a19f1f21ed5";
    		HttpResponseMessage response;
			byte[] byteData = GetImageAsByteArray(file);
            
            TagLabel.Text = "Analyzing, please wait...";
            string tempTag = "None";
            float tempProb = 0;
            Boolean check = false;
            using (var content = new ByteArrayContent(byteData))
			{         
				content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
				response = await client.PostAsync(url, content);

				if (response.IsSuccessStatusCode) 
				{
					var responseString = await response.Content.ReadAsStringAsync();
					JObject rss = JObject.Parse(responseString);

                    foreach (var c in rss["Predictions"]) //Check if there's a match.
                    {
                        if ((float)c["Probability"] > 0.5)
                        {
                            check = true;
                            tempTag = (string) c["Tag"];
                            tempProb = (float) c["Probability"];
                        }
                        
                    }
					
                    if (check == true) //If there's a match, save to database, otherwise, don't.
                    {
                        TagLabel.Text = "The item is a "+tempTag;
                        peripherals model = new peripherals()
                        {
                            Tag = tempTag,
                            Probability = tempProb

                        };
                        await AzureManager.AzureManagerInstance.postPeripheralInfo(model);
                        NoteLabel.Text = "Saving information to database: Tag="+tempTag+" Probability:="+tempProb;
                    }

                    else
                    {
                        TagLabel.Text = "The object is not a PC peripheral";
                    }
                    
                }

				
				file.Dispose();
			}

        }
	}
}