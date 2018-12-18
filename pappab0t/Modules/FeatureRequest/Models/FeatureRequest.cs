using System;
using System.Collections.Generic;

namespace pappab0t.Models
{
    public class FeatureRequest
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public string Text { get; set; }
        public List<object> Comments { get; set; }
    }
}
