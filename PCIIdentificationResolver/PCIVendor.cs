using System;
using System.Collections.Generic;
using System.Linq;

namespace PCIIdentificationResolver
{
    /// <summary>
    ///     Represents a PCI device vendor
    /// </summary>
    [Serializable]
    public class PCIVendor : IEquatable<PCIVendor>
    {
        private readonly List<PCIDevice> _devices = new List<PCIDevice>();

        internal PCIVendor(string vendorInfo)
        {
            var parts = vendorInfo.Split(new[] {"  "}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                throw new ArgumentException(@"Invalid vendor info format.", nameof(vendorInfo));
            }

            VendorId = Convert.ToUInt16(parts[0], 16);
            VendorName = string.Join(" ", parts.Skip(1));
        }

        public IEnumerable<PCIDevice> Devices
        {
            get => _devices.AsReadOnly();
        }

        /// <summary>
        ///     Gets the vendor identification number.
        /// </summary>

        public ushort VendorId { get; }

        /// <summary>
        ///     Gets the vendor friendly name.
        /// </summary>
        public string VendorName { get; }

        /// <inheritdoc />
        public bool Equals(PCIVendor other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return VendorId == other.VendorId;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIVendor" /> for equality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIVendor" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIVendor" /> class.</param>
        /// <returns>true if instances of <see cref="PCIVendor" /> class are equal; otherwise false.</returns>
        public static bool operator ==(PCIVendor left, PCIVendor right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIVendor" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIVendor" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIVendor" /> class.</param>
        /// <returns>true if instances of <see cref="PCIVendor" /> class are not equal; otherwise false.</returns>
        public static bool operator !=(PCIVendor left, PCIVendor right)
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

            return Equals(obj as PCIVendor);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return VendorId.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{VendorName} ({VendorId:X4})";
        }

        internal void AddDevice(PCIDevice device)
        {
            _devices.Add(device);
        }
    }
}