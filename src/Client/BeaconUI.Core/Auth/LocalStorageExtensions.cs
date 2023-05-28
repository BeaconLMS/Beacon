using Beacon.Common.Auth;
using Blazored.LocalStorage;

namespace BeaconUI.Core.Auth;

public static class LocalStorageExtensions
{
    public const string CurrentUserInfoKey = "CurrentUserInfo";

    public static async Task<UserDto?> GetCurrentUserInfo(this ILocalStorageService localStorage, CancellationToken ct = default)
    {
        return await localStorage.GetItemAsync<UserDto>(CurrentUserInfoKey, ct);
    }

    public static async Task ClearCurrentUserInfo(this ILocalStorageService localStorage, CancellationToken ct = default)
    {
        await localStorage.RemoveItemAsync(CurrentUserInfoKey, ct);
    }

    public static async Task SetCurrentUserInfo(this ILocalStorageService localStorage, UserDto user, CancellationToken ct = default)
    {
        await localStorage.SetItemAsync(CurrentUserInfoKey, user, ct);
    }
}
