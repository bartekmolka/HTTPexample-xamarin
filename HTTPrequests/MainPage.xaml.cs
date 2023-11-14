using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Diagnostics;

namespace HTTPrequests
{
    public class Root : ICloneable
    {
        public List<Todo> todos { get; set; }
        public int total { get; set; }
        public int skip { get; set; }
        public int limit { get; set; }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    public class Todo
    {
        public int id { get; set; }
        public string todo { get; set; }
        public bool completed { get; set; }
        public int userId { get; set; }
    }

    public partial class MainPage : ContentPage
    {
        public Root todoObject;


        public ICommand DeleteCommand { get; set; }
        public ObservableCollection<Todo> TodosCollection { get; set; }


        private async Task BombAnimationAsync()
        {
            Image bombImage = new Image
            {
                Source = "bomb_icon.png", 
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand
            };

            Content = bombImage;

            await bombImage.ScaleTo(1.2, 300, Easing.CubicInOut);
            await Task.Delay(100);
            await bombImage.ScaleTo(1, 200, Easing.CubicInOut);
            await Task.Delay(100);
            await bombImage.RotateTo(360, 500, Easing.SpringOut);
            await Task.Delay(100);
            await bombImage.RotateTo(0, 500, Easing.SpringIn);
        }


        public MainPage()
        {
            TodosCollection = new ObservableCollection<Todo>();
            
            InitializeComponent();

            Task.Run(async () =>
            {
                var o = await Request();
                todoObject = o; 

                if (TodosCollection.Count > 0 ? true : false) return;
                if (todoObject?.todos != null)
                {
                    foreach (var todo in todoObject.todos)
                    {
                        TodosCollection.Add(todo);
                        Console.WriteLine(todo);
                    }
                }

                Console.WriteLine(todoObject.total); 
                OnPropertyChanged(nameof(TodosCollection)); 
            });

             DeleteCommand = new Command<Todo>(item => 
            {
                TodosCollection.Clear();
                BombAnimationAsync();
                Console.WriteLine("This is a debug message.");
                List<Todo> copyList = new List<Todo>();
                int index = TodosCollection.IndexOf(item);
                for (int i = 0; i < copyList.Count; i++)
                {
                    if (i == 0) 
                    {
                        TodosCollection.Clear();
                        continue; 
                    }
                    copyList.Add(TodosCollection[i]);
                }
                ObservableCollection<Todo> cp = new ObservableCollection<Todo>(copyList);

                TodosCollection = cp;
            });

            BindingContext = this; 
        }
        

        async Task<Root> Request()
        {
            string URL = "https://dummyjson.com/todos";

            HttpClient client = new HttpClient();

            HttpResponseMessage response = await client.GetAsync(URL);

            string result = await response.Content.ReadAsStringAsync();

            Root obj = JsonConvert.DeserializeObject<Root>(result);

            return obj;
        }
    }
}
