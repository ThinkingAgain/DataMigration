using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace MigDashboard.Models
{
    [Table("query_config")]
    public class QueryConfig
    {
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        [Display(Name = "任务")]
        public string? Name { get; set; }
        [Required]
        [Column("mysql_db")]
        [StringLength(100)]
        [Display(Name = "Mysql数据库名")]
        public string? MysqlDB { get; set; }
        [Required]
        [Column("oracle_table")]
        [StringLength(100)]
        [Display(Name = "Oracle表名")]
        public string? OracleTable { get; set; }
        [Required]
        [Column("mysql_sql")]
        [StringLength(3600)]
        [Display(Name = "Mysql执行语句")]
        public string? MysqlSQL { get; set; }
        [StringLength(256)]
        [Display(Name = "说明")]
        public string? Memo { get; set; }
    }
}
