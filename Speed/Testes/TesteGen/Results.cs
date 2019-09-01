using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Speed;

namespace TesteGen
{

    public class Results : Dictionary<string, long>
    {

        string title;

        public Results(string title)
            : base(StringComparer.InvariantCultureIgnoreCase)
        {
            this.title = title;
        }

        public string Report()
        {
            StringBuilder b = new StringBuilder();
            if (this.Count == 0)
                b.ToString();

            var keys = (from c in this orderby c.Value select c.Key).ToArray();
            var minKey = keys[0];
            var minVal = this[minKey];

            foreach (var key in keys)
            {
                var value = this[key];
                b.AppendFormat("{0} - {1} - {2} - {3}\r\n", key.PadRight(30), S(value), TimeSpan.FromMilliseconds(value).ToString().Left(11), P(1.0 * value / minVal));
            }

            if (this.Count > 1)
            {
                b.AppendLine("Faster: " + minKey);
                b.AppendLine();

                // mostra os 2 mais rápidos
                for (int i = 1; i < this.Count; i++)
                    b.AppendLine(Report(keys[0], keys[i]));

                if (this.Count > 2)
                {
                    b.AppendLine();
                    for (int i = 2; i < this.Count; i++)
                        b.AppendLine(Report(keys[1], keys[i]));
                }
                b.AppendLine(new string('=', 80));
            }
            return b.ToString();
        }

        private string Report(string key1, string key2)
        {
            var val1 = Val(key1);
            var val2 = Val(key2);

            // se não achou
            if (val1 * val2 == 0)
                return null;

            double gt = Math.Max(val1, val2);
            double lt = Math.Min(val1, val2);

            return string.Format("'{0}' was {1:F2}% faster then '{2}'",
                (val1 < val2 ? key1 : key2),
                (gt / lt - 1) * 100,
                (val1 > val2 ? key1 : key2));
        }

        public double Val(string key)
        {
            var value = (from c in this where c.Key.ToLower().Contains(key.ToLower()) select 1.0 * c.Value).FirstOrDefault();
            return value;
        }

        static string S(double value)
        {
            return value.ToString("# ms").PadLeft(12);
        }

        static string P(double value)
        {
            return value.ToString("P2").PadLeft(12);
        }


    }

}
