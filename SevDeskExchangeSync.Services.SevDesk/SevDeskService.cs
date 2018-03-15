
#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SevDeskExchangeSync.Services.SevDesk.Addresses;
using SevDeskExchangeSync.Services.SevDesk.Categories;
using SevDeskExchangeSync.Services.SevDesk.CommunicationWayKinds;
using SevDeskExchangeSync.Services.SevDesk.CommunicationWays;
using SevDeskExchangeSync.Services.SevDesk.Contacts;
using SevDeskExchangeSync.Services.SevDesk.Models;
using SevDeskExchangeSync.Services.SevDesk.Users;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk
{
    /// <summary>
    /// Represents the service that communicates with the sevDesk API.
    /// </summary>
    public sealed class SevDeskService : IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="SevDeskService" /> instance.
        /// </summary>
        /// <param name="token">The token that is used to authenticate against the sevDesk API.</param>
        public SevDeskService(string token)
        {
            // Stores the token for further use
            this.Token = token;

            // Intializes the HTTP client
            this.httpClient = new HttpClient();
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Contains the URI of the sevDesk API.
        /// </summary>
        private readonly string apiUri = "https://my.sevdesk.de/api/v1"; 

        /// <summary>
        /// Contains the HTTP client that is used to communicate with the API.
        /// </summary>
        private readonly HttpClient httpClient;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the token that is used to authenticate against the sevDesk API.
        /// </summary>
        public string Token { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a new <see cref="Address" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created address instance.</returns>
        private Address GetAddress(Models.Addresses.Address model)
        {
            return new Address
            {
                Id = model.id,
                Name = model.name,
                CreationDateTime = DateTime.Parse(model.create),
                UpdateDateTime = DateTime.Parse(model.update),
                Street = model.street,
                City = model.city,
                ZipCode = model.zip,
                Country = this.GetCountry(model.country),
                Category = model.category == null ? null : this.GetCategory(model.category) as AddressCategory
            };
        }

        /// <summary>
        /// Creates a new <see cref="Country" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created country instance.</returns>
        private Country GetCountry(Models.Addresses.Country model)
        {
            return new Country
            {
                Id = model.id,
                Name = model.name,
                Code = model.code,
                EnglishName = model.nameEn,
                Priority = string.IsNullOrWhiteSpace(model.priority) ? (Nullable<int>)null : int.Parse(model.priority)
            };
        }

        /// <summary>
        /// Creates a new <see cref="CommunicationWay" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created communication way instance.</returns>
        private CommunicationWay GetCommunicationWay(Models.CommunicationWays.CommunicationWay model)
        {
            // Initializes the communication way
            CommunicationWay communicationWay;

            // Creates the appriopriate instance
            switch (model.type)
            {
                case "PHONE":
                    communicationWay = new PhoneNumber();
                    break;

                case "MOBILE":
                    communicationWay = new MobilePhoneNumber();
                    break;

                case "WEB":
                    communicationWay = new Website();
                    break;

                case "EMAIL":
                default:
                    communicationWay = new EmailAddress();
                    break;
            }

            // Sets the communication way specific properties
            communicationWay.Id = model.id;
            communicationWay.CreationDateTime = DateTime.Parse(model.create);
            communicationWay.UpdateDateTime = DateTime.Parse(model.update);
            communicationWay.IsPrimary = model.main != "0";
            communicationWay.Value = model.value;
            communicationWay.Kind = model.key == null ? null : this.GetCommunicationWayKind(model.key);

            // Returns the created communication way
            return communicationWay;
        }

        /// <summary>
        /// Creates a new <see cref="CommunicationWayKind" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created communication way kind instance.</returns>
        private CommunicationWayKind GetCommunicationWayKind(Models.CommunicationWayKeys.CommunicationWayKey model)
        {
            // Initializes the communication way kind
            CommunicationWayKind communicationWayKind;

            // Creates the appriopriate instance
            switch (model.translationCode)
            {
                case "COMM_WAY_KEY_AUTOBOX":
                    communicationWayKind = new AutoboxKind();
                    break;

                case "COMM_WAY_KEY_FAX":
                    communicationWayKind = new FaxKind();
                    break;

                case "COMM_WAY_KEY_INVOICE_ADDRESS":
                    communicationWayKind = new InvoiceAddressKind();
                    break;

                case "COMM_WAY_KEY_MOBILE":
                    communicationWayKind = new MobileKind();
                    break;

                case "COMM_WAY_KEY_NEWSLETTER":
                    communicationWayKind = new NewsletterKind();
                    break;

                case "COMM_WAY_KEY_PRIVAT":
                    communicationWayKind = new PrivateKind();
                    break;

                case "COMM_WAY_KEY_WORK":
                    communicationWayKind = new WorkKind();
                    break;

                default:
                case "COMM_WAY_KEY_EMPTY":
                    return null;
            }

            // Sets the communication way kind specific properties
            communicationWayKind.Id = model.id;
            communicationWayKind.CreationDateTime = string.IsNullOrWhiteSpace(model.create) ? (Nullable<DateTime>)null : DateTime.Parse(model.create);
            communicationWayKind.UpdateDateTime = string.IsNullOrWhiteSpace(model.update) ? (Nullable<DateTime>)null : DateTime.Parse(model.update);
            communicationWayKind.Name = model.name;

            // Returns the created communication way kind
            return communicationWayKind;
        }

        /// <summary>
        /// Creates a new <see cref="Category" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created category instance.</returns>
        private Category GetCategory(Models.Categories.Category model)
        {
            // Initializes the category
            Category category;

            // Checks whether the category is a contact category
            switch (model.objectType)
            {
                case "Contact":
                    category = new ContactCategory();
                    break;

                case "Document":
                    category = new DocumentCategory();
                    break;

                case "ProjectTime":
                    category = new ProjectTimeCategory();
                    break;

                case "Task":
                    category = new TaskCategory();
                    break;

                case "Part":
                    category = new PartCategory();
                    break;

                case "Project":
                    category = new ProjectCategory();
                    break;

                case "ContactAddress":
                    switch (model.translationCode)
                    {
                        case "CATEGORY_WORK":
                            category = new WorkAddressCategory();
                            break;

                        case "CATEGORY_PRIVAT":
                            category = new PrivateAddressCategory();
                            break;

                        case "CATEGORY_INVOICE_ADDRESS":
                            category = new InvoiceAddressCategory();
                            break;

                        case "CATEGORY_DELIVERY_ADDRESS":
                            category = new DeliveryAddressCategory();
                            break;

                        case "CATEGORY_PICKUP_ADDRESS":
                            category = new PickupAddressCategory();
                            break;

                        case "CATEGORY_EMPTY":
                        default:
                            return null;
                    }
                    break;

                default:
                    return null;
            }

            // Sets the category specific properties
            category.Id = model.id;
            category.CreationDateTime = string.IsNullOrWhiteSpace(model.create) ? (Nullable<DateTime>)null : DateTime.Parse(model.create);
            category.UpdateDateTime = string.IsNullOrWhiteSpace(model.update) ? (Nullable<DateTime>)null : DateTime.Parse(model.update);
            category.Name = model.name;
            category.Color = model.color;
            category.Code = model.code;
            category.Priority = int.Parse(model.priority);

            // Returns the created category
            return category;
        }

        /// <summary>
        /// Creates a new <see cref="Contact" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created contact instance.</returns>
        private Contact GetContact(Models.Contacts.Contact model)
        {
            // Initializes the contact
            Contact contact;

            // Checks whether the contact is an organization or a person
            if (string.IsNullOrWhiteSpace(model.familyname))
                contact = this.GetOrganization(model);
            else
                contact = this.GetPerson(model);

            // Sets the contact specific properties
            contact.Id = model.id;
            contact.CreationDateTime = string.IsNullOrWhiteSpace(model.create) ? DateTime.Now : DateTime.Parse(model.create);
            contact.UpdateDateTime = string.IsNullOrWhiteSpace(model.update) ? DateTime.Now :DateTime.Parse(model.update);
            contact.Category = model.category == null ? null : this.GetCategory(model.category) as ContactCategory;
            contact.CustomerNumber = model.customerNumber;
            contact.Description = model.description;

            // Returns the created contact
            return contact;
        }

        /// <summary>
        /// Creates a new <see cref="Organization" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created organization instance.</returns>
        private Organization GetOrganization(Models.Contacts.Contact model)
        {
            return new Organization
            {
                Name = model.name
            };
        }

        /// <summary>
        /// Creates a new <see cref="Person" /> instance from the provided model.
        /// </summary>
        /// <param name="model">The model coming from the API.</param>
        /// <returns>Returns the created person instance.</returns>
        private Person GetPerson(Models.Contacts.Contact model)
        {
            return new Person
            {
                FirstName = model.surename,
                LastName = model.familyname,
                Position = model.titel,
                AcademicTitle = model.academicTitle,
                Gender = model.gender == "m" ? Gender.Male : Gender.Female,
                Birthday = string.IsNullOrWhiteSpace(model.birthday) ? (Nullable<DateTime>)null : DateTime.Parse(model.birthday),
                Organization = model.parent == null ? null : this.GetContact(model.parent) as Organization
            };
        }

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
                UserName = model.username
            };
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets all contacts from the sevDesk API.
        /// </summary>
        /// <param name="skip">The number of contacts to skip.</param>
        /// <param name="take">The number of contacts to include in the results.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the list of contacts.</returns>
        public async Task<ContactList> GetContactsAsync(int skip = 0, int take = 100)
        {
            try
            {
                // Downloads the contacts
                string response = await this.httpClient.GetStringAsync($"{this.apiUri}/Contact?depth=1&countAll=1&embed=category,parent,parent.category&offset={skip}&limit={take}&token={this.Token}");

                // Parses the response
                ResultList<Models.Contacts.Contact> result = JsonConvert.DeserializeObject<ResultList<Models.Contacts.Contact>>(response);

                // Returns the result
                return new ContactList(result.objects.Select(contact => this.GetContact(contact)).ToList())
                {
                    Skip = skip,
                    Take = take,
                    Count = result.total
                };   
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }         
        }

        /// <summary>
        /// Updates an existing contact from the sevDesk API.
        /// </summary>
        /// <param name="id">The ID of the contact.</param>
        /// <param name="options">The options that contain the information about the contact.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the updated contact.</returns>
        public async Task<Contact> UpdateContactAsync(string id, ContactOptions options)
        {
            string result = null;
            Result<Models.Contacts.Contact> result2;
            try
            {
                // Initializes the key value pairs
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                if (!string.IsNullOrWhiteSpace(options.CustomerNumber))
                    values.Add(new KeyValuePair<string, string>("customerNumber", options.CustomerNumber));
                if (!string.IsNullOrWhiteSpace(options.Description))
                    values.Add(new KeyValuePair<string, string>("description", options.Description));
                if (!string.IsNullOrWhiteSpace(options.CategoryId))
                {
                    values.Add(new KeyValuePair<string, string>("category[id]", options.CategoryId));
                    values.Add(new KeyValuePair<string, string>("category[objectName]", "Category"));
                }

                // Adds the specific properties of a person
                PersonOptions personOptions = options as PersonOptions;
                if (personOptions != null)
                {
                    if (!string.IsNullOrWhiteSpace(personOptions.FirstName))
                        values.Add(new KeyValuePair<string, string>("surename", personOptions.FirstName));
                    if (!string.IsNullOrWhiteSpace(personOptions.LastName))
                        values.Add(new KeyValuePair<string, string>("familyname", personOptions.LastName));
                    if (!string.IsNullOrWhiteSpace(personOptions.AcademicTitle))
                        values.Add(new KeyValuePair<string, string>("academicTitle", personOptions.AcademicTitle));
                    if (!string.IsNullOrWhiteSpace(personOptions.Position))
                        values.Add(new KeyValuePair<string, string>("titel", personOptions.Position));
                    if (personOptions.Birthday != null)
                        values.Add(new KeyValuePair<string, string>("birthday", personOptions.Birthday?.ToString("yyyy-MM-ddT00:00:00")));
                    values.Add(new KeyValuePair<string, string>("gender", personOptions.Gender == Gender.Male ? "m" : "w"));
                    if (!string.IsNullOrWhiteSpace(personOptions.OrganizationId))
                    {
                        values.Add(new KeyValuePair<string, string>("parent[id]", personOptions.OrganizationId));
                        values.Add(new KeyValuePair<string, string>("parent[objectName]", "Contact"));
                    }
                }

                // Adds the specific properties of an organization
                OrganizationOptions organizationOptions = options as OrganizationOptions;
                if (organizationOptions != null)
                {
                    if (!string.IsNullOrWhiteSpace(organizationOptions.Name))
                        values.Add(new KeyValuePair<string, string>("name", organizationOptions.Name));
                }

                // Sends an HTTP request to endpoint
                HttpResponseMessage response = await httpClient.PutAsync($"{this.apiUri}/Contact/{id}?token={this.Token}", new FormUrlEncodedContent(values));

                // Returns the updated contact
                result = await response.Content.ReadAsStringAsync();
                result2 = JsonConvert.DeserializeObject<Result<Models.Contacts.Contact>>(await response.Content.ReadAsStringAsync());
                if (result2.objects.familyname == "Danneberg")
                {
                    
                }
                return this.GetContact(JsonConvert.DeserializeObject<Result<Models.Contacts.Contact>>(await response.Content.ReadAsStringAsync()).objects);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets all addresses of a contact from the sevDesk API.
        /// </summary>
        /// <param name="id">The ID of the contact.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the list of addresses.</returns>
        public async Task<IEnumerable<Address>> GetAddressesAsync(string id)
        {
            try
            {
                // Downloads the contact
                string response = await this.httpClient.GetStringAsync($"{this.apiUri}/Contact/{id}?embed=addresses,addresses.category,addresses.country&token={this.Token}");

                // Parses the response
                ResultList<Models.Contacts.Contact> result = JsonConvert.DeserializeObject<ResultList<Models.Contacts.Contact>>(response);

                // Returns the result
                return result.objects.Single().addresses.Select(address => this.GetAddress(address)).ToList();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Creates a new address for a contact from the sevDesk API.
        /// </summary>
        /// <param name="id">The ID of the contact.</param>
        /// <param name="options">The options that contain the information about the address.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the ID of the created address.</returns>
        public async Task<string> CreateAddressAsync(string id, AddressOptions options)
        {
            try
            {
                // Initializes the key value pairs
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                if (!string.IsNullOrWhiteSpace(options.Street))
                    values.Add(new KeyValuePair<string, string>("street", options.Street));
                if (!string.IsNullOrWhiteSpace(options.ZipCode))
                    values.Add(new KeyValuePair<string, string>("zip", options.ZipCode));
                if (!string.IsNullOrWhiteSpace(options.City))
                    values.Add(new KeyValuePair<string, string>("city", options.City));
                if (!string.IsNullOrWhiteSpace(options.CountryId))
                    values.Add(new KeyValuePair<string, string>("country", options.CountryId));
                if (!string.IsNullOrWhiteSpace(options.CategoryId))
                    values.Add(new KeyValuePair<string, string>("category", options.CategoryId));

                // Sends an HTTP request to endpoint
                HttpResponseMessage response = await httpClient.PostAsync($"{this.apiUri}/Contact/{id}/addAddress?token={this.Token}", new FormUrlEncodedContent(values));

                // Returns the result
                return JsonConvert.DeserializeObject<Result<Models.Addresses.Address>>(await response.Content.ReadAsStringAsync()).objects.id;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Updates an existing address for a contact from the sevDesk API.
        /// </summary>
        /// <param name="id">The ID of the address.</param>
        /// <param name="options">The options that contain the information about the address.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        public async Task UpdateAddressAsync(string id, AddressOptions options)
        {
            try
            {
                // Initializes the key value pairs
                List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
                if (!string.IsNullOrWhiteSpace(options.Street))
                    values.Add(new KeyValuePair<string, string>("street", options.Street));
                if (!string.IsNullOrWhiteSpace(options.ZipCode))
                    values.Add(new KeyValuePair<string, string>("zip", options.ZipCode));
                if (!string.IsNullOrWhiteSpace(options.City))
                    values.Add(new KeyValuePair<string, string>("city", options.City));
                if (!string.IsNullOrWhiteSpace(options.CountryId))
                {
                    values.Add(new KeyValuePair<string, string>("country[id]", options.CountryId));
                    values.Add(new KeyValuePair<string, string>("country[objectName]", "StaticCountry"));
                }
                if (!string.IsNullOrWhiteSpace(options.CategoryId))
                {
                    values.Add(new KeyValuePair<string, string>("category[id]", options.CategoryId));
                    values.Add(new KeyValuePair<string, string>("category[objectName]", "Category"));
                }
                
                // Sends an HTTP request to endpoint
                HttpResponseMessage response = await httpClient.PutAsync($"{this.apiUri}/ContactAddress/{id}?token={this.Token}", new FormUrlEncodedContent(values));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Deletes an existing address for a contact from the sevDesk API.
        /// </summary>
        /// <param name="id">The ID of the address.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        public async Task RemoveAddressAsync(string id)
        {
            try
            {
                // Sends an HTTP request to endpoint
                await httpClient.DeleteAsync($"{this.apiUri}/ContactAddress/{id}?token={this.Token}");
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets all categories from the sevDesk API.
        /// </summary>
        /// <param name="skip">The number of categories to skip.</param>
        /// <param name="take">The number of categories to include in the results.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the list of categories.</returns>
        public async Task<CategoryList> GetCategoriesAsync(int skip = 0, int take = 100)
        {
            try
            {
                // Downloads the contacts
                string response = await this.httpClient.GetStringAsync($"{this.apiUri}/Category?countAll=1&offset={skip}&limit={take}&token={this.Token}");

                // Parses the response
                ResultList<Models.Categories.Category> result = JsonConvert.DeserializeObject<ResultList<Models.Categories.Category>>(response);

                // Returns the result
                return new CategoryList(result.objects.Select(category => this.GetCategory(category)).Where(category => category != null).ToList())
                {
                    Skip = skip,
                    Take = take,
                    Count = result.total - 1 // The EMPTY category for contact addresses is not returned
                };
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets all countries from the sevDesk API.
        /// </summary>
        /// <param name="skip">The number of countries to skip.</param>
        /// <param name="take">The number of countries to include in the results.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the list of countries.</returns>
        public async Task<CountryList> GetCountriesAsync(int skip = 0, int take = 100)
        {
            try
            {
                // Downloads the countries
                string response = await this.httpClient.GetStringAsync($"{this.apiUri}/StaticCountry?countAll=1&offset={skip}&limit={take}&token={this.Token}");

                // Parses the response
                ResultList<Models.Addresses.Country> result = JsonConvert.DeserializeObject<ResultList<Models.Addresses.Country>>(response);

                // Returns the result
                return new CountryList(result.objects.Select(country => this.GetCountry(country)).ToList())
                {
                    Skip = skip,
                    Take = take,
                    Count = result.total
                };
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets the communication way kinds from the sevDesk API.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the list of communication way kinds.</returns>
        public async Task<IEnumerable<CommunicationWayKind>> GetCommunicationWayKindsAsync()
        {
            try
            {
                // Downloads the communication way keys
                string response = await this.httpClient.GetStringAsync($"{this.apiUri}/CommunicationWayKey?token={this.Token}");

                // Parses the response
                ResultList<Models.CommunicationWayKeys.CommunicationWayKey> result = JsonConvert.DeserializeObject<ResultList<Models.CommunicationWayKeys.CommunicationWayKey>>(response);

                // Returns the result
                return result.objects.Select(communicationWayKey => this.GetCommunicationWayKind(communicationWayKey)).Where(communicationWayKind => communicationWayKind != null).ToList();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets the communication ways of a contact from the sevDesk API.
        /// </summary>
        /// <param name="id">The ID of the contact.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the list of communication ways.</returns>
        public async Task<IEnumerable<CommunicationWay>> GetCommunicationWaysAsync(string id)
        {
            try
            {
                // Downloads the communiction ways
                string response = await this.httpClient.GetStringAsync($"{this.apiUri}/Contact/{id}/getCommunicationWays?token={this.Token}&embed=key");

                // Parses the response
                ResultList<Models.CommunicationWays.CommunicationWay> result = JsonConvert.DeserializeObject<ResultList<Models.CommunicationWays.CommunicationWay>>(response);

                // Returns the result
                return result.objects.Select(communicationWay => this.GetCommunicationWay(communicationWay)).ToList();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Creates a new communication way for a contact from the sevDesk API.
        /// </summary>
        /// <param name="id">The ID of the contact.</param>
        /// <param name="options">The options that contain the information about the communication way.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the ID of the created communication way.</returns>
        public async Task<string> CreateCommunicationWayAsync(string id, CommunicationWayOptions options)
        {
            try
            {
                // Gets the appriopriate endpoint
                string endpoint = null;
                if (options is EmailAddressOptions)
                    endpoint = "addEmail";
                if (options is PhoneNumberOptions)
                    endpoint = "addPhone";
                if (options is WebsiteOptions)
                    endpoint = "addWeb";
                if (options is MobilePhoneNumberOptions)
                    endpoint = "addMobile";

                // Sends an HTTP request to endpoint
                HttpResponseMessage response = await httpClient.PostAsync($"{this.apiUri}/Contact/{id}/{endpoint}?token={this.Token}", new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("value", options.Value),
                    new KeyValuePair<string, string>("key", options.KindId)
                }));

                // Returns the result
                return JsonConvert.DeserializeObject<Result<Models.CommunicationWays.CommunicationWay>>(await response.Content.ReadAsStringAsync()).objects.id;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Updates an existing communication way for a contact from the sevDesk API.
        /// </summary>
        /// <param name="id">The ID of the communication way.</param>
        /// <param name="options">The options that contain the information about the communication way.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        public async Task UpdateCommunicationWayAsync(string id, CommunicationWayOptions options)
        {
            try
            {
                // Sends an HTTP request to endpoint
                HttpResponseMessage response = await httpClient.PutAsync($"{this.apiUri}/CommunicationWay/{id}?token={this.Token}", new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("value", options.Value),
                    new KeyValuePair<string, string>("key[id]", options.KindId),
                    new KeyValuePair<string, string>("key[objectName]", "CommunicationWayKey")
                }));
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Deletes an existing communication way for a contact from the sevDesk API.
        /// </summary>
        /// <param name="id">The ID of the communication way.</param>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        public async Task RemoveCommunicationWayAsync(string id)
        {
            try
            {
                // Sends an HTTP request to endpoint
                await httpClient.DeleteAsync($"{this.apiUri}/CommunicationWay/{id}?token={this.Token}");
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Gets all users from the sevDesk API.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the API could not be reached or the token is not valid, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the list of users.</returns>
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            try
            {
                // Downloads the users
                string response = await this.httpClient.GetStringAsync($"{this.apiUri}/SevUser?token={this.Token}");

                // Parses the response
                ResultList<Models.Users.User> result = JsonConvert.DeserializeObject<ResultList<Models.Users.User>>(response);

                // Returns the result
                return result.objects.Select(user => this.GetUser(user)).ToList();
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