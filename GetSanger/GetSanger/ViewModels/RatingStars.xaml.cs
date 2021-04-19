using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GetSanger.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RatingStars : ContentView
    {
        #region Fields
        private Label ReviewsLabel { get; set; }
        private List<Image> StarImages { get; set; }
        #endregion

        #region properties
        //Add in configurable "Rating" double property from XAML, for setting the rating stars
        public static readonly BindableProperty RatingProperty = BindableProperty.Create(
                                                                              nameof(Rating),
                                                                              typeof(int),
                                                                              typeof(RatingStars),
                                                                              0,
                                                                              BindingMode.OneWay,
                                                                              propertyChanged: (bindable, oldValue, newValue) =>
                                                                              {
                                                                                  var ratingStars = (RatingStars)bindable;
                                                                                  ratingStars.updateStarsDisplay();
                                                                              }
                                                                          );

        //Rating is out of 10 
        public int Rating
        {
            get { return (int)GetValue(RatingProperty); }
            set { SetValue(RatingProperty, value); }
        }

        //Add in configurable "Rating" double property from XAML, for setting the rating stars

        public static BindableProperty ReviewsProperty = BindableProperty.Create(
            nameof(Reviews),
            typeof(int),
            typeof(RatingStars),
            0, BindingMode.OneWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ratingStars = (RatingStars)bindable;
                ratingStars.updateReviewsDisplay();
            }
        );


        //Count of the total number of reviews 
        public int Reviews
        {
            get { return (int)GetValue(ReviewsProperty); }
            set { SetValue(ReviewsProperty, value); }
        }
        #endregion

        #region Constructor
        public RatingStars()
        {
            GenerateDisplay();
        }

        #endregion

        #region Methods
        private void GenerateDisplay()
        {

            //Creates Review Count Label 
            ReviewsLabel = new Label
            {
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
            };

            //Create Star Image Placeholders 
            StarImages = new List<Image>();
            for (int i = 0; i < 5; i++)
                StarImages.Add(new Image());

            //Create Horizontal Stack containing stars and review count label 
            StackLayout starsStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Start,
                Padding = 0,
                Spacing = 0,
                Children = {
                    StarImages[0],
                    StarImages[1],
                    StarImages[2],
                    StarImages[3],
                    StarImages[4],
                    ReviewsLabel
                }
            };

            updateReviewsDisplay();

            updateStarsDisplay();
            this.Content = starsStack;
        }

        //Set the Display of the Reviews Label 
        public void updateReviewsDisplay()
        {
            ReviewsLabel.Text = Reviews > 0 ? " (" + Convert.ToString(Reviews) + ")" : "";
        }


        //Set the correct images for the stars based on the rating 
        public void updateStarsDisplay()
        {
            for (int i = 0; i < StarImages.Count; i++)
            {
                StarImages[i].Source = GetStarFileName(i);
            }
        }

        //uses zero based position for stars 
        private string GetStarFileName(int position)
        {
            int currentStarMaxRating = (position + 1) * 2;
            //Rating is out of 10 
            if (Rating >= currentStarMaxRating)
            {
                return "rating_star_on.png";
            }
            else if (Rating >= currentStarMaxRating - 1)
            {
                return "rating_star_half.png";
            }
            else
            {
                return "rating_star_off.png";
            }
        }


        #endregion

    }
}