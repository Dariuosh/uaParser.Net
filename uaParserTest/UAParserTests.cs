using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

using uaParserLibrary;

using uaParserTest.Models;

using Xunit;

namespace uaParserTest
{
    public class UAParserTests
    {
        [Fact]
        public void GetBrowser_CheckTestCaseinBrowserTestFile_PassAll()
        {
            var jsonString = File.ReadAllText("StaticFile/browser-test.json");
            var browserTestList = JsonSerializer.Deserialize<List<BrowserTest>>(jsonString);


            var result = browserTestList.FirstOrDefault(testItem => !testItem.Validate(UAParser.GetBrowser(testItem.ua)));

            Assert.Null(result);
       
        }

        [Fact]
        public void GetCPU_CheckTestCaseinCPUTestFile_PassAll()
        {
            var jsonString = File.ReadAllText("StaticFile/cpu-test.json");
            var cpuTestList = JsonSerializer.Deserialize<List<CPUTest>>(jsonString);


            var result = cpuTestList.FirstOrDefault(testItem => !testItem.Validate(UAParser.GetCPU(testItem.ua)));

            Assert.Null(result);
           
        }

        [Fact]
        public void GetEngine_CheckTestCaseinEngineTestFile_PassAll()
        {
            var jsonString = File.ReadAllText("StaticFile/engine-test.json");
            var engineTestList = JsonSerializer.Deserialize<List<EngineTest>>(jsonString);


            var result = engineTestList.FirstOrDefault(testItem => !testItem.Validate(UAParser.GetEngine(testItem.ua)));


            Assert.Null(result);
        }

        [Fact]
        public void GetOS_CheckTestCaseinOSTestFile_PassAll()
        {
            var jsonString = File.ReadAllText("StaticFile/os-test.json");
            var osTestList = JsonSerializer.Deserialize<List<OSTest>>(jsonString);


            var result = osTestList.FirstOrDefault(testItem => !testItem.Validate(UAParser.GetOS(testItem.ua)));


            Assert.Null(result);
        }

        [Fact]
        public void GetGPU_CheckTestCaseinGPUTestFile_PassAll()
        {
            var jsonString = File.ReadAllText("StaticFile/gpu-test.json");
            var gpuTestList = JsonSerializer.Deserialize<List<GPUTest>>(jsonString);


            var result = gpuTestList.FirstOrDefault(testItem => !testItem.Validate(UAParser.GetGPU(testItem.renderer)));


            Assert.Null(result);
        }

        [Fact]
        public void GetDevice_CheckTestCaseinDeviceTestFile_PassAll()
        {
            var jsonString = File.ReadAllText("StaticFile/device-test.json");
            var deviceTestList = JsonSerializer.Deserialize<List<DeviceTest>>(jsonString);


            var result = deviceTestList.FirstOrDefault(testItem => !testItem.Validate(UAParser.GetDevice(testItem.ua)));


            Assert.Null(result);
        }
    }
}