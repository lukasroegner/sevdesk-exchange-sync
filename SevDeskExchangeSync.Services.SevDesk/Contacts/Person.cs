
#region Using Directives

using System;

#endregion

namespace SevDeskExchangeSync.Services.SevDesk.Contacts
{
    /// <summary>
    /// Represents a person in the sevDesk web application.
    /// </summary>
    public sealed class Person : Contact
    {
        #region Constructors

        /// <summary>
        /// Initializes a new <see cref="Person" /> instance.
        /// </summary>
        internal Person() { }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the first name of the person.
        /// </summary>
        public string FirstName { get; internal set; }

        /// <summary>
        /// Gets the last name of the person.
        /// </summary>
        public string LastName { get; internal set; }

        /// <summary>
        /// Gets the position of the person in the company.
        /// </summary>
        public string Position { get; internal set; }

        /// <summary>
        /// Gets the academic title of the person.
        /// </summary>
        public string AcademicTitle { get; internal set; }

        /// <summary>
        /// Gets the gender of the person.
        /// </summary>
        public Gender Gender { get; internal set; }

        /// <summary>
        /// Gets the birthday of the person.
        /// </summary>
        public Nullable<DateTime> Birthday { get; internal set; }

        /// <summary>
        /// Gets the organization to which the person belongs.
        /// </summary>
        public Organization Organization { get; internal set; }

        #endregion
    }
}