using System;
using System.Collections.Generic;
using System.Linq;

namespace PCIIdentificationResolver
{
    /// <summary>
    ///     Represents a PCI device subsystem
    /// </summary>
    [Serializable]
    public class PCISubSystem : IEquatable<PCISubSystem>
    {
        internal PCISubSystem(PCIVendor parentVendor, PCIDevice parentDevice, string subSystemInfo)
        {
            ParentVendor = parentVendor;
            ParentDevice = parentDevice;
            var parts = subSystemInfo.Split(new[] {"  "}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                throw new ArgumentException(@"Invalid sub device info format.", nameof(subSystemInfo));
            }

            var idParts = parts[0].Split(' ');

            if (idParts.Length < 2)
            {
                throw new ArgumentException(@"Invalid sub device info format.", nameof(subSystemInfo));
            }

            VendorId = Convert.ToUInt16(idParts[0], 16);
            DeviceId = Convert.ToUInt16(idParts[1], 16);

            SubSystemName = string.Join(" ", parts.Skip(1));
        }

        /// <summary>
        ///     Gets the subsystem device.
        /// </summary>
        public PCIDevice Device
        {
            get => PCIIdentificationDatabase.GetDevice(VendorId, DeviceId);
        }

        /// <summary>
        ///     Gets the subsystem device identification number.
        /// </summary>
        public ushort DeviceId { get; }

        /// <summary>
        ///     Gets the subsystem parent device.
        /// </summary>
        public PCIDevice ParentDevice { get; }

        /// <summary>
        ///     Gets the subsystem parent device vendor.
        /// </summary>
        public PCIVendor ParentVendor { get; }

        /// <summary>
        ///     Gets the subsystem friendly name.
        /// </summary>
        public string SubSystemName { get; }

        /// <summary>
        ///     Gets the subsystem vendor.
        /// </summary>
        public PCIVendor Vendor
        {
            get => PCIIdentificationDatabase.GetVendor(VendorId);
        }

        /// <summary>
        ///     Gets the subsystem vendor identification number.
        /// </summary>
        public ushort VendorId { get; }

        /// <inheritdoc />
        public bool Equals(PCISubSystem other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return DeviceId == other.DeviceId && ParentDevice == other.ParentDevice && VendorId == other.VendorId;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCISubSystem" /> for equality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCISubSystem" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCISubSystem" /> class.</param>
        /// <returns>true if instances of <see cref="PCISubSystem" /> class are equal; otherwise false.</returns>
        public static bool operator ==(PCISubSystem left, PCISubSystem right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCISubSystem" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCISubSystem" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCISubSystem" /> class.</param>
        /// <returns>true if instances of <see cref="PCISubSystem" /> class are not equal; otherwise false.</returns>
        public static bool operator !=(PCISubSystem left, PCISubSystem right)
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

            return Equals(obj as PCISubSystem);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DeviceId.GetHashCode();
                hashCode = (hashCode * 397) ^ (ParentDevice != null ? ParentDevice.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ VendorId.GetHashCode();

                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var parentParts = new List<string>();

            if (ParentVendor != null)
            {
                parentParts.Add(ParentVendor.VendorName);
            }

            if (ParentDevice != null)
            {
                parentParts.Add(ParentDevice.DeviceName);
            }

            var subSystemParts = new List<string>();

            var vendor = Vendor;

            if (vendor != null)
            {
                subSystemParts.Add(vendor.VendorName);
            }

            var device = Device;

            if (device != null)
            {
                subSystemParts.Add(device.DeviceName);
            }

            return
                $"{string.Join(" ", subSystemParts)} [{SubSystemName}] ({DeviceId:X4}{VendorId:X4}) @ {string.Join(" ", parentParts)}"
                    .Trim();
        }
    }
}