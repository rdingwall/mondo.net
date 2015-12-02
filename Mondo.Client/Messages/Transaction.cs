using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mondo.Client.Messages
{
    public sealed class Transaction
    {
        [JsonProperty("account_balance")]
        public int AccountBalance { get; set; }

        /// <summary>
        /// The amount of the transaction in minor units of currency. For example pennies in the case of GBP. A negative amount indicates a debit (most card transactions will have a negative amount)
        /// </summary>
        [JsonProperty("amount")]
        public int Amount { get; set; }

        /// <summary>
        /// This is only present on declined transactions! Valid values are INSUFFICIENT_FUNDS, CARD_INACTIVE, CARD_BLOCKED or OTHER.
        /// </summary>
        [JsonProperty("decline_reason")]
        public string DeclineReason { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// This contains the merchant_id of the merchant that this transaction was made at. If you pass ?expand[]=merchant in your request URL, it will contain lots of information about the merchant.
        /// </summary>
        [JsonProperty("merchant")]
        [JsonConverter(typeof(MerchantJsonConverter))]
        public Merchant Merchant { get; set; }

        [JsonProperty("metadata")]
        public IDictionary<string, string> Metadata { get; set; }

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
        /// The category can be set for each transaction by the user. Over time we learn which merchant goes in which category and auto-assign the category of a transaction. If the user hasn’t set a category, we’ll return the default category of the merchant on this transactions. Top-ups have category “mondo”. Valid values are general, eating_out, expenses, transport, cash, bills, entertainment, shopping, holidays, groceries
        /// </summary>
        [JsonProperty("category")]
        public string Category { get; set; }
    }
}