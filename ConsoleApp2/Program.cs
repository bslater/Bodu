// See https://aka.ms/new-console-template for more information
using Bodu.Collections.Extensions;
using Bodu.Collections.Generic.Extensions;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

var source = Enumerable.Range(0, 1024).Select(i => i);

foreach (var i in source.Randomize())
	Console.WriteLine(i);