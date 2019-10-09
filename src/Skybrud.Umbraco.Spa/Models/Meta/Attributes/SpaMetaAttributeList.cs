using System;
using System.Collections;
using System.Collections.Generic;

namespace Skybrud.Umbraco.Spa.Models.Meta.Attributes  {

    /// <summary>
    /// Class representing a list of attributes of an HTML element.
    /// </summary>
    public class SpaMetaAttributeList : IEnumerable<KeyValuePair<string, string>> {

        private readonly Dictionary<string, string> _attributes = new Dictionary<string, string>();

        #region Properties

        /// <summary>
        /// Gets the amount of attributes in the list.
        /// </summary>
        public int Count => _attributes.Count;

        /// <summary>
        /// Gets or sets the value of the attribute with the specified <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        public string this[string name] {
            get => _attributes.TryGetValue(name, out string value) ? value : null;
            set => _attributes[name] = value;
        }

        /// <summary>
        /// Gets a collection of all the attribute names in the list.
        /// </summary>
        public Dictionary<string, string>.KeyCollection Keys => _attributes.Keys;

        #endregion

        #region Member methods

        /// <summary>
        /// Gets whether an attribute with the specified <paramref name="name"/> exists in the list.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <returns><c>true</c> if the attribute exists; otherwise <c>false</c>.</returns>
        public bool ContainsKey(string name) {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            return _attributes.ContainsKey(name);
        }

        /// <summary>
        /// Adds a new attribute with the specified <paramref name="name"/> and <paramref name="value"/>. If an
        /// attribute with the same name already exists, the existing attribute will be overwritten.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public void Add(string name, string value) {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            _attributes[name] = value;
        }

        /// <summary>
        /// Adds a new attribute with the specified <paramref name="name"/> and <paramref name="value"/>.
        /// </summary>
        /// <param name="name">The name of the attribute.</param>
        /// <param name="value">The value of the attribute.</param>
        public bool TryGetValue(string name, out string value) {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            return _attributes.TryGetValue(name, out value);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the a <see cref="SpaMetaAttributeList"/>.
        /// </summary>
        /// <returns>A <see cref="Dictionary{TKey,TValue}.Enumerator"/> structure for the <see cref="SpaMetaAttributeList"/>.</returns>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
            return _attributes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        #endregion

    }

}