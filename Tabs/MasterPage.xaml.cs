using System.Collections.Generic;
using Xamarin.Forms;

namespace Tabs
{


    public partial class MasterPage : ContentPage
    {
        public ListView ListView { get { return listView; } }

        public MasterPage()
        {
            InitializeComponent();

            var masterPageItems = new List<MasterPageItem>();
            masterPageItems.Add(new MasterPageItem
            {
                Title = "PC Vision",
                TargetType = typeof(PCVision)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "Database",
                TargetType = typeof(AzureTable)
            });
            masterPageItems.Add(new MasterPageItem
            {
                Title = "About me",
                TargetType = typeof(Info)
            });



            listView.ItemsSource = masterPageItems;
        }
    }
}