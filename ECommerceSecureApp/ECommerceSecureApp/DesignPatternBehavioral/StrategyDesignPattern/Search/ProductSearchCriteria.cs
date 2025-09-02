namespace ECommerceSecureApp.BehavioralDesignPattern.StrategyDesignPattern.Search
{
    public class ProductSearchCriteria
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

    }
}
