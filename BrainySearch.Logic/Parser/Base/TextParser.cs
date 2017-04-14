using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Parser.Base
{
    public abstract class TextParser : IParser<string>
    {
        public abstract string Parse(string html);
    }
}
