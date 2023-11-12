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

        public ObservableCollection<Todo> TodosCollection { get; set; }

        public MainPage()
        {
            InitializeComponent();
            TodosCollection = new ObservableCollection<Todo>();

            // Call the request method and wait for the result
            Task.Run(async () =>
            {
                var o = await Request();
                todoObject = o; // Assign the result to your todoObject field

                if (todoObject?.todos != null)
                {
                    foreach (var todo in todoObject.todos)
                    {
                        TodosCollection.Add(todo);
                    }
                }

                Console.WriteLine(todoObject.total); // Access a property of the Root object
                OnPropertyChanged(nameof(TodosCollection)); // Notify that the TodosCollection property has changed
            });

            BindingContext = this; // Set the BindingContext to the MainPage instance
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
