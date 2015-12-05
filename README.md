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

### Usage Example

```csharp
using (var client = new MondoApiClient(url, clientId, clientSecret))
{
    await client.RequestAccessTokenAsync(username, password);

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



### Contributions
Contributions and pull requests are more than welcome!

### API Documentation

Check out the full [Mondo API Documentation here](https://getmondo.co.uk/docs/).
