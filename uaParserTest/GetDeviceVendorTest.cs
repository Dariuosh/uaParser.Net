using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using uaParserLibrary;

using uaParserResource;

using uaParserTest.Models;

using Xunit;

namespace uaParserTest
{
    public class GetDeviceVendorTest
    {
        private IReadOnlyList<DeviceTest> deviceTestList;

        public GetDeviceVendorTest()
        {
            var jsonString = File.ReadAllText("StaticFile/device-test.json");
            deviceTestList = JsonSerializer.Deserialize<List<DeviceTest>>(jsonString);
        }

        [Fact]
        public void GetSamsung_CheckTestCaseinBrowserTestFile_PassAll()
        {
            var result =
                deviceTestList
                .Where(testItem => testItem.expect.vendor == Vendors.Samsung)
                .FirstOrDefault(testItem => !testItem.Validate(UAParser.GetDevice(testItem.ua)));

            Assert.Null(result);
        }

        [Fact]
        public void GetApple_CheckTestCaseinBrowserTestFile_PassAll()
        {
            var result =
                deviceTestList
                .Where(testItem => testItem.expect.vendor == Vendors.Apple)
                .FirstOrDefault(testItem => !testItem.Validate(UAParser.GetDevice(testItem.ua)));

            Assert.Null(result);
        }

        [Fact]
        public void GetHuawei_CheckTestCaseinBrowserTestFile_PassAll()
        {
            var result =
                deviceTestList
                .Where(testItem => testItem.expect.vendor == Vendors.Huawei)
                .FirstOrDefault(testItem => !testItem.Validate(UAParser.GetDevice(testItem.ua)));

            Assert.Null(result);
        }

        [Fact]
        public void GetXiaomi_CheckTestCaseinBrowserTestFile_PassAll()
        {
            var result =
                deviceTestList
                .Where(testItem => testItem.expect.vendor == Vendors.Xiaomi)
                .FirstOrDefault(testItem => !testItem.Validate(UAParser.GetDevice(testItem.ua)));

            Assert.Null(result);
        }

        [Fact]
        public void GetMotorola_CheckTestCaseinBrowserTestFile_PassAll()
        {
            var result =
                deviceTestList
                .Where(testItem => testItem.expect.vendor == Vendors.Motorola)
                .FirstOrDefault(testItem => !testItem.Validate(UAParser.GetDevice(testItem.ua)));

            Assert.Null(result);
        }

        [Fact]
        public void GetLG_CheckTestCaseinBrowserTestFile_PassAll()
        {
            var result =
                deviceTestList
                .Where(testItem => testItem.expect.vendor == Vendors.LG)
                .FirstOrDefault(testItem => !testItem.Validate(UAParser.GetDevice(testItem.ua)));

            Assert.Null(result);
        }
    }
}