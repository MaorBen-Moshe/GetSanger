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

        public static BindableProperty ReviewProperty = BindableProperty.Create(
            nameof(Review),
            typeof(string),
            typeof(RatingStars),
            "No Description"
            , BindingMode.OneWay,
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
                    ReviewsLabel,
                    StarImages[0],
                    StarImages[1],
                    StarImages[2],
                    StarImages[3],
                    StarImages[4]
                }
            };

            updateReviewsDisplay();

            updateStarsDisplay();
            this.Content = starsStack;
        }

        //Set the Display of the Review Label 
        public void updateReviewsDisplay()
        {
            ReviewsLabel.Text = Review;
        }


        //Set the correct images for the stars based on the rating 
        public void updateStarsDisplay()
        {
            for (int i = 0; i < 5; i++) // "rating" insated of 5 will show just fulll stars. 
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