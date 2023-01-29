using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using FireSharp;

namespace Pizza
{

    public partial class MainWindow : Window
    {
        static SpeechSynthesizer ss = new SpeechSynthesizer();
        static SpeechRecognitionEngine sre;
        static Pizza pizza = new Pizza()
        {
            Catagory = "wybierz",
            Dough = "wybierz",
            ExtraIngredient = "wybierz",
            Sauce = "wybierz",
            Size = "wybierz"
        };
        static List<Pizza> order = new List<Pizza>();
        static bool pizzaAdded;
        static bool doughSelected = false;
        static bool categorySelected = false;
        static bool sizeSelected = false;
        static bool sauceSelected = false;
        static bool ingriSelected = false;
        int pizzaPrice;
        int orderPrice;
        IFirebaseConfig ifc = new FirebaseConfig()
        {
            AuthSecret = "gyyQK8RSKnWJrcqOD3TVa4uoJ1yoiwQJJPvJiF5o",
            BasePath = "https://sangiovanni-ca76f-default-rtdb.firebaseio.com/"
        };
        IFirebaseClient client;
        public MainWindow()
        {
            pizzaPrice = 0;
            orderPrice = 0;
            ss.SetOutputToDefaultAudioDevice();
            CultureInfo ci = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += Sre_SpeechRecognized;
            sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;
            Grammar grammar = new Grammar(".\\Grammar\\IngredientsGrammar.xml")
            {
                Enabled = true
            };
            sre.LoadGrammar(grammar);
            sre.RecognizeAsync(RecognizeMode.Multiple);
            ss.SpeakAsync("Witaj w asystencie głosowym zamawiania pizzy. Zadam ci kilka pytań.");
            ss.SpeakAsync("Jakie ciasto?");
            Loaded += MainWindow_Loaded;

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadComboboxesOptions();
        }

        private void Sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            // ss.Speak("Nie rozpoznano wypowiedzi. Proszę powtórzyć!");
        }

        private async void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            float confidence = e.Result.Confidence;

           // confidenceText.Content = "" + confidence;

            if (confidence < 0.6)
            {
                ss.SpeakAsync("Proszę powtórzyć!");
            }
            else
            {
                string dough = pizzaDough.Text == "wybierz" ? e.Result.Semantics["dough"].Value.ToString() : pizzaDough.Text;
                string category = selectedPizza.Text == "wybierz" ? e.Result.Semantics["category"].Value.ToString() : selectedPizza.Text;
                string size = pizza_size.Text == "wybierz" ? e.Result.Semantics["size"].Value.ToString() : pizza_size.Text;
                string sauce = pizzaSauce.Text == "wybierz" ? e.Result.Semantics["sauce"].Value.ToString() : pizzaSauce.Text;
                string extraIngredient = additionalIngredients.Text == "wybierz" ? e.Result.Semantics["ingredient"].Value.ToString() : additionalIngredients.Text;
                if (dough != "wybierz" && !doughSelected)
                {
                    doughSelected = true;
                    pizza.Dough = dough;
                    pizzaDough.SelectedItem = dough;
                    pizzaDough.IsEnabled = false;
                    if (dough == "grube")
                    {
                        ss.SpeakAsync("Wybrano ciasto grube");

                    }
                    else if (dough == "cienkie")
                    {
                        ss.SpeakAsync("Wybrano ciasto cienkie");
                    }
                }

                if (category != "wybierz" && !categorySelected)
                {
                    categorySelected = true;
                    pizza.Catagory = category;
                    selectedPizza.SelectedItem = category;
                    selectedPizza.IsEnabled = false;

                    if (category == "bella")
                    {
                        ss.SpeakAsync("Wybrano pizze bella");

                        image.Source = new BitmapImage(new Uri("Images_ofPizza/Bella.png", UriKind.Relative));

                    }
                    else if (category == "chłopska")
                    {
                        ss.SpeakAsync("Wybrano pizze chłopską");
                        image.Source = new BitmapImage(new Uri("Images_ofPizza/Chłopska.png", UriKind.Relative));
                    }
                    else if (category == "diablo")
                    {
                        ss.SpeakAsync("Wybrano pizze diablo");
                        image.Source = new BitmapImage(new Uri("Images_ofPizza/Diablo.png", UriKind.Relative));
                    }
                    else if (category == "fiore")
                    {
                        ss.SpeakAsync("Wybrano pizze fiore");
                        image.Source = new BitmapImage(new Uri("Images_ofPizza/Fiore.png", UriKind.Relative));
                    }
                    else if (category == "góralska")
                    {
                        ss.SpeakAsync("Wybrano pizze góralską");
                        image.Source = new BitmapImage(new Uri("Images_ofPizza/Góralska.png", UriKind.Relative));
                    }
                    else if (category == "grecka")
                    {
                        ss.SpeakAsync("Wybrano pizze grecką");
                        image.Source = new BitmapImage(new Uri("Images_ofPizza/Grecka.png", UriKind.Relative));
                    }


                }

                if (size != "wybierz" && !sizeSelected)
                {
                    sizeSelected = true;
                    pizza.Size = size;
                    pizza_size.SelectedItem = size;
                    pizza_size.IsEnabled = false;
                    if (size == "mała")
                    {
                        ss.SpeakAsync("Wybrano rozmiar mały");
                    }
                    else if (size == "średnia")
                    {
                        ss.SpeakAsync("Wybrano rozmiar średni");
                    }
                    else if (size == "duża")
                    {
                        ss.SpeakAsync("Wybrano rozmiar duży");
                    }
                }

                if (sauce != "wybierz" && !sauceSelected)
                {
                    sauceSelected = true;
                    pizza.Sauce = sauce;
                    pizzaSauce.SelectedItem = sauce;
                    pizzaSauce.IsEnabled = false;

                    if (sauce == "łagodny")
                    {
                        ss.SpeakAsync("Wybrano sos łagodny");

                    }
                    else if (sauce == "mieszany")
                    {
                        ss.SpeakAsync("Wybrano sos mieszany");

                    }
                    else if (sauce == "ostry")
                    {
                        ss.SpeakAsync("Wybrano sos ostry");

                    }
                }

                if (extraIngredient != "wybierz" && !ingriSelected)
                {
                    ingriSelected = true;
                    pizza.ExtraIngredient = extraIngredient;
                    additionalIngredients.SelectedItem = extraIngredient;
                    additionalIngredients.IsEnabled = false;
                    if (extraIngredient == "brak")
                    {
                        ss.SpeakAsync("Nie wybrano dodatkowego składnika");
                    }
                    else if (extraIngredient != "wybierz")
                    {
                        ss.SpeakAsync("Wybrano dodatkowy składnik " + extraIngredient.ToString());
                    }
                }

                ContinueQuestions();
            }
        }

        private void ContinueQuestions()
        {
            if (pizza.Dough == "wybierz")
            {
                ss.SpeakAsync("Jakie ciasto?");
            }
            else if (pizza.Catagory == "wybierz")
            {
                ss.SpeakAsync("Jaki rodzaj pizzy?");
            }
            else if (pizza.Size == "wybierz")
            {
                ss.SpeakAsync("Jaki rozmiar?");
            }
            else if (pizza.Sauce == "wybierz")
            {
                ss.SpeakAsync("Jaki sos?");
            }
            else if (pizza.ExtraIngredient == "wybierz")
            {
                ss.Speak("Jaki dodatkowy składnik?");
            }
            else
            {
                sre.SpeechRecognized -= Sre_SpeechRecognized;
                sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected;
                sre.UnloadAllGrammars();

                Grammar grammar = new Grammar(".\\Grammar\\OrderGrammar.xml")
                {
                    Enabled = true
                };
                sre.LoadGrammar(grammar);
                sre.SpeechRecognized += Sre_SpeechRecognized_Order;
                sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected_Order;
                SayIngredients(pizza);
                ss.SpeakAsync("Czy dodać do zamówienia?");

            }
        }

        private void LoadComboboxesOptions()
        {
            var doughs = new string[] { "wybierz", "grube", "cienkie" };
            var pizza = new string[] { "wybierz", "bella", "chłopska", "diablo", "fiore", "góralska", "grecka" };
            var size = new string[] { "wybierz", "mała", "średnia", "duża" };
            var souce = new string[] { "wybierz", "łagodny", "mieszany", "ostry" };
            var extraIngri = new string[] { "wybierz", "mozzarella", "feta", "szynka", "pieczarki", "ananas", "cebula", "czosnek", "bekon", "pepperoni", "wołowina", "krewetki", "kiełbasa", "ogórek", "oliwki", "rukola", "pomidor", "jajko", "kukurydza", "brak" };



            pizzaDough.ItemsSource = doughs;
            pizzaDough.IsEnabled = true;
            pizzaDough.SelectedIndex = 0;
            pizzaDough.SelectionChanged += PizzaDough_SelectionChanged;

            selectedPizza.ItemsSource = pizza;
            selectedPizza.IsEnabled = true;
            selectedPizza.SelectedIndex = 0;
            selectedPizza.SelectionChanged += SelectedPizza_SelectionChanged;

            pizza_size.ItemsSource = size;
            pizza_size.IsEnabled = true;
            pizza_size.SelectedIndex = 0;
            pizza_size.SelectionChanged += Pizza_size_SelectionChanged;

            pizzaSauce.ItemsSource = souce;
            pizzaSauce.IsEnabled = true;
            pizzaSauce.SelectedIndex = 0;
            pizzaSauce.SelectionChanged += PizzaSauce_SelectionChanged;

            additionalIngredients.ItemsSource = extraIngri;
            additionalIngredients.IsEnabled = true;
            additionalIngredients.SelectedIndex = 0;
            additionalIngredients.SelectionChanged += AdditionalIngredients_SelectionChanged;
        }

        private void AdditionalIngredients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newSelected = (string)additionalIngredients.SelectedItem;
            if (newSelected != "wybierz" && !ingriSelected)
            {
                ingriSelected = true;
                pizza.ExtraIngredient = newSelected;
                additionalIngredients.IsEnabled = false;
                if (newSelected == "brak")
                {
                    ss.SpeakAsync("Nie wybrano dodatkowego składnika");
                }
                else if (newSelected != "wybierz")
                {
                    ss.SpeakAsync("Wybrano dodatkowy składnik " + newSelected.ToString());
                }
                ContinueQuestions();
            }

        }

        private void PizzaSauce_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newSelected = (string)pizzaSauce.SelectedItem;
            if (newSelected != "wybierz" && !sauceSelected)
            {
                sauceSelected = true;
                pizza.Sauce = newSelected;
                pizzaSauce.IsEnabled = false;

                if (newSelected == "łagodny")
                {
                    ss.SpeakAsync("Wybrano sos łagodny");

                }
                else if (newSelected == "mieszany")
                {
                    ss.SpeakAsync("Wybrano sos mieszany");

                }
                else if (newSelected == "ostry")
                {
                    ss.SpeakAsync("Wybrano sos ostry");

                }
                ContinueQuestions();
            }

        }

        private void Pizza_size_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newSelected = (string)pizza_size.SelectedItem;
            if (newSelected != "wybierz" && !sizeSelected)
            {
                sizeSelected = true;
                pizza.Size = newSelected;
                pizza_size.IsEnabled = false;
                if (newSelected == "mała")
                {
                    ss.SpeakAsync("Wybrano rozmiar mały");
                }
                else if (newSelected == "średnia")
                {
                    ss.SpeakAsync("Wybrano rozmiar średni");
                }
                else if (newSelected == "duża")
                {
                    ss.SpeakAsync("Wybrano rozmiar duży");
                }
                ContinueQuestions();
            }

        }

        private void SelectedPizza_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newSelected = (string)selectedPizza.SelectedItem;
            if (newSelected != "wybierz" && !categorySelected)
            {
                categorySelected = true;
                pizza.Catagory = newSelected;
                selectedPizza.IsEnabled = false;

                if (newSelected == "bella")
                {
                    ss.SpeakAsync("Wybrano pizze bella");   
                    image.Source = new BitmapImage(new Uri("Images/Bella.png", UriKind.Relative));

                }
                else if (newSelected == "chłopska")
                {
                    ss.SpeakAsync("Wybrano pizze chłopska"); // Do frontu: kiełbasa, wiejska kiełbasa, bekon, ogórek, cebula
                    image.Source = new BitmapImage(new Uri("Images/Chłopska.png", UriKind.Relative));
                }
                else if (newSelected == "diablo")
                {
                    ss.SpeakAsync("Wybrano pizze diablo"); // Do frontu: cebula, oliwki, ser favita, pomidor
                    image.Source = new BitmapImage(new Uri("Images/Diablo.png", UriKind.Relative));
                }
                else if (newSelected == "Fiore")
                {
                    ss.SpeakAsync("Wybrano pizze fiore"); // Do frontu: szynka, jajko, pieczarki, bekon
                    image.Source = new BitmapImage(new Uri("Images/Fiore.png", UriKind.Relative));
                }
                else if (newSelected == "góralska")
                {
                    ss.SpeakAsync("Wybrano pizze góralską"); // Do frontu: kiełbasa, cebula, podwójna wołowina
                    image.Source = new BitmapImage(new Uri("Images/Góralska.png", UriKind.Relative));
                }
                else if (newSelected == "grecka")
                {
                    ss.SpeakAsync("Wybrano pizze grecką"); // Do frontu: pomidorki coctailowe, szynka dojrzewająca, rucola, parmezan
                    image.Source = new BitmapImage(new Uri("Images/grecka.png", UriKind.Relative));
                }

                ContinueQuestions();
            }

        }

        private void PizzaDough_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var newSelected = (string)pizzaDough.SelectedItem;
            if (newSelected != "wybierz" && !doughSelected)
            {
                pizza.Dough = newSelected;
                doughSelected = true;
                if (newSelected == "grube")
                {
                    ss.SpeakAsync("Wybrano ciasto grube");
                }
                else if (newSelected == "cienkie")
                {
                    ss.SpeakAsync("Wybrano ciasto cienkie");
                }
                pizzaDough.IsEnabled = false;
                ContinueQuestions();
            }


        }

        private void Again()
        {
            pizza = new Pizza()
            {
                Catagory = "wybierz",
                Dough = "wybierz",
                ExtraIngredient = "wybierz",
                Sauce = "wybierz",
                Size = "wybierz"
            };
            doughSelected = false;
            categorySelected = false;
            sizeSelected = false;
            sauceSelected = false;
            ingriSelected = false;
            LoadComboboxesOptions();
        }

        private void Sre_SpeechRecognitionRejected_Order(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            ss.Speak("Czy dodać do zamówienia?");
        }
        private async void Sre_SpeechRecognized_Order(object sender, SpeechRecognizedEventArgs e)
        {
            String confirmation = e.Result.Semantics["confirmation"].Value.ToString();


            float confidence = e.Result.Confidence;
            //confidenceText.Content = "" + confidence;

            if (confidence < 0.6)
            {
                ss.SpeakAsync("Proszę powtórzyć!");
            }
            else
            {
                if (confirmation != "none")
                {
                    if (pizzaAdded == false)
                    {
                        if (confirmation == "tak")
                        {
                            order.Add(pizza);
                            pizzaAdded = true;
                            pizza = new Pizza();
                            await Task.FromResult(ss.SpeakAsync("Pomyślnie dodano. Twoje zamówienie zawiera " + order.Count + " pizz"));
                            await Task.FromResult(ss.SpeakAsync("Czy to koniec zamówienia?"));
                            order_Price.Content = "Wartość zamówienia: " + orderPrice;

                        }
                        else if (confirmation == "nie")
                        {
                            pizza = new Pizza();
                            sre.SpeechRecognized -= Sre_SpeechRecognized_Order;
                            sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected_Order;
                            sre.UnloadAllGrammars();
                            Grammar grammar = new Grammar(".\\Grammar\\IngredientsGrammar.xml")
                            {
                                Enabled = true
                            };
                            sre.LoadGrammar(grammar);
                            await Task.FromResult(ss.SpeakAsync("Odrzucono zamówienie, zacznijmy od nowa"));
                            await Task.FromResult(ss.SpeakAsync("Jakie ciasto?"));
                            Again();
                            sre.SpeechRecognized += Sre_SpeechRecognized;
                            sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;

                        }
                    }
                    else if (pizzaAdded == true)
                    {
                        if (confirmation == "tak")
                        {
                            sre.SpeechRecognized -= Sre_SpeechRecognized_Order;
                            sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected_Order;
                            sre.UnloadAllGrammars();
                            await Task.FromResult(ss.SpeakAsync("Twoje zamówienie zawiera " + order.Count + "pizz, jego cena to " + orderPrice.ToString() + "złotych."));
                            order_Price.Content = "Wartość zamówienia: " + orderPrice;
                            await Task.FromResult(ss.SpeakAsync("Dziękujemy i zapraszamy ponownie"));

                        }
                        else if (confirmation == "nie")
                        {
                            pizza = new Pizza(); ;
                            pizzaAdded = false;
                            image.Source = null;


                            sre.SpeechRecognized -= Sre_SpeechRecognized_Order;
                            sre.SpeechRecognitionRejected -= Sre_SpeechRecognitionRejected_Order;
                            sre.UnloadAllGrammars();
                            Grammar grammar = new Grammar(".\\Grammar\\IngredientsGrammar.xml")
                            {
                                Enabled = true
                            };
                            sre.LoadGrammar(grammar);
                            await Task.FromResult(ss.SpeakAsync("Wybrałeś dodanie kolejnej pozycji do zamówienia"));

                            await Task.FromResult(ss.SpeakAsync("Jakie ciasto?"));
                            Again();
                            sre.SpeechRecognized += Sre_SpeechRecognized;
                            sre.SpeechRecognitionRejected += Sre_SpeechRecognitionRejected;

                        }
                    }

                }
                else if (confirmation == "none")
                {
                    if (pizzaAdded == true)
                    {
                        await Task.FromResult(ss.SpeakAsync("Czy to koniec zamówienia?"));
                    }
                    else if (pizzaAdded == false)
                    {
                        await Task.FromResult(ss.SpeakAsync("Czy dodać do zamówienia?"));
                    }

                }

            }
        }
        private async Task<int> GetPriceAsync(string ing)
        {

            client = new FirebaseClient(ifc);
            if (client != null)
            {

                FirebaseResponse response = await client.GetAsync("/Ingredient/" + ing);

                int p = response.ResultAs<int>();
                return p;
            }
            return 0;
        }
        private async Task<int> Price(Pizza pizza)
        {
            pizzaPrice += await GetPriceAsync(pizza.Catagory);
            pizzaPrice += await GetPriceAsync(pizza.Dough);
            pizzaPrice += await GetPriceAsync(pizza.Size);
            orderPrice += pizzaPrice;
            return pizzaPrice;
        }

        private async void SayIngredients(Pizza pizza)
        {
            ss.SpeakAsync("Wybrałeś " + pizza.Size + " pizzę na ");
            if (pizza.Dough == "grube") ss.SpeakAsync("grubym cieście, " + pizza.Catagory);
            else if (pizza.Dough == "cienkie") ss.SpeakAsync("cienkim cieście, " + pizza.Catagory);
            if (pizza.Sauce == "łagodny") ss.SpeakAsync("z sosem łagodnym.");
            if (pizza.Sauce == "mieszany") ss.SpeakAsync("z sosem mieszanym.");
            if (pizza.Sauce == "ostry") ss.SpeakAsync("z sosem ostrym.");
            await Task.FromResult(ss.SpeakAsync("Cena tej pizzy wynosi " + await Price(pizza) + " złotych,"));
            pizza_price.Content = pizzaPrice;


            Pizza pizzaDb = new Pizza(pizza.Dough, pizza.Catagory, pizza.Sauce, pizza.Size);
            try
            {
                client = new FirebaseClient(ifc);
                client.Set("Order/" + pizza.GetHashCode(), pizzaDb);
            }
            catch
            {
                MessageBox.Show("problem");
            }

        }

    }
}
