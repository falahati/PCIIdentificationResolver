using System;
using System.Collections.Generic;
using System.Linq;

namespace PCIIdentificationResolver
{
    /// <summary>
    ///     Represents a PCI device
    /// </summary>
    [Serializable]
    public class PCIDevice : IEquatable<PCIDevice>
    {
        private readonly List<PCISubSystem> _subDevices = new List<PCISubSystem>();

        internal PCIDevice(PCIVendor parentVendor, string deviceInfo)
        {
            ParentVendor = parentVendor;
            var parts = deviceInfo.Split(new[] {"  "}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                throw new ArgumentException(@"Invalid device info format.", nameof(deviceInfo));
            }

            DeviceId = Convert.ToUInt16(parts[0], 16);
            DeviceName = string.Join(" ", parts.Skip(1));
        }

        /// <summary>
        ///     Gets the device identification number.
        /// </summary>
        public ushort DeviceId { get; }

        /// <summary>
        ///     Gets the device friendly name.
        /// </summary>
        public string DeviceName { get; }

        /// <summary>
        ///     Gets the device vendor.
        /// </summary>
        public PCIVendor ParentVendor { get; }

        /// <summary>
        ///     Gets the device known subsystems.
        /// </summary>
        public IEnumerable<PCISubSystem> SubSystems
        {
            get => _subDevices.AsReadOnly();
        }

        /// <inheritdoc />
        public bool Equals(PCIDevice other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return DeviceId == other.DeviceId && ParentVendor == other.ParentVendor;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIDevice" /> for equality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIDevice" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIDevice" /> class.</param>
        /// <returns>true if instances of <see cref="PCIDevice" /> class are equal; otherwise false.</returns>
        public static bool operator ==(PCIDevice left, PCIDevice right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIDevice" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIDevice" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIDevice" /> class.</param>
        /// <returns>true if instances of <see cref="PCIDevice" /> class are not equal; otherwise false.</returns>
        public static bool operator !=(PCIDevice left, PCIDevice right)
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

            return Equals(obj as PCIDevice);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (DeviceId.GetHashCode() * 397) ^ (ParentVendor != null ? ParentVendor.GetHashCode() : 0);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ParentVendor != null
                ? $"{ParentVendor.VendorName} {DeviceName} ({DeviceId:X4})"
                : $"{DeviceName} ({DeviceId:X4})";
        }

        internal void AddSubSystem(PCISubSystem subSystem)
        {
            _subDevices.Add(subSystem);
        }
    }
}