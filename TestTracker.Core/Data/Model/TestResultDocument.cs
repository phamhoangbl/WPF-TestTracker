using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTracker.Core.Data.Model
{
    [Table("TestResultDocument")]
    public class TestResultDocument
    {
        [Key]
        public int TestDocumentResultId { get; set; }
        public int TestQueueId { get; set; }
        public string FolderLocation { get; set; }
        public string FileName { get; set; }
    }
}
