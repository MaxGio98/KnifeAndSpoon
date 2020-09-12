using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InsertPage : ContentPage
    {
        private int count_ingredienti = 0;
        private int count_passaggi = 0;
        public InsertPage()
        {
            InitializeComponent();
            this.Category.Items.Add("Antipasto");
            this.Category.Items.Add("Primo");
            this.Category.Items.Add("Secondo");
            this.Category.Items.Add("Contorno");
            this.Category.Items.Add("Dolce");
            this.Category.SelectedIndex = 0;
            this.Title = "Nuova ricetta";
        }
        public void AddIngrediente(object sender, EventArgs args)
        {
            count_ingredienti++;
            StackLayout stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;
            Button del_button = new Button();
            del_button.Clicked += RemoveIngrediente;
            del_button.CommandParameter = stack;
            del_button.HeightRequest = 40;
            del_button.WidthRequest = 40;
            del_button.VerticalOptions = LayoutOptions.Center;
            del_button.HorizontalOptions = LayoutOptions.End;
            del_button.BackgroundColor = Color.FromHex("#b10000");
            del_button.CornerRadius = 50;
            del_button.Margin = new Thickness(10, 0, 5, 0);
            del_button.TextColor = Color.White;
            del_button.ImageSource = "remove";
            stack.Children.Add(del_button);
            Entry nome = new Entry();
            nome.FontSize = 20;
            nome.TextColor = Color.Black;
            nome.VerticalOptions = LayoutOptions.Center;
            nome.WidthRequest = 150;
            stack.Children.Add(nome);
            Entry qt = new Entry();
            qt.FontSize = 20;
            qt.TextColor = Color.Black;
            qt.VerticalOptions = LayoutOptions.Center;
            qt.WidthRequest = 50;
            stack.Children.Add(qt);
            Picker ut = new Picker();
            ut.Items.Add("unità");
            ut.Items.Add("grammi");
            ut.Items.Add("kg");
            ut.Items.Add("bicchiere");
            ut.Items.Add("litri");
            ut.Items.Add("cucchiaio");
            ut.Items.Add("cucchiaino");
            ut.Items.Add("millilitri");
            ut.Items.Add("q.b.");
            ut.SelectedIndex = 0;
            ut.FontSize = 20;
            ut.TextColor = Color.Black;
            ut.VerticalOptions = LayoutOptions.Center;
            ut.HorizontalOptions = LayoutOptions.FillAndExpand;
            ut.Margin = new Thickness(0, 0, 10, 0);
            stack.Children.Add(ut);
            lst_ingredienti.Children.Add(stack);
            /*for (int i = 0; i < lst_ingredienti.Children.Count; i++)
            {
                Console.WriteLine(((Entry)((StackLayout)lst_ingredienti.Children[i]).Children[1]).Text);
                Console.WriteLine(((Entry)((StackLayout)lst_ingredienti.Children[i]).Children[2]).Text);
                Console.WriteLine(((Picker)((StackLayout)lst_ingredienti.Children[i]).Children[3]).SelectedItem.ToString());
            }*/
        }

        public void AddPassaggio(object sender, EventArgs args)
        {
            count_passaggi++;
            StackLayout stack = new StackLayout();
            stack.Orientation = StackOrientation.Horizontal;
            Button del_button = new Button();
            del_button.Clicked += RemovePassaggio;
            del_button.CommandParameter = stack;
            del_button.HeightRequest = 40;
            del_button.WidthRequest = 40;
            del_button.VerticalOptions = LayoutOptions.Center;
            del_button.HorizontalOptions = LayoutOptions.End;
            del_button.BackgroundColor = Color.FromHex("#b10000");
            del_button.CornerRadius = 50;
            del_button.Margin = new Thickness(10,0,5,0);
            del_button.TextColor = Color.White;
            del_button.ImageSource = "remove";
            stack.Children.Add(del_button);
            Editor passText = new Editor();
            passText.Margin = new Thickness(0, 0, 10, 0);
            passText.Keyboard = Keyboard.Chat;
            passText.HorizontalOptions = LayoutOptions.FillAndExpand;
            passText.HeightRequest = 100;
            stack.Children.Add(passText);
            lst_passaggi.Children.Add(stack);
            /*
            for(int i = 0; i < lst_passaggi.Children.Count; i++)
            {
                Console.WriteLine(((Editor)((StackLayout)lst_passaggi.Children[i]).Children[1]).Text);
            }*/
        }

        public void RemoveIngrediente(object sender, EventArgs args)
        {
            Button temp = (Button)sender;
            lst_ingredienti.Children.Remove((StackLayout)temp.CommandParameter);
        }

        public void RemovePassaggio(object sender, EventArgs args)
        {
            Button temp = (Button)sender;
            lst_passaggi.Children.Remove((StackLayout)temp.CommandParameter);
        }

        private async void checkPermissions(object sender, EventArgs e)
        {
            if(await GetPermissions())
            {

                string action = await DisplayActionSheet("Inserisci foto da...", "Annulla", null, "Fotocamera", "Galleria");
                if(action.Equals("Fotocamera"))
                {
                    await CrossMedia.Current.Initialize();
                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        DisplayAlert("No Camera", ":( No camera available.", "OK");
                        return;
                    }
                    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg"
                    });
                    if (file == null)
                        return;
                    imgToUpload.Source = ImageSource.FromStream(() =>
                    {
                        var stream = file.GetStream();
                        return stream;
                    });
                }
                else if(action.Equals("Galleria"))
                {
                    (sender as Button).IsEnabled = false;
                    Stream stream = await DependencyService.Get<IPhotoPickerService>().GetImageStreamAsync();
                    if (stream != null)
                    {
                        imgToUpload.Source = ImageSource.FromStream(() => stream);
                    }
                    (sender as Button).IsEnabled = true;
                }
            }
            else
            {
                await DisplayAlert("Non ho le autorizzazioni necessarie", "Per favore approva tutto", "OK");
            }
        }


        public static async Task<bool> GetPermissions()
        {
            bool permissionsGranted = true;

            var permissionsStartList = new List<Permission>()
        {
            Permission.Storage,
            Permission.Camera
        };

            var permissionsNeededList = new List<Permission>();
            try
            {
                foreach (var permission in permissionsStartList)
                {
                    var status = await CrossPermissions.Current.CheckPermissionStatusAsync(permission);
                    if (status != PermissionStatus.Granted)
                    {
                        permissionsNeededList.Add(permission);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            var results = await CrossPermissions.Current.RequestPermissionsAsync(permissionsNeededList.ToArray());

            try
            {
                foreach (var permission in permissionsNeededList)
                {
                    var status = PermissionStatus.Unknown;
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(permission))
                        status = results[permission];
                    if (status == PermissionStatus.Granted || status == PermissionStatus.Unknown)
                    {
                        permissionsGranted = true;
                    }
                    else
                    {
                        permissionsGranted = false;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return permissionsGranted;
        }
    }
}