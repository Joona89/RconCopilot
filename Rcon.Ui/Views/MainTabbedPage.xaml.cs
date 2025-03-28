﻿namespace Rcon.Ui.Views
{
    public partial class MainTabbedPage : TabbedPage
    {
        public MainTabbedPage()
        {
            InitializeComponent();

            // Open an initial server tab.
            AddNewServerTab();

            // Toolbar button to add new server tabs.
            ToolbarItems.Add(new ToolbarItem("Connect", null, () =>
            {
                AddNewServerTab();
            }));
        }

        private void AddNewServerTab()
        {
            var newServerTab = new ServerTab();
            newServerTab.Title = $"Server {Children.Count + 1}";
            Children.Add(newServerTab);
            CurrentPage = newServerTab;
        }
    }
}
