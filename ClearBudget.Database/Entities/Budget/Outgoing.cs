namespace PersonalFinance.Models
{
    public enum OutgoingCategoryType
    {
        HouseholdAndLiving = 1,   
        TransportAndVehicle = 2,  
        FinancialCommitments = 3, 
        SubscriptionsAndServices = 4, 
        PersonalAndLifestyle = 5, 
        HealthAndWellbeing = 6,   
        FamilyAndChildcare = 7,   
        SavingsAndInvestments = 8,
        Miscellaneous = 99       
    }

    public enum TransactionTypeEnum
    {
        Unknown,
        Income,
        Outcome,
        Interest,
        Tax
    }

    public enum TransactionRecurringTypeEnum
    {
        None,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Custom
    }

    public class OutgoingCategory
    {
        public int UserId { get; set; }
        public OutgoingCategoryType CategoryType { get; set; }

        // Optional custom label if Miscellaenous
        public string DisplayName { get; set; }  

        // Navigation
        public ICollection<OutgoingItem> Items { get; set; }
    }

    public class OutgoingItem
    {
        public int OutgoingItemId { get; set; }
        public int OutgoingCategoryId { get; set; }

        public string ItemName { get; set; }   // e.g. Rent, Netflix, Fuel

        //work values off based on recurring. If monthly work yearly and weekly out ect
        public TransactionRecurringTypeEnum Recurring { get; set; }
        public decimal YearlyAmount { get; set; }
        public decimal MonthlyAmount { get; set; }
        public decimal WeeklyAmount { get; set; }

        public int Rank { get; set; }
        public bool? Essential { get; set; }
    }
}
