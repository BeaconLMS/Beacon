﻿using Beacon.Common.Services;
using Blazored.LocalStorage;

namespace BeaconUI.Core.Services;

public sealed class LabContext : ILabContext, IDisposable
{
    private readonly ILocalStorageService _localStorage;

    public Guid LaboratoryId { get; private set; }

    public LabContext(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
        _localStorage.Changed += HandleChange;

        Initialize();
    }

    public void Dispose()
    {
        _localStorage.Changed -= HandleChange;
    }

    private async void Initialize()
    {
        LaboratoryId = await _localStorage.GetItemAsync<Guid>("CurrentLaboratoryId");
    }

    private void HandleChange(object? o, ChangedEventArgs e)
    {
        if (e.Key != "CurrentLaboratoryId")
            return;

        LaboratoryId = e.NewValue is Guid id ? id : Guid.Empty;
    }
}
