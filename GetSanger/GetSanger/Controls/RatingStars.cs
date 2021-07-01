using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace GetSanger.Controls
{
    public class RatingStars : ContentView
    {
        #region Fields
        private Label ReviewsLabel { get; set; }
        private List<ImageButton> StarImages { get; set; }
        #endregion

        #region properties
        //Add in configurable "Rating" double property from XAML, for setting the rating stars
        public static readonly BindableProperty RatingProperty = BindableProperty.Create(
                                                                              nameof(Rating),
                                                                              typeof(int),
                                                                              typeof(RatingStars),
                                                                              0,
                                                                              BindingMode.TwoWay,
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

        public static BindableProperty ReviewProperty = BindableProperty.Create(
            nameof(Review),
            typeof(string),
            typeof(RatingStars),
            "No Description"
            , BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ratingStars = (RatingStars)bindable;
                ratingStars.updateReviewsDisplay();
            }
        );

        public string Review
        {
            get { return (string)GetValue(ReviewProperty); }
            set { SetValue(ReviewProperty, value); }
        }

        public static BindableProperty IsImageEnabledProperty = BindableProperty.Create(
            nameof(IsImageEnabled),
            typeof(bool),
            typeof(RatingStars),
            false
            , BindingMode.OneWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var ratingStars = (RatingStars)bindable;
                ratingStars.updateIsEnabled();
            }
        );

        public bool IsImageEnabled
        {
            get { return (bool)GetValue(IsImageEnabledProperty); }
            set { SetValue(IsImageEnabledProperty, value); }
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
            StarImages = new List<ImageButton>();
            for (int i = 0; i < 5; i++)
            {
                StarImages.Add(new ImageButton { BackgroundColor = Color.Transparent });
            }

            //Create Horizontal Stack containing stars and review count label 
            StackLayout starsStack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Start,
                Padding = 0,
                Spacing = 0,
                Children = {
                    ReviewsLabel,
                    StarImages[0],
                    StarImages[1],
                    StarImages[2],
                    StarImages[3],
                    StarImages[4]
                }
            };

            updateReviewsDisplay();
            updateIsEnabled();
            updateStarsDisplay();
            this.Content = starsStack;
        }

        private void RatingStars_Clicked(object sender, EventArgs e)
        {
            ImageButton current = sender as ImageButton;
            int counter = 0;
            bool found = false;
            foreach(ImageButton image in StarImages)
            {
                if(found == false)
                {
                    counter++;
                    image.Source = "ratingStarOn.png";
                }
                else
                {
                    image.Source = "ratingStarOff.png";
                }

                if (image.Equals(current))
                {
                    found = true;
                }
            }

            Rating = counter;
        }

        public void updateIsEnabled()
        {
            foreach (ImageButton image in StarImages)
            {
                if (IsImageEnabled)
                {
                    image.Clicked += RatingStars_Clicked;
                    image.IsEnabled = true;
                }
                else
                {
                    image.Clicked -= RatingStars_Clicked;
                    image.IsEnabled = false;
                }
            }
        }

        //Set the Display of the Review Label 
        public void updateReviewsDisplay()
        {
            _ = string.IsNullOrWhiteSpace(Review) ? ReviewsLabel.IsVisible = false : ReviewsLabel.IsVisible = true;
            ReviewsLabel.Text = Review;
        }


        //Set the correct images for the stars based on the rating 
        public void updateStarsDisplay()
        {
            for (int i = 0; i < 5; i++) // "rating" instead of 5 will show just full stars. 
            {
                StarImages[i].Source = GetStarFileName(i);
            }
        }

        //uses zero based position for stars 
        private string GetStarFileName(int position)
        {
            int currentStarMaxRating = (position + 1) ;
            //Rating is out of 5
            if (Rating >= currentStarMaxRating)
            {
                return "ratingStarOn.png";
            }
            else
            {
                return "ratingStarOff.png";
            }
        }


        #endregion
    }
}