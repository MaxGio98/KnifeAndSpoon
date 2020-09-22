using KnifeAndSpoon.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.FirebaseAuth;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Plugin.GoogleClient;
using Plugin.Media;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using Plugin.Media.Abstractions;
using Plugin.FirebaseStorage;
using Plugin.CloudFirestore;

namespace KnifeAndSpoon
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        private Utente utente;
        private MediaFile imgFile;
        private Command backReturn;
        public SettingsPage(string mode, Utente usr)
        {
            InitializeComponent();
            utente = usr;
            //caricamento ui corretta a seconda del tipo di utente che entra nelle impostazioni
            if (CrossFirebaseAuth.Current.Instance.CurrentUser.IsAnonymous)
            {
                ImgUtente.Source = "pizza";
                approve.IsVisible = false;
                approve.IsEnabled = false;
                propic.IsVisible = false;
                propic.IsEnabled = false;
                logbutton.Text = "Effettua il Log-In";
            }
            else
            {
                ImgUtente.Source = usr.Immagine;
            }
            if (mode == "Admin")
            {
                approve.IsEnabled = true;
                approve.IsVisible = true;
            }
        }

        private void Back(object sender, EventArgs args)
        {
            Navigation.PopAsync();
        }

        private void Approve(object sender, EventArgs args)
        {
            PushPage(new ApprovePage(utente));
        }

        private void LogOut(object sender, EventArgs args)
        {
            CrossFirebaseAuth.Current.Instance.SignOut();
            CrossGoogleClient.Current.Logout();
            App.Current.MainPage = new NavigationPage(new MainPage());
        }

        private void UpdatePhoto(object sender, EventArgs args)
        {
            checkPermissions();
        }

        //controlla permessi
        private async void checkPermissions()
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

        //metodo per acquisire un'immagine dalla fotocamera
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
                PhotoSize = PhotoSize.Medium,
                RotateImage = true,
                CompressionQuality = 10,
                Directory = "Profilo",
                Name = "test.jpg"
            });
            if (file == null)
                return;
            if (file.GetStream().Length < (700 * 1024))
            {
                imgFile = file;
                Navigation.PopModalAsync();
                ImgUtente.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
                uploadPhotoToFirebaseAsync();
            }
            else
            {
                await Navigation.PushModalAsync(new ErrorDialog("Per Favore metti un immagine di dimensioni minori (minore di 700kb)"));
            }
        }

        //metodo per acquisire un'immagine dalla galleria
        private async void getPhotoFromGalleryAsync()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await Navigation.PushModalAsync(new ErrorDialog("Acesso alle foto non consentito"));
                return;
            }
            var file = await Plugin.Media.CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
            {
                RotateImage = true,
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Medium,
                CompressionQuality = 10
            });
            if (file == null)
                return;
            if (file.GetStream().Length < (700 * 1024))
            {
                imgFile = file;
                Navigation.PopModalAsync();
                ImgUtente.Source = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
                uploadPhotoToFirebaseAsync();
            }
            else
            {
                await Navigation.PushModalAsync(new ErrorDialog("Per Favore metti un immagine di dimensioni minori (minore di 700kb)"));
            }
        }

        //carica immagine a firebase
        private async Task uploadPhotoToFirebaseAsync()
        {
            loadOverlay.IsVisible = true;
            string filename = utente.Nome + ".jpg";
            var reference = CrossFirebaseStorage.Current.Instance.RootReference.GetChild(filename);
            var uploadProgress = new Progress<IUploadState>();
            uploadProgress.ProgressChanged += (sender, e) =>
            {
                var progress = e.TotalByteCount > 0 ? 100.0 * e.BytesTransferred / e.TotalByteCount : 0;
            };
            await reference.PutStreamAsync(imgFile.GetStream(), progress: uploadProgress);
            String path = (await reference.GetDownloadUrlAsync()).ToString();
            await CrossCloudFirestore.Current.Instance.GetCollection("Utenti").GetDocument(utente.Id).UpdateDataAsync(new { Immagine = path });
            backReturn.Execute(backReturn);
            loadOverlay.IsVisible = false;
        }

        //richiede i permessi
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
        public void enableBackReturn(Command command)
        {
            backReturn = command;
        }

        private async void PushPage(ContentPage page)
        {
            await Navigation.PushAsync(page);
        }
    }
}