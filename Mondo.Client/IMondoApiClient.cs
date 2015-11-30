using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mondo.Client.Messages;

namespace Mondo.Client
{
    /// <summary>
    /// An authenticated Mondo API client.
    /// </summary>
    public interface IMondoApiClient : IDisposable
    {
        /// <summary>
        /// Returns a list of accounts owned by the currently authorised user.
        /// </summary>
        /// <returns></returns>
        Task<IList<Account>> ListAccountsAsync();

        /// <summary>
        /// Returns an individual transaction, fetched by its id.
        /// </summary>
        Task<Transaction> RetrieveTransactionAsync(string transactionId);

        /// <summary>
        /// Returns a list of transactions on the user’s account.
        /// </summary>
        /// <param name="accountId">The account to retrieve transactions from.</param>
        Task<IList<Transaction>> ListTransactionsAsync(string accountId);

        /// <summary>
        /// You may store your own key-value annotations against a transaction in its metadata.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <param name="metadata">Include each key you would like to modify. To delete a key, set its value to an empty string.</param>
        /// <remarks>Metadata is private to your application.</remarks>
        Task<Transaction> AnnotateTransactionAsync(string transactionId, IDictionary<string, string> metadata);

        /// <summary>
        /// Creates a new feed item on the user’s feed.
        /// </summary>
        /// <param name="accountId">The account to create feed item for.</param>
        /// <param name="type">Type of feed item. Currently only basic is supported.</param>
        /// <param name="params">A map of parameters which vary based on type</param>
        /// <param name="url">A URL to open when the feed item is tapped. If no URL is provided, the app will display a fallback view based on the title & body.</param>
        Task CreateFeedItemAsync(string accountId, string type, string url, IDictionary<string, string> @params);

        /// <summary>
        /// Each time a matching event occurs, we will make a POST call to the URL you provide. If the call fails, we will retry up to a maximum of 5 attempts, with exponential backoff.
        /// </summary>
        /// <param name="accountId">The account to receive notifications for.</param>
        /// <param name="url">The URL we will send notifications to.</param>
        Task<Webhook> RegisterWebhookAsync(string accountId, string url);

        /// <summary>
        /// List the web hooks registered on an account.
        /// </summary>
        /// <param name="accountId">The account to list registered web hooks for.</param>
        Task<IList<Webhook>> ListWebhooksAsync(string accountId);

        /// <summary>
        /// When you delete a web hook, we will no longer send notifications to it.
        /// </summary>
        Task DeleteWebhookAsync(string webhookId);

        /// <summary>
        /// The first step when uploading an attachment is to obtain a temporary URL to which the file can be uploaded. The response will include a file_url which will be the URL of the resulting file, and an upload_url to which the file should be uploaded to.
        /// </summary>
        /// <param name="filename">The name of the file to be uploaded</param>
        /// <param name="fileType">The content type of the file</param>
        Task<UploadAttachmentResponse> UploadAttachmentAsync(string filename, string fileType);

        /// <summary>
        /// Once you have obtained a URL for an attachment, either by uploading to the upload_url obtained from the upload endpoint above or by hosting a remote image, this URL can then be registered against a transaction. Once an attachment is registered against a transaction this will be displayed on the detail page of a transaction within the Mondo app.
        /// </summary>
        /// <param name="externalId">The id of the transaction to associate the attachment with.</param>
        /// <param name="fileUrl">The URL of the uploaded attachment.</param>
        /// <param name="fileType">The content type of the attachment.</param>
        Task<Attachment> RegisterAttachmentAsync(string externalId, string fileUrl, string fileType);

        /// <summary>
        /// To remove an attachment, simply deregister this using its id
        /// </summary>
        /// <param name="id">The id of the attachment to deregister.</param>
        Task DeregisterAttachmentAsync(string id);
    }
}