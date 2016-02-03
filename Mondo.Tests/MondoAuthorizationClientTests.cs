using Microsoft.Owin.Testing;
using NUnit.Framework;
using Owin;

namespace Mondo.Tests
{
    [TestFixture]
    public sealed class MondoAuthorizationClientTests
    {
        [Test]
        public void GetLoginPageUrl()
        {
            using (var client = new MondoAuthorizationClient("testClientId", "testClientSecret", "http://foo"))
            {
                var loginPageUrl = client.GetAuthorizeUrl("testState", "testRedirectUri");

                Assert.AreEqual("https://auth.getmondo.co.uk/?response_type=code&client_id=testClientId&state=testState&redirect_uri=testRedirectUri", loginPageUrl);
            }
        }

        [Test]
        public async void ExchangeCodeForAccessTokenAsync()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("/oauth2/token", context.Request.Uri.PathAndQuery);

                    var formCollection = await context.Request.ReadFormAsync();

                    Assert.AreEqual("authorization_code", formCollection["grant_type"]);
                    Assert.AreEqual("testClientId", formCollection["client_id"]);
                    Assert.AreEqual("testClientSecret", formCollection["client_secret"]);
                    Assert.AreEqual("testRedirectUri", formCollection["redirect_uri"]);
                    Assert.AreEqual("testCode", formCollection["code"]);

                    await context.Response.WriteAsync(
                        @"{
                            'access_token': 'testAccessToken',
                            'client_id': 'client_id',
                            'expires_in': 21600,
                            'refresh_token': 'testRefreshToken',
                            'token_type': 'Bearer',
                            'user_id': 'testUserId'
                        }"
                    );
                });
            }))
            {
                using (var client = new MondoAuthorizationClient(server.HttpClient, "testClientId", "testClientSecret"))
                {
                    var accessToken = await client.GetAccessTokenAsync("testCode", "testRedirectUri");

                    Assert.AreEqual("testAccessToken", accessToken.Value);
                    Assert.AreEqual("testRefreshToken", accessToken.RefreshToken);
                    Assert.AreEqual("testUserId", accessToken.UserId);
                    Assert.AreEqual(21600, accessToken.ExpiresIn);
                }
            }
        }

        [Test]
        public async void Authenticate()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("/oauth2/token", context.Request.Uri.PathAndQuery);

                    var formCollection = await context.Request.ReadFormAsync();

                    Assert.AreEqual("password", formCollection["grant_type"]);
                    Assert.AreEqual("testClientId", formCollection["client_id"]);
                    Assert.AreEqual("testClientSecret", formCollection["client_secret"]);
                    Assert.AreEqual("testUsername", formCollection["username"]);
                    Assert.AreEqual("testPassword", formCollection["password"]);

                    await context.Response.WriteAsync(
                        @"{
                            'access_token': 'testAccessToken',
                            'client_id': 'client_id',
                            'expires_in': 21600,
                            'refresh_token': 'testRefreshToken',
                            'token_type': 'Bearer',
                            'user_id': 'testUserId'
                        }"
                    );
                });
            }))
            {
                using (var client = new MondoAuthorizationClient(server.HttpClient, "testClientId", "testClientSecret"))
                {
                    var accessToken = await client.AuthenticateAsync("testUsername", "testPassword");

                    Assert.AreEqual("testAccessToken", accessToken.Value);
                    Assert.AreEqual("testRefreshToken", accessToken.RefreshToken);
                    Assert.AreEqual("testUserId", accessToken.UserId);
                    Assert.AreEqual(21600, accessToken.ExpiresIn);
                }
            }
        }

        [Test]
        public async void RefreshAccessToken()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("/oauth2/token", context.Request.Uri.PathAndQuery);

                    var formCollection = await context.Request.ReadFormAsync();

                    Assert.AreEqual("refresh_token", formCollection["grant_type"]);
                    Assert.AreEqual("testClientId", formCollection["client_id"]);
                    Assert.AreEqual("testClientSecret", formCollection["client_secret"]);
                    Assert.AreEqual("testAccessToken1", formCollection["refresh_token"]);

                    await context.Response.WriteAsync(
                        @"{
                            'access_token': 'testAccessToken2',
                            'client_id': 'client_id',
                            'expires_in': 21600,
                            'refresh_token': 'testRefreshToken2',
                            'token_type': 'Bearer',
                            'user_id': 'testUserId'
                        }"
                    );
                });
            }))
            {
                using (var client = new MondoAuthorizationClient(server.HttpClient, "testClientId", "testClientSecret"))
                {
                    var accessToken = await client.RefreshAccessTokenAsync("testAccessToken1");

                    Assert.AreEqual("testAccessToken2", accessToken.Value);
                    Assert.AreEqual("testRefreshToken2", accessToken.RefreshToken);
                    Assert.AreEqual(21600, accessToken.ExpiresIn);
                }
            }
        }
    }
}
