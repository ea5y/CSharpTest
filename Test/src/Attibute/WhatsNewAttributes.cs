using System;

namespace WhatsNewAttributes
{
    [AttributeUsageAttribute(
            AttributeTargets.Class | AttributeTargets.Method,
            AllowMultiple = true, Inherited = false)]
    public class LastModifiedAttribute : Attribute
    {
        private readonly DateTime _dateModified;
        private readonly string _changes;

        public LastModifiedAttribute(string dateModified, string changes)
        {
            this._dateModified = DateTime.Parse(dateModified);
            _changes = changes;
        }

        public DateTime DateModified
        {
            get { return this._dateModified; }
        }

        public string Changes
        {
            get { return this._changes; }
        }

        public string Issues { get; set; }

    }

    [AttributeUsageAttribute(AttributeTargets.Assembly)]
    public class SupportsWhatNewAttribute : Attribute
    {
    }
}
