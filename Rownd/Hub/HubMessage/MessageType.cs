using System;
using System.Runtime.Serialization;

namespace Rownd.Maui.Hub.HubMessage
{
    public enum MessageType
    {
        [EnumMember(Value = "authentication")]
        Authentication,

        [EnumMember(Value = "sign_out")]
        SignOut,

        [EnumMember(Value = "close_hub_view_controller")]
        CloseHub,

        [EnumMember(Value = "trigger_sign_in_with_apple")]
        TriggerSignInWithApple,

        [EnumMember(Value = "user_data_update")]
        UserDataUpdate,

        [EnumMember(Value = "trigger_sign_in_with_google")]
        TriggerSignInWithGoogle,

        [EnumMember(Value = "trigger_sign_up_with_passkey")]
        TriggerSignUpWithPasskey,

        [EnumMember(Value = "trigger_sign_in_with_passkey")]
        TriggerSignInWithPasskey,

        [EnumMember(Value = "hub_loaded")]
        HubLoaded,

        [EnumMember(Value = "hub_resize")]
        HubResize,

        [EnumMember(Value = "can_touch_background_to_dismiss")]
        CanTouchBackgroundToDismiss,

        [EnumMember(Value = "event")]
        Event,

        [EnumMember(Value = "try_again")]
        TryAgain,

        [EnumMember(Value = "auth_challenge_initiated")]
        AuthChallengeInitiated,

        [EnumMember(Value = "auth_challenge_cleared")]
        AuthChallengeCleared,
    }
}