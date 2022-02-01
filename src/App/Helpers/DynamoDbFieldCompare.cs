using Amazon.DynamoDBv2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Helpers
{
    public class DynamoDbFieldCompare
    {
        public string FieldName { get; set; }
        public bool Asc { get; set; }

        public DynamoDbFieldCompare(string fieldName, bool asc)
        {
            this.FieldName = fieldName;
            this.Asc = asc;
        }

        public int CompareN(Dictionary<string, AttributeValue> x,
            Dictionary<string, AttributeValue> y)
        {
            int result = 0;

            if (Asc)
                result = double.Parse(x[FieldName].N).CompareTo(double.Parse(y[FieldName].N));
            else
                result = double.Parse(y[FieldName].N).CompareTo(double.Parse(x[FieldName].N));

            return result;
        }
    }
}
