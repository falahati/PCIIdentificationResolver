using System;
using System.Linq;

namespace PCIIdentificationResolver
{
    /// <summary>
    ///     Represents a PCI subclass programing interface
    /// </summary>
    [Serializable]
    public class PCIDeviceClassProgramingInterface : IEquatable<PCIDeviceClassProgramingInterface>
    {
        internal PCIDeviceClassProgramingInterface(
            PCIDeviceBaseClass parentBaseClass,
            PCIDeviceSubClass parentSubClass,
            string subSystemInfo)
        {
            ParentBaseClass = parentBaseClass;
            ParentSubClass = parentSubClass;
            var parts = subSystemInfo.Split(new[] {"  "}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                throw new ArgumentException(@"Invalid device class programing interface info format.",
                    nameof(subSystemInfo));
            }

            InterfaceId = Convert.ToByte(parts[0], 16);
            InterfaceName = string.Join(" ", parts.Skip(2));
        }

        /// <summary>
        ///     Gets the subclass interface identification number
        /// </summary>
        public byte InterfaceId { get; }

        /// <summary>
        ///     Gets the subclass interface friendly name
        /// </summary>
        public string InterfaceName { get; }

        /// <summary>
        ///     Gets the parent base class.
        /// </summary>
        public PCIDeviceBaseClass ParentBaseClass { get; }

        /// <summary>
        ///     Gets the parent sub class.
        /// </summary>
        public PCIDeviceSubClass ParentSubClass { get; }

        /// <inheritdoc />
        public bool Equals(PCIDeviceClassProgramingInterface other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return InterfaceId == other.InterfaceId && ParentSubClass == other.ParentSubClass;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIDeviceClassProgramingInterface" /> for equality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIDeviceClassProgramingInterface" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIDeviceClassProgramingInterface" /> class.</param>
        /// <returns>true if instances of <see cref="PCIDeviceClassProgramingInterface" /> class are equal; otherwise false.</returns>
        public static bool operator ==(PCIDeviceClassProgramingInterface left, PCIDeviceClassProgramingInterface right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIDeviceClassProgramingInterface" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIDeviceClassProgramingInterface" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIDeviceClassProgramingInterface" /> class.</param>
        /// <returns>true if instances of <see cref="PCIDeviceClassProgramingInterface" /> class are not equal; otherwise false.</returns>
        public static bool operator !=(PCIDeviceClassProgramingInterface left, PCIDeviceClassProgramingInterface right)
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

            return Equals(obj as PCIDeviceClassProgramingInterface);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (InterfaceId.GetHashCode() * 397) ^ (ParentSubClass != null ? ParentSubClass.GetHashCode() : 0);
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            if (ParentBaseClass != null && ParentSubClass != null)
            {
                return
                    $"{ParentBaseClass.BaseClassName} {ParentSubClass.SubClassName} {InterfaceName} ({ParentBaseClass.BaseClassId:X2}{ParentSubClass.SubClassId:X2}{InterfaceId:X2})";
            }

            if (ParentSubClass != null)
            {
                return
                    $"{ParentSubClass.SubClassName} {InterfaceName} ({ParentSubClass.SubClassId:X2}{InterfaceId:X2})";
            }

            return $"{InterfaceName} ({InterfaceId:X2})";
        }
    }
}