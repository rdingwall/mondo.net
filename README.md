# Mondo.NET

![Mondo](https://twitter.com/getmondo/profile_image?size=original)

[![NuGet version](https://img.shields.io/nuget/v/Mondo.svg)](http://nuget.org/List/Packages/Mondo)  [![NuGet downloads](https://img.shields.io/nuget/dt/Mondo.svg)](http://nuget.org/List/Packages/Mondo)  [![Build status](https://ci.appveyor.com/api/projects/status/p26nu5fypp5c4qon?svg=true)](https://ci.appveyor.com/project/rdingwall/mondotnet) [![License](http://img.shields.io/:license-MIT-blue.svg)](https://raw.githubusercontent.com/rdingwall/mondo.net/master/LICENSE)

Mondo.NET is a .NET client library for the [Mondo bank API](https://getmondo.co.uk/docs/). Use it to build apps and view your accounts, balances and transactions, create feed items, manage webhooks and attachments, and more!

### [>>> Get Mondo.NET via NuGet](http://nuget.org/List/Packages/Mondo)

```
Install-Package Mondo
```

Supported target frameworks: .NET 4.5, ASP.NET Core 5.0, Windows 8, Windows Phone 8.1

### Supported Features

- 100% async task-based API
- OAuth 2.0 authentication
- Web application flow (Authorization Code Grant)
- Native CLR types
- Access token refreshing
- Built for unit testing
- List accounts, transactions, and balances
- Create feed items
- Manage webhooks and attachments
- Upload attachments

## Usage Examples

##### Authentication, Accounts, Balances, and Transactions
To authenticate using OAuth 2.0 **Web application flow (Authorization Code Grant)** and retrieve a list of accounts:

```csharp
public class HomeController : Controller
{
    IMondoAuthorizationClient _authClient = new MondoAuthorizationClient(YOUR_CLIENT_ID, YOUR_CLIENT_SECRET);

    [HttpGet]
    public ActionResult Login()
    {
        // an unguessable random string which is used to protect against cross-site request forgery attacks
        string state = ...; 

        // the URL the user should be redirected back to following a successful Mondo login
        string redirectUrl = Url.Action("OAuthCallback", "Home", null, Request.Url.Scheme);

        string mondoLoginPageUrl = _authClient.GetAuthorizeUrl(state, redirectUrl);

        // 1. Send user to Mondo's login page
        return Redirect(mondoLoginPageUrl);
    }

    [HttpGet]
    public async Task<ActionResult> OAuthCallback(string code, string state)
    {
        // confirm the redirect url was valid
        string redirectUrl = Url.Action("OAuthCallback", "Home", null, Request.Url.Scheme);
    
        // 2. Exchange authorization code for access token
        AccessToken accessToken = await _authClient.GetAccessTokenAsync(code, redirectUrl);
            
        // 3. Begin fetching accounts, transactions etc
        using (var client = new MondoClient(accessToken.Value))
        {
            IList<Account> accounts = await client.GetAccountsAsync();

            // ... etc
        }
    }
} 
```

##### Feed Items
To create a feed item:
```csharp
// create feed item
var parameters = new Dictionary<string, string>
{
    {"title", "My custom item"},
    {"image_url", "www.example.com/image.png"},
    {"background_color", "#FCF1EE"},
    {"body_color", "#FCF1EE"},
    {"title_color", "#333"},
    {"body", "Some body text to display"},
};

await client.CreateFeedItemAsync("myaccountid", "basic", parameters, "https://www.example.com/a_page_to_open_on_tap.html");
```

##### Webhooks
To register, delete and list webhooks:
```csharp
// list webhooks
IList<Transaction> webhooks = await client.ListTransactionsAsync("myaccountid");

// register new webhook
Webhook webhook = await client.RegisterWebhookAsync("myaccountid", "http://example.com/webhook");

// delete webhook
await client.DeleteWebhookAsync(webhook.Id);
```

##### Attachments
To upload, register and remove transaction attachments:
```csharp
// upload and register an attachment
using (var stream = File.OpenRead(@"C:\example.png"))
{
    Attachment attachment = await client.UploadAttachmentAsync("example.png", "image/png", transaction.Id, stream);
}

// register an attachment that is already hosted somewhere
Attachment attachment = await client.RegisterAttachmentAsync(transaction.Id, "http://example.com/pic.png", "image/png");

// remove attachment
await client.DeregisterAttachmentAsync(attachment.Id);
```

##### Refreshing your Access Token
OAuth 2.0 access tokens expire and must be periodically refreshed to maintain API access. Here is an example using an Rx [IScheduler](https://msdn.microsoft.com/en-us/library/hh242963(v=vs.103).aspx):
```csharp
private _refreshDisposable = new SerialDisposable();

// schedule automatic token refresh
private void EnqueueRefresh()
{
     DateTimeOffset refreshTime = DateTimeOffset.UtcNow.AddSeconds(_accessToken.ExpiresIn);

    _refreshDisposable.Disposable = Scheduler.Default.Schedule(refreshTime, async () =>
    {
        await _authClient.RefreshAccessTokenAsync(_accessToken.RefreshToken);
        EnqueueRefresh();
    });
}
```

## Samples

#### ASP.NET MVC

Check out the ASP.NET MVC Web Application Sample demonstrating OAuth 2.0 Web application flow (Authorization Code Grant):

https://github.com/rdingwall/MondoAspNetMvcSample

![screenshot](http://i.imgur.com/jNL2lUL.png)

#### Universal Windows
Also the Universal Windows Sample application using Mondo.NET, Rx and MVVM:

https://github.com/rdingwall/MondoUniversalWindowsSample

![screenshot](http://i.imgur.com/xYkRAzh.png)

## Contributions
Contributions and pull requests are more than welcome! :gift:
