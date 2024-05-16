using System.Collections.Generic;

namespace AAI.DAL.Models
{
    public class ListBaseDto<T> where T : class
    {
        public List<T> lData { get; set; } = new List<T>();
        public int TotalRow { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
