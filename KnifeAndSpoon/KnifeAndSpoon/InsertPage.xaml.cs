using System;
using System.Collections.Generic;
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


    }
}