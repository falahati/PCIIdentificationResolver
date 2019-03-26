using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PCIIdentificationResolver.Properties;

namespace PCIIdentificationResolver
{
    /// <summary>
    ///     Represents the PCI identification database and provides access to the list of known vendors, devices, classes and
    ///     subsystems
    /// </summary>
    public static class PCIIdentificationDatabase
    {
        private static List<PCIDeviceBaseClass> _baseClasses;
        private static List<PCIVendor> _vendors;

        private static readonly object LockObject = new object();

        /// <summary>
        ///     Gets the readonly list of known base classes.
        /// </summary>
        public static IEnumerable<PCIDeviceBaseClass> BaseClasses
        {
            get
            {
                if (_baseClasses == null)
                {
                    Reload();
                }

                return _baseClasses?.AsReadOnly();
            }
        }

        /// <summary>
        ///     Gets the readonly list of known vendors.
        /// </summary>
        public static IEnumerable<PCIVendor> Vendors
        {
            get
            {
                if (_vendors == null)
                {
                    Reload();
                }

                return _vendors?.AsReadOnly();
            }
        }

        /// <summary>
        ///     Searches for a device based on the passed identification numbers.
        /// </summary>
        /// <param name="vendorId">The device vendor identification number.</param>
        /// <param name="deviceId">The device identification number.</param>
        /// <returns>An instance of <see cref="PCIDevice" /> class if a device is found; otherwise null.</returns>
        public static PCIDevice GetDevice(ushort vendorId, ushort deviceId)
        {
            return GetVendor(vendorId)?.Devices.FirstOrDefault(device => device.DeviceId == deviceId);
        }

        /// <summary>
        ///     Searches for a device based on the passed <see cref="PCIAddress" /> instance.
        /// </summary>
        /// <param name="address">
        ///     An instance of <see cref="PCIAddress" /> class containing information required to search for a
        ///     device.
        /// </param>
        /// <returns>An instance of <see cref="PCIDevice" /> class if a device is found; otherwise null.</returns>
        public static PCIDevice GetDevice(PCIAddress address)
        {
            return GetDevice(address.VendorId, address.DeviceId);
        }

        /// <summary>
        ///     Searches for a subsystem device based on the passed <see cref="PCIAddressSubSystem" /> instance.
        /// </summary>
        /// <param name="subSystem">
        ///     An instance of <see cref="PCIAddressSubSystem" /> class containing information required to
        ///     search for a subsystem device.
        /// </param>
        /// <returns>An instance of <see cref="PCIDevice" /> class if a device is found; otherwise null.</returns>
        public static PCIDevice GetDevice(PCIAddressSubSystem subSystem)
        {
            return GetDevice(subSystem.VendorId, subSystem.DeviceId);
        }

        /// <summary>
        ///     Searches for a device base class based on the passed base class identification number
        /// </summary>
        /// <param name="baseClassId">The base class identification number to search for.</param>
        /// <returns>An instance of <see cref="PCIDeviceBaseClass" /> class if a base class is found; otherwise null.</returns>
        public static PCIDeviceBaseClass GetDeviceBaseClass(byte baseClassId)
        {
            return BaseClasses.FirstOrDefault(baseClass => baseClass.BaseClassId == baseClassId);
        }

        /// <summary>
        ///     Searches for a device base class based on the passed <see cref="PCIAddressClass" /> instance.
        /// </summary>
        /// <param name="addressClass">
        ///     An instance of <see cref="PCIAddressClass" /> class containing information required to
        ///     search for a device base class.
        /// </param>
        /// <returns>An instance of <see cref="PCIDeviceBaseClass" /> class if a base class is found; otherwise null.</returns>
        public static PCIDeviceBaseClass GetDeviceBaseClass(PCIAddressClass addressClass)
        {
            return GetDeviceBaseClass(addressClass.BaseClassId);
        }

        /// <summary>
        ///     Searches for a device subclass programing interface based on the passed <see cref="PCIAddressClass" /> instance.
        /// </summary>
        /// <param name="addressClass">
        ///     An instance of <see cref="PCIAddressClass" /> class containing information required to
        ///     search for a device subclass programing interface.
        /// </param>
        /// <returns>
        ///     An instance of <see cref="PCIDeviceClassProgramingInterface" /> class if a sub class programing interface is
        ///     found; otherwise null.
        /// </returns>
        public static PCIDeviceClassProgramingInterface GetDeviceClassProgramingInterface(PCIAddressClass addressClass)
        {
            if (addressClass == null)
            {
                throw new ArgumentNullException(nameof(addressClass));
            }

            if (addressClass.ProgramingInterfaceId == null)
            {
                return null;
            }

            return GetDeviceClassProgramingInterface(addressClass.BaseClassId, addressClass.SubClassId,
                addressClass.ProgramingInterfaceId.Value);
        }

        /// <summary>
        ///     Searches for a device subclass programing interface based on the passed identification numbers.
        /// </summary>
        /// <param name="baseClassId">The base class identification number.</param>
        /// <param name="subClassId">The subclass identification number.</param>
        /// <param name="interfaceId">The subclass programing interface identification number.</param>
        /// <returns>
        ///     An instance of <see cref="PCIDeviceClassProgramingInterface" /> class if a sub class programing interface is
        ///     found; otherwise null.
        /// </returns>
        // ReSharper disable once TooManyArguments
        public static PCIDeviceClassProgramingInterface GetDeviceClassProgramingInterface(
            byte baseClassId,
            byte subClassId,
            byte interfaceId)
        {
            return GetDeviceSubClass(baseClassId, subClassId)?.ProgramingInterfaces.FirstOrDefault(
                programingInterface => programingInterface.InterfaceId == interfaceId
            );
        }

        /// <summary>
        ///     Searches for a device subclass based on the passed identification numbers.
        /// </summary>
        /// <param name="baseClassId">The base class identification number.</param>
        /// <param name="subClassId">The subclass identification number.</param>
        /// <returns>An instance of <see cref="PCIDeviceSubClass" /> class if a subclass is found; otherwise null.</returns>
        public static PCIDeviceSubClass GetDeviceSubClass(byte baseClassId, byte subClassId)
        {
            return GetDeviceBaseClass(baseClassId)?.SubClasses
                .FirstOrDefault(subClass => subClass.SubClassId == subClassId);
        }

        /// <summary>
        ///     Searches for a device subclass based on the passed <see cref="PCIAddressClass" /> instance.
        /// </summary>
        /// <param name="addressClass">
        ///     An instance of <see cref="PCIAddressClass" /> class containing information required to
        ///     search for a device subclass.
        /// </param>
        /// <returns>An instance of <see cref="PCIDeviceSubClass" /> class if a subclass is found; otherwise null.</returns>
        public static PCIDeviceSubClass GetDeviceSubClass(PCIAddressClass addressClass)
        {
            return GetDeviceSubClass(addressClass.BaseClassId, addressClass.SubClassId);
        }

        /// <summary>
        ///     Searches for a device subsystem based on the passed <see cref="PCIAddress" /> instance.
        /// </summary>
        /// <param name="address">
        ///     An instance of <see cref="PCIAddress" /> class containing information required to search for a
        ///     device subsystem.
        /// </param>
        /// <returns>An instance of <see cref="PCISubSystem" /> class if a subsystem is found; otherwise null.</returns>
        public static PCISubSystem GetSubSystem(PCIAddress address)
        {
            return GetSubSystem(address.VendorId, address.DeviceId, address.SubSystem.VendorId,
                address.SubSystem.DeviceId);
        }

        /// <summary>
        ///     Searches for a device subsystem based on the passed identification numbers.
        /// </summary>
        /// <param name="vendorId">The device vendor identification number.</param>
        /// <param name="deviceId">The device identification number.</param>
        /// <param name="subVendorId">The subsystem vendor identification number.</param>
        /// <param name="subDeviceId">The subsystem device identification number.</param>
        /// <returns>An instance of <see cref="PCISubSystem" /> class if a subsystem is found; otherwise null.</returns>
        // ReSharper disable once TooManyArguments
        public static PCISubSystem GetSubSystem(
            ushort vendorId,
            ushort deviceId,
            ushort subVendorId,
            ushort subDeviceId)
        {
            return GetDevice(vendorId, deviceId)?.SubSystems.FirstOrDefault(
                device => device.VendorId == subVendorId && device.DeviceId == subDeviceId
            );
        }

        /// <summary>
        ///     Searches for a device vendor based on the passed device vendor identification number.
        /// </summary>
        /// <param name="vendorId">The device vendor identification number.</param>
        /// <returns>An instance of <see cref="PCIVendor" /> class if a device vendor is found; otherwise null.</returns>
        public static PCIVendor GetVendor(ushort vendorId)
        {
            return Vendors.FirstOrDefault(vendor => vendor.VendorId == vendorId);
        }

        /// <summary>
        ///     Searches for a device vendor based on the passed <see cref="PCIAddress" /> instance.
        /// </summary>
        /// <param name="address">
        ///     An instance of <see cref="PCIAddress" /> class containing information required to search for a
        ///     device vendor.
        /// </param>
        /// <returns>An instance of <see cref="PCIVendor" /> class if a device vendor is found; otherwise null.</returns>
        public static PCIVendor GetVendor(PCIAddress address)
        {
            return GetVendor(address.VendorId);
        }

        /// <summary>
        ///     Searches for a device subsystem vendor based on the passed <see cref="PCIAddress" /> instance.
        /// </summary>
        /// <param name="subSystem">
        ///     An instance of <see cref="PCIAddressSubSystem" /> class containing information required to
        ///     search for a subsystem device vendor.
        /// </param>
        /// <returns>An instance of <see cref="PCIVendor" /> class if a subsystem device vendor is found; otherwise null.</returns>
        public static PCIVendor GetVendor(PCIAddressSubSystem subSystem)
        {
            return GetVendor(subSystem.VendorId);
        }

        /// <summary>
        ///     Reloads cached list of vendors and classes from disk and into memory
        /// </summary>
        // ReSharper disable once ExcessiveIndentation
        public static void Reload()
        {
            lock (LockObject)
            {
                _vendors = new List<PCIVendor>();
                _baseClasses = new List<PCIDeviceBaseClass>();

                using (var memoryStream = new MemoryStream(Resources.pci))
                {
                    using (var streamReader = new StreamReader(memoryStream))
                    {
                        PCIVendor lastVendor = null;
                        PCIDevice lastDevice = null;
                        PCIDeviceBaseClass lastBaseClass = null;
                        PCIDeviceSubClass lastSubClass = null;

                        while (!streamReader.EndOfStream)
                        {
                            var line = streamReader.ReadLine();

                            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                            {
                                continue;
                            }

                            if (line.StartsWith("C "))
                            {
                                // Parse base class
                                lastVendor = null;
                                lastDevice = null;

                                try
                                {
                                    lastBaseClass = new PCIDeviceBaseClass(line.Substring(2).Trim());
                                    _baseClasses.Add(lastBaseClass);
                                }
                                catch
                                {
                                    lastBaseClass = null;
                                }
                            }
                            else if (line.StartsWith("\t\t"))
                            {
                                if (lastDevice != null)
                                {
                                    // Parse sub device
                                    try
                                    {
                                        var subDevice = new PCISubSystem(lastVendor, lastDevice, line.Trim());
                                        lastDevice.AddSubSystem(subDevice);
                                    }
                                    catch
                                    {
                                        // ignored
                                    }
                                }
                                else if (lastSubClass != null)
                                {
                                    // Parse class programing interface
                                    try
                                    {
                                        var programingInterface =
                                            new PCIDeviceClassProgramingInterface(lastBaseClass, lastSubClass,
                                                line.Trim());
                                        lastSubClass.AddProgramingInterface(programingInterface);
                                    }
                                    catch
                                    {
                                        // ignored
                                    }
                                }
                            }
                            else if (line.StartsWith("\t"))
                            {
                                if (lastVendor != null)
                                {
                                    // Parse device
                                    try
                                    {
                                        lastDevice = new PCIDevice(lastVendor, line.Trim());
                                        lastVendor.AddDevice(lastDevice);
                                    }
                                    catch
                                    {
                                        lastDevice = null;
                                        lastVendor = null;
                                    }
                                }
                                else if (lastBaseClass != null)
                                {
                                    // Parse sub class
                                    try
                                    {
                                        lastSubClass = new PCIDeviceSubClass(lastBaseClass, line.Trim());
                                        lastBaseClass.AddSubClass(lastSubClass);
                                    }
                                    catch
                                    {
                                        lastSubClass = null;
                                        lastBaseClass = null;
                                    }
                                }
                            }
                            else
                            {
                                // Parse vendor
                                lastBaseClass = null;
                                lastSubClass = null;

                                try
                                {
                                    lastVendor = new PCIVendor(line.Trim());
                                    _vendors.Add(lastVendor);
                                }
                                catch
                                {
                                    lastVendor = null;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}