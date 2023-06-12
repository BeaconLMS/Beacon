﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace BeaconUI.Core;

public static class JsonDefaults
{
    public static JsonSerializerOptions JsonSerializerOptions
    {
        get
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            options.Converters.Add(new JsonStringEnumConverter());
            return options;
        }
    }
}