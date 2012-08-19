// Copyright (c) ThunderMain Ltd. All rights reserved. See LICENSE in the project root for license information.

namespace ThunderMain.GedcomReader
{
    internal struct GedcomAttribute
    {
        private readonly GedcomAttributeType _type;
        private readonly char[] _attributeValue;

        /// <summary>
        /// Creates a new attribute of the given type (REF or ID) with the
        /// given string value.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="attributeValue"></param>
        public GedcomAttribute(GedcomAttributeType type, char[] attributeValue)
        {
            _type = type;
            _attributeValue = attributeValue;
        }

        /// <summary>
        /// Gets the name of this attribute. GEDML only has 2 attributes -
        /// REF and ID.
        /// </summary>
        public string Name
        {
            get
            {
                switch (_type)
                {
                    case GedcomAttributeType.Ref:
                        return "REF";
                    case GedcomAttributeType.Id:
                        return "ID";
                    default:
                        return "UNKNOWN";
                }
            }
        }

        /// <summary>Gets the value of the attribute</summary>
        public string Value
        {
            get { return new string(_attributeValue); }
        }
    }
}