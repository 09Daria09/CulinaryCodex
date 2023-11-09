using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using Newtonsoft.Json;

namespace CulinaryCodex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Recipe> Recipes { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private Recipe _selectedRecipe;
        public Recipe SelectedRecipe
        {
            get { return _selectedRecipe; }
            set
            {
                if (_selectedRecipe != value)
                {
                    _selectedRecipe = value;
                    OnPropertyChanged(nameof(SelectedRecipe));
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadRecipes();
            this.DataContext = this;
        }

        private void LoadRecipes()
        {
            string jsonFilePath = "Recipes.json";

            using (StreamReader file = File.OpenText(jsonFilePath))
            {
                JsonSerializer serializer = new JsonSerializer();
                List<Recipe> recipesList = (List<Recipe>)serializer.Deserialize(file, typeof(List<Recipe>));
                Recipes = new ObservableCollection<Recipe>();

                foreach (var recipe in recipesList)
                {
                    recipe.GenerateContent(); 
                    Recipes.Add(recipe);
                }
            }

            RecipesListBox.ItemsSource = Recipes;
        }

        private void RecipesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedRecipe = RecipesListBox.SelectedItem as Recipe;
        }


    }

    public class Recipe : INotifyPropertyChanged
    {
        private string name;
        private string contentText;
        private string imagePath;
        private FlowDocument content;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private List<string> _ingredients;

        public List<string> Ingredients
        {
            get => _ingredients;
            set
            {
                if (_ingredients != value)
                {
                    _ingredients = value;
                    OnPropertyChanged(nameof(Ingredients));
                }
            }
        }
        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public string ContentText
        {
            get => contentText;
            set
            {
                if (contentText != value)
                {
                    contentText = value;
                    OnPropertyChanged(nameof(ContentText));
                }
            }
        }

        public string ImagePath
        {
            get => imagePath;
            set
            {
                if (imagePath != value)
                {
                    imagePath = value;
                    OnPropertyChanged(nameof(ImagePath));
                }
            }
        }

        public FlowDocument Content
        {
            get => content;
            set
            {
                if (content != value)
                {
                    content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }
        public void GenerateContent()
        {
            Content = new FlowDocument();

            LinearGradientBrush gradientBrush = new LinearGradientBrush();
            gradientBrush.StartPoint = new Point(0, 0);
            gradientBrush.EndPoint = new Point(1, 1);
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(225, 153, 83), 0.0)); 
            gradientBrush.GradientStops.Add(new GradientStop(Color.FromRgb(245, 245, 220), 1.0));

            Content.Background = gradientBrush;

            Style titleStyle = new Style(typeof(Paragraph));
            titleStyle.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0)));
            titleStyle.Setters.Add(new Setter(Paragraph.TextAlignmentProperty, TextAlignment.Center));
            titleStyle.Setters.Add(new Setter(Paragraph.FontSizeProperty, 24.0));
            titleStyle.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.Bold));
            titleStyle.Setters.Add(new Setter(Paragraph.ForegroundProperty, new SolidColorBrush(Color.FromRgb(139, 69, 19)))); 

            Paragraph titleParagraph = new Paragraph(new Run(Name)) { Style = titleStyle };
            Content.Blocks.Add(titleParagraph);

            Style ingredientsStyle = new Style(typeof(Paragraph));
            ingredientsStyle.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0, 10, 0, 10)));
            ingredientsStyle.Setters.Add(new Setter(Paragraph.FontSizeProperty, 14.0));
            ingredientsStyle.Setters.Add(new Setter(Paragraph.FontWeightProperty, FontWeights.Medium));
            ingredientsStyle.Setters.Add(new Setter(Paragraph.ForegroundProperty, Brushes.Black));



            if (!string.IsNullOrEmpty(ImagePath))
            {
                BitmapImage bitmapImage = new BitmapImage(new Uri(ImagePath, UriKind.RelativeOrAbsolute));
                Image image = new Image { Source = bitmapImage, Width = 300 };
                BlockUIContainer container = new BlockUIContainer(image);

                Figure figure = new Figure
                {
                    Width = new FigureLength(300),
                    Height = new FigureLength(300),
                    HorizontalAnchor = FigureHorizontalAnchor.PageLeft,
                    CanDelayPlacement = false,
                    WrapDirection = WrapDirection.Both
                };
                figure.Blocks.Add(container);

                Paragraph paragraphWithImage = new Paragraph { Style = titleStyle };
                paragraphWithImage.Inlines.Add(figure);
                Content.Blocks.Add(paragraphWithImage);
            }

            ingredientsStyle.Setters.Add(new Setter(TextElement.FontWeightProperty, FontWeights.Bold));

            if (Ingredients != null && Ingredients.Any())
            {
                Paragraph ingredientsParagraph = new Paragraph();
                ingredientsParagraph.Style = ingredientsStyle;
                ingredientsParagraph.Inlines.Add(new Run("Ингредиенты:\n"));

                foreach (string ingredient in Ingredients)
                {
                    Run ingredientRun = new Run(ingredient)
                    {
                        FontWeight = FontWeights.Bold
                    };
                    ingredientsParagraph.Inlines.Add(ingredientRun);
                    ingredientsParagraph.Inlines.Add(new LineBreak());
                }

                Content.Blocks.Add(ingredientsParagraph);
            }


            Style descriptionStyle = new Style(typeof(Paragraph));
            descriptionStyle.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(0, 10, 0, 10)));
            descriptionStyle.Setters.Add(new Setter(Paragraph.FontSizeProperty, 14.0));
            descriptionStyle.Setters.Add(new Setter(Paragraph.ForegroundProperty, Brushes.Black));

            Paragraph paragraphWithText = new Paragraph(new Run(ContentText)) { Style = descriptionStyle };
            Content.Blocks.Add(paragraphWithText);
        }
    }


}
