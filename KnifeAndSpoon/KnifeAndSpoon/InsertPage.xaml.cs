using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using Plugin.FirebaseStorage;
using Plugin.Media;
using Plugin.Media.Abstractions;
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
        private MediaFile imgFile=null;
        private Utente utente;
        public InsertPage(Utente usr)
        {
            InitializeComponent();
            this.Category.Items.Add("Antipasto");
            this.Category.Items.Add("Primo");
            this.Category.Items.Add("Secondo");
            this.Category.Items.Add("Contorno");
            this.Category.Items.Add("Dolce");
            this.Category.SelectedIndex = 0;
            this.Title = "Nuova ricetta";
            utente = usr;
            Servings.TextChanged += OnTextChanged;
            Time.TextChanged += OnTextChanged;
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
            qt.Keyboard = Keyboard.Numeric;
            qt.TextChanged += OnTextChanged;
            stack.Children.Add(qt);
            Picker ut = new Picker();
            ut.HorizontalTextAlignment = TextAlignment.End;
            ut.Items.Add("unità");
            ut.Items.Add("grammi");
            ut.Items.Add("kg");
            ut.Items.Add("bicchiere");
            ut.Items.Add("litri");
            ut.Items.Add("cucchiaio");
            ut.Items.Add("cucchiaino");
            ut.Items.Add("millilitri");
            ut.Items.Add("q.b.");
            ut.SelectedIndexChanged += (object s, EventArgs e) =>
            {
                ut.Unfocus();
                if (ut.SelectedItem.Equals("q.b."))
                {
                    qt.Text = "0";
                    qt.IsVisible = false;
                    qt.IsEnabled = false;
                }
                else
                {
                    qt.Text = "";
                    qt.IsVisible = true;
                    qt.IsEnabled = true;
                }
            };
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
        public void OnTextChanged(object s, EventArgs e)
        {
            Entry entry = s as Entry;
            if (entry.Text.Contains("-"))
            {
                entry.Text = entry.Text.Replace("-", "");
            }
            if (entry.Text.Contains("."))
            {
                entry.Text = entry.Text.Replace(".", "");
            }
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
                if (action != null)
                {
                    if (action.Equals("Fotocamera"))
                    {
                        await CrossMedia.Current.Initialize();
                        if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                        {
                            DisplayAlert("No Camera", ":( No camera available.", "OK");
                            return;
                        }
                        var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                        {
                            AllowCropping = true,
                            CompressionQuality = 1,
                            Directory = "Ricette",
                            Name = "test.jpg"
                        });
                        if (file == null)
                            return;
                        imgFile = file;
                        imgToUpload.Source = ImageSource.FromStream(() =>
                        {
                            var stream = file.GetStream();
                            return stream;
                        });
                    }
                    else if (action.Equals("Galleria"))
                    {
                        if (!CrossMedia.Current.IsPickPhotoSupported)
                        {
                            DisplayAlert("Photos Not Supported", ":( Permission not granted to photos.", "OK");
                            return;
                        }
                        var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                        {
                            PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                            CompressionQuality = 1
                        });


                        if (file == null)
                            return;

                        imgFile = file;
                        imgToUpload.Source = ImageSource.FromStream(() =>
                        {
                            var stream = file.GetStream();
                            return stream;
                        });

                    }
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

        public void publishRecipeToFirebase(object sender, EventArgs e)
        {
            if (imgFile == null)
            {
                Navigation.PushModalAsync(new ErrorDialog("Inserisci un'immagine della ricetta"));
            }
            else
            {
                upload();
            }
        }

        public async void upload()
        {
            
            //Inizializzazione Ricetta
            Timestamp t = new Timestamp(DateTime.Now);
            Ricetta ricetta = new Ricetta();
            //Controlli sui campi
            if (Name.Text!=null)
            {
                if (!Name.Text.Trim().Equals(""))
                {
                    ricetta.Titolo = Name.Text;
                }
                else
                {
                    Navigation.PushModalAsync(new ErrorDialog("Il titolo non può essere vuoto"));
                    return;
                }
                
            }
            else
            {
                Navigation.PushModalAsync(new ErrorDialog("Il titolo non può essere vuoto"));
                return;
            }
            
            ricetta.Autore = utente.Id;

            ricetta.Timestamp = t;

            ricetta.Categoria = Category.SelectedItem.ToString();

            if (Time.Text != null)
            {
                if(!Time.Text.Trim().Equals(""))
                {
                    ricetta.TempoPreparazione = Time.Text.ToString();
                }
                
            }
            else
            {
                Navigation.PushModalAsync(new ErrorDialog("Controlla il tempo di preparazione"));
                return;
            }
            
            if (Servings.Text != null)
            {
                if (!Servings.Text.Trim().Equals(""))
                {
                    ricetta.NumeroPersone = Servings.Text.ToString();
                }
                else
                {
                    Navigation.PushModalAsync(new ErrorDialog("Controlla il numero delle persone"));
                    return;
                }
                
            }
            else
            {
                Navigation.PushModalAsync(new ErrorDialog("Controlla il numero delle persone"));
                return;
            }


 
            ricetta.isApproved = false;

            List<IDictionary<string, object>> ingredienti = new List<IDictionary<string, object>>();
            if (lst_ingredienti.Children.Count != 0)
            {
                for (int i = 0; i < lst_ingredienti.Children.Count; i++)
                {
                    IDictionary<string, object> ingrediente = new Dictionary<string, object>();
                    string nome = (((Entry)((StackLayout)lst_ingredienti.Children[i]).Children[1]).Text);
                    if (nome != null)
                    {
                        if (!nome.Trim().Equals(""))
                        {
                            ingrediente.Add("Nome", nome);
                        }
                        else
                        {
                            Navigation.PushModalAsync(new ErrorDialog("Il nome dell'ingrediente n°" + (i + 1) + " è vuoto"));
                            return;
                        }
                    }
                    else
                    {
                        Navigation.PushModalAsync(new ErrorDialog("Il nome dell'ingrediente n°" + (i + 1) + " è vuoto"));
                        return;
                    }


                    string unit = ((Picker)((StackLayout)lst_ingredienti.Children[i]).Children[3]).SelectedItem.ToString();
                    ingrediente.Add("Unità misura", unit);

                    string quant = ((Entry)((StackLayout)lst_ingredienti.Children[i]).Children[2]).Text;
                    if (!unit.Equals("q.b."))
                    {
                        if (quant != null)
                        {
                            if (!quant.Trim().Equals(""))
                            {
                                if (int.Parse(quant) > 0)
                                {
                                    ingrediente.Add("Quantità", quant);
                                }
                                else
                                {
                                    Navigation.PushModalAsync(new ErrorDialog("La quantità dell'ingrediente n°" + (i + 1) + " non può essere 0"));
                                    return;
                                }
                                
                            }
                            else
                            {
                                Navigation.PushModalAsync(new ErrorDialog("La quantità dell'ingrediente n°" + (i + 1) + " è vuoto"));
                                return;
                            }
                            
                        }
                        else
                        {
                            Navigation.PushModalAsync(new ErrorDialog("La quantità dell'ingrediente n°" + (i + 1) + " è vuoto"));
                            return;
                        }

                    }
                    else
                    {
                        ingrediente.Add("Quantità", quant);
                    }

                    ingredienti.Add(ingrediente);
                }
            }
            else
            {
                Navigation.PushModalAsync(new ErrorDialog("Inserisci almeno un ingrediente"));
                return;
            }

            List<string> passaggi = new List<string>();
            if(lst_passaggi.Children.Count!=0)
            {
                for (int i = 0; i < lst_passaggi.Children.Count; i++)
                {
                    string pass = ((Editor)((StackLayout)lst_passaggi.Children[i]).Children[1]).Text;
                    if (pass != null)
                    {
                        if (!pass.Trim().Equals(""))
                        {
                            passaggi.Add(pass);
                        }
                        else
                        {
                            Navigation.PushModalAsync(new ErrorDialog("Il passaggio n°" + (i + 1) + " è vuoto"));
                            return;
                        }
                    }
                    else
                    {
                        Navigation.PushModalAsync(new ErrorDialog("Il passaggio n°" + (i + 1) + " è vuoto"));
                        return;
                    }

                }
            }
            else
            {
                Navigation.PushModalAsync(new ErrorDialog("Inserisci almeno un passaggio"));
                return;
            }
           
            ricetta.Passaggi = passaggi;
            ricetta.Ingredienti = ingredienti;

            Navigation.PushModalAsync(new ConfirmDialog("Sei sicuro?", new Command(
                async() => {
                    await Navigation.PopModalAsync();
                    //Upload Immagine
                    loadOverlay.IsVisible = true;
                    string filename = Guid.NewGuid().ToString() + ".jpg";
                    var reference = CrossFirebaseStorage.Current.Instance.RootReference.GetChild(filename);
                    var uploadProgress = new Progress<IUploadState>();
                    uploadProgress.ProgressChanged += (sender, e) =>
                    {
                        var progress = e.TotalByteCount > 0 ? 100.0 * e.BytesTransferred / e.TotalByteCount : 0;
                    };
                    await reference.PutStreamAsync(imgFile.GetStream(), progress: uploadProgress);
                    ricetta.Thumbnail = (await reference.GetDownloadUrlAsync()).ToString();
                    await CrossCloudFirestore.Current.Instance.GetCollection("Ricette").AddDocumentAsync(ricetta);
                    await Navigation.PushModalAsync(new ErrorDialog("La tua ricetta è in fase di approvazione"));
                    await Navigation.PopAsync();
                })));
        }
    }
}