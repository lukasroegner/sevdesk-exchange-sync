
#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SevDeskExchangeSync.Services.Exchange;
using SevDeskExchangeSync.Services.Exchange.ExtendedProperties;
using SevDeskExchangeSync.Services.SevDesk;

#endregion

namespace SevDeskExchangeSync.Services.Sync
{
    /// <summary>
    /// Represents the service that synchronizes sevDesk and Exchange entities.
    /// </summary>
    public sealed class SyncService
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="SyncService" /> instance.
        /// </summary>
        public SyncService(SevDeskService sevDeskService, ExchangeService exchangeService)
        {
            // Stores the services for further use
            this.SevDeskService = sevDeskService;
            this.ExchangeService = exchangeService;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the service for the sevDesk web application.
        /// </summary>
        public SevDeskService SevDeskService { get; private set; } 

        /// <summary>
        /// Gets the service for the Azure Active Directory and Exchange APIs.
        /// </summary>
        public ExchangeService ExchangeService { get; private set; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gets the IDs of all Exchange users.
        /// </summary>
        /// <returns>Returns a list of all IDs..</returns>
        private async Task<IEnumerable<string>> GetExchangeUserIdsAsync()
        {
            // Gets the user IDs from Azure Active Directory
            List<string> exchangeUserIds = new List<string>();
            SkipToken skipToken = null;
            do
            {
                Exchange.Users.UserList userList = await this.ExchangeService.GetUsersAsync(skipToken);
                skipToken = userList.SkipToken;
                exchangeUserIds.AddRange(userList.Select(user => user.Id));
            }
            while (skipToken != null);

            // Returns the user IDs
            return exchangeUserIds;
        }

        /// <summary>
        /// Gets all contacts of an Exchange user.
        /// </summary>
        /// <param name="exchangeUserId">The ID of the user for which the contacts are to be retrieved.</param>
        /// <returns>Returns the contacts.</returns>
        private async Task<IEnumerable<Exchange.Contacts.Contact>> GetExchangeContactsAsync(string exchangeUserId)
        {
            // Gets the contacts of the user
            List<Exchange.Contacts.Contact> exchangeContacts = new List<Exchange.Contacts.Contact>();
            SkipToken skipToken = null;
            do
            {
                Exchange.Contacts.ContactList contactList = await this.ExchangeService.GetContactsAsync(exchangeUserId, null, skipToken);
                skipToken = contactList.SkipToken;
                exchangeContacts.AddRange(contactList);
            }
            while (skipToken != null);

            // Returns the contacts
            return exchangeContacts;
        }

        /// <summary>
        /// Gets all sevDesk contacts that belong to the specified categories.
        /// </summary>
        /// <param name="contactCategoryIds">The categories of the contacts that should by synchronized. If <c>null</c>, all contacts are returned.</param>
        /// <returns>Returns the list of contacts.</returns>
        private async Task<IEnumerable<SevDesk.Contacts.Person>> GetSevDeskContactsAsync(IEnumerable<string> contactCategoryIds)
        {
            // Gets all contacts from sevDesk
            List<SevDesk.Contacts.Contact> sevDeskContacts = new List<SevDesk.Contacts.Contact>();
            SevDesk.Contacts.ContactList sevDeskContactList = null;
            do
            {
                sevDeskContactList = await this.SevDeskService.GetContactsAsync(sevDeskContacts.Count);
                sevDeskContacts.AddRange(sevDeskContactList);
            }
            while (sevDeskContactList.Count > sevDeskContacts.Count);

            // Sorts out the contacts of other categories
            if (contactCategoryIds != null)
                sevDeskContacts = sevDeskContacts.Where(contact => contactCategoryIds.Contains(contact.Category.Id)).ToList();

            // Sorts out the organization
            return sevDeskContacts.OfType<SevDesk.Contacts.Person>().OrderBy(person => person.LastName).ThenBy(person => person.FirstName).ToList();
        }

        /// <summary>
        /// Synchronizes the contact from sevDesk to Exchange.
        /// </summary>
        /// <param name="sevDeskContact">The sevDesk contact.</param>
        /// <param name="exchangeUserId">The ID of the Exchange user.</param>
        /// <param name="exchangeContactId">The ID of the Exchange contact.</param>
        /// <returns>Returns the sync state of this operation.</returns>
        private async Task<ContactSyncState> SynchronizeContactFromSevDeskToExchangeAsync(SevDesk.Contacts.Person sevDeskContact, string exchangeUserId, string exchangeContactId)
        {
            // Gets the addresses and communication ways of the sevDesk contact
            IEnumerable<SevDesk.Addresses.Address> contactAddresses = await this.SevDeskService.GetAddressesAsync(sevDeskContact.Id);
            IEnumerable<SevDesk.Addresses.Address> organizationAddresses = sevDeskContact.Organization == null ? new List<SevDesk.Addresses.Address>() : await this.SevDeskService.GetAddressesAsync(sevDeskContact.Organization.Id);
            IEnumerable<SevDesk.CommunicationWays.CommunicationWay> contactCommunicationWays = await this.SevDeskService.GetCommunicationWaysAsync(sevDeskContact.Id);
            IEnumerable<SevDesk.CommunicationWays.CommunicationWay> organizationCommunicationWays = sevDeskContact.Organization == null ? new List<SevDesk.CommunicationWays.CommunicationWay>() : await this.SevDeskService.GetCommunicationWaysAsync(sevDeskContact.Organization.Id);

            // Gets the home address
            SevDesk.Addresses.Address homeAddress = contactAddresses.FirstOrDefault(address => address.Category is SevDesk.Categories.PrivateAddressCategory);
            if (homeAddress == null)
                homeAddress = organizationAddresses.FirstOrDefault(address => address.Category is SevDesk.Categories.PrivateAddressCategory);

            // Gets the business address
            SevDesk.Addresses.Address businessAddress = contactAddresses.FirstOrDefault(address => address.Category is SevDesk.Categories.WorkAddressCategory);
            if (businessAddress == null)
                businessAddress = organizationAddresses.FirstOrDefault(address => address.Category is SevDesk.Categories.WorkAddressCategory);

            // Gets the home phone numbers
            List<string> homePhoneNumbers = new List<string>();
            homePhoneNumbers.AddRange(contactCommunicationWays.OfType<SevDesk.CommunicationWays.PhoneNumber>().Where(phoneNumber => phoneNumber.Kind is SevDesk.CommunicationWayKinds.PrivateKind).Select(phoneNumber => phoneNumber.Value));
            homePhoneNumbers.AddRange(organizationCommunicationWays.OfType<SevDesk.CommunicationWays.PhoneNumber>().Where(phoneNumber => phoneNumber.Kind is SevDesk.CommunicationWayKinds.PrivateKind).Select(phoneNumber => phoneNumber.Value));
            homePhoneNumbers = homePhoneNumbers.Distinct().ToList();

            // Gets the mobile phone number
            string mobilePhoneNumber = contactCommunicationWays.OfType<SevDesk.CommunicationWays.MobilePhoneNumber>().Where(phoneNumber => phoneNumber.Kind is SevDesk.CommunicationWayKinds.MobileKind).OrderBy(phoneNumber => phoneNumber.IsPrimary).Select(phoneNumber => phoneNumber.Value).FirstOrDefault();
            if (mobilePhoneNumber == null)
                mobilePhoneNumber = organizationCommunicationWays.OfType<SevDesk.CommunicationWays.MobilePhoneNumber>().Where(phoneNumber => phoneNumber.Kind is SevDesk.CommunicationWayKinds.MobileKind).OrderBy(phoneNumber => phoneNumber.IsPrimary).Select(phoneNumber => phoneNumber.Value).FirstOrDefault();

            // Gets the business website
            string businessWebsite = contactCommunicationWays.OfType<SevDesk.CommunicationWays.Website>().Where(website => website.Kind is SevDesk.CommunicationWayKinds.WorkKind).OrderBy(website => website.IsPrimary).Select(website => website.Value).FirstOrDefault();
            if (businessWebsite == null)
                businessWebsite = organizationCommunicationWays.OfType<SevDesk.CommunicationWays.Website>().Where(website => website.Kind is SevDesk.CommunicationWayKinds.WorkKind).OrderBy(website => website.IsPrimary).Select(website => website.Value).FirstOrDefault();

            // Gets the business numbers
            List<string> businessPhoneNumbers = new List<string>();
            businessPhoneNumbers.AddRange(contactCommunicationWays.OfType<SevDesk.CommunicationWays.PhoneNumber>().Where(phoneNumber => phoneNumber.Kind is SevDesk.CommunicationWayKinds.WorkKind).Select(phoneNumber => phoneNumber.Value));
            businessPhoneNumbers.AddRange(organizationCommunicationWays.OfType<SevDesk.CommunicationWays.PhoneNumber>().Where(phoneNumber => phoneNumber.Kind is SevDesk.CommunicationWayKinds.WorkKind).Select(phoneNumber => phoneNumber.Value));
            businessPhoneNumbers = businessPhoneNumbers.Distinct().ToList();

            // Gets the business numbers
            List<Exchange.Contacts.EmailAddress> emailAddresses = new List<Exchange.Contacts.EmailAddress>();
            emailAddresses.AddRange(contactCommunicationWays.OfType<SevDesk.CommunicationWays.EmailAddress>().Select(emailAddress => new Exchange.Contacts.EmailAddress($"{sevDeskContact.FirstName} {sevDeskContact.LastName}", emailAddress.Value)));
            emailAddresses.AddRange(organizationCommunicationWays.OfType<SevDesk.CommunicationWays.EmailAddress>().Select(emailAddress => new Exchange.Contacts.EmailAddress($"{sevDeskContact.Organization.Name}", emailAddress.Value)));
            emailAddresses = emailAddresses.Distinct().ToList();

            // Updates the contact
            Exchange.Contacts.Contact exchangeContact = await this.ExchangeService.UpdateContactAsync(exchangeUserId, exchangeContactId, new Exchange.Contacts.ContactOptions
            {
                FirstName = sevDeskContact.FirstName,
                LastName = sevDeskContact.LastName,
                Birthday = sevDeskContact.Birthday,
                BusinessAddress = businessAddress == null ? null : new Exchange.Contacts.Address(businessAddress.Street, businessAddress.ZipCode, businessAddress.City, null, businessAddress.Country.Name),
                HomeAddress = homeAddress == null ? null : new Exchange.Contacts.Address(homeAddress.Street, homeAddress.ZipCode, homeAddress.City, null, homeAddress.Country.Name),
                CompanyName = sevDeskContact.Organization?.Name,
                JobTitle = sevDeskContact.Position,
                PersonalNotes = sevDeskContact.Description,
                Title = sevDeskContact.AcademicTitle,
                BusinessPhoneNumbers = businessPhoneNumbers,
                HomePhoneNumbers = homePhoneNumbers,
                MobilePhoneNumber = mobilePhoneNumber,
                BusinessWebsite = businessWebsite,
                EmailAddresses = emailAddresses
            });

            // returns the new sync state
            return new ContactSyncState
            {
                ExchangeContactId = exchangeContact.Id,
                ExchangeContactLastModifiedDateTime = exchangeContact.LastModifiedDateTime,
                ExchangeUserId = exchangeUserId,
                SevDeskContactId = sevDeskContact.Id,
                SevDeskContactUpdateDateTime = sevDeskContact.UpdateDateTime
            };
        }

        /// <summary>
        /// Synchronizes the contact from Exchange to sevDesk.
        /// </summary>
        /// <param name="exchangeUserId">The ID of the Exchange user.</param>
        /// <param name="exchangeContact">The Exchange contact.</param>
        /// <param name="sevDeskContact">The sevDesk contact.</param>
        /// <returns>Returns the sync state of this operation.</returns>
        private async Task<ContactSyncState> SynchronizeContactFromExchangeToSevDeskAsync(string exchangeUserId, Exchange.Contacts.Contact exchangeContact, SevDesk.Contacts.Person sevDeskContact)
        {
            // Updates the contact
            SevDesk.Contacts.Contact updatedSevDeskContact = await this.SevDeskService.UpdateContactAsync(sevDeskContact.Id, new SevDesk.Contacts.PersonOptions 
            { 
                AcademicTitle = exchangeContact.Title,
                Birthday = exchangeContact.Birthday,
                CategoryId = sevDeskContact.Category?.Id,
                CustomerNumber = sevDeskContact.CustomerNumber,
                Description = exchangeContact.PersonalNotes,
                Gender = sevDeskContact.Gender,
                OrganizationId = sevDeskContact.Organization?.Id,
                Position = exchangeContact.JobTitle,
                FirstName = exchangeContact.FirstName,
                LastName = exchangeContact.LastName
            });

            // returns the new sync state
            return new ContactSyncState
            {
                ExchangeContactId = exchangeContact.Id,
                ExchangeContactLastModifiedDateTime = exchangeContact.LastModifiedDateTime,
                ExchangeUserId = exchangeUserId,
                SevDeskContactId = updatedSevDeskContact.Id,
                SevDeskContactUpdateDateTime = updatedSevDeskContact.UpdateDateTime
            };
        }

        /// <summary>
        /// Synchronizes the sevDesk contacts and the Exchange contacts of a single user.
        /// </summary>
        /// <param name="syncStates">The sync states of the last synchronization.</param>
        /// <param name="exchangeUserId">The ID of the exchange user.</param>
        /// <param name="contactCategoryIds">The sevDesk categories of contacts that should be synchronized.</param>
        /// <param name="progress">The progress that emits status messages.</param>
        /// <returns>Returns the new sync states.</returns>
        private async Task<IEnumerable<ContactSyncState>> SynchronizeContactsAsync(IEnumerable<ContactSyncState> syncStates, string exchangeUserId, IEnumerable<string> contactCategoryIds, IProgress<string> progress)
        {
            // Initialiezs new the sync states
            List<ContactSyncState> newSyncStates = new List<ContactSyncState>();

            // Gets the sevDesk contacts and Exchange contacts
            List<SevDesk.Contacts.Person> sevDeskContacts = (await this.GetSevDeskContactsAsync(contactCategoryIds)).ToList();
            List<Exchange.Contacts.Contact> exchangeContacts = (await this.GetExchangeContactsAsync(exchangeUserId)).ToList();

            // Creates contacts that do not exist in Exchange yet
            foreach (SevDesk.Contacts.Person sevDeskContact in sevDeskContacts.Where(sevDeskContact => !syncStates.Any(state => state.SevDeskContactId == sevDeskContact.Id) || syncStates.Any(state => state.SevDeskContactId == sevDeskContact.Id && !exchangeContacts.Any(exchangeContact => state.ExchangeContactId == exchangeContact.Id))).ToList())
            {
                // Creates the contact in Exchange
                progress.Report($"Adding new contact to Exchange (sevDESK user with ID \"{sevDeskContact.Id}\").");
                Exchange.Contacts.Contact newContact = await this.ExchangeService.CreateContactAsync(exchangeUserId, new Exchange.Contacts.ContactOptions
                {
                    LastName = sevDeskContact.LastName,
                    FirstName = sevDeskContact.FirstName
                });

                // Synchronizes the data from sevDesk to Exchange
                newSyncStates.Add(await this.SynchronizeContactFromSevDeskToExchangeAsync(sevDeskContact, exchangeUserId, newContact.Id));

                // Removes the contact from the list
                sevDeskContacts.Remove(sevDeskContact);
            }

            // Removes the contacts that no longer exist in sevDesk
            foreach (Exchange.Contacts.Contact exchangeContact in exchangeContacts.Where(exchangeContact => syncStates.Any(state => state.ExchangeContactId == exchangeContact.Id && !sevDeskContacts.Any(sevDeskContact => state.SevDeskContactId == sevDeskContact.Id))).ToList())
            {
                // Removes the contact from Exchange
                progress.Report($"Removing contact from Exchange (Exchange user with ID \"{exchangeContact.Id}\").");
                await this.ExchangeService.RemoveContactAsync(exchangeUserId, exchangeContact.Id);

                // Removes the contact from the lsit
                exchangeContacts.Remove(exchangeContact);
            }

            // Cycles over all contacts and checks for updates
            foreach (SevDesk.Contacts.Person sevDeskContact in sevDeskContacts)
            {
                // Gets the sync state and the exchange contact
                ContactSyncState contactSyncState = syncStates.Single(state => state.SevDeskContactId == sevDeskContact.Id);
                Exchange.Contacts.Contact exchangeContact = exchangeContacts.Single(contact => contact.Id == contactSyncState.ExchangeContactId);

                // Checks if both contacts have not changed
                if (contactSyncState.SevDeskContactUpdateDateTime == sevDeskContact.UpdateDateTime && contactSyncState.ExchangeContactLastModifiedDateTime == exchangeContact.LastModifiedDateTime)
                {
                    progress.Report($"No changes for contact (sevDesk user with ID \"{sevDeskContact.Id}\", Exchange user with ID \"{exchangeContact.Id}\").");
                    newSyncStates.Add(contactSyncState);
                    continue;
                }

                // Checks if the sevDesk contact changed, but the Exchange contact did not
                if (contactSyncState.SevDeskContactUpdateDateTime != sevDeskContact.UpdateDateTime && contactSyncState.ExchangeContactLastModifiedDateTime == exchangeContact.LastModifiedDateTime)
                {
                    progress.Report($"Synchronizing from sevDesk to Exchange (sevDesk user with ID \"{sevDeskContact.Id}\", Exchange user with ID \"{exchangeContact.Id}\").");
                    newSyncStates.Add(await this.SynchronizeContactFromSevDeskToExchangeAsync(sevDeskContact, exchangeUserId, exchangeContact.Id));
                    continue;
                }

                // Checks if the Exchange contact changed, but the sevDesk contact did not
                if (contactSyncState.SevDeskContactUpdateDateTime == sevDeskContact.UpdateDateTime && contactSyncState.ExchangeContactLastModifiedDateTime != exchangeContact.LastModifiedDateTime)
                {
                    progress.Report($"Synchronizing from Exchange to sevDesk (sevDesk user with ID \"{sevDeskContact.Id}\", Exchange user with ID \"{exchangeContact.Id}\").");
                    newSyncStates.Add(await this.SynchronizeContactFromExchangeToSevDeskAsync(exchangeUserId, exchangeContact, sevDeskContact));
                    continue;
                }

                // As both contacts changed, checks which one is newer
                if (sevDeskContact.UpdateDateTime > exchangeContact.LastModifiedDateTime)
                {
                    progress.Report($"Synchronizing from sevDesk to Exchange (sevDesk user with ID \"{sevDeskContact.Id}\", Exchange user with ID \"{exchangeContact.Id}\").");
                    newSyncStates.Add(await this.SynchronizeContactFromSevDeskToExchangeAsync(sevDeskContact, exchangeUserId, exchangeContact.Id));
                }
                else
                {
                    progress.Report($"Synchronizing from Exchange to sevDesk (sevDesk user with ID \"{sevDeskContact.Id}\", Exchange user with ID \"{exchangeContact.Id}\").");
                    newSyncStates.Add(await this.SynchronizeContactFromExchangeToSevDeskAsync(exchangeUserId, exchangeContact, sevDeskContact));
                }
            }

            // Returns the new sync states
            return newSyncStates;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the contacts between sevDesk and the Exchange accounts of the Active Directory members.
        /// </summary>
        /// <param name="syncStates">The sync states of the last synchronization.</param>
        /// <param name="contactCategoryIds">The IDs of the categories for which contacts should be synchronized. Can be <c>null</c> if all categories should be synchronized.</param>
        /// <param name="azureActiveDirectoryUserIds">The IDs of the Active Directory users that should participate in the synchronization. Can be <c>null</c> if all users should participate.</param>
        /// <param name="progress">The progress that emits status messages.</param>
        /// <exception cref="InvalidOperationException">If the synchronization fails, an <see cref="InvalidOperationException" /> is thrown.</exception>
        /// <returns>Returns the new sync states.</returns>
        public async Task<IEnumerable<ContactSyncState>> SynchronizeContactsAsnyc(IEnumerable<ContactSyncState> syncStates, IEnumerable<string> contactCategoryIds, IEnumerable<string> azureActiveDirectoryUserIds, IProgress<string> progress)
        {
            try
            {
                // Initialiezs new the sync states
                List<ContactSyncState> newSyncStates = new List<ContactSyncState>();

                // Gets the Azure Active Directory users
                IEnumerable<string> exchangeUserIds;
                if (azureActiveDirectoryUserIds == null)
                    exchangeUserIds = await this.GetExchangeUserIdsAsync();
                else
                    exchangeUserIds = azureActiveDirectoryUserIds;

                // Cycles over all users and performs the synchronization
                foreach (string exchangeUserId in exchangeUserIds)
                {
                    // Synchronizes the contacts of sevDesk and the current Exchange user
                    progress.Report($"Synchronizing contacts for Exchange user \"{exchangeUserId}\"...");
                    newSyncStates.AddRange(await this.SynchronizeContactsAsync(syncStates.Where(state => state.ExchangeUserId == exchangeUserId).ToList(), exchangeUserId, contactCategoryIds, progress));
                    progress.Report($"Synchronization of contacts for Exchange user \"{exchangeUserId}\" finished.");
                }

                // Returns the new sync states
                return newSyncStates;
            }
            catch (Exception e)
            {
                throw new InvalidOperationException(e.Message, e);
            }         
        }

        #endregion
    }
}