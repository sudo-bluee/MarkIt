﻿using MarkIt.Common.Models;
using MarkItDesktop.Data;
using MarkItDesktop.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MarkItDesktop.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _client;
        private readonly IClientDataStore _store;

        public AuthService(HttpClient client, IClientDataStore store)
        {
            _client = client;
            _store = store;
        }

        public async Task<bool> LoginAsync(string username, string password)
        {
            HttpResponseMessage response =  await _client.PostAsJsonAsync<LoginApiModel>("login", new() { 
                Username = username,
                Password = password
            });
            // TODO : Handle unexpected responses.
            var content = await response.Content.ReadFromJsonAsync<APIResponseModel<LoginResponseModel>>();
            if (content is { } model)
            {
                if (!model.Succeeded)
                    throw new AuthenticationException(model.Errors.FirstOrDefault());

                await _store.AddLoginDataAsync(content.Response);

                return model.Succeeded;
            }
            return false;
        }

        public async Task<bool> RegisterAsync(string username, string password, string email, string fullName)
        {
            HttpResponseMessage response = await _client.PostAsJsonAsync<RegisterApiModel>("register", new()
            {
                Username = username,
                Password = password,
                Email = email,
                FullName = fullName
            });


            var content = await response.Content.ReadFromJsonAsync<APIResponseModel<RegisterResponseModel>>();
            if (content is { } model)
            {
                if (!model.Succeeded)
                    throw new AuthenticationException(model.Errors.FirstOrDefault());

                return model.Succeeded;
            }
            return false;
        }
    }
}
