// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for license information.
// Code generated by Microsoft (R) AutoRest Code Generator.
// Changes may cause incorrect behavior and will be lost if the code is regenerated.

namespace Microsoft.Azure.PowerShell.Cmdlets.MySql.Support
{

    /// <summary>
    /// Specifies the state of the Advanced Threat Protection, whether it is enabled or disabled on the server.
    /// </summary>
    public partial struct AdvancedThreatProtectionState :
        System.IEquatable<AdvancedThreatProtectionState>
    {
        public static Microsoft.Azure.PowerShell.Cmdlets.MySql.Support.AdvancedThreatProtectionState Disabled = @"Disabled";

        public static Microsoft.Azure.PowerShell.Cmdlets.MySql.Support.AdvancedThreatProtectionState Enabled = @"Enabled";

        /// <summary>
        /// the value for an instance of the <see cref="AdvancedThreatProtectionState" /> Enum.
        /// </summary>
        private string _value { get; set; }

        /// <summary>
        /// Creates an instance of the <see cref="AdvancedThreatProtectionState"/> Enum class.
        /// </summary>
        /// <param name="underlyingValue">the value to create an instance for.</param>
        private AdvancedThreatProtectionState(string underlyingValue)
        {
            this._value = underlyingValue;
        }

        /// <summary>Conversion from arbitrary object to AdvancedThreatProtectionState</summary>
        /// <param name="value">the value to convert to an instance of <see cref="AdvancedThreatProtectionState" />.</param>
        internal static object CreateFrom(object value)
        {
            return new AdvancedThreatProtectionState(global::System.Convert.ToString(value));
        }

        /// <summary>Compares values of enum type AdvancedThreatProtectionState</summary>
        /// <param name="e">the value to compare against this instance.</param>
        /// <returns><c>true</c> if the two instances are equal to the same value</returns>
        public bool Equals(Microsoft.Azure.PowerShell.Cmdlets.MySql.Support.AdvancedThreatProtectionState e)
        {
            return _value.Equals(e._value);
        }

        /// <summary>
        /// Compares values of enum type AdvancedThreatProtectionState (override for Object)
        /// </summary>
        /// <param name="obj">the value to compare against this instance.</param>
        /// <returns><c>true</c> if the two instances are equal to the same value</returns>
        public override bool Equals(object obj)
        {
            return obj is AdvancedThreatProtectionState && Equals((AdvancedThreatProtectionState)obj);
        }

        /// <summary>Returns hashCode for enum AdvancedThreatProtectionState</summary>
        /// <returns>The hashCode of the value</returns>
        public override int GetHashCode()
        {
            return this._value.GetHashCode();
        }

        /// <summary>Returns string representation for AdvancedThreatProtectionState</summary>
        /// <returns>A string for this value.</returns>
        public override string ToString()
        {
            return this._value;
        }

        /// <summary>Implicit operator to convert string to AdvancedThreatProtectionState</summary>
        /// <param name="value">the value to convert to an instance of <see cref="AdvancedThreatProtectionState" />.</param>

        public static implicit operator AdvancedThreatProtectionState(string value)
        {
            return new AdvancedThreatProtectionState(value);
        }

        /// <summary>Implicit operator to convert AdvancedThreatProtectionState to string</summary>
        /// <param name="e">the value to convert to an instance of <see cref="AdvancedThreatProtectionState" />.</param>

        public static implicit operator string(Microsoft.Azure.PowerShell.Cmdlets.MySql.Support.AdvancedThreatProtectionState e)
        {
            return e._value;
        }

        /// <summary>Overriding != operator for enum AdvancedThreatProtectionState</summary>
        /// <param name="e1">the value to compare against <paramref name="e2" /></param>
        /// <param name="e2">the value to compare against <paramref name="e1" /></param>
        /// <returns><c>true</c> if the two instances are not equal to the same value</returns>
        public static bool operator !=(Microsoft.Azure.PowerShell.Cmdlets.MySql.Support.AdvancedThreatProtectionState e1, Microsoft.Azure.PowerShell.Cmdlets.MySql.Support.AdvancedThreatProtectionState e2)
        {
            return !e2.Equals(e1);
        }

        /// <summary>Overriding == operator for enum AdvancedThreatProtectionState</summary>
        /// <param name="e1">the value to compare against <paramref name="e2" /></param>
        /// <param name="e2">the value to compare against <paramref name="e1" /></param>
        /// <returns><c>true</c> if the two instances are equal to the same value</returns>
        public static bool operator ==(Microsoft.Azure.PowerShell.Cmdlets.MySql.Support.AdvancedThreatProtectionState e1, Microsoft.Azure.PowerShell.Cmdlets.MySql.Support.AdvancedThreatProtectionState e2)
        {
            return e2.Equals(e1);
        }
    }
}