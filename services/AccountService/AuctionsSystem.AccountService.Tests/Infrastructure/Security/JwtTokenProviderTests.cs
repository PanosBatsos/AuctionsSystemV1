using AuctionsSystem.AccountService.Domain.Enums;
using AuctionsSystem.AccountService.Infrastructure.Configuration;
using AuctionsSystem.AccountService.Infrastructure.Security;
using FluentAssertions;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;


namespace AuctionsSystem.AccountService.Tests.Infrastructure.Security
{
    public class JwtTokenProviderTests
    {
        [Fact]
        public void GenerateToken_ShouldReturnValidJwt_WithCorrectClaims()
        {
            // Arrange
            using var rsa = RSA.Create();
            var privateKey = rsa.ExportPkcs8PrivateKeyPem(); 

            var jwtSettings = new JwtSettings
            {
                PrivateKey = privateKey,
                Issuer = "TestAccountService",
                Audience = "TestAuctionApp"
            };

            var optionsMock = Options.Create(jwtSettings);
            var sut = new TokenProvider(optionsMock);

            var accountId = Guid.NewGuid();
            var username = "npappas";
            var role = UserRole.USER;

            // Act
            var tokenString = sut.GenerateToken(accountId, username, role);

            // Assert
            tokenString.Should().NotBeNullOrWhiteSpace();

      
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(tokenString);

            
            jwtToken.Issuer.Should().Be(jwtSettings.Issuer);
            jwtToken.Audiences.Should().Contain(jwtSettings.Audience);

            
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == accountId.ToString());
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Name && c.Value == username);
            jwtToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == role.ToString());
            jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti); 
        }
    }
}
