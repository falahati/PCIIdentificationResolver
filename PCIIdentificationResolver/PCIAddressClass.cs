using System;

namespace PCIIdentificationResolver
{
    /// <summary>
    ///     Holds information regarding a PCI address device class
    /// </summary>
    [Serializable]
    public class PCIAddressClass : IEquatable<PCIAddressClass>
    {
        internal const string ClassIdentifier = "CC_";

        /// <summary>
        ///     Creates an instance of <see cref="PCIAddressClass" /> class.
        /// </summary>
        /// <param name="baseClassId">The device base class identification number.</param>
        /// <param name="subClassId">The device sub class identification number.</param>
        public PCIAddressClass(byte baseClassId, byte subClassId)
        {
            BaseClassId = baseClassId;
            SubClassId = subClassId;
        }

        /// <summary>
        ///     Creates an instance of <see cref="PCIAddressClass" /> class.
        /// </summary>
        /// <param name="baseClassId">The device base class identification number.</param>
        /// <param name="subClassId">The device sub class identification number.</param>
        /// <param name="programingInterfaceId">The sub class programing interface identification number.</param>
        public PCIAddressClass(byte baseClassId, byte subClassId, byte programingInterfaceId) : this(baseClassId,
            subClassId)
        {
            ProgramingInterfaceId = programingInterfaceId;
        }

        /// <summary>
        ///     Gets the device base class identification number.
        /// </summary>
        public byte BaseClassId { get; }

        /// <summary>
        ///     Gets the sub class programing interface identification number.
        /// </summary>
        public byte? ProgramingInterfaceId { get; }

        /// <summary>
        ///     Gets the device sub class identification number.
        /// </summary>
        public byte SubClassId { get; }

        /// <inheritdoc />
        public bool Equals(PCIAddressClass other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return BaseClassId == other.BaseClassId &&
                   SubClassId == other.SubClassId &&
                   ProgramingInterfaceId == other.ProgramingInterfaceId;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIAddressClass" /> for equality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIAddressClass" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIAddressClass" /> class.</param>
        /// <returns>true if instances of <see cref="PCIAddressClass" /> class are equal; otherwise false.</returns>
        public static bool operator ==(PCIAddressClass left, PCIAddressClass right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIAddressClass" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIAddressClass" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIAddressClass" /> class.</param>
        /// <returns>true if instances of <see cref="PCIAddressClass" /> class are not equal; otherwise false.</returns>
        public static bool operator !=(PCIAddressClass left, PCIAddressClass right)
        {
            return !(left == right);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            return Equals(obj as PCIAddressClass);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = BaseClassId.GetHashCode();
                hashCode = (hashCode * 397) ^ SubClassId.GetHashCode();
                hashCode = (hashCode * 397) ^ ProgramingInterfaceId.GetHashCode();

                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (ProgramingInterfaceId != null)
            {
                return $"{ClassIdentifier}_{BaseClassId:X2}{SubClassId:X2}{ProgramingInterfaceId.Value:X2}";
            }

            return $"{ClassIdentifier}_{BaseClassId:X2}{SubClassId:X2}";
        }
    }
}