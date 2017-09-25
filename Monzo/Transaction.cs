using System;
using System.Collections.Generic;
using System.Diagnostics;
using Monzo.Messages;
using Newtonsoft.Json;

namespace Monzo
{
    /// <summary>
    /// Transactions are movements of funds into or out of an account. Negative transactions represent debits (ie. spending money) and positive transactions represent credits (ie. receiving money).
    /// </summary>
    [DebuggerDisplay("[{Id,nq} {Amount} {Currency,nq} {Description}]")]
    public sealed class Transaction
    {
        /// <summary>
        /// The currently available balance of the account, as a 64bit integer in minor units of the currency, eg. pennies for GBP, or cents for EUR and USD.
        /// </summary>
        [JsonProperty("account_balance")]
        public long AccountBalance { get; set; }

        /// <summary>
        /// The amount of the transaction in minor units of currency. For example pennies in the case of GBP. A negative amount indicates a debit (most card transactions will have a negative amount)
        /// </summary>
        [JsonProperty("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// This is only present on declined transactions! Valid values are INSUFFICIENT_FUNDS, CARD_INACTIVE, CARD_BLOCKED or OTHER.
        /// </summary>
        [JsonProperty("decline_reason")]
        public string DeclineReason { get; set; }

        /// <summary>
        /// Time the transaction was created.
        /// </summary>
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// The ISO 4217 currency code.
        /// </summary>
        [JsonProperty("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// Transaction description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; }

        /// <summary>
        /// The transaction's Id.
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// This contains the merchant_id of the merchant that this transaction was made at. If you pass ?expand[]=merchant in your request URL, it will contain lots of information about the merchant.
        /// </summary>
        [JsonProperty("merchant")]
        [JsonConverter(typeof(MerchantJsonConverter))]
        public Merchant Merchant { get; set; }

        /// <summary>
        /// You may store your own key-value annotations against a transaction in its metadata. Metadata is private to your application.
        /// </summary>
        [JsonProperty("metadata")]
        public IDictionary<string, string> Metadata { get; set; }

        /// <summary>
        /// Notes entered by the user against the transaction.
        /// </summary>
        [JsonProperty("notes")]
        public string Notes { get; set; }

        /// <summary>
        /// Top-ups to an account are represented as transactions with a positive amount and is_load = true. Other transactions such as refunds, reversals or chargebacks may have a positive amount but is_load = false
        /// </summary>
        [JsonProperty("is_load")]
        public bool IsLoad { get; set; }

        /// <summary>
        /// You probably don’t need to worry about this. Card transactions only settle 24-48 hours (sometimes even more!) after the purchase; until then they are just “authorised” and settled = false on them.
        /// </summary>
        [JsonProperty("settled")]
        public string Settled { get; set; }

        /// <summary>
        /// The category can be set for each transaction by the user. Over time we learn which merchant goes in which category and auto-assign the category of a transaction. If the user hasn’t set a category, we’ll return the default category of the merchant on this transactions. Top-ups have category “monzo”. Valid values are general, eating_out, expenses, transport, cash, bills, entertainment, shopping, holidays, groceries
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }
    }
}