﻿using MarkItDesktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkItDesktop.ViewModels
{
    public class ApplicationViewModel : BaseViewModel
    {
		private ApplicationPage _currentPage;

		public ApplicationPage CurrentPage
		{
			get => _currentPage;
			private set
			{
				_currentPage = value;
				OnPropertyChanged();
                OnPropertyChanged(nameof(IsMenuOpen));

            }
        }

		public bool IsMenuOpen => CurrentPage == ApplicationPage.MainPage;

		public ApplicationViewModel()
		{
			CurrentPage = ApplicationPage.LaunchPage;
		}


		public void NavigateTo(ApplicationPage page)
		{
			CurrentPage = page;
		}
	}
}
