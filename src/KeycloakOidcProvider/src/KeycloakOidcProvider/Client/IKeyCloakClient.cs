﻿using OidcProvider.Models;

namespace Keycloak.OidcProvider.Client;

public interface IKeyCloakClient
{
    Task<GroupCreatedResponse> CreateGroup(string name);
}