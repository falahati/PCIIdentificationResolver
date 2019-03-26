using System;

namespace PCIIdentificationResolver
{
    /// <summary>
    ///     Holds information regarding a PCI address subsystem
    /// </summary>
    [Serializable]
    public class PCIAddressSubSystem : IEquatable<PCIAddressSubSystem>
    {
        internal const string SubSystemIdentifier = "SUBSYS_";

        /// <summary>
        ///     Creates an instance of <see cref="PCIAddressSubSystem" /> class.
        /// </summary>
        /// <param name="vendorId">The subsystem vendor identification number.</param>
        /// <param name="deviceId">The subsystem device identification number.</param>
        public PCIAddressSubSystem(ushort vendorId, ushort deviceId)
        {
            VendorId = vendorId;
            DeviceId = deviceId;
        }

        /// <summary>
        ///     Gets the subsystem device identification number
        /// </summary>
        public ushort DeviceId { get; }

        /// <summary>
        ///     Gets the subsystem device identification number
        /// </summary>
        public ushort VendorId { get; }

        /// <inheritdoc />
        public bool Equals(PCIAddressSubSystem other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return VendorId == other.VendorId && DeviceId == other.DeviceId;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIAddressSubSystem" /> for equality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIAddressSubSystem" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIAddressSubSystem" /> class.</param>
        /// <returns>true if instances of <see cref="PCIAddressSubSystem" /> class are equal; otherwise false.</returns>
        public static bool operator ==(PCIAddressSubSystem left, PCIAddressSubSystem right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIAddressSubSystem" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIAddressSubSystem" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIAddressSubSystem" /> class.</param>
        /// <returns>true if instances of <see cref="PCIAddressSubSystem" /> class are not equal; otherwise false.</returns>
        public static bool operator !=(PCIAddressSubSystem left, PCIAddressSubSystem right)
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


            return Equals(obj as PCIAddressSubSystem);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (VendorId.GetHashCode() * 397) ^ DeviceId.GetHashCode();
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{SubSystemIdentifier}_{DeviceId:X4}{VendorId:X4}";
        }
    }
}