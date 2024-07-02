using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ReduxSimple;
using Rownd.Maui.Core;
using Rownd.Maui.Models.Domain;
using Rownd.Maui.Utils;

namespace Rownd.Maui.Models.Repos
{
    public class StateRepo
    {
        public ReduxStore<GlobalState> Store
        {
            get;
            private set;
        }

        public static StateRepo Get()
        {
            return Shared.ServiceProvider.GetService<StateRepo>();
        }

        private IDisposable statePersistenceListener;

        public StateRepo()
        {
        }

        public void Setup()
        {
            LoadState();
            SetupAsync();
        }

        private void SetupAsync()
        {
            Task.Run(async () =>
            {
                var appConfigRepo = AppConfigRepo.Get();
                Console.WriteLine("Going to load app config");
                await appConfigRepo.LoadAppConfigAsync();
                Console.WriteLine("Done loading app config");

                if (Store.State.Auth.IsAuthenticated)
                {
                    // Ensure we have a valid access token
                    await AuthRepo.Get().GetAccessToken();

                    await UserRepo.GetInstance().FetchUser();
                }
                else
                {
                    SignInLinkHandler.Get().HandleSignInLinkIfPresent();
                }

                // Initialization complete
                Store.State.IsReady = true;
                Store.Dispatch(new StateActions.SetGlobalState()
                {
                    GlobalState = Store.State
                });

                // Check to see if we were handling an existing auth challenge
                // (maybe the app crashed or got OOM killed)
                if (Store.State.Auth.ChallengeId != null && Store.State.Auth.UserIdentifier != null)
                {
                    Shared.Rownd.RequestSignIn(new RowndSignInJsOptions
                    {
                        SignInStep = SignInStep.Completing,
                        ChallengeId = Store.State.Auth.ChallengeId,
                        UserIdentifier = Store.State.Auth.UserIdentifier
                    });
                }
            });
        }

        // TODO: Provide a generic interface for specific portions of the state tree.
        public IDisposable Subscribe(Action<StateBase> action)
        {
            return Store.Select(state => state.Auth).Subscribe(action);
        }

        private void LoadState()
        {
            try
            {
                var existingStateJsonStr = Preferences.Get("rownd_state", null);
                GlobalState? existingState = JsonConvert.DeserializeObject<GlobalState>(existingStateJsonStr);
                Console.WriteLine($"Restoring existing state: {existingState}");
                InitializeStore(existingState);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting existing state. This is expected on first run. {ex}");
                InitializeStore();
                return;
            }
        }

        private void SaveState(GlobalState state)
        {
            var stateJson = JsonConvert.SerializeObject(state);
            Console.WriteLine($"Saving serialized state to storage: {stateJson}");
            Preferences.Set("rownd_state", stateJson);
        }

        private void InitializeStore(GlobalState existingState = null)
        {
            if (Store != null)
            {
                return;
            }

            if (existingState == null)
            {
                existingState = new GlobalState();
            }

            existingState.IsInitialized = true;

            Store = new ReduxStore<GlobalState>(StateReducers.CreateReducers(), existingState);

            statePersistenceListener = Store.Select()
                .Subscribe(state =>
                {
                    // Listening to the full state (when any property changes)
                    Console.WriteLine($"Storing Rownd state: {state}");
                    SaveState(state);

                    if (state.IsReady)
                    {
                        AutomationProcessor.RunAutomationsIfNeeded(state);
                    }
                });
        }
    }
}
