﻿using MarkIt.Common.Models;
using MarkItDesktop.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MarkItDesktop.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<TodoItemViewModel> TodoItems { get; set; } = new();

        #region Private members

        private string _text = string.Empty;
        private readonly ITodoService _todoService;
        private bool _ignoreChanges;

        #endregion

        #region Public properties


        public string Text
        {
            get => _text;
            set 
            { 
                _text = value;
                OnPropertyChanged();
            }
        }


        #endregion

        #region Commands
        
        public ICommand AddCommand { get; private set; }

        #endregion

        public MainViewModel(ITodoService todoService)
        {
            AddCommand = new RelayComamnd(AddTodo);
            this._todoService = todoService;
            // TODO : Call this when window has loaded
            Task.Run(LoadItems);
        }

        async void LoadItems()
        {
            IList<TodoResponseModel>? items = await _todoService.GetTodosAsync();
            if (items is not null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if(!_ignoreChanges)
                    {
                        _ignoreChanges = true;
                        TodoItems.Clear();
                        foreach (TodoResponseModel todo in items)
                        {
                            TodoItems.Add(new(todo.Id, this)
                            {
                                Text = todo.Text,
                                IsCompleted = todo.IsCompleted
                            });
                        }
                        _ignoreChanges = false;
                    }
                });
            }
        }

        public async void AddTodo()
        {
            if (string.IsNullOrWhiteSpace(Text))
                return;

            TodoResponseModel? todo = await _todoService.CreateTodoAsync(
                new TodoApiModel()
                {
                    Text = Text,
                    IsCompleted = false
                });

            Text = string.Empty;

            if (todo is null)
                return;
            if (!_ignoreChanges)
            {
                _ignoreChanges = true;
                TodoItems.Add(new(todo.Id, this)
                {
                    Text = todo.Text,
                    IsCompleted = todo.IsCompleted
                });
                _ignoreChanges = false;
            }
        }

        public async void UpdateTodo(TodoItemViewModel item)
        {
            if (_ignoreChanges) return;
            TodoResponseModel? todo = await _todoService.UpdateTodoAsync( item.Id,
                new TodoApiModel()
                {
                    IsCompleted = item.IsCompleted,
                    Text = item.Text
                });

        }
    }
}
