using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Spatial;

namespace FairyTale.Models
{

    public partial class Favorite
    {
        [Key]
        [Column(Order = 0)]
        public string UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TalesId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public virtual Tale Tales { get; set; }
    }
}
