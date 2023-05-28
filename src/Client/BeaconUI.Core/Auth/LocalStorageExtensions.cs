using Beacon.Common.Auth;
using Blazored.LocalStorage;

namespace BeaconUI.Core.Auth;

public static class LocalStorageExtensions
{
    public const string CurrentUserInfoKey = "CurrentUserInfo";

    public static async Task<AuthenticatedUserInfo?> GetCurrentUserInfo(this ILocalStorageService localStorage, CancellationToken ct = default)
    {
        return await localStorage.GetItemAsync<AuthenticatedUserInfo>(CurrentUserInfoKey, ct);
    }

    public static async Task ClearCurrentUserInfo(this ILocalStorageService localStorage, CancellationToken ct = default)
    {
        await localStorage.RemoveItemAsync(CurrentUserInfoKey, ct);
    }

    public static async Task SetCurrentUserInfo(this ILocalStorageService localStorage, AuthenticatedUserInfo user, CancellationToken ct = default)
    {
        await localStorage.SetItemAsync(CurrentUserInfoKey, user, ct);
    }
}
