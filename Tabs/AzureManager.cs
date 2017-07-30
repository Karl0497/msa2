using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;

namespace Tabs
{
    public class AzureManager
	{

		private static AzureManager instance;
		private MobileServiceClient client;
		private IMobileServiceTable<peripherals> pTable;

		private AzureManager()
		{
			this.client = new MobileServiceClient("https://pc-peripheral-checker.azurewebsites.net");
            this.pTable = this.client.GetTable<peripherals>();
		}

		public MobileServiceClient AzureClient
		{
			get { return client; }
		}

		public static AzureManager AzureManagerInstance
		{
			get
			{
				if (instance == null)
				{
					instance = new AzureManager();
				}

				return instance;
			}
		}

		public async Task<List<peripherals>> getPeripheralInfo()
		{
			return await this.pTable.ToListAsync();
		}

        public async Task postPeripheralInfo(peripherals peripherals)
		{
			await this.pTable.InsertAsync(peripherals);
		}
	}
}
