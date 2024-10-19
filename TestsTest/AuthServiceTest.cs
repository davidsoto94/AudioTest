using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using test.Database.Entities;
using test.Services;

namespace TestsTest;

[TestClass]
public class AuthServiceTests
{
    private AuthService _authService;
    public AuthServiceTests()
    {
        _authService = new AuthService();
        Environment.SetEnvironmentVariable("TEST_PRIVATE_KEY", "this_is_a_very_strong_key_12345678");
    }

    [TestMethod]
    public void GenerateToken_ReturnsValidToken_WhenUserIsValid()
    {
        // Arrange
        var userId = Guid.NewGuid(); // Create a new Guid for user ID
        var user = new User(userId, "FullName", "test@example.com", "password",  ["User", "Admin"]);

        // Act
        var token = _authService.GenerateToken(user);

        // Assert
        Assert.IsNotNull(token);
        Assert.IsInstanceOfType(token, typeof(string));

        // Validate the token
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Check that the claims include the user's email
        var emailClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "email");
        Assert.IsNotNull(emailClaim, "Email claim was not found in the token.");
        Assert.AreEqual(user.Email, emailClaim.Value);

        // Check roles
        var roleClaims = jwtToken.Claims.Where(claim => claim.Type == "role").ToList();
        Assert.AreEqual(2, roleClaims.Count); // Ensure roles are added
        Assert.IsTrue(roleClaims.Any(claim => claim.Value == "User"));
        Assert.IsTrue(roleClaims.Any(claim => claim.Value == "Admin"));
    }
}