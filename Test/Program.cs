// See https://aka.ms/new-console-template for more information
using TPF113B;

Console.WriteLine("Hello, World!");
ITPF113B tpf113B = new TPF113B.TPF113B(new Communication.Bus.PhysicalPort.SerialPort("COM3", 38400));
await tpf113B.OpenAsync();
var rs = await tpf113B.Read("01");

Console.ReadLine();