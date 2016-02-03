using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Mondo
{
    /// <summary>
    /// An authenticated Mondo API client.
    /// </summary>
    public interface IMondoClient : IDisposable
    {
        /// <summary>
        /// Your OAuth 2.0 access token.
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// Returns a list of accounts owned by the currently authorised user.
        /// </summary>
        Task<IList<Account>> GetAccountsAsync();

        /// <summary>
        /// Returns balance information for a specific account.
        /// </summary>
        /// <param name="accountId">The id of the account.</param>
        Task<Balance> GetBalanceAsync(string accountId);

        /// <summary>
        /// Returns an individual transaction, fetched by its id.
        /// </summary>
        /// <param name="transactionId">The transaction ID.</param>
        /// <param name="expand">Can be merchant.</param>
        Task<Transaction> GetTransactionAsync(string transactionId, string expand = null);

        /// <summary>
        /// Returns a list of transactions on the user’s account.
        /// </summary>
        /// <param name="accountId">The account to retrieve transactions from.</param>
        /// <param name="expand">Can be merchant.</param>
        /// <param name="paginationOptions">This endpoint can be paginated.</param>
        Task<IList<Transaction>> GetTransactionsAsync(string accountId, string expand = null, PaginationOptions paginationOptions = null);

        /// <summary>
        /// You may store your own key-value annotations against a transaction in its metadata.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="metadata">Include each key you would like to modify. To delete a key, set its value to an empty string.</param>
        /// <remarks>Metadata is private to your application.</remarks>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task<Transaction> AnnotateTransactionAsync(string transactionId, IDictionary<string, string> metadata, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Creates a new feed item on the user’s feed.
        /// </summary>
        /// <param name="accountId">The account to create feed item for.</param>
        /// <param name="type">Type of feed item. Currently only basic is supported.</param>
        /// <param name="params">A map of parameters which vary based on type</param>
        /// <param name="url">A URL to open when the feed item is tapped. If no URL is provided, the app will display a fallback view based on the title &amp; body.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task CreateFeedItemAsync(string accountId, string type, IDictionary<string, string> @params, string url = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Each time a matching event occurs, we will make a POST call to the URL you provide. If the call fails, we will retry up to a maximum of 5 attempts, with exponential backoff.
        /// </summary>
        /// <param name="accountId">The account to receive notifications for.</param>
        /// <param name="url">The URL we will send notifications to.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task<Webhook> CreateWebhookAsync(string accountId, string url, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// List the web hooks registered on an account.
        /// </summary>
        /// <param name="accountId">The account to list registered web hooks for.</param>
        Task<IList<Webhook>> GetWebhooksAsync(string accountId);

        /// <summary>
        /// When you delete a web hook, we will no longer send notifications to it.
        /// </summary>
        /// <param name="webhookId">The id of the webhook to delete.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task DeleteWebhookAsync(string webhookId, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Uploads an attachment from a file stream.
        /// </summary>
        /// <param name="filename">The name of the file to be uploaded</param>
        /// <param name="fileType">The content type of the file</param>
        /// <param name="externalId">The id of the transaction to associate the attachment with.</param>
        /// <param name="stream">The file stream.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task<Attachment> UploadAttachmentAsync(string filename, string fileType, string externalId, Stream stream, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Once you have obtained a URL for an attachment, either by uploading to the upload_url obtained from the upload endpoint above or by hosting a remote image, this URL can then be registered against a transaction. Once an attachment is registered against a transaction this will be displayed on the detail page of a transaction within the Mondo app.
        /// </summary>
        /// <param name="externalId">The id of the transaction to associate the attachment with.</param>
        /// <param name="fileUrl">The URL of the uploaded attachment.</param>
        /// <param name="fileType">The content type of the attachment.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task<Attachment> CreateAttachmentAsync(string externalId, string fileUrl, string fileType, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// To remove an attachment, simply deregister this using its id
        /// </summary>
        /// <param name="id">The id of the attachment to deregister.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        Task DeleteAttachmentAsync(string id, CancellationToken cancellationToken = default(CancellationToken));
    }
}