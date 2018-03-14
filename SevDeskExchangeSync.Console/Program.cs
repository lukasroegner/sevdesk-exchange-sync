
#region Using Directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SevDeskExchangeSync.Services.Exchange;
using SevDeskExchangeSync.Services.Exchange.Users;
using SevDeskExchangeSync.Services.SevDesk;
using SevDeskExchangeSync.Services.SevDesk.Categories;
using SevDeskExchangeSync.Services.Sync;

#endregion

namespace SevDeskExchangeSync.Console
{
    /// <summary>
    /// Represents the entry point of the application.
    /// </summary>
    public class Program
    {
        #region Private Static Methods

        /// <summary>
        /// Loads the configuration from a file.
        /// </summary>
        /// <param name="fileName">The file name of the configuration.</param>
        /// <returns>Returns the configuration of the synchronization process.</returns>
        private static Configuration LoadConfiguration(string fileName)
        {
            try
            {
                // Reads the content of the file
                System.Console.WriteLine($"Loading configuration from file \"{fileName}\"...");
                string configurationFile = File.ReadAllText(fileName);
                System.Console.WriteLine($"Configuration loaded.");
                System.Console.WriteLine();

                // Returns the parsed configuration
                return JsonConvert.DeserializeObject<Configuration>(configurationFile);
            }
            catch (Exception e)
            {
                throw new InvalidOperationException($"Could not load configuration: {e.Message}", e);
            }
        }

        /// <summary>
        /// Saves the configuration to file.
        /// </summary>
        /// <param name="configuration">The configuration of the synchronization process.</param>
        private static void SaveConfiguration(Configuration configuration)
        {
            System.Console.WriteLine("6. Save Configuration");
            System.Console.WriteLine("Please provide a file name for the configuration that is created.");
            System.Console.WriteLine();

            // Gets the file name from the user
            string fileName = null;
            do
            {
                System.Console.Write("File name: ");
                fileName = System.Console.ReadLine();

                // Validates the input
                try
                {
                    File.WriteAllText(fileName, JsonConvert.SerializeObject(configuration));
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"The file could not be saved at the provided path: {e.Message}");
                    System.Console.WriteLine();
                }

            }
            while (string.IsNullOrWhiteSpace(fileName));
        }

        /// <summary>
        /// Synchronizes the contacts.
        /// </summary>
        /// <param name="configuration">The configuration of the synchronization process.</param>
        private static async Task SynchronizeContactsAsync(Configuration configuration)
        {
            try
            {
                System.Console.WriteLine($"Synchronizing contacts started...");
                System.Console.WriteLine();

                // Reads the content of the file
                string contactSyncStatesFile = null;
                if (File.Exists(configuration.ContactSyncStatesFileName))
                {
                    System.Console.WriteLine($"Loading contact sync states from file \"{configuration.ContactSyncStatesFileName}\"...");
                    contactSyncStatesFile = File.ReadAllText(configuration.ContactSyncStatesFileName);
                    System.Console.WriteLine($"Contact sync states loaded.");
                    System.Console.WriteLine();
                }

                // Returns the parsed file
                IEnumerable<ContactSyncState> contactSyncStates = string.IsNullOrWhiteSpace(contactSyncStatesFile) ? new List<ContactSyncState>() : JsonConvert.DeserializeObject<IEnumerable<ContactSyncState>>(contactSyncStatesFile);

                // Creates the services for synchronization
                using (SevDeskService sevDeskService = new SevDeskService(configuration.SevDeskApiToken))
                {
                    using (ExchangeService exchangeService = new ExchangeService(configuration.AzureActiveDirectoryTenant, configuration.AzureClientId, configuration.AzureClientSecret))
                    {
                        // Configures progress reporting
                        Progress<string> progress = new Progress<string>(status => System.Console.WriteLine(status));

                        // Synchronizes the contacts
                        SyncService syncService = new SyncService(sevDeskService, exchangeService);
                        contactSyncStates = await syncService.SynchronizeContactsAsnyc(contactSyncStates, configuration.SevDeskContactCategories, configuration.AzureActiveDirectoryUsers, progress);
                    }
                }

                // Saves the file
                System.Console.WriteLine();
                System.Console.WriteLine($"Saving contact sync states to file \"{configuration.ContactSyncStatesFileName}\"...");
                File.WriteAllText(configuration.ContactSyncStatesFileName, JsonConvert.SerializeObject(contactSyncStates));
                System.Console.WriteLine($"Contact sync states saved.");

                System.Console.WriteLine();
                System.Console.WriteLine($"Synchronizing contacts finished.");
                System.Console.WriteLine();
                System.Console.WriteLine();
            }
            catch (Exception e)
            {
                System.Console.WriteLine();
                throw new InvalidOperationException($"Could not synchronize contacts: {e.Message}", e);
            }
        }

        /// <summary>
        /// Starts the program.
        /// </summary>
        private static void Start()
        {
            System.Console.WriteLine("sevDesk Exchange Sync");
            System.Console.WriteLine();
            System.Console.WriteLine("This tool can be used to synchronize the sevDesk contacts with contacts list of Exchange Online accounts (being managed by Azure Active Directory).");
            System.Console.WriteLine();
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine();
            System.Console.WriteLine("* Start this tool without a command line argument to get into the interactive mode. In This mode, you have to configure your sevDesk account and the Azure AD credentials. At the end of the process a configuration file is created.");
            System.Console.WriteLine();
            System.Console.WriteLine("* Run the tool with the path to an existing configuration file in order to perform the actual synchronization. The tool exits after the synchronization, use a task scheduler to execute it from time to time.");
            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine();
        }

        /// <summary>
        /// Asks the user for the API token for sevDesk.
        /// </summary>
        /// <param name="configuration">The configuration of the synchronization process.</param>
        private static async Task AskForSevDeskApiTokenAsync(Configuration configuration)
        {
            System.Console.WriteLine("1. sevDesk - API Token");
            System.Console.WriteLine("You can get the API token for sevDesk in the user management. Simply click on a user and scroll down to the \"Token\" text field.");
            System.Console.WriteLine();

            // Gets the token from the user
            string sevDeskApiToken = null;
            do
            {
                System.Console.Write("sevDESK API token: ");
                sevDeskApiToken = System.Console.ReadLine();

                // Validates the input
                try
                {
                    System.Console.WriteLine("Validating API token...");
                    using (SevDeskService sevDeskService = new SevDeskService(sevDeskApiToken))
                    {
                        await sevDeskService.GetUsersAsync();
                    }
                    System.Console.WriteLine("API token is valid.");
                }
                catch (Exception e)
                {
                    System.Console.WriteLine($"The sevDESK API token is invalid: {e.Message}");
                    System.Console.WriteLine("Please try a different API token.");
                    System.Console.WriteLine();
                }

            }
            while (string.IsNullOrWhiteSpace(sevDeskApiToken));

            // Stores the API token
            configuration.SevDeskApiToken = sevDeskApiToken;
            System.Console.WriteLine();
            System.Console.WriteLine();
        }

        /// <summary>
        /// Asks the user for the contact categories that should be synchronized.
        /// </summary>
        /// <param name="configuration">The configuration of the synchronization process.</param>
        private static async Task AskForSevDeskContactCategoriesAsync(Configuration configuration)
        {
            System.Console.WriteLine("2. sevDesk - Select Categories for Contacts");
            System.Console.WriteLine("Please select the categories of the contacts that should be synchronized with the Exchange Online accounts. Leave the input blank if contacts of all categories should be synchronized; otherwise, provide the numbers of the selection separated by commas.");
            System.Console.WriteLine();

            // Gets the categories from the sevDesk API
            List<string> categoryIds = new List<string>();
            try
            {
                // Downloads all categories
                System.Console.WriteLine("Getting categories from sevDesk...");
                using (SevDeskService sevDeskService = new SevDeskService(configuration.SevDeskApiToken))
                {
                    List<Category> categories = new List<Category>();
                    CategoryList categoryList = null;
                    do
                    {
                        categoryList = await sevDeskService.GetCategoriesAsync(categories.Count);
                        categories.AddRange(categoryList);
                    }
                    while (categoryList.Count > categories.Count);

                    // Sorts out the categories
                    List<ContactCategory> contactCategories = categories.OfType<ContactCategory>().ToList();

                    // Prints out the categories
                    System.Console.WriteLine();
                    for (int i = 1; i <= contactCategories.Count; i++)
                        System.Console.WriteLine($"[{i}]\t{contactCategories[i - 1].Name}");

                    // Gets the list of categories
                    System.Console.WriteLine();
                    do
                    {
                        System.Console.Write("Categories: ");
                        string categoriesString = System.Console.ReadLine();

                        // Checks whether nothing has been provided
                        if (string.IsNullOrWhiteSpace(categoriesString))
                        {
                            categoryIds = null;
                            break;
                        }

                        // Cycles through the numbers
                        foreach (string categoryString in categoriesString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                        {
                            if (!int.TryParse(categoryString, out int number) || number < 1 || number > contactCategories.Count())
                            {
                                System.Console.WriteLine("Please provide a valid list of categories.");
                                System.Console.WriteLine();
                                continue;
                            }

                            // Adds the ID of the category
                            categoryIds.Add(contactCategories[number - 1].Id);
                        }
                    }
                    while (categoryIds != null && !categoryIds.Any());
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Categories could not be retrieved: {e.Message}");
                System.Console.WriteLine("Please try a different API token.");
                System.Console.WriteLine();
                System.Console.WriteLine();

                // Redirects the user to the first step
                configuration.SevDeskApiToken = null;
                await Program.AskForSevDeskApiTokenAsync(configuration);
                await Program.AskForSevDeskContactCategoriesAsync(configuration);
                return;
            }

            // Stores the category IDs
            configuration.SevDeskContactCategories = categoryIds;
            System.Console.WriteLine();
            System.Console.WriteLine();
        }

        /// <summary>
        /// Asks the user to create an Azure application.
        /// </summary>
        /// <param name="configuration">The configuration of the synchronization process.</param>
        private static async Task AskForAzureCredentialsAsync(Configuration configuration)
        {
            System.Console.WriteLine("3. Azure - Create Application");
            System.Console.WriteLine("Create a new application in Azure, which has the permissions to read and write the contacts of the members of the Azure Active Directory. You have to be administrator for the directory to complete these steps.");
            System.Console.WriteLine();

            // Gets settings of the application
            string tenant = null;
            string clientId = null;
            string clientSecret = null;
            do
            {
                System.Console.WriteLine("3.1. Go to the Azure Portal and navigate to the Azure Active Directory you want to synchronize with. Go to the properties tab and copy the following property:");
                System.Console.WriteLine();

                // Gets the tenant
                System.Console.Write("Tenant (\"Directory ID\"): ");
                tenant = System.Console.ReadLine();
                System.Console.WriteLine();

                System.Console.WriteLine("3.2. Go to the app registrations and create a new app. Use the application type \"Web App/Web API\" and a random sign-in URI.");
                System.Console.WriteLine();

                System.Console.Write("Press enter to continue...");
                System.Console.ReadLine();
                System.Console.WriteLine();

                System.Console.WriteLine("3.3. Navigate to the settings of the created app and copy the following property from the properties tab:");
                System.Console.WriteLine();

                // Gets the client ID
                System.Console.Write("Application ID: ");
                clientId = System.Console.ReadLine();
                System.Console.WriteLine();


                System.Console.WriteLine("3.4. Go to the API keys tab and create a new key. After saving the changes, you can copy the key.");
                System.Console.WriteLine();

                // Gets the client secret
                System.Console.Write("API key: ");
                clientSecret = System.Console.ReadLine();
                System.Console.WriteLine();

                System.Console.WriteLine("3.5. Navigate to the permissions tab and add a new one. Choose \"Microsoft Graph\" as API and add the application permission \"Read and write contacts in all mailboxes\" (Contacts.ReadWrite) and \"Read directory data\" (Directory.Read.All).");
                System.Console.WriteLine();

                System.Console.Write("Press enter to continue...");
                System.Console.ReadLine();
                System.Console.WriteLine();

                // Creates the service
                using (ExchangeService exchangeService = new ExchangeService(tenant, clientId, clientSecret))
                {
                    System.Console.WriteLine("3.6. Copy the following URL into your browser and grant admin content to the application:");
                    System.Console.Write(exchangeService.AdminConsentUri);
                    System.Console.WriteLine();

                    System.Console.Write("Press enter to continue...");
                    System.Console.ReadLine();
                    System.Console.WriteLine();

                    // Tests the provided credentials
                    try
                    {
                        System.Console.WriteLine("Validating Azure application settings...");
                        await exchangeService.GetUsersAsync();
                        System.Console.WriteLine("Azure application settings are valid.");
                    }
                    catch (Exception e)
                    {
                        clientId = null;
                        tenant = null;
                        clientSecret = null;
                        System.Console.WriteLine($"The Azure application settings are invalid: {e.Message}");
                        System.Console.WriteLine("Please try again.");
                        System.Console.WriteLine();
                    }
                }
            }
            while (string.IsNullOrWhiteSpace(tenant) || string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret));

            // Stores the credentials
            configuration.AzureClientId = clientId;
            configuration.AzureClientSecret = clientSecret;
            configuration.AzureActiveDirectoryTenant = tenant;
            System.Console.WriteLine();
            System.Console.WriteLine();
        }

        /// <summary>
        /// Asks the user for the users that should participate in the synchronization.
        /// </summary>
        /// <param name="configuration">The configuration of the synchronization process.</param>
        private static async Task AskForAzureActiveDirectoryUsersAsync(Configuration configuration)
        {
            System.Console.WriteLine("4. Azure Active Directory - Select Users");
            System.Console.WriteLine("Please select the users that should participate in the synchronization. Leave the input blank if all users should be synchronized; otherwise, provide the numbers of the selection separated by commas.");
            System.Console.WriteLine();

            // Gets the users from the API
            List<string> userIds = new List<string>();
            try
            {
                // Downloads all users
                System.Console.WriteLine("Getting users from Azure Active Directory...");
                using (ExchangeService exchangeService = new ExchangeService(configuration.AzureActiveDirectoryTenant, configuration.AzureClientId, configuration.AzureClientSecret))
                {
                    List<User> users = new List<User>();
                    SkipToken skipToken = null;
                    do
                    {
                        UserList userList = await exchangeService.GetUsersAsync(skipToken);
                        skipToken = userList.SkipToken;
                        users.AddRange(userList);
                    }
                    while (skipToken != null);

                    // Prints out the users
                    System.Console.WriteLine();
                    for (int i = 1; i <= users.Count(); i++)
                        System.Console.WriteLine($"[{i}]\t{users.ElementAt(i - 1).UserPrincipalName}");

                    // Gets the list of users
                    System.Console.WriteLine();
                    do
                    {
                        System.Console.Write("Users: ");
                        string usersString = System.Console.ReadLine();

                        // Checks whether nothing has been provided
                        if (string.IsNullOrWhiteSpace(usersString))
                        {
                            userIds = null;
                            break;
                        }

                        // Cycles through the numbers
                        foreach (string userString in usersString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList())
                        {
                            if (!int.TryParse(userString, out int number) || number < 1 || number > users.Count())
                            {
                                System.Console.WriteLine("Please provide a valid list of users.");
                                System.Console.WriteLine();
                                continue;
                            }

                            // Adds the ID of the category
                            userIds.Add(users.ElementAt(number - 1).Id);
                        }
                    }
                    while (userIds != null && !userIds.Any());
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine($"Users could not be retrieved: {e.Message}");
                System.Console.WriteLine("Please reenter the Azure application credentials.");
                System.Console.WriteLine();
                System.Console.WriteLine();

                // Redirects the user to the third step
                configuration.AzureActiveDirectoryTenant = null;
                configuration.AzureClientId = null;
                configuration.AzureClientSecret = null;
                await Program.AskForAzureCredentialsAsync(configuration);
                await Program.AskForAzureActiveDirectoryUsersAsync(configuration);
                return;
            }

            // Stores the user IDs
            configuration.AzureActiveDirectoryUsers = userIds;
            System.Console.WriteLine();
            System.Console.WriteLine();
        }

        /// <summary>
        /// Asks the user for the file name of the contact sync states.
        /// </summary>
        /// <param name="configuration">The configuration of the synchronization process.</param>
        private static void AskForContactSyncStatesFileName(Configuration configuration)
        {
            System.Console.WriteLine("5. Files");
            System.Console.WriteLine("Please provide a file name for a file that contains the state of the synchronization.");
            System.Console.WriteLine();

            // Gets the file name from the user
            string contactSyncStatesFileName = null;
            do
            {
                System.Console.Write("File name: ");
                contactSyncStatesFileName = System.Console.ReadLine();

                // Validates the input
                if (string.IsNullOrWhiteSpace(contactSyncStatesFileName))
                {
                    System.Console.WriteLine("Please provide a valid file name.");
                    System.Console.WriteLine();
                }
            }
            while (string.IsNullOrWhiteSpace(contactSyncStatesFileName));

            // Stores the API token
            configuration.ContactSyncStatesFileName = contactSyncStatesFileName;
            System.Console.WriteLine();
            System.Console.WriteLine();
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args) => Program.MainAsync(args).Wait();

        /// <summary>
        /// The asynchrounous entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static async Task MainAsync(string[] args)
        {
            // Initializes a new Configuration
            Configuration configuration = new Configuration();

            // Prints out the instructions
            Program.Start();

            // Checks whether the configuration should be loaded from file
            if (args.Length > 0)
            {

                // Tries to execute the application, if anything fails, the console application is exited
                try
                {
                    // Loads the configuration
                    configuration = Program.LoadConfiguration(args[0]);

                    // Starts the synchronization
                    await Program.SynchronizeContactsAsync(configuration);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                }
                return;
            }

            // Goes through the interactive mode process
            await Program.AskForSevDeskApiTokenAsync(configuration);
            await Program.AskForSevDeskContactCategoriesAsync(configuration);
            await Program.AskForAzureCredentialsAsync(configuration);
            await Program.AskForAzureActiveDirectoryUsersAsync(configuration);
            Program.AskForContactSyncStatesFileName(configuration);
            Program.SaveConfiguration(configuration);
        }

        #endregion
    }
}