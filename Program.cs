using CheckDomains.Service;
using System;
using System.Diagnostics;
using System.Linq;

namespace CheckDomains
{
	class Program
	{
		static void Main(string[] args)
		{

			Console.WriteLine("Begin: " + DateTime.Now.ToString("HH:mm:ss fff"));
			Stopwatch watch = new Stopwatch();
			watch.Start();

			var service = new DomainService();
			Enumerable.Range(1, 12).AsParallel().ForAll((fileIndex) => service.UpdateDomains(fileIndex));

			watch.Stop();
			Console.WriteLine("End: " + DateTime.Now.ToString("HH:mm:ss fff"));
			Console.WriteLine("Milisecond Time: " + watch.ElapsedMilliseconds);

			Console.ReadLine();
		}
	}
}
