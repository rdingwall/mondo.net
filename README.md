# Unofficial C# Mondo API Client

[![NuGet version](https://img.shields.io/nuget/v/Mondo.Client.svg)](http://nuget.org/List/Packages/Mondo.Client)  [![NuGet downloads](https://img.shields.io/nuget/dt/Mondo.Client.svg)](http://nuget.org/List/Packages/Mondo.Client)  [![Build status](https://ci.appveyor.com/api/projects/status/p26nu5fypp5c4qon?svg=true)](https://ci.appveyor.com/project/rdingwall/mondotnet)

### [>>> Get Mondo.Client via NuGet](http://nuget.org/List/Packages/Mondo.Client)

```
Install-Package Mondo.Client
```

### Usage Example

```csharp
var mondoApiClientFactory = new MondoApiClientFactory(url, clientId, clientSecret);

using (IMondoApiClient client = await mondoApiClientFactory.Authenticate(username, password))
{
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
