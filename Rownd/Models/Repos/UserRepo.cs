using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using Rownd.Maui.Core;
using Rownd.Maui.Models.Domain;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace Rownd.Maui.Models.Repos
{
    public class UserRepo
    {
        private readonly StateRepo stateRepo = StateRepo.Get();

        public static UserRepo GetInstance()
        {
            return Shared.ServiceProvider.GetService<UserRepo>();
        }

        public UserRepo() { }

        public Dictionary<string, dynamic> Get()
        {
            return stateRepo.Store.State.User.Data;
        }

        public T Get<T>(string field)
        {
            return stateRepo.Store.State.User.Get<T>(field);
        }

        public void Set(Dictionary<string, dynamic> data)
        {
            stateRepo.Store.State.User.Set(data);
        }

        public void Set(string field, dynamic value)
        {
            stateRepo.Store.State.User.Set(field, value);
        }

        internal async Task<UserState> SaveUser(UserState user)
        {
            var apiClient = ApiClient.Get();
            try
            {
                var request = new RestRequest($"me/applications/{stateRepo.Store.State.AppConfig.Id}/data")
                    .AddBody(user);
                var response = await apiClient.Client.ExecutePutAsync<UserState>(request);
                Device.BeginInvokeOnMainThread(() =>
                    stateRepo.Store.Dispatch(new StateActions.SetUserState()
                    {
                        UserState = response.Data
                    })
);
                return response.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to save the user: {ex}");
                return null;
            }
        }

        internal async Task<UserState> FetchUser()
        {
            var apiClient = ApiClient.Get();
            try
            {
                var response = await apiClient.Client.GetJsonAsync<UserState>($"me/applications/{stateRepo.Store.State.AppConfig.Id}/data");
                Device.BeginInvokeOnMainThread(() =>
                    stateRepo.Store.Dispatch(new StateActions.SetUserState()
                    {
                        UserState = response
                    })
);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch the user: {ex}");
                return null;
            }
        }
    }
}
