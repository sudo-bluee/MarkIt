﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MarkItDesktop.ViewModels
{
    public class SideMenuViewModel : BaseViewModel
    {
		private string? _username;
        private string? _fullname;
        private bool _isMenuOpen;

        public string? Username
		{
			get => _username;
			set
			{
				_username = value;
				OnPropertyChanged();
			}
		}

		public string? FullName
		{
			get => _fullname;
			set
			{
				_fullname = value;
				OnPropertyChanged();
			}
		}
		public bool IsMenuOpen
		{
			get => _isMenuOpen;
			set
			{
				_isMenuOpen = value;
				OnPropertyChanged();
			}
		}


		public ICommand	OpenSettingsCommand { get; private set; }

		public SideMenuViewModel()
		{
			OpenSettingsCommand = new RelayCommand(OpenSettings);
		}

		private void OpenSettings()
		{
			App.AppHost.Services.GetRequiredService<SettingsViewModel>().IsSettingsOpen = true;
		}

	}
}
