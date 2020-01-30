using App.Helpers;
using PayPal.Api;
using System.ComponentModel.DataAnnotations;

namespace App.ViewModels.Client.Customer
{
    public class RegisterMembershipViewModel
    {
        [Required]
        [EnumDataType(typeof(MembershipTypeEnum))]
        public int MembershipType { get; set; }

        public enum MembershipTypeEnum
        {
            Monthly = CommonConstants.MONTHLY,
            Annual = CommonConstants.ANNUAL,
        }

        public string PaymentMethod { get; set; }

        public CreditCard CreditCard { get; set; }
    }
}