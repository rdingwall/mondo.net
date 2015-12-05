# Mondo.NET

![Mondo](https://twitter.com/getmondo/profile_image?size=original)

[![NuGet version](https://img.shields.io/nuget/v/Mondo.Client.svg)](http://nuget.org/List/Packages/Mondo.Client)  [![NuGet downloads](https://img.shields.io/nuget/dt/Mondo.Client.svg)](http://nuget.org/List/Packages/Mondo.Client)  [![Build status](https://ci.appveyor.com/api/projects/status/p26nu5fypp5c4qon?svg=true)](https://ci.appveyor.com/project/rdingwall/mondotnet)

Mondo.NET is a .NET client library for the [Mondo](http://getmondo.co.uk) bank HTTP API. Use it to build apps and view your accounts, balances and transactions, create feed items, manage webhooks and attachments, and more!

### [>>> Get Mondo.Client via NuGet](http://nuget.org/List/Packages/Mondo.Client)

```
Install-Package Mondo.Client
```

### Supported Features

- 100% async task-based API
- OAuth 2.0 authentication
- Native CLR types
- Access token refreshing
- Built for unit testing
- List accounts, transactions, and balances
- Create feed items
- Manage webhooks and attachments

### Not Finished Yet

- Pagination
- Attachment file IO
- Portable class libraries
- Three-Legged OAuth 2.0
- More samples

## Usage Examples

##### Authenticate and retrieve transactions
To authenticate using OAuth 2.0, retrieve a list of accounts and list of corresponding transactions:
```csharp
using (var client = new MondoClient(url, clientId, clientSecret))
{
    // authenticate
    await client.RequestAccessTokenAsync(username, password);

    // read balance
    BalanceResponse balance = await client.ReadBalanceAsync(accounts[0].Id);

    // list accounts
    IList<Account> accounts = await client.ListAccountsAsync();
    foreach (Account a in accounts)
    {
        Console.WriteLine($"{a.Id} {a.Created} {a.Description}");
    }
    
    // list transactions
    IList<Transaction> transactions = await client.ListTransactionsAsync(accounts[0].Id);
    foreach (Transaction t in transactions)
    {
        Console.WriteLine($"{t.Created} {t.Description} {t.Amount}, {t.Currency}, {t.AccountBalance}");
    }
}    
```

##### Managing webhooks
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
To upload, register and remove attachments:
```csharp
using (var stream = File.OpenRead(@"C:\example.png"))
{
    // upload and register attachment
    Attachment attachment = await client.UploadAttachmentAsync("example.png", "image/png", transactions[0].Id, stream);

    // remove attachment
    await client.DeregisterAttachmentAsync(attachment.Id);
}
```

##### Refresh Access Tokens
OAuth 2.0 access tokens expire and must be periodically refreshed to maintain API access. This example sets up an automatic refresh schedule using an Rx [IScheduler](https://msdn.microsoft.com/en-us/library/hh242963(v=vs.103).aspx):
```csharp
private _refreshDisposable = new SerialDisposable();

// schedule automatic token refresh
private void EnqueueRefresh()
{
    _refreshDisposable.Disposable = Scheduler.Default.Schedule(_client.AccessTokenExpiresAt, async () =>
    {
        await _client.RefreshAccessTokenAsync();
        EnqueueRefresh();
    });
}
```

## Contributions
Contributions and pull requests are more than welcome!

## API Documentation

Check out the full [Mondo API Documentation here](https://getmondo.co.uk/docs/).
