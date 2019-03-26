using System;
using ConsoleUtilities;
using PCIIdentificationResolver;

namespace PCIIdentificationResolverSample
{
    internal class Program
    {
        private static void ListBaseClasses(int i, ConsoleNavigationItem consoleNavigationItem)
        {
            foreach (var baseClass in PCIIdentificationDatabase.BaseClasses)
            {
                ConsoleWriter.Default.PrintSuccess(baseClass.ToString());
            }

            ConsoleWriter.Default.PrintMessage("Press enter to go back.");
            Console.ReadLine();
        }

        private static void ListVendors(int i, ConsoleNavigationItem consoleNavigationItem)
        {
            foreach (var vendor in PCIIdentificationDatabase.Vendors)
            {
                ConsoleWriter.Default.PrintSuccess(vendor.ToString());
            }

            ConsoleWriter.Default.PrintMessage("Press enter to go back.");
            Console.ReadLine();
        }

        private static void Main()
        {
            ConsoleWriter.Default.PrintCaption("EXECUTION PATHS");
            ConsoleNavigation.Default.PrintNavigation(new[]
            {
                new ConsoleNavigationItem("List Vendors", ListVendors),
                new ConsoleNavigationItem("Search Vendor By Vendor Id", SearchVendorByInt),
                new ConsoleNavigationItem("Search Vendor By Hexadecimal Vendor Id", SearchVendorByHex),
                new ConsoleNavigationItem("Search By PCI Address", SearchByPCIAddress),
                new ConsoleNavigationItem("List Device Base Classes", ListBaseClasses)
            }, "Select an execution path to continue.");
        }

        private static void SearchByPCIAddress(int i, ConsoleNavigationItem consoleNavigationItem)
        {
            string address;
            PCIAddress pciAddress;

            do
            {
                address = ConsoleWriter.Default.PrintQuestion<string>("Enter a valid PCI Address");
            } while (!PCIAddress.TryParse(address, out pciAddress) || pciAddress == null);

            var vendor = PCIIdentificationDatabase.GetVendor(pciAddress.VendorId);
            ConsoleWriter.Default.WriteColoredText("Vendor: ", ConsoleWriter.Default.Theme.MessageColor);

            if (vendor != null)
            {
                ConsoleWriter.Default.WriteColoredTextLine(vendor.ToString(), ConsoleWriter.Default.Theme.SuccessColor);
            }
            else
            {
                ConsoleWriter.Default.WriteColoredTextLine("Not Found", ConsoleWriter.Default.Theme.ErrorColor);
            }

            var device = PCIIdentificationDatabase.GetDevice(pciAddress.VendorId, pciAddress.DeviceId);
            ConsoleWriter.Default.WriteColoredText("Device: ", ConsoleWriter.Default.Theme.MessageColor);

            if (device != null)
            {
                ConsoleWriter.Default.WriteColoredTextLine(device.ToString(), ConsoleWriter.Default.Theme.SuccessColor);
            }
            else
            {
                ConsoleWriter.Default.WriteColoredTextLine("Not Found", ConsoleWriter.Default.Theme.ErrorColor);
            }

            if (pciAddress.SubSystem != null)
            {
                var subSystemVendor = PCIIdentificationDatabase.GetVendor(pciAddress.SubSystem);
                ConsoleWriter.Default.WriteColoredText("SubSystem Vendor: ", ConsoleWriter.Default.Theme.MessageColor);

                if (subSystemVendor != null)
                {
                    ConsoleWriter.Default.WriteColoredTextLine(
                        subSystemVendor.ToString(),
                        ConsoleWriter.Default.Theme.SuccessColor
                    );
                }
                else
                {
                    ConsoleWriter.Default.WriteColoredTextLine("Not Found", ConsoleWriter.Default.Theme.ErrorColor);
                }


                var subSystemDevice = PCIIdentificationDatabase.GetDevice(pciAddress.SubSystem);
                ConsoleWriter.Default.WriteColoredText("SubSystem Device: ", ConsoleWriter.Default.Theme.MessageColor);

                if (subSystemDevice != null)
                {
                    ConsoleWriter.Default.WriteColoredTextLine(
                        subSystemDevice.ToString(),
                        ConsoleWriter.Default.Theme.SuccessColor
                    );
                }
                else
                {
                    ConsoleWriter.Default.WriteColoredTextLine("Not Found", ConsoleWriter.Default.Theme.ErrorColor);
                }

                var subSystem = PCIIdentificationDatabase.GetSubSystem(pciAddress);

                if (subSystem != null)
                {
                    ConsoleWriter.Default.WriteColoredText("SubSystem: ", ConsoleWriter.Default.Theme.MessageColor);
                    ConsoleWriter.Default.WriteColoredTextLine(
                        subSystem.ToString(),
                        ConsoleWriter.Default.Theme.SuccessColor
                    );
                }
            }

            ConsoleWriter.Default.PrintMessage("Press enter to go back.");
            Console.ReadLine();
        }

        private static void SearchVendorByHex(int i, ConsoleNavigationItem consoleNavigationItem)
        {
            ushort vendorId;

            while (true)
            {
                var venderIdHex = ConsoleWriter.Default.PrintQuestion<string>("Enter a valid Hexadecimal Vendor Id");

                try
                {
                    vendorId = Convert.ToUInt16(venderIdHex, 16);

                    break;
                }
                catch
                {
                    // ignored
                }
            }

            var vendor = PCIIdentificationDatabase.GetVendor(vendorId);
            ConsoleWriter.Default.WriteColoredText("Vendor: ", ConsoleWriter.Default.Theme.MessageColor);

            if (vendor != null)
            {
                ConsoleWriter.Default.WriteColoredTextLine(vendor.ToString(), ConsoleWriter.Default.Theme.SuccessColor);
            }
            else
            {
                ConsoleWriter.Default.WriteColoredTextLine("Not Found", ConsoleWriter.Default.Theme.ErrorColor);
            }

            ConsoleWriter.Default.PrintMessage("Press enter to go back.");
            Console.ReadLine();
        }

        private static void SearchVendorByInt(int i, ConsoleNavigationItem consoleNavigationItem)
        {
            var vendorId = ConsoleWriter.Default.PrintQuestion<ushort>("Enter a valid Vendor Id");

            var vendor = PCIIdentificationDatabase.GetVendor(vendorId);
            ConsoleWriter.Default.WriteColoredText("Vendor: ", ConsoleWriter.Default.Theme.MessageColor);

            if (vendor != null)
            {
                ConsoleWriter.Default.WriteColoredTextLine(vendor.ToString(), ConsoleWriter.Default.Theme.SuccessColor);
            }
            else
            {
                ConsoleWriter.Default.WriteColoredTextLine("Not Found", ConsoleWriter.Default.Theme.ErrorColor);
            }

            ConsoleWriter.Default.PrintMessage("Press enter to go back.");
            Console.ReadLine();
        }
    }
}