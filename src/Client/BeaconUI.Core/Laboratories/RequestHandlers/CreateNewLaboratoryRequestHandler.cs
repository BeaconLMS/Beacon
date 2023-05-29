﻿using Beacon.Common;
using Beacon.Common.Laboratories;
using Beacon.Common.Laboratories.Events;
using Beacon.Common.Laboratories.Requests;
using BeaconUI.Core.Helpers;
using ErrorOr;
using MediatR;
using System.Net.Http.Json;

namespace BeaconUI.Core.Laboratories.RequestHandlers;

public class CreateNewLaboratoryRequestHandler : IApiRequestHandler<CreateNewLaboratoryRequest, LaboratoryDto>
{
    private readonly HttpClient _httpClient;
    private readonly IPublisher _publisher;

    public CreateNewLaboratoryRequestHandler(HttpClient httpClient, IPublisher publisher)
    {
        _httpClient = httpClient;
        _publisher = publisher;
    }

    public async Task<ErrorOr<LaboratoryDto>> Handle(CreateNewLaboratoryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync("api/laboratories", request, cancellationToken);
        var result = await response.ToErrorOrResult<LaboratoryDto>(cancellationToken);

        if (!result.IsError)
            await _publisher.Publish(new LaboratoryCreatedEvent(result.Value), cancellationToken);

        return result;
    }
}
