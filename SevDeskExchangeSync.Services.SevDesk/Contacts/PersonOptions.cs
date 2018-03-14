
#region Using Directives

using System;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Contacts
{
    /// <summary>
    /// Represents a the options that are used to create or update a person.
    /// </summary>
    public sealed class PersonOptions : ContactOptions
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the first name of the person.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the person.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the position of the person in the company.
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets the academic title of the person.
        /// </summary>
        public string AcademicTitle { get; set; }

        /// <summary>
        /// Gets or sets the gender of the person.
        /// </summary>
        public Gender Gender { get; set; }

        /// <summary>
        /// Gets or sets the birthday of the person.
        /// </summary>
        public Nullable<DateTime> Birthday { get; set; }

        /// <summary>
        /// Gets or sets the ID of the organization to which the person belongs.
        /// </summary>
        public string OrganizationId { get; set; }

        #endregion
    }
}