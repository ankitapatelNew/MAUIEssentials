namespace MAUIEssentials.AppCode.Helpers
{
    public static class BadgeCounterService
    {
        private static TabCounter _tabCounter;
        public static TabCounter TabCounter
        {
            get => _tabCounter;
            private set
            {
                _tabCounter = value;
                CountChanged?.Invoke(null, _tabCounter);
            }
        }

        public static void SetCount(TabCounter tabCounter) => TabCounter = tabCounter;


        public static event EventHandler<TabCounter> CountChanged;
    }

    public class TabCounter
    {
        public int TabNumber { get; set; }
        public string BadgeText { get; set; }
    }
}