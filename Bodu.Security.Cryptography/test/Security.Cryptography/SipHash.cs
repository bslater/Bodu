using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Security.Cryptography
{
	public abstract partial class SipHashTests<T>
		: KeyedHashAlgorithmTests<T>
		where T : SipHash
	{ }
}