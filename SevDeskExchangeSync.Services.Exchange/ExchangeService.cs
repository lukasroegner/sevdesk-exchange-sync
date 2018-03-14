
#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SevDeskExchangeSync.Services.Exchange.Contacts;
using SevDeskExchangeSync.Services.Exchange.ExtendedProperties;
using SevDeskExchangeSync.Services.Exchange.Models;
using SevDeskExchangeSync.Services.Exchange.Users;

#endregion

namespace SevDeskExchangeSync.Services.Exchange
{
    /// <summary>
    /// Represents the service that communicates with the Microsoft Graph.
    /// </summary>
    public sealed class ExchangeService : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="ExchangeService" /> instance.
        /// </summary>
        /// <param name="tenant">The tenant of the Azure Active Directory.</param>
        /// <param name="clientId">The ID of the application.</param>
        /// <param name="clientSecret">The secret of the application.</param>
        public ExchangeService(string tenant, string clientId, string clientSecret)
        {
            // Stores the credentials
            this.Tenant = tenant;
            this.ClientId = clientId;
            this.ClientSecret = clientSecret;

            // Intializes the HTTP client
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Contains the URI of the token endpoint.
        /// </summary>
        private readonly string authorizationUri = "https://login.microsoftonline.com"; 

        /// <summary>
        /// Contains the URI of the Microsoft Graph endpoint.
        /// </summary>
        private readonly string graphUri = "https://graph.microsoft.com/v1.0";

        /// <summary>
        /// Contains the required scope for the Microsoft Graph.
        /// </summary>
        private readonly string graphScope = "https://graph.microsoft.com/.default";

        /// <summary>
        /// Contains the HTTP client that is used to communicate with the graph.
        /// </summary>
        private readonly HttpClient httpClient;

        /// <summary>
        /// Contains the access token for the Microsoft Graph.
        /// </summary>
        private string accessToken = null;

        /// <summary>
        /// Contains the expiration date time of the access token for the Microsoft Graph.
        /// </summary>
        private DateTime accessTokenExpirationDateTime = DateTime.MinValue;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the tenant of the Azure Active Directory.
        /// </summary>
        public string Tenant { get; private set; }

        /// <summary>
        /// Gets the ID of the application.
        /// </summary>
        public string ClientId { get; private set; }

        /// <summary>
        /// Gets the secret of the application.
        /// </summary>
        public string ClientSecret { get; private set; }

        /// <summary>
        /// Gets the URI at which the admin consent can be granted.
        /// </summary>
        public string AdminConsentUri => $"{this.authorizationUri}/{this.Tenant}/adminconsent?client_id={this.ClientId}";

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the access token from the token endpoint.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the access token could not be retrieved, a <see cref="InvalidOperationException"/> is thrown.</exception>
        /// <returns>The retrieved acess token.</returns>
        private async Task<string> GetAcessTokenAsync()
        {
            try
            {
                if (this.accessToken == null || this.accessTokenExpirationDateTime < DateTime.UtcNow)
                {
                    // Sends an HTTP request to the token endpoint
                    HttpResponseMessage response = await httpClient.PostAsync($"{this.authorizationUri}/{this.Tenant}/oauth2/v2.0/token", new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("client_id", this.ClientId),
                        new KeyValuePair<string, string>("client_secret", this.ClientSecret),
                        new KeyValuePair<string, string>("scope", this.graphScope),
                        new KeyValuePair<string, string>("grant_type", "client_credentials")
                    }));

                    // Parses the token
                    Token token = JsonConvert.DeserializeObject<Token>(await response.Content.ReadAsStringAsync());

                    // Sets the access token
                    this.accessToken = token.access_token;
                    this.accessTokenExpirationDateTime = DateTime.UtcNow.AddSeconds(token.expires_in);
                }

                // Returns the access token
                return this.accessToken;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a new <see cref="User" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created user instance.</returns>
        private User GetUser(Models.Users.User model)
        {
            return new User
            {
                Id = model.id,
                UserPrincipalName = model.userPrincipalName
            };
        }

        /// <summary>
        /// Creates a new <see cref="EmailAddress" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created email address instance.</returns>
        private EmailAddress GetEmailAddress(Models.Contacts.EmailAddress model) => new EmailAddress(model.name, model.address);

        /// <summary>
        /// Creates a new <see cref="Address" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created address instance.</returns>
        private Address GetAddress(Models.Contacts.Address model) => new Address(model.street, model.postalCode, model.city, model.state, model.countryOrRegion);

        /// <summary>
        /// Creates a new <see cref="Contact" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created contact instance.</returns>
        private Contact GetContact(Models.Contacts.Contact model)
        {
            return new Contact
            {
                ChangeKey = model.changeKey,
                Birthday = string.IsNullOrWhiteSpace(model.birthday) ? (Nullable<DateTime>)null : DateTime.Parse(model.birthday),
                BusinessAddress = model.businessAddress == null ? null : this.GetAddress(model.businessAddress),
                HomeAddress = model.homeAddress == null ? null : this.GetAddress(model.homeAddress),
                OtherAddress = model.otherAddress == null ? null : this.GetAddress(model.otherAddress),
                EmailAddresses = model.emailAddresses == null ? new List<EmailAddress>() : model.emailAddresses.Select(emailAddress => this.GetEmailAddress(emailAddress)).ToList(),
                BusinessPhoneNumbers = model.businessPhones == null ? new List<string>() : model.businessPhones.ToList(),
                BusinessWebsite = model.businessHomePage,
                CompanyName = model.companyName,
                CreationDateTime = string.IsNullOrWhiteSpace(model.createdDateTime) ? DateTime.UtcNow : DateTime.Parse(model.createdDateTime),
                FirstName = model.givenName,
                LastName = model.surname,
                HomePhoneNumbers = model.homePhones == null ? new List<string>() : model.homePhones.ToList(),
                Id = model.id,
                JobTitle = model.jobTitle,
                LastModifiedDateTime = string.IsNullOrWhiteSpace(model.lastModifiedDateTime) ? (string.IsNullOrWhiteSpace(model.createdDateTime) ? DateTime.UtcNow : DateTime.Parse(model.createdDateTime)) : DateTime.Parse(model.lastModifiedDateTime),
                MobilePhoneNumber = model.mobilePhone,
                PersonalNotes = model.personalNotes,
                Title = model.title,
                ExtendedProperties = model.singleValueExtendedProperties == null ? new List<ExtendedProperty>() : model.singleValueExtendedProperties.Select(e => new ExtendedProperty(new ExtendedPropertyId(e.id), e.value)).ToList()
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all users from the Azure Active Directory.
        /// </summary>
        /// <param name="skipToken">The token that can be used to get further results.</param>
        /// <param name="take">The number of users to include in the results.</param>
        /// <exception cref="InvalidOperationException">If the server could not be reached or the credentials are invalid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the list of users.</returns>
        public async Task<UserList> GetUsersAsync(SkipToken skipToken = null, int take = 10)
        {
            try
            {
                // Creates the request
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{this.graphUri}/users?$top={take}{(skipToken == null ? "" : (skipToken.Skip.HasValue ? $"&$skip={skipToken.Skip}" : $"&$skiptoken={skipToken.Token}"))}");
                string accessToken = await this.GetAcessTokenAsync();
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Downloads the users
                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                // Parses the response
                ResultList<Models.Users.User> result = JsonConvert.DeserializeObject<ResultList<Models.Users.User>>(await response.Content.ReadAsStringAsync());

                // Returns the result
                return new UserList(result.value.Select(user => this.GetUser(user)).ToList())
                {
                    SkipToken = string.IsNullOrWhiteSpace(result.odataNextLink) ? null : new SkipToken(result.odataNextLink),
                    Take = take
                };
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets all contacts of the specified user.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="extendedPropertyIds">If provided, the extended properties with the specified IDs are included in the results.</param>
        /// <param name="skipToken">The token that can be used to get further results.</param>
        /// <param name="take">The number of users to include in the results.</param>
        /// <exception cref="InvalidOperationException">If the server could not be reached or the credentials are not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the list of contacts.</returns>
        public async Task<ContactList> GetContactsAsync(string id, IEnumerable<ExtendedPropertyId> extendedPropertyIds = null, SkipToken skipToken = null, int take = 10)
        {
            try
            {
                // Creates the request
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, $"{this.graphUri}/users/{id}/contacts?$top={take}{(skipToken == null ? "" : (skipToken.Skip.HasValue ? $"&$skip={skipToken.Skip}" : $"&$skiptoken={skipToken.Token}"))}{(extendedPropertyIds == null ? "" : $"&$expand=singleValueExtendedProperties($filter={string.Join(" or ", extendedPropertyIds.Select(extendedProperty => $"id eq '{extendedProperty}'"))})")}");
                string accessToken = await this.GetAcessTokenAsync();
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Downloads the contacts
                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                // Parses the response
                ResultList<Models.Contacts.Contact> result = JsonConvert.DeserializeObject<ResultList<Models.Contacts.Contact>>(await response.Content.ReadAsStringAsync());

                // Returns the result
                return new ContactList(result.value.Select(contact => this.GetContact(contact)).ToList())
                {
                    SkipToken = string.IsNullOrWhiteSpace(result.odataNextLink) ? null : new SkipToken(result.odataNextLink),
                    Take = take
                };
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Creates a new contact on the Exchange account.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="options">The options that contain the information about the contact.</param>
        /// <exception cref="InvalidOperationException">If the server could not be reached or the credentials are not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the created contact.</returns>
        public async Task<Contact> CreateContactAsync(string id, ContactOptions options)
        {
            try
            {
                // Creates the model
                Models.Contacts.Contact contact = new Models.Contacts.Contact
                {
                    birthday = options.Birthday == null ? null : options.Birthday.ToString(),
                    businessAddress = options.BusinessAddress == null ? null : new Models.Contacts.Address
                    {
                        city = options.BusinessAddress.City,
                        street = options.BusinessAddress.Street,
                        postalCode = options.BusinessAddress.ZipCode,
                        state = options.BusinessAddress.State,
                        countryOrRegion = options.BusinessAddress.CountryOrRegion
                    },
                    businessHomePage = options.BusinessWebsite,
                    businessPhones = options.BusinessPhoneNumbers == null ? new List<string>() : options.BusinessPhoneNumbers.ToList(),
                    companyName = options.CompanyName,
                    givenName = options.FirstName,
                    surname = options.LastName,
                    emailAddresses = options.EmailAddresses == null ? new List<Models.Contacts.EmailAddress>() : options.EmailAddresses.Select(emailAddress => new Models.Contacts.EmailAddress
                    {
                        name = emailAddress.Name,
                        address = emailAddress.Address
                    }).ToList(),
                    homeAddress = options.HomeAddress == null ? null : new Models.Contacts.Address
                    {
                        city = options.HomeAddress.City,
                        street = options.HomeAddress.Street,
                        postalCode = options.HomeAddress.ZipCode,
                        state = options.HomeAddress.State,
                        countryOrRegion = options.HomeAddress.CountryOrRegion
                    },
                    homePhones = options.HomePhoneNumbers == null ? new List<string>() : options.HomePhoneNumbers.ToList(),
                    mobilePhone = options.MobilePhoneNumber,
                    otherAddress = options.OtherAddress == null ? null : new Models.Contacts.Address
                    {
                        city = options.OtherAddress.City,
                        street = options.OtherAddress.Street,
                        postalCode = options.OtherAddress.ZipCode,
                        state = options.OtherAddress.State,
                        countryOrRegion = options.OtherAddress.CountryOrRegion
                    },
                    personalNotes = options.PersonalNotes,
                    singleValueExtendedProperties = options.ExtendedProperties?.Select(extendedProperty => new Models.SingleValueExtendedProperty
                    {
                        id = extendedProperty.Id.ToString(),
                        value = extendedProperty.Value
                    }).ToList(),
                    title = options.Title,
                    jobTitle = options.JobTitle
                };

                // Creates the request
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, $"{this.graphUri}/users/{id}/contacts");
                string accessToken = await this.GetAcessTokenAsync();
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(contact, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), System.Text.Encoding.UTF8, "application/json");

                // Downloads the users
                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                // Parses the response
                return this.GetContact(JsonConvert.DeserializeObject<Models.Contacts.Contact>(await response.Content.ReadAsStringAsync()));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Updates an existing contact on the Exchange account.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="contactId">The ID of the contact.</param>
        /// <param name="options">The options that contain the information about the contact.</param>
        /// <exception cref="InvalidOperationException">If the server could not be reached or the credentials are not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the updated contact.</returns>
        public async Task<Contact> UpdateContactAsync(string id, string contactId, ContactOptions options)
        {
            try
            {
                // Creates the model
                Models.Contacts.Contact contact = new Models.Contacts.Contact
                {
                    birthday = options.Birthday == null ? null : options.Birthday.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    businessAddress = options.BusinessAddress == null ? null : new Models.Contacts.Address
                    {
                        city = options.BusinessAddress.City,
                        street = options.BusinessAddress.Street,
                        postalCode = options.BusinessAddress.ZipCode,
                        state = options.BusinessAddress.State,
                        countryOrRegion = options.BusinessAddress.CountryOrRegion
                    },
                    businessHomePage = options.BusinessWebsite,
                    businessPhones = options.BusinessPhoneNumbers == null ? new List<string>() : options.BusinessPhoneNumbers.Take(2).ToList(),
                    companyName = options.CompanyName,
                    givenName = options.FirstName,
                    surname = options.LastName,
                    emailAddresses = options.EmailAddresses == null ? new List<Models.Contacts.EmailAddress>() : options.EmailAddresses.Take(3).Select(emailAddress => new Models.Contacts.EmailAddress
                    {
                        name = emailAddress.Name,
                        address = emailAddress.Address
                    }).ToList(),
                    homeAddress = options.HomeAddress == null ? null : new Models.Contacts.Address
                    {
                        city = options.HomeAddress.City,
                        street = options.HomeAddress.Street,
                        postalCode = options.HomeAddress.ZipCode,
                        state = options.HomeAddress.State,
                        countryOrRegion = options.HomeAddress.CountryOrRegion
                    },
                    homePhones = options.HomePhoneNumbers == null ? new List<string>() : options.HomePhoneNumbers.Take(2).ToList(),
                    mobilePhone = options.MobilePhoneNumber,
                    otherAddress = options.OtherAddress == null ? null : new Models.Contacts.Address
                    {
                        city = options.OtherAddress.City,
                        street = options.OtherAddress.Street,
                        postalCode = options.OtherAddress.ZipCode,
                        state = options.OtherAddress.State,
                        countryOrRegion = options.OtherAddress.CountryOrRegion
                    },
                    personalNotes = options.PersonalNotes,
                    singleValueExtendedProperties = options.ExtendedProperties?.Select(extendedProperty => new Models.SingleValueExtendedProperty
                    {
                        id = extendedProperty.Id.ToString(),
                        value = extendedProperty.Value
                    }).ToList(),
                    title = options.Title,
                    jobTitle = options.JobTitle
                };

                // Creates the request
                HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), $"{this.graphUri}/users/{id}/contacts/{contactId}");
                string accessToken = await this.GetAcessTokenAsync();
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(contact, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }), System.Text.Encoding.UTF8, "application/json");

                // Downloads the users
                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                // Parses the response
                return this.GetContact(JsonConvert.DeserializeObject<Models.Contacts.Contact>(await response.Content.ReadAsStringAsync()));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Deletes a new contact on the Exchange account.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <param name="contactId">The ID of the contact.</param>
        /// <exception cref="InvalidOperationException">If the server could not be reached or the credentials are not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        public async Task RemoveContactAsync(string id, string contactId)
        {
            try
            {
                // Creates the request
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"{this.graphUri}/users/{id}/contacts/{contactId}");
                string accessToken = await this.GetAcessTokenAsync();
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Downloads the users
                HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Disposes of the service.
        /// </summary>
        public void Dispose() => this.httpClient.Dispose();

        #endregion
    }
}