using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace ColdWatersMod
{
    internal class JsonBuilder
    {
        private StringBuilder sb;
        private bool isInsert;

        public JsonBuilder()
        {
            this.sb = new StringBuilder();
            this.isInsert = false;
        }

        public JsonBuilder Add(string name, string value)
        {
            if (this.isInsert)
            {
                this.sb.Append(", ");
            }
            else
            {
                this.isInsert = true;
            }

            this.sb.Append('"');
            this.EscapeJsonString(name, this.sb);
            this.sb.Append("\":");

            if (value == null)
            {
                this.sb.Append("null");
            }
            else
            {
                this.sb.Append('"');
                this.EscapeJsonString(value, this.sb);
                this.sb.Append('"');
            }

            return this;
        }

        public JsonBuilder Add(string name, bool value)
        {
            if (this.isInsert)
            {
                this.sb.Append(", ");
            }
            else
            {
                this.isInsert = true;
            }

            this.sb.Append('"');
            this.EscapeJsonString(name, this.sb);
            this.sb.Append("\":");

            this.sb.Append(value ? "true" : "false");

            return this;
        }

        public JsonBuilder Add(string name, bool? value)
        {
            if (value.HasValue)
            {
                return this.Add(name, value.Value);
            }
            else
            {
                return this.AddNull(name);
            }
        }

        public JsonBuilder Add(string name, int value)
        {
            if (this.isInsert)
            {
                this.sb.Append(", ");
            }
            else
            {
                this.isInsert = true;
            }

            this.sb.Append('"');
            this.EscapeJsonString(name, this.sb);
            this.sb.Append("\":");

            this.sb.Append(value);

            return this;
        }

        public JsonBuilder Add(string name, int? value)
        {
            if (value.HasValue)
            {
                return this.Add(name, value.Value);
            }
            else
            {
                return this.AddNull(name);
            }
        }

        public JsonBuilder Add(string name, float value)
        {
            if (this.isInsert)
            {
                this.sb.Append(", ");
            }
            else
            {
                this.isInsert = true;
            }

            this.sb.Append('"');
            this.EscapeJsonString(name, this.sb);
            this.sb.Append("\":");
            this.sb.Append(value.ToString(CultureInfo.InvariantCulture));

            return this;
        }

        public JsonBuilder Add(string name, float? value)
        {
            if (value.HasValue)
            {
                return this.Add(name, value.Value);
            }
            else
            {
                return this.AddNull(name);
            }
        }

        public JsonBuilder AddNull(string name)
        {
            if (this.isInsert)
            {
                this.sb.Append(", ");
            }
            else
            {
                this.isInsert = true;
            }

            this.sb.Append('"');
            this.EscapeJsonString(name, this.sb);
            this.sb.Append("\":null");

            return this;
        }

        public string ToJson()
        {
            return string.Concat("{", this.sb.ToString(), "}");
        }

        public override string ToString()
        {
            return this.ToJson();
        }

        private void EscapeJsonString(string s, StringBuilder outSb)
        {
            for (int i = 0; i < s.Length; i += 1)
            {
                char c = s[i];
                switch (c)
                {
                    case '\\':
                    case '"':
                        outSb.Append('\\');
                        outSb.Append(c);
                        break;
                    case '/':
                        outSb.Append('\\');
                        outSb.Append(c);
                        break;
                    case '\b':
                        outSb.Append("\\b");
                        break;
                    case '\t':
                        outSb.Append("\\t");
                        break;
                    case '\n':
                        outSb.Append("\\n");
                        break;
                    case '\f':
                        outSb.Append("\\f");
                        break;
                    case '\r':
                        outSb.Append("\\r");
                        break;
                    default:
                        if (c < ' ')
                        {
                            string t = string.Concat("000", string.Format("{0:X}", c));
                            outSb.Append("\\u" + t.Substring(t.Length - 4));
                        }
                        else
                        {
                            outSb.Append(c);
                        }
                        break;
                }
            }
        }
    }
}
