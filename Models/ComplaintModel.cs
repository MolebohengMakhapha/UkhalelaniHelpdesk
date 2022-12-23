using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;


namespace Ukhalelani_Helpdesk.Models
{
    public class ComplaintModel
    {


        [Key]
        [Display(Name = "Reference Number")]
        public string complaintID { get; set; }



        [Required(ErrorMessage = "ComplaintType is required.")]

        [Display(Name = "Complaint Type")]


        public ComplaintTypes ComplaintType { get; set; }
        public enum ComplaintTypes
        {
            [Display(Name = "Road")]
            Road = 0,
            [Display(Name = "Water Leakage")]
            WaterLeakage = 1,
            [Display(Name = "Waste Removal")]
            WasteRemoval = 2,
            [Display(Name = "Streets light")]
            StreetLight = 3,
        }





        [Required(ErrorMessage = "Complainant Name is required.")]

        [Display(Name = "Complainant Name and Surname")]
        public string Name { get; set; }




        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Date is required.")]
        [Display(Name = "Date")]
        public DateTime ComDate { get; set; }




        [Required(ErrorMessage = "Street Name is required.")]
        [StringLength(50, MinimumLength = 2)]
        [Display(Name = "Street Name")]
        public string Street { get; set; }



        [Required(ErrorMessage = "City is required.")]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "City Name is required.")]
        [Display(Name = "City")]
        public string City { get; set; }




        [Required(ErrorMessage = "Province is required.")]
        [StringLength(50, MinimumLength = 2)]
        [RegularExpression(@"^([a-zA-Z0-9 .&'-]+)$", ErrorMessage = "Province Name is required.")]
        [Display(Name = "Province")]
        public string Province { get; set; }

        
        [Display(Name = "Status")]
        public Statuses Status { get; set; }

        public enum Statuses
        {
            [Display(Name = "Pending")]
            Pending = 0,
            [Display(Name = "Complete")]
            Complete = 1,
        }


        [Required(ErrorMessage = "Postal Code is required.")]
        [Display(Name = "Postal Code")]
        [RegularExpression("^[0-9]+$", ErrorMessage = "Only numbers are allowed")]
        public long PostalCode { get; set; }


        [Display(Name = "Description")]
        public string Description { get; set; }

        public byte[] Image { get; set; }

        [Required(ErrorMessage = "Response is required.")]
        [Display(Name = "Response")]
        public string Response { get; set; }

    }
}