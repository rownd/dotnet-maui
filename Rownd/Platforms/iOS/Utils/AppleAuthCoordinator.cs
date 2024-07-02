using AuthenticationServices;
using Foundation;
using Rownd.Maui.Core;
using Rownd.Maui.Models.Repos;
using Rownd.Maui.Utils;
using UIKit;

[assembly: Dependency(typeof(Rownd.Maui.iOS.AppleAuthCoordinator))]
namespace Rownd.Maui.iOS
{
    public class AppleAuthCoordinator : NSObject, IAppleAuthCoordinator, IASAuthorizationControllerDelegate, IASAuthorizationControllerPresentationContextProviding
    {
        private RowndInstance? rownd;
        private AuthRepo? authRepo;
        private SignInIntent? intent;

        public void Inject(RowndInstance rownd, AuthRepo authRepo)
        {
            this.rownd = rownd;
            this.authRepo = authRepo;
        }

        public void SignIn()
        {
            SignIn(null);
        }

        public void SignIn(SignInIntent? intent)
        {
            Console.WriteLine("AppleAuthCoordinator.SignIn(intent)");
            this.intent = intent;

            // Create an object of the ASAuthorizationAppleIDProvider
            var appleIDProvider = new ASAuthorizationAppleIdProvider();

            // Create a request
            var request = appleIDProvider.CreateRequest();

            // Define the scope of the request
            request.RequestedScopes = new ASAuthorizationScope[] { ASAuthorizationScope.FullName, ASAuthorizationScope.Email };

            // Make the request
            var authorizationController = new ASAuthorizationController(new ASAuthorizationRequest[] { request })
            {
                // Assigning the delegates
                PresentationContextProvider = this,
                Delegate = this
            };

            authorizationController.PerformRequests();
        }

        public UIWindow GetPresentationAnchor(ASAuthorizationController controller)
        {
            var vc = UIApplication.SharedApplication.Windows[^1].RootViewController;
            return vc?.View?.Window!;
        }

        private string GetFullName(string firstName, string lastName)
        {
            return $"{firstName ?? ""} {lastName ?? ""}";
        }

        #region IASAuthorizationController Delegate

        // If authorization is successful then this method will get triggered
        [Export("authorizationController:didCompleteWithAuthorization:")]
        public void DidComplete(ASAuthorizationController controller, ASAuthorization authorization)
        {
            // TODO: Send "completing" notification to Rownd shared lib
            rownd?.RequestSignIn(new RowndSignInJsOptions
            {
                SignInStep = SignInStep.Completing
            });

            if (authorization.GetCredential<ASAuthorizationAppleIdCredential>() is ASAuthorizationAppleIdCredential appleIdCredential)
            {
                // Create an account in your system.
                // let userIdentifier = appleIDCredential.user
                var fullName = appleIdCredential.FullName;
                var email = appleIdCredential.Email;
                var identityToken = appleIdCredential.IdentityToken;

                if (identityToken != null && new NSString(identityToken, NSStringEncoding.ASCIIStringEncoding) is NSString urlContent)
                {
                    var idToken = urlContent.ToString();

                    _ = authRepo?.HandleThirdPartySignIn(new Models.ThirdPartySignInData
                    {
                        Token = idToken,
                        Intent = intent,
                        SignInMethod = SignInMethod.Apple,
                        UserData = new Dictionary<string, string>
                        {
                            { "email", email },
                            { "first_name", fullName?.GivenName },
                            { "last_name", fullName?.FamilyName },
                            { "full_name", GetFullName(fullName?.GivenName, fullName?.FamilyName) }
                        }
                    });
                    //Task {
                    //    do
                    //    {
                    //        let tokenResponse = try await Auth.fetchToken(idToken: idToken, intent: intent);





                    //        Task {
                    //            @MainActor in
                    //    Rownd.requestSignIn(jsFnOptions: RowndSignInJsOptions(
                    //        loginStep: RowndSignInLoginStep.success,
                    //        intent: self.intent,
                    //        userType: tokenResponse?.userType
                    //    ));
                    //        }

                    //        // Prevent fast auth handshakes from feeling jarring to the user
                    //        try await Task.sleep(nanoseconds: UInt64(2 * Double(NSEC_PER_SEC)))





                    //DispatchQueue.main.async {
                    //            store.dispatch(store.state.auth.onReceiveAuthTokens(
                    //                AuthState(
                    //                    accessToken: tokenResponse?.accessToken,
                    //                    refreshToken: tokenResponse?.refreshToken
                    //                )
                    //            ))

                    //    store.dispatch(SetLastSignInMethod(payload: SignInMethodTypes.apple))

                    //    store.dispatch(Thunk<RowndState> {
                    //                dispatch, getState in
                    //        guard let state = getState() else { return }

                    //                var userData = state.user.data

                    //        let defaults = UserDefaults.standard
                    //        //use UserDefault values for Email and fullName if available
                    //        if let userAppleSignInData = defaults.object(forKey: appleSignInDataKey) as? Data {
                    //                    let decoder = JSONDecoder()
                    //            if let loadedAppleSignInData = try? decoder.decode(AppleSignInData.self, from: userAppleSignInData) {
                    //                        userData["email"] = AnyCodable.init(loadedAppleSignInData.email)
                    //                userData["first_name"] = AnyCodable.init(loadedAppleSignInData.firstName)
                    //                userData["last_name"] = AnyCodable.init(loadedAppleSignInData.lastName)
                    //                userData["full_name"] = AnyCodable.init(loadedAppleSignInData.fullName)
                    //            }
                    //                    } else
                    //                    {
                    //                        if let email = email {
                    //                            userData["email"] = AnyCodable.init(email)
                    //                                userData["first_name"] = AnyCodable.init(fullName?.givenName)
                    //                                userData["last_name"] = AnyCodable.init(fullName?.familyName)
                    //                                userData["full_name"] = AnyCodable.init(String("\(fullName?.givenName) \(fullName?.familyName)"))
                    //                            }
                    //                    }

                    //                    if (!userData.isEmpty)
                    //                    {
                    //                        DispatchQueue.main.async {
                    //                            dispatch(UserData.save(userData))
                    //                            }
                    //                    }
                    //                })
                    //}
                    //        } catch ApiError.generic(let errorInfo) {
                    //            if errorInfo.code == "E_SIGN_IN_USER_NOT_FOUND" {
                    //                Rownd.requestSignIn(jsFnOptions: RowndSignInJsOptions(
                    //                    token: idToken,
                    //                    loginStep: .noAccount,
                    //                    intent: .signIn
                    //                ))
                    //                }
                    //            else
                    //            {
                    //                DispatchQueue.main.async {
                    //                    Rownd.requestSignIn(jsFnOptions: RowndSignInJsOptions(
                    //                        loginStep: .error,
                    //                        signInType: .apple
                    //                    ))
                    //                    }
                    //            }
                    //        } catch
                    //        {
                    //            DispatchQueue.main.async {
                    //                Rownd.requestSignIn(jsFnOptions: RowndSignInJsOptions(
                    //                    loginStep: .error,
                    //                    signInType: .apple
                    //                ))
                    //                }
                    //        }
                    //        }
                }
                else
                {
                    // TODO: Show error hub state
                    //                logger.error("Missing data from Apple sign-in response: \(String(describing: appleIDCredential))")
                    //Rownd.requestSignIn(jsFnOptions: RowndSignInJsOptions(
                    //    loginStep: .error,
                    //    signInType: .apple
                    //))
                }
            }
            else if (authorization.GetCredential<ASAuthorizationPlatformPublicKeyCredentialRegistration>() is ASAuthorizationPlatformPublicKeyCredentialRegistration credentialRegistration)
            {
                var attestationObject = credentialRegistration.RawAttestationObject;
                var clientDataJSON = credentialRegistration.RawClientDataJson;
                var credentialID = credentialRegistration.CredentialId; // TODO: need this as base64url encoded

                // TODO: Handle registration API stuff in shared code
            }
            else if (authorization.GetCredential<ASAuthorizationPlatformPublicKeyCredentialAssertion>() is ASAuthorizationPlatformPublicKeyCredentialAssertion credentialAssertion)
            {
                var signature = credentialAssertion.Signature;
                var clientDataJSON = credentialAssertion.RawClientDataJson;
                var userId = credentialAssertion.UserId;
                var credentialID = credentialAssertion.CredentialId; // TODO: need this as base64url encoded
                var authenticatorData = credentialAssertion.RawAuthenticatorData;

                // TODO: Handle passkey authorization/authentication API stuff in shared code
            }
            else
            {
                // TODO: Show error. This is an unknown credential type
                // logger.error("Unknown credential type returned from Apple ID sign-in: \(String(describing: authorization.credential))")
                //        Rownd.requestSignIn(jsFnOptions: RowndSignInJsOptions(
                //    loginStep: .error,
                //    signInType: .apple
                // ))
            }
        }

        // Handle authorization failures
        [Export("authorizationController:didCompleteWithError:")]
        public void DidComplete(ASAuthorizationController controller, NSError error)
        {
            // If there is any error, we'll get it here
            Console.WriteLine($"An error occurred while signing in with Apple. Error: {error}");

            if (error == null)
            {
                return;
            }

            // Error is a user cancellation
            if (error.Domain == "com.apple.AuthenticationServices.AuthorizationError" && error.Code == 1001)
            {
                return;
            }

            rownd?.RequestSignIn(new RowndSignInJsOptions
            {
                SignInStep = SignInStep.Error,
                SignInType = SignInType.Apple,
            });
            return;
        }

        #endregion
    }
}