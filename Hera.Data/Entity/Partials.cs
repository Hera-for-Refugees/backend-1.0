using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hera.Data.Entity
{
    #region Category
    public partial class Category
    {
        [NotMapped]
        public string ParentCategory { get; set; }
    }
    #endregion

    #region Location
    [MetadataType(typeof(SLocation_Country_Metadata))]
    public partial class SLocation_Country
    {
        internal sealed class SLocation_Country_Metadata
        {
            [Required(ErrorMessage = "Country name is required !")]
            public string Name { get; set; }
            [Required(ErrorMessage = "Country phone code is required !")]
            public string PhoneCode { get; set; }
        }
    }

    [MetadataType(typeof(SLocation_City_Metadata))]
    public partial class SLocation_City
    {
        internal sealed class SLocation_City_Metadata
        {
            [Required(ErrorMessage = "City name is required !")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Country is required !")]
            public int CountryId { get; set; }
        }
    }

    [MetadataType(typeof(SLocation_Region_Metadata))]
    public partial class SLocation_Region
    {
        internal sealed class SLocation_Region_Metadata
        {
            [Required(ErrorMessage = "Region name is required !")]
            public string Name { get; set; }

            [Required(ErrorMessage = "City is required !")]
            public int CityId { get; set; }
        }
    }
    #endregion
     
    #region CategoryQuestionAnser
    [MetadataType(typeof(Category_Question_Answer_Metadata))]
    public partial class Category_Question_Answer
    {
        internal sealed class Category_Question_Answer_Metadata
        {
            [Required]
            public int DataTypeId { get; set; }
        }
    }
    #endregion
}
