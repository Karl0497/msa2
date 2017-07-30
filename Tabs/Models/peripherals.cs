using System;
using Newtonsoft.Json;

namespace Tabs
{
    public class peripherals
    {
		[JsonProperty(PropertyName = "Id")]
		public string ID { get; set; }

		[JsonProperty(PropertyName = "Tag")]
        public string Tag { get; set; }

		[JsonProperty(PropertyName = "Probability")]
		public float Probability { get; set; }

     
    }
}
