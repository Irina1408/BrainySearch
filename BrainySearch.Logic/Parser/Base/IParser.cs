using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainySearch.Logic.Parser.Base
{
    /// <summary>
    /// Html page parser
    /// </summary>
    public interface IParser<TParserResult>
        //where TParserResult : IParserResult
    {
        /// <summary>
        /// Parse html and returns result
        /// </summary>
        TParserResult Parse(string html);
    }
}
