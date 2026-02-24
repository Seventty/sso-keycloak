using BusinessLayer.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace sso_keycloak.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly KeycloakAuthService _authService;

    public AuthController(KeycloakAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var profile = await _authService.LoginAsync(dto.User, dto.Password);

        if (profile == null)
            return Unauthorized(new { message = "Credenciales inválidas" });

        return Ok(profile);
    }
}

