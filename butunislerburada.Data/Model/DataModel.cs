using PagedList;
using butunislerburada.Data.Entity;
using System.Collections.Generic;
using System.Web;

namespace butunislerburada.Data.Model
{
    public class DataModel
    {
        public Job Job { get; set; }
        public List<Job> Jobs { get; set; }
        public IPagedList<Job> JobsPaged { get; set; }
    }
}
