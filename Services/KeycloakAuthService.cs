using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using BusinessLayer.Dtos;
using Microsoft.Extensions.Configuration;

public class KeycloakAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public KeycloakAuthService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<UserProfileDto?> LoginAsync(string username, string password)
    {
        var tokenEndpoint =
            $"{_config["Keycloak:BaseUrl"]}/realms/{_config["Keycloak:Realm"]}/protocol/openid-connect/token";

        var request = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "client_id", _config["Keycloak:ClientId"]! },
            { "client_secret", _config["Keycloak:ClientSecret"]! },
            { "grant_type", "password" },
            { "username", username },
            { "password", password },
        });

        var response = await _httpClient.PostAsync(tokenEndpoint, request);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var token = JsonSerializer.Deserialize<TokenResponse>(json);

        if (token == null)
            return null;

        // Decodificar JWT
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token.access_token);

        var userId = jwt.Claims.First(c => c.Type == "sub").Value;
        var userUsername = jwt.Claims.FirstOrDefault(c => c.Type == "uername")?.Value ?? username;
        var firstName = jwt.Claims.FirstOrDefault(c => c.Type == "given_name")?.Value ?? "";
        var lastName = jwt.Claims.FirstOrDefault(c => c.Type == "family_name")?.Value ?? "";

        // Extraer roles del realm
        var realmAccess = jwt.Claims.FirstOrDefault(c => c.Type == "realm_access");

        var roles = new List<string>();

        if (realmAccess != null)
        {
            var realmAccessObj = JsonSerializer
                .Deserialize<Dictionary<string, List<string>>>(realmAccess.Value);

            if (realmAccessObj != null && realmAccessObj.ContainsKey("roles"))
            {
                roles = realmAccessObj["roles"];
            }
        }

        return new UserProfileDto(userId, userUsername, firstName, lastName, roles);
    }
}
