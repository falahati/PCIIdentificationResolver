using System;
using System.Collections.Generic;

namespace PCIIdentificationResolver
{
    /// <summary>
    ///     Represents a PCI hardware or compatible address
    /// </summary>
    [Serializable]
    public class PCIAddress : IEquatable<PCIAddress>
    {
        private const string DeviceIdentifier = "DEV_";
        private const string DeviceTypeIdentifier = "DT_";
        private const string RevisionIdentifier = "REV_";
        private const string VendorIdentifier = "VEN_";

        /// <summary>
        ///     Creates a new instance of <see cref="PCIAddress" /> class.
        /// </summary>
        /// <param name="vendorId">The device vendor identification number.</param>
        /// <param name="deviceId">The device identification number.</param>
        public PCIAddress(ushort vendorId, ushort deviceId) : this(vendorId, deviceId, null, null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PCIAddress" /> class.
        /// </summary>
        /// <param name="vendorId">The device vendor identification number.</param>
        /// <param name="deviceId">The device identification number.</param>
        /// <param name="subSystem">The PCI address subsystem information.</param>
        public PCIAddress(ushort vendorId, ushort deviceId, PCIAddressSubSystem subSystem) : this(vendorId, deviceId,
            subSystem, null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PCIAddress" /> class.
        /// </summary>
        /// <param name="vendorId">The device vendor identification number.</param>
        /// <param name="deviceId">The device identification number.</param>
        /// <param name="addressClass">The PCI address class information.</param>
        public PCIAddress(ushort vendorId, ushort deviceId, PCIAddressClass addressClass) : this(vendorId, deviceId,
            null, addressClass)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PCIAddress" /> class.
        /// </summary>
        /// <param name="vendorId">The device vendor identification number.</param>
        /// <param name="deviceId">The device identification number.</param>
        /// <param name="subSystem">The PCI address subsystem information.</param>
        /// <param name="addressClass">The PCI address class information.</param>
        // ReSharper disable once TooManyDependencies
        public PCIAddress(
            ushort vendorId,
            ushort deviceId,
            PCIAddressSubSystem subSystem,
            PCIAddressClass addressClass) : this(
            vendorId, deviceId, subSystem, addressClass, null, null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="PCIAddress" /> class.
        /// </summary>
        /// <param name="vendorId">The device vendor identification number.</param>
        /// <param name="deviceId">The device identification number.</param>
        /// <param name="subSystem">The PCI address subsystem information.</param>
        /// <param name="addressClass">The PCI address class information.</param>
        /// <param name="revision">The device revision.</param>
        /// <param name="deviceType">The device type.</param>
        // ReSharper disable once TooManyDependencies
        public PCIAddress(
            ushort vendorId,
            ushort deviceId,
            PCIAddressSubSystem subSystem,
            PCIAddressClass addressClass,
            byte? revision,
            ushort? deviceType)
        {
            VendorId = vendorId;
            DeviceId = deviceId;
            SubSystem = subSystem;
            Class = addressClass;
            Revision = revision;
            DeviceType = deviceType;
        }

        /// <summary>
        ///     Gets the PCI address class information.
        /// </summary>
        public PCIAddressClass Class { get; }

        /// <summary>
        ///     Gets the device identification number.
        /// </summary>
        public ushort DeviceId { get; }

        /// <summary>
        ///     Gets the device type
        /// </summary>
        public ushort? DeviceType { get; }

        /// <summary>
        ///     Gets the device revision
        /// </summary>
        public byte? Revision { get; }

        /// <summary>
        ///     Gets the PCI address sub system information
        /// </summary>
        public PCIAddressSubSystem SubSystem { get; }

        /// <summary>
        ///     Gets the device vendor identification number
        /// </summary>
        public ushort VendorId { get; }

        /// <inheritdoc />
        public bool Equals(PCIAddress other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return DeviceId == other.DeviceId &&
                   Revision == other.Revision &&
                   SubSystem == other.SubSystem &&
                   Class == other.Class &&
                   DeviceType == other.DeviceType &&
                   VendorId == other.VendorId;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIAddress" /> for equality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIAddress" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIAddress" /> class.</param>
        /// <returns>true if instances of <see cref="PCIAddress" /> class are equal; otherwise false.</returns>
        public static bool operator ==(PCIAddress left, PCIAddress right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIAddress" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIAddress" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIAddress" /> class.</param>
        /// <returns>true if instances of <see cref="PCIAddress" /> class are not equal; otherwise false.</returns>
        public static bool operator !=(PCIAddress left, PCIAddress right)
        {
            return !(left == right);
        }

        /// <summary>
        ///     Parses an string representing a PCI address and return a new instance of <see cref="PCIAddress" /> class.
        /// </summary>
        /// <param name="address">The PCI address represented as an string.</param>
        /// <returns>The newly created instance of <see cref="PCIAddress" />.</returns>
        // ReSharper disable once ExcessiveIndentation
        public static PCIAddress Parse(string address)
        {
            ushort? vendorId = null;
            ushort? deviceId = null;
            PCIAddressSubSystem subSystem = null;
            PCIAddressClass addressClass = null;
            byte? revision = null;
            ushort? deviceType = null;

            var parts = address.ToUpper().Split(new[] {"&", "\\", "/"}, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                if (part.StartsWith(VendorIdentifier))
                {
                    var vendorParts = part.Split('_');

                    if (vendorParts.Length == 2 && vendorParts[1].Length == 4)
                    {
                        vendorId = Convert.ToUInt16(vendorParts[1], 16);
                    }
                }
                else if (part.StartsWith(DeviceIdentifier))
                {
                    var deviceParts = part.Split('_');

                    if (deviceParts.Length == 2 && deviceParts[1].Length == 4)
                    {
                        deviceId = Convert.ToUInt16(deviceParts[1], 16);
                    }
                }
                else if (part.StartsWith(PCIAddressSubSystem.SubSystemIdentifier))
                {
                    var subSystemParts = part.Split('_');

                    if (subSystemParts.Length == 2 && subSystemParts[1].Length == 8)
                    {
                        var subSystemDeviceId = Convert.ToUInt16(subSystemParts[1].Substring(0, 4), 16);
                        var subSystemVendorId = Convert.ToUInt16(subSystemParts[1].Substring(4), 16);
                        subSystem = new PCIAddressSubSystem(subSystemVendorId, subSystemDeviceId);
                    }
                }
                else if (part.StartsWith(PCIAddressClass.ClassIdentifier))
                {
                    var classParts = part.Split('_');

                    if (classParts.Length == 2 && classParts[1].Length >= 4)
                    {
                        var baseClassCode = Convert.ToByte(classParts[1].Substring(0, 2), 16);
                        var subClassCode = Convert.ToByte(classParts[1].Substring(2), 16);

                        if (classParts[1].Length == 6)
                        {
                            var interfaceCode = Convert.ToByte(classParts[1].Substring(4), 16);
                            addressClass = new PCIAddressClass(baseClassCode, subClassCode, interfaceCode);
                        }
                        else
                        {
                            addressClass = new PCIAddressClass(baseClassCode, subClassCode);
                        }
                    }
                }
                else if (part.StartsWith(RevisionIdentifier))
                {
                    var revisionParts = part.Split('_');

                    if (revisionParts.Length == 2 && revisionParts[1].Length == 2)
                    {
                        revision = Convert.ToByte(revisionParts[1], 16);
                    }
                }
                else if (part.StartsWith(DeviceTypeIdentifier))
                {
                    var deviceTypeParts = part.Split('_');

                    if (deviceTypeParts.Length == 2 && deviceTypeParts[1].Length == 4)
                    {
                        deviceType = Convert.ToUInt16(deviceTypeParts[1], 16);
                    }
                }
            }

            if (vendorId == null || deviceId == null)
            {
                throw new FormatException("Invalid address format provided.");
            }

            return new PCIAddress(vendorId.Value, deviceId.Value, subSystem, addressClass, revision, deviceType);
        }

        /// <summary>
        ///     Tries to parse an string representing a PCI address and return a new instance of <see cref="PCIAddress" /> class.
        /// </summary>
        /// <param name="address">The PCI address represented as an string.</param>
        /// <param name="pciAddress">The newly created instance of <see cref="PCIAddress" />.</param>
        /// <returns>true if parsed successfully; otherwise false</returns>
        public static bool TryParse(string address, out PCIAddress pciAddress)
        {
            try
            {
                pciAddress = Parse(address);

                return true;
            }
            catch
            {
                pciAddress = null;

                return false;
            }
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

            return Equals(obj as PCIAddress);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DeviceId.GetHashCode();
                hashCode = (hashCode * 397) ^ Revision.GetHashCode();
                hashCode = (hashCode * 397) ^ SubSystem.GetHashCode();
                hashCode = (hashCode * 397) ^ Class.GetHashCode();
                hashCode = (hashCode * 397) ^ DeviceType.GetHashCode();
                hashCode = (hashCode * 397) ^ VendorId.GetHashCode();

                return hashCode;
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var parts = new List<string>();

            if (VendorId > 0)
            {
                parts.Add($"{VendorIdentifier}_{VendorId:X4}");
            }

            if (DeviceId > 0)
            {
                parts.Add($"{DeviceIdentifier}_{DeviceId:X4}");
            }

            if (SubSystem != null)
            {
                parts.Add(SubSystem.ToString());
            }

            if (Class != null)
            {
                parts.Add(Class.ToString());
            }

            if (DeviceType != null)
            {
                parts.Add($"{DeviceTypeIdentifier}_{DeviceType:X4}");
            }

            if (Revision != null)
            {
                parts.Add($"{RevisionIdentifier}_{Revision:X2}");
            }

            return "PCI\\" + string.Join("&", parts);
        }
    }
}