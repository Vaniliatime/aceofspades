using MemoryGame.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace MemoryGame.ViewModels
{
    public enum SlideCategories
    {
        Animals,
        Cars,
        Foods
    }
    public class GameViewModel : ObservableObject
    {
        //Collection of slides 
        public SlideCollectionViewModel Slides { get; private set; }
        // scores,
        public GameInfoViewModel GameInfo { get; private set; }
        // timer 
        public TimerViewModel Timer { get; private set; }
        //Category
        public SlideCategories Category { get; private set; }

        public GameViewModel(SlideCategories category)
        {
            Category = category;
            SetupGame(category);
        }

      
        private void SetupGame(SlideCategories category)
        {

            Slides = new SlideCollectionViewModel();
            Timer = new TimerViewModel(new TimeSpan(0, 0, 1));
            GameInfo = new GameInfoViewModel();

            //,ax allowed
            GameInfo.ClearInfo();

            //display
            Slides.CreateSlides("Assets/" + category.ToString());
            Slides.Memorize();

            
            Timer.Start();

           
            OnPropertyChanged("Slides");
            OnPropertyChanged("Timer");
            OnPropertyChanged("GameInfo");
        }

        //clicked
        public void ClickedSlide(object slide)
        {
            if(Slides.canSelect)
            {
                var selected = slide as PictureViewModel;
                Slides.SelectSlide(selected);
            }

            if(!Slides.areSlidesActive)
            {
                if (Slides.CheckIfMatched())
                    GameInfo.Award(); //Correct match
                else
                    GameInfo.Penalize();//Incorrect match
            }

            GameStatus();
        }

        
        private void GameStatus()
        {
            if(GameInfo.MatchAttempts < 0)
            {
                GameInfo.GameStatus(false);
                Slides.RevealUnmatched();
                Timer.Stop();
            }

            if(Slides.AllSlidesMatched)
            {
                GameInfo.GameStatus(true);
                Timer.Stop();
            }
        }

       
        public void Restart()
        {
            SoundManager.PlayIncorrect();
            SetupGame(Category);
        }
    }
}
