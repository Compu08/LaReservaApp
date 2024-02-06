using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.OpenWhatsApp;
using Xamarin.Forms.Xaml;

namespace TurnosFutbol.Views.ContactUs
{
    /// <summary>
    /// Page to contact with user name, email and message
    /// </summary>
    [Preserve(AllMembers = true)]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactUsPage
    {
        private double frameWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactUsPage" /> class.
        /// </summary>
        public ContactUsPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when view size is changed.
        /// </summary>
        /// <param name="width">The Width</param>
        /// <param name="height">The Height</param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height)
            {
                if (Device.Idiom == TargetIdiom.Desktop || Device.Idiom == TargetIdiom.Tablet)
                {
                    this.frameWidth = width / 2;
                    MainStack.Orientation = StackOrientation.Horizontal;
                    MainFrame.VerticalOptions = LayoutOptions.FillAndExpand;
                    MainFrame.Margin = new Thickness(0);
                    MainFrame.CornerRadius = 0;
                    MainFrame.HasShadow = false;
                    MainFrameStack.VerticalOptions = LayoutOptions.StartAndExpand;
                    if (this.frameWidth > 0)
                    {
                        MainFrame.WidthRequest = this.frameWidth;
                        //Map.WidthRequest = this.frameWidth;
                    }
                }
                else
                {
                    this.DefaultStyle(height);
                }
            }
            else
            {
                this.DefaultStyle(height);
            }
        }

        /// <summary>
        /// This default style method is called when the device is portrait mode.
        /// This method is also called when the android and ios devices are landscape mode
        /// </summary>
        /// <param name="height">The height</param>
        private void DefaultStyle(double height)
        {
            if (Device.Idiom == TargetIdiom.Tablet)
            {
                MainFrame.HeightRequest = height / 2;
                //Map.HeightRequest = height / 2;
                MainStack.Orientation = StackOrientation.Vertical;
                MainFrame.VerticalOptions = LayoutOptions.End;
                MainFrame.Margin = new Thickness(0);
                MainFrame.CornerRadius = 0;
                MainFrame.HasShadow = false;
                MainFrameStack.VerticalOptions = LayoutOptions.StartAndExpand;
                MainFrameStack.Margin = new Thickness(20, 0, 20, 0);
            }
            else
            {
                MainStack.Orientation = StackOrientation.Vertical;
                MainFrame.VerticalOptions = LayoutOptions.End;
                MainFrame.Margin = new Thickness(15, 0, 15, 15);
                MainFrameStack.VerticalOptions = LayoutOptions.EndAndExpand;
                MainFrame.CornerRadius = 5;
                MainFrame.HasShadow = true;
                MainFrameStack.Margin = new Thickness(0);
            }
        }

        private void SendClicked(object sender, System.EventArgs e)
        {
            List<string> toAddress = new List<string>
            {
                "urbansolucionesinformaticas@gmail.com"
            };
            string body = "From: " + EmailEntry.Text + " Text:" + ContactText.Text;
            SendEmail(ContactName.Text, body, toAddress);
        }

        private async void SendEmail(string subject, string body, List<string> recipients)
        {
            try
            {
                EmailMessage message = new EmailMessage
                {
                    Subject = subject,
                    Body = body,
                    To = recipients
                };
                await Email.ComposeAsync(message);
            }
            catch (Exception)
            {
                await DisplayAlert("¡Error!", "No se puedo enviar el E-mail", "Ok");
            }
        }

        private void ContactUs(object sender, System.EventArgs e)
        {

            var text = "¡Hola! Me gustaría unirme a La Reserva";

            Chat.Open("5401137582872", text);
        }
    }
}