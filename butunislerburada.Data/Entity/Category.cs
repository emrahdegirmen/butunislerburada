﻿using System;

namespace butunislerburada.Data.Entity
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string SimilarName { get; set; }
        public string Description { get; set; }
        public string Detail { get; set; }
        public string Link { get; set; }
        public string SimilarNameLink { get; set; }
        public DateTime CreatedDate { get; set; }
        public int StatusID { get; set; }
    }
}
