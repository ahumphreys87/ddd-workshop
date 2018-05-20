using Xunit;

namespace VaughnVernon.Mockroservices.Tests
{
	public class KVStoreTest
	{
		[Fact]
		public void TestPutGet()
		{
			string key = "k1";
			string value = "v1";

			string name = "test";
			KVStore store = KVStore.Open(name);

			store.Put(key, value);

			Assert.Equal(name, store.Name);
			Assert.Equal(value, store.get(key));
		}
	}
}
