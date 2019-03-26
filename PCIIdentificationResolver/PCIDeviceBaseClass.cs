using System;
using System.Collections.Generic;
using System.Linq;

namespace PCIIdentificationResolver
{
    /// <summary>
    ///     Represents a PCI base class
    /// </summary>
    [Serializable]
    public class PCIDeviceBaseClass : IEquatable<PCIDeviceBaseClass>
    {
        private readonly List<PCIDeviceSubClass> _subClasses = new List<PCIDeviceSubClass>();

        internal PCIDeviceBaseClass(string baseClassInfo)
        {
            var parts = baseClassInfo.Split(new[] {"  "}, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                throw new ArgumentException(@"Invalid device base class info format.", nameof(baseClassInfo));
            }

            BaseClassId = Convert.ToByte(parts[0], 16);
            BaseClassName = string.Join(" ", parts.Skip(1));
        }

        /// <summary>
        ///     Gets the base class identification number
        /// </summary>
        public byte BaseClassId { get; }

        /// <summary>
        ///     Gets the base class friendly name
        /// </summary>
        public string BaseClassName { get; }

        /// <summary>
        ///     Gets the base class known subclasses
        /// </summary>
        public IEnumerable<PCIDeviceSubClass> SubClasses
        {
            get => _subClasses.AsReadOnly();
        }

        /// <inheritdoc />
        public bool Equals(PCIDeviceBaseClass other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return BaseClassId == other.BaseClassId;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIDeviceBaseClass" /> for equality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIDeviceBaseClass" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIDeviceBaseClass" /> class.</param>
        /// <returns>true if instances of <see cref="PCIDeviceBaseClass" /> class are equal; otherwise false.</returns>
        public static bool operator ==(PCIDeviceBaseClass left, PCIDeviceBaseClass right)
        {
            return Equals(left, right) || left?.Equals(right) == true;
        }

        /// <summary>
        ///     Compares two instances of <see cref="PCIDeviceBaseClass" /> for inequality.
        /// </summary>
        /// <param name="left">The first instance of <see cref="PCIDeviceBaseClass" /> class.</param>
        /// <param name="right">The second instance of <see cref="PCIDeviceBaseClass" /> class.</param>
        /// <returns>true if instances of <see cref="PCIDeviceBaseClass" /> class are not equal; otherwise false.</returns>
        public static bool operator !=(PCIDeviceBaseClass left, PCIDeviceBaseClass right)
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

            return Equals(obj as PCIDeviceBaseClass);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return BaseClassId.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{BaseClassName} ({BaseClassId:X2})";
        }

        internal void AddSubClass(PCIDeviceSubClass subClass)
        {
            _subClasses.Add(subClass);
        }
    }
}