using System;
using System.Collections.Generic;
using System.Linq;

namespace PCIIdentificationResolver
{
    /// <summary>
    ///     Represents a PCI subclass
    /// </summary>
    [Serializable]
    public class PCIDeviceSubClass : IEquatable<PCIDeviceSubClass>
    {
        private readonly List<PCIDeviceClassProgramingInterface> _programingInterfaces =
            new List<PCIDeviceClassProgramingInterface>();

        internal PCIDeviceSubClass(PCIDeviceBaseClass parentBaseClass, string deviceInfo)
        {
            ParentBaseClass = parentBaseClass;
            var parts = deviceInfo.Split(new[] {"  "}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                throw new ArgumentException(@"Invalid device sub class info format.", nameof(deviceInfo));
            }

            SubClassId = Convert.ToByte(parts[0], 16);
            SubClassName = string.Join(" ", parts.Skip(1));
        }

        /// <summary>
        ///     Gets the parent base class.
        /// </summary>
        public PCIDeviceBaseClass ParentBaseClass { get; }


        /// <summary>
        ///     Gets a list of known programing interfaces that can be used with this subclass.
        /// </summary>
        public IEnumerable<PCIDeviceClassProgramingInterface> ProgramingInterfaces
        {
            get => _programingInterfaces.AsReadOnly();
        }

        /// <summary>
        ///     Gets the subclass identification number.
        /// </summary>
        public byte SubClassId { get; }

        /// <summary>
        ///     Gets the subclass friendly name.
        /// </summary>
        public string SubClassName { get; }

        /// <inheritdoc />
        public bool Equals(PCIDeviceSubClass other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return ParentBaseClass == other.ParentBaseClass && SubClassId == other.SubClassId;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIDeviceSubClass" /> for equality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIDeviceSubClass" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIDeviceSubClass" /> class.</param>
        /// <returns>true if instances of <see cref="PCIDeviceSubClass" /> class are equal; otherwise false.</returns>
        public static bool operator ==(PCIDeviceSubClass left, PCIDeviceSubClass right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIDeviceSubClass" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIDeviceSubClass" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIDeviceSubClass" /> class.</param>
        /// <returns>true if instances of <see cref="PCIDeviceSubClass" /> class are not equal; otherwise false.</returns>
        public static bool operator !=(PCIDeviceSubClass left, PCIDeviceSubClass right)
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

            return Equals(obj as PCIDeviceSubClass);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((ParentBaseClass != null ? ParentBaseClass.GetHashCode() : 0) * 397) ^ SubClassId.GetHashCode();
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return ParentBaseClass != null
                ? $"{ParentBaseClass.BaseClassName} {SubClassName} ({ParentBaseClass.BaseClassId:X2}{SubClassId:X2})"
                : $"{SubClassName} ({SubClassId:X2})";
        }

        internal void AddProgramingInterface(PCIDeviceClassProgramingInterface programingInterface)
        {
            _programingInterfaces.Add(programingInterface);
        }
    }
}