﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin.Testing;
using NUnit.Framework;
using Owin;

namespace Monzo.Tests
{
    [TestFixture]
    public sealed class MonzoApiClientTests
    {
        [Test]
        public async void GetAccounts()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("/accounts", context.Request.Uri.PathAndQuery);

                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);

                    await context.Response.WriteAsync(
                        @"{
                            'accounts': [
                                {
                                    'id': 'acc_00009237aqC8c5umZmrRdh',
                                    'description': 'Peter Pan\'s Account',
                                    'created': '2015-11-13T12:17:42Z'
                                }
                            ]
                        }"
                    );
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    var accounts = await client.GetAccountsAsync();

                    Assert.AreEqual(1, accounts.Count);
                    Assert.AreEqual("acc_00009237aqC8c5umZmrRdh", accounts[0].Id);
                    Assert.AreEqual("Peter Pan's Account", accounts[0].Description);
                    Assert.AreEqual(new DateTime(2015, 11, 13, 12, 17, 42, DateTimeKind.Utc), accounts[0].Created);
                }
            }
        }

        [Test]
        public async void GetBalance()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("/balance?account_id=1", context.Request.Uri.PathAndQuery);

                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);

                    await context.Response.WriteAsync(
                        @"{
                            'balance': 5000,
                            'currency': 'GBP',
                            'spend_today': -100
                        }"
                    );
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    var balance = await client.GetBalanceAsync("1");

                    Assert.AreEqual(5000, balance.Value);
                    Assert.AreEqual("GBP", balance.Currency);
                    Assert.AreEqual(-100, balance.SpendToday);
                }
            }
        }

        [Test]
        public async void GetTransactions()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    // workaround for mono bug
                    Assert.That(context.Request.Uri.PathAndQuery, Is.EqualTo("/transactions?account_id=1"));

                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);

                    await context.Response.WriteAsync(
                        @"{
                            'transactions': [
                                {
                                    'account_balance': 13013,
                                    'amount': -510,
                                    'created': '2015-08-22T12:20:18Z',
                                    'currency': 'GBP',
                                    'description': 'THE DE BEAUVOIR DELI C LONDON        GBR',
                                    'id': 'tx_00008zIcpb1TB4yeIFXMzx',
                                    'merchant': 'merch_00008zIcpbAKe8shBxXUtl',
                                    'metadata': {},
                                    'notes': 'Salmon sandwich 🍞',
                                    'is_load': false,
                                    'settled': true,
                                    'category': 'eating_out'
                                },
                                {
                                    'account_balance': 12334,
                                    'amount': -679,
                                    'created': '2015-08-23T16:15:03Z',
                                    'currency': 'GBP',
                                    'description': 'VUE BSL LTD            ISLINGTON     GBR',
                                    'id': 'tx_00008zL2INM3xZ41THuRF3',
                                    'merchant': 'merch_00008z6uFVhVBcaZzSQwCX',
                                    'metadata': {},
                                    'notes': '',
                                    'is_load': false,
                                    'settled': true,
                                    'category': 'eating_out'
                                },
                            ]
                        }"
                    );
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    var transactions = await client.GetTransactionsAsync("1");

                    Assert.AreEqual(2, transactions.Count);
                    
                    Assert.AreEqual(13013, transactions[0].AccountBalance);
                    Assert.AreEqual(-510, transactions[0].Amount);
                    Assert.AreEqual(new DateTime(2015, 08, 22, 12, 20, 18, DateTimeKind.Utc), transactions[0].Created);
                    Assert.AreEqual("GBP", transactions[0].Currency);
                    Assert.AreEqual("THE DE BEAUVOIR DELI C LONDON        GBR", transactions[0].Description);
                    Assert.AreEqual("tx_00008zIcpb1TB4yeIFXMzx", transactions[0].Id);
                    Assert.AreEqual("merch_00008zIcpbAKe8shBxXUtl", transactions[0].Merchant.Id);
                    Assert.AreEqual(new Dictionary<string, string>(), transactions[0].Metadata);
                    Assert.AreEqual("Salmon sandwich 🍞", transactions[0].Notes);
                    Assert.IsFalse(transactions[0].IsLoad);
                    Assert.AreEqual("eating_out", transactions[0].Category);
                }
            }
        }

        [Test]
        public async void GetTransactionsPaginated()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    // workaround for mono bug
                    Assert.That(context.Request.Uri.PathAndQuery, Is.EqualTo("/transactions?account_id=1&limit=40&since=2015-04-05T18:01:32Z&before=2015-12-25T18:01:32Z"));

                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);

                    await context.Response.WriteAsync(
                        @"{
                            'transactions': [
                                {
                                    'account_balance': 13013,
                                    'amount': -510,
                                    'created': '2015-08-22T12:20:18Z',
                                    'currency': 'GBP',
                                    'description': 'THE DE BEAUVOIR DELI C LONDON        GBR',
                                    'id': 'tx_00008zIcpb1TB4yeIFXMzx',
                                    'merchant': 'merch_00008zIcpbAKe8shBxXUtl',
                                    'metadata': {},
                                    'notes': 'Salmon sandwich 🍞',
                                    'is_load': false,
                                    'settled': true,
                                    'category': 'eating_out'
                                },
                                {
                                    'account_balance': 12334,
                                    'amount': -679,
                                    'created': '2015-08-23T16:15:03Z',
                                    'currency': 'GBP',
                                    'description': 'VUE BSL LTD            ISLINGTON     GBR',
                                    'id': 'tx_00008zL2INM3xZ41THuRF3',
                                    'merchant': 'merch_00008z6uFVhVBcaZzSQwCX',
                                    'metadata': {},
                                    'notes': '',
                                    'is_load': false,
                                    'settled': true,
                                    'category': 'eating_out'
                                },
                            ]
                        }"
                    );
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    var transactions = await client.GetTransactionsAsync("1", null, new PaginationOptions { SinceTime = new DateTime(2015, 4, 5, 18, 1, 32, DateTimeKind.Utc), Limit = 40, BeforeTime = new DateTime(2015, 12, 25, 18, 1, 32, DateTimeKind.Utc) });

                    Assert.AreEqual(2, transactions.Count);

                    Assert.AreEqual(13013, transactions[0].AccountBalance);
                    Assert.AreEqual(-510, transactions[0].Amount);
                    Assert.AreEqual(new DateTime(2015, 08, 22, 12, 20, 18, DateTimeKind.Utc), transactions[0].Created);
                    Assert.AreEqual("GBP", transactions[0].Currency);
                    Assert.AreEqual("THE DE BEAUVOIR DELI C LONDON        GBR", transactions[0].Description);
                    Assert.AreEqual("tx_00008zIcpb1TB4yeIFXMzx", transactions[0].Id);
                    Assert.AreEqual("merch_00008zIcpbAKe8shBxXUtl", transactions[0].Merchant.Id);
                    Assert.AreEqual(new Dictionary<string, string>(), transactions[0].Metadata);
                    Assert.AreEqual("Salmon sandwich 🍞", transactions[0].Notes);
                    Assert.IsFalse(transactions[0].IsLoad);
                    Assert.AreEqual("eating_out", transactions[0].Category);
                }
            }
        }

        [Test]
        public async void GetTransaction()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);

                    // workaround for mono bug
                    Assert.That(context.Request.Uri.PathAndQuery, Is.EqualTo("/transactions/1"));

                    await context.Response.WriteAsync(
                        @"{
                            'transaction': {
                                'account_balance': 13013,
                                'amount': -510,
                                'created': '2015-08-22T12:20:18Z',
                                'currency': 'GBP',
                                'description': 'THE DE BEAUVOIR DELI C LONDON        GBR',
                                'id': 'tx_00008zIcpb1TB4yeIFXMzx',
                                'merchant': 'merch_00008zIcpbAKe8shBxXUtl',
                                'metadata': {},
                                'notes': 'Salmon sandwich 🍞',
                                'is_load': false,
                                'settled': true
                            }
                        }"
                    );
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    var transaction = await client.GetTransactionAsync("1");

                    Assert.AreEqual(13013, transaction.AccountBalance);
                    Assert.AreEqual(-510, transaction.Amount);
                    Assert.AreEqual(new DateTime(2015, 8, 22, 12, 20, 18, DateTimeKind.Utc), transaction.Created);
                    Assert.AreEqual("GBP", transaction.Currency);
                    Assert.AreEqual("THE DE BEAUVOIR DELI C LONDON        GBR", transaction.Description);
                    Assert.AreEqual("tx_00008zIcpb1TB4yeIFXMzx", transaction.Id);
                    Assert.AreEqual(new Dictionary<string, string>(), transaction.Metadata);
                    Assert.AreEqual("Salmon sandwich 🍞", transaction.Notes);
                    Assert.IsFalse(transaction.IsLoad);

                    Assert.AreEqual("merch_00008zIcpbAKe8shBxXUtl", transaction.Merchant.Id);
                }
            }
        }

        [Test]
        public async void GetTransactionExpanded()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);
                    
                    // workaround for mono bug
                    Assert.That(context.Request.Uri.PathAndQuery, Is.EqualTo("/transactions/1?expand[]=merchant").Or.EqualTo("/transactions/1?expand%5B%5D=merchant"));

                    await context.Response.WriteAsync(
                        @"{
                            'transaction': {
                                'account_balance': 13013,
                                'amount': -510,
                                'created': '2015-08-22T12:20:18Z',
                                'currency': 'GBP',
                                'description': 'THE DE BEAUVOIR DELI C LONDON        GBR',
                                'id': 'tx_00008zIcpb1TB4yeIFXMzx',
                                'merchant': {
                                    'address': {
                                        'address': '98 Southgate Road',
                                        'city': 'London',
                                        'country': 'GB',
                                        'latitude': 51.54151,
                                        'longitude': -0.08482400000002599,
                                        'postcode': 'N1 3JD',
                                        'region': 'Greater London'
                                    },
                                    'created': '2015-08-22T12:20:18Z',
                                    'group_id': 'grp_00008zIcpbBOaAr7TTP3sv',
                                    'id': 'merch_00008zIcpbAKe8shBxXUtl',
                                    'logo': 'https://pbs.twimg.com/profile_images/527043602623389696/68_SgUWJ.jpeg',
                                    'emoji': '🍞',
                                    'name': 'The De Beauvoir Deli Co.',
                                    'category': 'eating_out'
                                },
                                'metadata': {},
                                'notes': 'Salmon sandwich 🍞',
                                'is_load': false,
                                'settled': true
                            }
                        }"
                    );
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    var transaction = await client.GetTransactionAsync("1", "merchant");

                    Assert.AreEqual(13013, transaction.AccountBalance);
                    Assert.AreEqual(-510, transaction.Amount);
                    Assert.AreEqual(new DateTime(2015, 8, 22, 12, 20, 18, DateTimeKind.Utc), transaction.Created);
                    Assert.AreEqual("GBP", transaction.Currency);
                    Assert.AreEqual("THE DE BEAUVOIR DELI C LONDON        GBR", transaction.Description);
                    Assert.AreEqual("tx_00008zIcpb1TB4yeIFXMzx", transaction.Id);
                    Assert.AreEqual(new Dictionary<string, string>(), transaction.Metadata);
                    Assert.AreEqual("Salmon sandwich 🍞", transaction.Notes);
                    Assert.IsFalse(transaction.IsLoad);

                    Assert.AreEqual("98 Southgate Road", transaction.Merchant.Address.Address);
                    Assert.AreEqual("London", transaction.Merchant.Address.City);
                    Assert.AreEqual(51.54151, transaction.Merchant.Address.Latitude);
                    Assert.AreEqual(-0.08482400000002599, transaction.Merchant.Address.Longitude);
                    Assert.AreEqual("N1 3JD", transaction.Merchant.Address.Postcode);
                    Assert.AreEqual("Greater London", transaction.Merchant.Address.Region);

                    Assert.AreEqual(new DateTime(2015, 8, 22, 12, 20, 18, DateTimeKind.Utc), transaction.Merchant.Created);
                    Assert.AreEqual("grp_00008zIcpbBOaAr7TTP3sv", transaction.Merchant.GroupId);
                    Assert.AreEqual("merch_00008zIcpbAKe8shBxXUtl", transaction.Merchant.Id);
                    Assert.AreEqual("https://pbs.twimg.com/profile_images/527043602623389696/68_SgUWJ.jpeg", transaction.Merchant.Logo);
                    Assert.AreEqual("🍞", transaction.Merchant.Emoji);
                    Assert.AreEqual("The De Beauvoir Deli Co.", transaction.Merchant.Name);
                    Assert.AreEqual("eating_out", transaction.Merchant.Category);
                }
            }
        }

        [Test]
        public async void AnnotateTransaction()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);
                    Assert.AreEqual("PATCH", context.Request.Method);
                    Assert.AreEqual("/transactions/1", context.Request.Uri.PathAndQuery);

                    var formCollection = await context.Request.ReadFormAsync();

                    Assert.AreEqual("value1", formCollection["metadata[key1]"]);
                    Assert.AreEqual("", formCollection["metadata[key2]"]);

                    await context.Response.WriteAsync(
                        @"{
                            'transaction': {
                                'account_balance': 12334,
                                'amount': -679,
                                'created': '2015-08-23T16:15:03Z',
                                'currency': 'GBP',
                                'description': 'VUE BSL LTD            ISLINGTON     GBR',
                                'id': 'tx_00008zL2INM3xZ41THuRF3',
                                'merchant': 'merch_00008z6uFVhVBcaZzSQwCX',
                                'metadata': {
                                    'foo': 'bar'
                                },
                                'notes': '',
                                'is_load': false,
                                'settled': true,
                                'category': 'eating_out'
                            }
                        }"
                    );
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    var transaction = await client.AnnotateTransactionAsync("1", new Dictionary<string, string> { { "key1", "value1" }, {"key2", "" } });

                    Assert.AreEqual("foo", transaction.Metadata.First().Key);
                    Assert.AreEqual("bar", transaction.Metadata.First().Value);
                }
            }
        }

        [Test]
        public async void CreateFeedItem()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);
                    Assert.AreEqual("POST", context.Request.Method);
                    Assert.AreEqual("/feed", context.Request.Uri.PathAndQuery);

                    var formCollection = await context.Request.ReadFormAsync();
                    Assert.AreEqual("1", formCollection["account_id"]);
                    Assert.AreEqual("basic", formCollection["type"]);
                    Assert.AreEqual("https://www.example.com/a_page_to_open_on_tap.html", formCollection["url"]);
                    Assert.AreEqual("My custom item", formCollection["params[title]"]);

                    await context.Response.WriteAsync("{}");
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    await client.CreateFeedItemAsync("1", "basic", new Dictionary<string, string> { {"title", "My custom item" } }, "https://www.example.com/a_page_to_open_on_tap.html");
                }
            }
        }

        [Test]
        public async void CreateWebhook()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);
                    Assert.AreEqual("POST", context.Request.Method);
                    Assert.AreEqual("/webhooks", context.Request.Uri.PathAndQuery);

                    var formCollection = await context.Request.ReadFormAsync();
                    Assert.AreEqual("1", formCollection["account_id"]);
                    Assert.AreEqual("http://example.com", formCollection["url"]);

                    await context.Response.WriteAsync(
                        @"{
                            'webhook': {
                                'account_id': 'account_id',
                                'id': 'webhook_id',
                                'url': 'http://example.com'
                            }
                        }"
                    );
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    var webhook = await client.CreateWebhookAsync("1", "http://example.com");

                    Assert.AreEqual("account_id", webhook.AccountId);
                    Assert.AreEqual("webhook_id", webhook.Id);
                    Assert.AreEqual("http://example.com", webhook.Url);
                }
            }
        }

        [Test]
        public async void GetWebhooks()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("/webhooks?account_id=1", context.Request.Uri.PathAndQuery);

                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);

                    await context.Response.WriteAsync(
                        @"{
                            'webhooks': [
                                {
                                    'account_id': 'acc_000091yf79yMwNaZHhHGzp',
                                    'id': 'webhook_000091yhhOmrXQaVZ1Irsv',
                                    'url': 'http://example.com/callback'
                                }
                            ]
                        }"
                    );
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    var webhooks = await client.GetWebhooksAsync("1");

                    Assert.AreEqual(1, webhooks.Count);
                    Assert.AreEqual("webhook_000091yhhOmrXQaVZ1Irsv", webhooks[0].Id);
                    Assert.AreEqual("acc_000091yf79yMwNaZHhHGzp", webhooks[0].AccountId);
                    Assert.AreEqual("http://example.com/callback", webhooks[0].Url);
                }
            }
        }

        [Test]
        public async void DeleteWebhook()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("/webhooks/1", context.Request.Uri.PathAndQuery);
                    Assert.AreEqual("DELETE", context.Request.Method);

                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);

                    await context.Response.WriteAsync("{}");
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    await client.DeleteWebhookAsync("1");
                }
            }
        }

        [Test]
        public async void CreateAttachment()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("/attachment/register", context.Request.Uri.PathAndQuery);
                    Assert.AreEqual("POST", context.Request.Method);

                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);

                    var formCollection = await context.Request.ReadFormAsync();
                    Assert.AreEqual("tx_00008zIcpb1TB4yeIFXMzx", formCollection["external_id"]);
                    Assert.AreEqual("image/png", formCollection["file_type"]);
                    Assert.AreEqual("https://s3-eu-west-1.amazonaws.com/mondo-image-uploads/user_00009237hliZellUicKuG1/LcCu4ogv1xW28OCcvOTL-foo.png", formCollection["file_url"]);

                    await context.Response.WriteAsync(
                        @"{
                            'attachment': {
                                'id': 'attach_00009238aOAIvVqfb9LrZh',
                                'user_id': 'user_00009238aMBIIrS5Rdncq9',
                                'external_id': 'tx_00008zIcpb1TB4yeIFXMzx',
                                'file_url': 'https://s3-eu-west-1.amazonaws.com/mondo-image-uploads/user_00009237hliZellUicKuG1/LcCu4ogv1xW28OCcvOTL-foo.png',
                                'file_type': 'image/png',
                                'created': '2015-11-12T18:37:02Z'
                            }
                        }"
                    );
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    var attachment = await client.CreateAttachmentAsync("tx_00008zIcpb1TB4yeIFXMzx", "https://s3-eu-west-1.amazonaws.com/mondo-image-uploads/user_00009237hliZellUicKuG1/LcCu4ogv1xW28OCcvOTL-foo.png", "image/png");

                    Assert.AreEqual("attach_00009238aOAIvVqfb9LrZh", attachment.Id);
                    Assert.AreEqual("user_00009238aMBIIrS5Rdncq9", attachment.UserId);
                    Assert.AreEqual("tx_00008zIcpb1TB4yeIFXMzx", attachment.ExternalId);
                    Assert.AreEqual("https://s3-eu-west-1.amazonaws.com/mondo-image-uploads/user_00009237hliZellUicKuG1/LcCu4ogv1xW28OCcvOTL-foo.png", attachment.FileUrl);
                    Assert.AreEqual("image/png", attachment.FileType);
                    Assert.AreEqual(new DateTime(2015, 11, 12, 18, 37, 2, DateTimeKind.Utc), attachment.Created);
                }
            }
        }

        [Test]
        public async void DeleteAttachment()
        {
            using (var server = TestServer.Create(app =>
            {
                app.Run(async context =>
                {
                    Assert.AreEqual("/attachment/deregister", context.Request.Uri.PathAndQuery);
                    Assert.AreEqual("POST", context.Request.Method);

                    Assert.AreEqual("Bearer testAccessToken", context.Request.Headers["Authorization"]);

                    var formCollection = await context.Request.ReadFormAsync();
                    Assert.AreEqual("attach_00009238aOAIvVqfb9LrZh", formCollection["id"]);

                    await context.Response.WriteAsync("{}");
                });
            }))
            {
                using (var client = new MonzoClient(server.HttpClient, "testAccessToken"))
                {
                    await client.DeleteAttachmentAsync("attach_00009238aOAIvVqfb9LrZh");
                }
            }
        }
    }
}
