using CheckDomains.Repository;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CheckDomains.Service
{
	public class DomainService
	{
		public DomainService()
		{
			this.DatabaseDomains = GroupbyDomains(Repository.Get());
			this.IntersectionsDomains = new HashSet<string>();
		}

		public void UpdateDomains(int fileIndex)
		{
			foreach (var item in GroupbyDomains(ReadFiles(fileIndex)))
			{
				Task.Run(() => UpdateExistsDomains(item));
				//UpdateExistsDomains(item);
			}
		}

		private void UpdateExistsDomains(KeyValuePair<string, HashSet<string>> domains)
		{
			// Domains key are exists in database key
			var existsDatabase = this.DatabaseDomains.Where(x => x.Key == domains.Key);
			if ((existsDatabase != null) && (existsDatabase.Any() == false)) return;

			// Check domains are exists in database
			var existsFilesDomains = existsDatabase.First().Value.Intersect(domains.Value);
			if ((existsFilesDomains == null) || (existsFilesDomains.Any() == false)) return;

			// Update exist domain list
			UpdateExistsDomains(existsFilesDomains);
		}
		private void UpdateExistsDomains(IEnumerable<string> domains)
		{
			try
			{
				Monitor.Enter(_object);

				var existsdomains = domains.Where(x => (this.IntersectionsDomains.Contains(x) == false)).ToList();

				if ((existsdomains == null)) return;

				this.IntersectionsDomains = this.IntersectionsDomains.Union(existsdomains).ToHashSet();

				Task.Run(() => this.Repository.Insert(existsdomains));
			}
			finally
			{
				Monitor.Exit(_object);
			}
		}

		private string[] ReadFiles(int fileIndex)
		{
			return File.ReadAllLines(string.Format(filePath, fileIndex));
		}

		private Dictionary<string, HashSet<string>> GroupbyDomains(IEnumerable<string> domains)
		{
			var domainGroup = new Dictionary<string, HashSet<string>>();

			foreach (var item in domains.GroupBy(x => GetRefix(x)))
			{
				domainGroup.Add(item.Key, new HashSet<string>(item));
			}

			return domainGroup;
		}

		private string GetRefix(string domain)
		{
			return (domain.Length >= REFIX_LENGTH ? domain.Substring(0, REFIX_LENGTH) : domain);
		}

		#region
		private DomainRepository Repository = new DomainRepository();
		private Dictionary<string, HashSet<string>> DatabaseDomains { get; set; }
		private HashSet<string> IntersectionsDomains { get; set; }
		private const string filePath = "/media/hope/Relax/DomainFiles/Domain{0}.txt";
		//private const string filePath = "F:/DomainFiles/Domain{0}.txt";
		private const short REFIX_LENGTH = 1;
		static readonly object _object = new object();
		#endregion
	}
}
