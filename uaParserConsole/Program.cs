using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;

using uaParserConsole.Models;

using uaParserLibrary;
using uaParserLibrary.Models;

using uaParserResource;

namespace uaParserConsole
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string uaString = string.Empty;

            // uaString = @"Mozilla/5.0 (iPhone; CPU iPhone OS 5_1_1 like Mac OS X)";

            // uaString = @"AppleWebKit/534.46 (KHTML, like Gecko) Version/5.1 Mobile/9B206 Safari/7534.48.3";

            // uaString = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/92.0.4501.0 Safari/537.36 Edg/92.0.891.0";

            // uaString = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36";

            // uaString = @"Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:88.0) Gecko/20100101 Firefox/88.0";

            // uaString = @"Mozilla/5.0 (Windows; Windows NT 5.1; rv:2.0b3pre) Gecko/20100727 Minefield/4.0.1pre";

            // _ - _ _ - _ _ - _ _ - _ _ - _ _ - _ _ - _ _ - _ _ - _ _ - _ _ - _ _ - _ _ - _ _ - _ _

            string jsonString = string.Empty;
            //jsonString = File.ReadAllText("StaticFile/browser-test.json");
            //var browserTestList = JsonSerializer.Deserialize<List<BrowserTest>>(jsonString);

            //jsonString = File.ReadAllText("StaticFile/cpu-test.json");
            //var cpuTestList = JsonSerializer.Deserialize<List<CPUTest>>(jsonString);

            //jsonString = File.ReadAllText("StaticFile/engine-test.json");
            //var engingTestList = JsonSerializer.Deserialize<List<EngineTest>>(jsonString);

            //jsonString = File.ReadAllText("StaticFile/os-test.json");
            //var osTestList = JsonSerializer.Deserialize<List<OSTest>>(jsonString);

            //jsonString = File.ReadAllText("StaticFile/gpu-test.json");
            //var gpuTestList = JsonSerializer.Deserialize<List<GPUTest>>(jsonString);

            jsonString = File.ReadAllText("StaticFile/device-test.json");
            var deviceTestList = JsonSerializer.Deserialize<List<DeviceTest>>(jsonString);

            //Browser browser;
            //CPU cpu;
            //Engine engine;
            //OS os;
            //GPU gpu;
            Device device;
            ClientInfo result;
            Stopwatch stopwatch = new Stopwatch();
            int loopCount = 10;
            if (uaString != string.Empty)
            {
                int z = 0;
                while (z < loopCount)
                {
                    Console.WriteLine($" UserAgent    => {uaString} \r");

                    //stopwatch.Restart();
                    //browser = UAParser.GetBrowser(uaString);
                    //stopwatch.Stop();
                    //PrintBrowser(new BrowserTest(), browser, stopwatch, z);

                    //stopwatch.Restart();
                    //cpu = UAParser.GetCPU(uaString);
                    //stopwatch.Stop();
                    //PrintCPU(new CPUTest(), cpu, stopwatch, z);

                    //stopwatch.Restart();
                    //engine = UAParser.GetEngine(uaString);
                    //stopwatch.Stop();
                    //PrintEngine(new EngineTest(), engine, stopwatch, z);
                    //PrintDivider();

                    //stopwatch.Restart();
                    //os = UAParser.GetOS(uaString);
                    //stopwatch.Stop();
                    //PrintOS(new OSTest(), os, stopwatch, z);
                    //PrintDivider();

                    stopwatch.Restart();
                    result = UAParser.GetClientInfo(uaString);
                    stopwatch.Stop();
                    PrintResult(result, stopwatch);
                    PrintDivider();

                    ++z;
                }
            }
            else
            {
                int z = 0;
                while (z < loopCount)
                {
                    int i = 0;

                    //browserTestList.ForEach(testItem =>
                    //{
                    //    stopwatch.Restart();
                    //    browser = UAParser.GetBrowser(testItem.ua);
                    //    stopwatch.Stop();
                    //    PrintBrowser(testItem, browser, stopwatch, i++);
                    //    PrintDivider();
                    //});
                    //cpuTestList.ForEach(testItem =>
                    //{
                    //    stopwatch.Restart();
                    //    cpu = UAParser.GetCPU(testItem.ua);
                    //    stopwatch.Stop();
                    //    PrintCPU(testItem, cpu, stopwatch, i++);
                    //    PrintDivider();
                    //});

                    //engingTestList.ForEach(testItem =>
                    //{
                    //    stopwatch.Restart();
                    //    engine = UAParser.GetEngine(testItem.ua);
                    //    stopwatch.Stop();
                    //    PrintEngine(testItem, engine, stopwatch, i++);
                    //    PrintDivider();
                    //});

                    //osTestList.ForEach(testItem =>
                    //{
                    //    stopwatch.Restart();
                    //    os = UAParser.GetOS(testItem.ua);
                    //    stopwatch.Stop();
                    //    PrintOS(testItem, os, stopwatch, i++);
                    //    PrintDivider();
                    //});

                    //gpuTestList.ForEach(testItem =>
                    //{
                    //    stopwatch.Restart();
                    //    gpu = UAParser.GetGPU(testItem.renderer);
                    //    stopwatch.Stop();
                    //    PrintGPU(testItem, gpu, stopwatch, i++);
                    //    PrintDivider();
                    //});

                    deviceTestList
                        //.Where(testItem =>
                        //    testItem.expect.vendor == Vendors.Apple
                        //       && (testItem.expect.type == Keywords.Mobile || testItem.expect.type == Keywords.Tablet)
                        //    )
                        .ToList().ForEach(testItem =>
                        {
                            stopwatch.Restart();
                            device = UAParser.GetDevice(testItem.ua);
                            stopwatch.Stop();
                            //if (!testItem.Validate(device))
                            //{
                            PrintDevice(testItem, device, stopwatch, i++);
                            PrintDivider();
                            // }
                        });
                    ++z;
                }
            }

            Console.ReadLine();

            static void PrintCPU(CPUTest testItem, CPU cpu, Stopwatch stopwatch, int i)
            {
                if (testItem.expect is not null)
                {
                    Console.WriteLine($" {i:000}          => Description :{testItem.desc} \r");
                    Console.WriteLine($" UserAgent    => {testItem.ua} \r");
                    Console.WriteLine($" Expect       => Architecture :{testItem.expect.architecture,-25} ");
                }
                Console.WriteLine($" CPU          => Architecture :{cpu.Architecture,-68} ElapsedTicks : {stopwatch.ElapsedTicks} \r\r");
            }

            static void PrintBrowser(BrowserTest testItem, Browser browser, Stopwatch stopwatch, int i)
            {
                if (testItem.expect is not null)
                {
                    Console.WriteLine($" {++i:000}          => Description :{testItem.desc} \r");
                    Console.WriteLine($" UserAgent    => {testItem.ua} \r");
                    Console.WriteLine($" Expect       => Name :{testItem.expect.name,-25} Version :{testItem.expect.version,-20} , Major :{testItem.expect.major,-10}");
                }
                Console.WriteLine($" Browser      => Name :{browser.Name,-25} Version :{browser.Version,-20} , Major :{browser.Major,-10}  ElapsedTicks : {stopwatch.ElapsedTicks} \r\r");
            }

            static void PrintEngine(EngineTest testItem, Engine engine, Stopwatch stopwatch, int i)
            {
                if (testItem.expect is not null)
                {
                    Console.WriteLine($" {++i:000}          => Description :{testItem.desc} \r");
                    Console.WriteLine($" UserAgent    => {testItem.ua} \r");
                    Console.WriteLine($" Expect       => Name :{testItem.expect.name,-25} Version :{testItem.expect.version,-20}");
                }
                Console.WriteLine($" Engine       => Name :{engine.Name,-25} Version :{engine.Version,-20}  ElapsedTicks : {stopwatch.ElapsedTicks} \r\r");
            }

            static void PrintOS(OSTest testItem, OS os, Stopwatch stopwatch, int i)
            {
                if (testItem.expect is not null)
                {
                    Console.WriteLine($" {++i:000}          => Description :{testItem.desc} \r");
                    Console.WriteLine($" UserAgent    => {testItem.ua} \r");
                    Console.WriteLine($" Expect       => Name :{testItem.expect.name,-25} Version :{testItem.expect.version,-20}");
                }
                Console.WriteLine($" OS           => Name :{os.Name,-25} Version :{os.Version,-20}  ElapsedTicks : {stopwatch.ElapsedTicks} \r\r");
            }

            static void PrintGPU(GPUTest testItem, GPU gpu, Stopwatch stopwatch, int i)
            {
                if (testItem.expect is not null)
                {
                    Console.WriteLine($" {++i:000}          => Description :{testItem.desc} \r");
                    Console.WriteLine($" Renderer     => {testItem.renderer} \r");
                    Console.WriteLine($" Expect       => Name :{testItem.expect.vendor,-25} Version :{testItem.expect.model,-20}");
                }
                Console.WriteLine($" GPU          => Name :{gpu.Vendor,-25} Version :{gpu.Model,-20}  ElapsedTicks : {stopwatch.ElapsedTicks} \r\r");
            }

            static void PrintDevice(DeviceTest testItem, Device device, Stopwatch stopwatch, int i)
            {
                if (testItem.expect is not null)
                {
                    Console.WriteLine($" {++i:000}          => Description :{testItem.desc} \r");
                    Console.WriteLine($" UserAgent    => {testItem.ua} \r");
                    Console.WriteLine($" Expect       => Vendor :{testItem.expect.vendor,-25} Model :{testItem.expect.model,-20} Type :{testItem.expect.type,-20}");
                }
                Console.WriteLine($" Device       => Vendor :{device.Vendor,-25} Model :{device.Model,-20} Type :{device.Type,-20}  ElapsedTicks : {stopwatch.ElapsedTicks} \r\r");
            }

            static void PrintDivider()
            {
                Console.WriteLine(" -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  -  - \r\r");
            }

            static void PrintResult(ClientInfo result, Stopwatch stopwatch)
            {
                Console.WriteLine($" CPU       => Architecture  :{result.CPU.Architecture,-76} ElapsedTicks : {stopwatch.ElapsedTicks} \r\r");
                Console.WriteLine($" Browser   => Name          :{result.Browser.Name,-25} Version :{result.Browser.Version,-20}   Major :{result.Browser.Major,-10}  ElapsedTicks : {stopwatch.ElapsedTicks} \r\r");
                Console.WriteLine($" Engine    => Name          :{result.Engine.Name,-25} Version :{result.Engine.Version,-40}  ElapsedTicks : {stopwatch.ElapsedTicks} \r\r");
                Console.WriteLine($" OS        => Name          :{result.OS.Name,-25} Version :{result.OS.Version,-40}  ElapsedTicks : {stopwatch.ElapsedTicks} \r\r");
                Console.WriteLine($" Device    => Vendor        :{result.Device.Vendor,-25} Type    :{result.Device.Type,-21}  Model :{result.Device.Model,-10}  ElapsedTicks : {stopwatch.ElapsedTicks} \r\r");
            }
        }
    }
}