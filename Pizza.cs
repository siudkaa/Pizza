namespace Pizza
{

    public partial class MainWindow
    {
        public class Pizza
        {
            public string Dough { get; set; }
            public string Catagory { get; set; }
            public string ExtraIngredient { get; set; }
            public string Sauce { get; set; }
            public string Size { get; set; }
            public Pizza() { }
            public Pizza(string dough, string catagory, string sauce, string size)
            {
                Dough = dough;
                Catagory = catagory;
                Sauce = sauce;
                Size = size;
            }

        }

    }
}
