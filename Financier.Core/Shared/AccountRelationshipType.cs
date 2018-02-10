using System;

namespace Financier
{
    public enum AccountRelationshipType
    {
        PhysicalToLogical,
        PrepaymentToExpense
    }

    public static class AccountRelationshipTypeExtensions
    {
        public static string ToReadableString(this AccountRelationshipType? type)
        {
            if (type.HasValue)
            {
                switch (type)
                {
                    case AccountRelationshipType.PhysicalToLogical:
                        return "Physical to logical";
                    case AccountRelationshipType.PrepaymentToExpense:
                        return "Prepayment to expense";
                    default:
                        throw new ArgumentException(nameof(type), "Unknown AccountRelationshipType value");
                }
                    
            }
            else
            {
                return "(All Types)";
            }
        }
    }
}
