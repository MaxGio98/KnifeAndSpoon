using KnifeAndSpoon.Model;
using Plugin.CloudFirestore;
using Plugin.FirebaseStorage;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InsertPage : ContentPage
    {
        private MediaFile imgFile = null;
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
        private void AddIngrediente(object sender, EventArgs args)
        {
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
            nome.Keyboard = Keyboard.Text;
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
        }

        private void longClickAddPicFab(object sender,EventArgs e)
        {
            DependencyService.Get<IAndroidPopUp>().ShowSnackbar("Aggiungi una foto");
        }
        private void longClickBackFab(object sender, EventArgs e)
        {
            DependencyService.Get<IAndroidPopUp>().ShowSnackbar("Torna indietro");
        }
        

        private void OnTextChanged(object s, EventArgs e)
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

        private void AddPassaggio(object sender, EventArgs args)
        {
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
            del_button.Margin = new Thickness(10, 0, 5, 0);
            del_button.TextColor = Color.White;
            del_button.ImageSource = "remove";
            stack.Children.Add(del_button);
            Editor passText = new Editor();
            passText.Keyboard = Keyboard.Text;
            passText.Margin = new Thickness(0, 0, 10, 0);
            passText.Keyboard = Keyboard.Chat;
            passText.HorizontalOptions = LayoutOptions.FillAndExpand;
            passText.HeightRequest = 100;
            stack.Children.Add(passText);
            lst_passaggi.Children.Add(stack);
        }

        private void RemoveIngrediente(object sender, EventArgs args)
        {
            Button temp = (Button)sender;
            lst_ingredienti.Children.Remove((StackLayout)temp.CommandParameter);
        }

        private void RemovePassaggio(object sender, EventArgs args)
        {
            Button temp = (Button)sender;
            lst_passaggi.Children.Remove((StackLayout)temp.CommandParameter);
        }

        private async void checkPermissions(object sender, EventArgs e)
        {
            if (await GetPermissions())
            {
                await Navigation.PushModalAsync(new ImageSelectionDialog(
                    "Inserisci foto da ...",
                    new Command(() => { getPhotoFromCamera(); }),
                    new Command(() => { getPhotoFromGalleryAsync(); })
                    )
                    );
            }
            else
            {
                await Navigation.PushModalAsync(new ErrorDialog("Non ho le autorizzazioni necessarie"));
            }
        }

        private async void getPhotoFromCamera()
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await Navigation.PushModalAsync(new ErrorDialog("Nessuna fotocamera disponibile"));
                return;
            }
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                AllowCropping = true,
                CompressionQuality = 5,
                Directory = "Ricette",
                Name = "test.jpg"
            });
            if (file == null)
                return;
            imgFile = file;
            Navigation.PopModalAsync();
            imgToUpload.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

        }

        private async void getPhotoFromGalleryAsync()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await Navigation.PushModalAsync(new ErrorDialog("Acesso alle foto non consentito"));
                return;
            }
            var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 5
            });


            if (file == null)
                return;

            imgFile = file;
            Navigation.PopModalAsync();
            imgToUpload.Source = ImageSource.FromStream(() =>
            {
                var stream = file.GetStream();
                return stream;
            });

        }

        private static async Task<bool> GetPermissions()
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

        private async void publishRecipeToFirebase(object sender, EventArgs e)
        {
            confirmFab.IsEnabled = false;
            if (imgFile == null)
            {
                await Navigation.PushModalAsync(new ErrorDialog("Inserisci un'immagine della ricetta"));
                confirmFab.IsEnabled = true;
            }
            else
            {
                upload();
            }
        }

        private async void upload()
        {
            //Inizializzazione Ricetta
            Timestamp t = new Timestamp(DateTime.Now);
            Ricetta ricetta = new Ricetta();
            //Controlli sui campi
            if (Name.Text != null)
            {
                if (!Name.Text.Trim().Equals(""))
                {
                    ricetta.Titolo = Name.Text;
                }
                else
                {
                    await Navigation.PushModalAsync(new ErrorDialog("Il titolo non può essere vuoto"));
                    confirmFab.IsEnabled = true;
                    return;
                }

            }
            else
            {
                await Navigation.PushModalAsync(new ErrorDialog("Il titolo non può essere vuoto"));
                confirmFab.IsEnabled = true;
                return;
            }

            ricetta.Autore = utente.Id;

            ricetta.Timestamp = t;

            ricetta.Categoria = Category.SelectedItem.ToString();

            if (Time.Text != null)
            {
                if (!Time.Text.Trim().Equals(""))
                {
                    ricetta.TempoPreparazione = Time.Text.ToString();
                }

            }
            else
            {
                await Navigation.PushModalAsync(new ErrorDialog("Controlla il tempo di preparazione"));
                confirmFab.IsEnabled = true;
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
                    await Navigation.PushModalAsync(new ErrorDialog("Controlla il numero delle persone"));
                    confirmFab.IsEnabled = true;
                    return;
                }

            }
            else
            {
                await Navigation.PushModalAsync(new ErrorDialog("Controlla il numero delle persone"));
                confirmFab.IsEnabled = true;
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
                            await Navigation.PushModalAsync(new ErrorDialog("Il nome dell'ingrediente n°" + (i + 1) + " è vuoto"));
                            confirmFab.IsEnabled = true;
                            return;
                        }
                    }
                    else
                    {
                        await Navigation.PushModalAsync(new ErrorDialog("Il nome dell'ingrediente n°" + (i + 1) + " è vuoto"));
                        confirmFab.IsEnabled = true;
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
                                    await Navigation.PushModalAsync(new ErrorDialog("La quantità dell'ingrediente n°" + (i + 1) + " non può essere 0"));
                                    confirmFab.IsEnabled = true;
                                    return;
                                }

                            }
                            else
                            {
                                await Navigation.PushModalAsync(new ErrorDialog("La quantità dell'ingrediente n°" + (i + 1) + " è vuoto"));
                                confirmFab.IsEnabled = true;
                                return;
                            }

                        }
                        else
                        {
                            await Navigation.PushModalAsync(new ErrorDialog("La quantità dell'ingrediente n°" + (i + 1) + " è vuoto"));
                            confirmFab.IsEnabled = true;
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
                await Navigation.PushModalAsync(new ErrorDialog("Inserisci almeno un ingrediente"));
                confirmFab.IsEnabled = true;
                return;
            }

            List<string> passaggi = new List<string>();
            if (lst_passaggi.Children.Count != 0)
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
                            await Navigation.PushModalAsync(new ErrorDialog("Il passaggio n°" + (i + 1) + " è vuoto"));
                            confirmFab.IsEnabled = true;
                            return;
                        }
                    }
                    else
                    {
                        await Navigation.PushModalAsync(new ErrorDialog("Il passaggio n°" + (i + 1) + " è vuoto"));
                        confirmFab.IsEnabled = true;
                        return;
                    }

                }
            }
            else
            {
                await Navigation.PushModalAsync(new ErrorDialog("Inserisci almeno un passaggio"));
                confirmFab.IsEnabled = true;
                return;
            }

            ricetta.Passaggi = passaggi;
            ricetta.Ingredienti = ingredienti;

            await Navigation.PushModalAsync(new ConfirmDialog("Sei sicuro?", new Command(
                async () =>
                {
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

        public void Back(object sender, EventArgs args)
        {
            Navigation.PushModalAsync(new ConfirmDialog("Sei sicuro di voler uscire?", new Command(async () =>
            {
                await Navigation.PopModalAsync();
                await Navigation.PopAsync();
            })));
        }
        protected override bool OnBackButtonPressed()
        {
            //Are you sure?
            Navigation.PushModalAsync(new ConfirmDialog("Sei sicuro di voler uscire?", new Command(async () =>
             {
                 await Navigation.PopModalAsync();
                 await Navigation.PopAsync();
             })));
            return true;
        }
    }
}