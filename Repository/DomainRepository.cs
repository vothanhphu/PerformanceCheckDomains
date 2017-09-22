using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace CheckDomains.Repository
{
	public class DomainRepository
	{
		public DomainRepository()
		{
			this.Client = new MongoClient("mongodb://localhost:27017");
			this.Database = this.Client.GetDatabase("DOMAINDB");
		}

		public IEnumerable<string> Get()
		{
			var collection = this.Database.GetCollection<MY_DOMAIN>("MY_DOMAIN");
			return collection.Find(new BsonDocument()).ToList().Select(x => x.domain).AsEnumerable();
		}

		public void Insert(IEnumerable<string> emails)
		{
			if ((emails == null) || (emails.Any() == false)) return;

			var collection = this.Database.GetCollection<MY_DOMAIN>("DB_RESULT");
			collection.InsertMany(emails.Select(x => new MY_DOMAIN { domain = x }));
		}

		private class MY_DOMAIN
		{
			[BsonRepresentation(BsonType.ObjectId)]
			public string Id { get; set; }
			public string domain { get; set; }
		}

		#region
		MongoClient Client { get; set; }
		IMongoDatabase Database { get; set; }
		#endregion
	}
}