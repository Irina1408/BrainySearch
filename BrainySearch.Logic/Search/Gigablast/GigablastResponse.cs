using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BrainySearch.Logic.Search.Gigablast
{
    public class GigablastResponse
    {
        /// <summary>
        /// This is zero on a successful query. Otherwise it will be a non-zero number indicating the error code.
        /// "statusCode":0
        /// </summary>
        public int statusCode { get; set; }

        /// <summary>
        /// Similar to above, this is "Success" on a successful query. Otherwise it will indicate an error message corresponding to the statusCode above.
        /// "statusMsg":"Success"
        /// </summary>
        public string statusMsg { get; set; }

        /// <summary>
        /// This is the current time in UTC in unix timestamp format (seconds since the epoch) that the server has when generating this JSON response.
        /// "currentTimeUTC":1404588231
        /// </summary>
        public long currentTimeUTC { get; set; }

        /// <summary>
        /// This is how long it took in milliseconds to generate the JSON response from reception of the request.
        /// "responseTimeMS":312
        /// </summary>
        public int responseTimeMS { get; set; }

        /// <summary>
        /// This is how many matches were excluded from the search results because they were considered duplicates, banned, 
        /// had errors generating the summary, or were from an over-represented site. To show them use the &sc &dr &pss &sb and &showerrors input parameters described above.
        /// "numResultsOmitted":3
        /// </summary>
        public int numResultsOmitted { get; set; }

        // This is how many shards failed to 
        // return results. Gigablast gets results 
        // from multiple shards (computers) and 
        // merges them to get the final result 
        // set. Some times a shard is down or 
        // malfunctioning so it will not 
        // contribute to the results. So If this 
        // number is non-zero then you had such a 
        // shard.
        // "numShardsSkipped":0
        public int numShardsSkipped { get; set; }

        // This is how many shards are ideally 
        // in use by Gigablast to generate search 
        // results.
        // "totalShards":159
        public int totalShards { get; set; }

        // This is how many total documents are 
        // in the collection being searched.
        // "docsInCollection":226
        public int docsInCollection { get; set; }

        // This is how many of those documents 
        // matched the query.
        // "hits":193
        public int hits { get; set; }

        // This is 1 if more search results are 
        // available, otherwise it is 0.
        // "moreResultsFollow":1
        public bool moreResultsFollow { get; set; }

        // Start of query-based information
        public QueryInfo queryInfo { get; set; }

        // The start of the gigabits array. 
        // Each gigabit is mined from the content 
        // of the search results. The top N 
        // results are mined, and you can control 
        // N with the &dsrt input parameter 
        // described above.
        public Gigabit[] gigabits { get; set; }

        // Start of the facets array, if any.
        public Facet[] facets { get; set; }

        // Start of the JSON array of 
        // individual search results.
        public Result[] results { get; set; }
    }

    // Start of query-based information.
    public class QueryInfo
    {

        // The entire query that was received, 
        // represented as a single string.
        // "fullQuery":"test"
        public string fullQuery { get; set; }

        // The language of the query. This is 
        // the 'preferred' language of the search 
        // results. It is reflecting the &qlang 
        // input parameter described above. Search 
        // results in this language (or an unknown 
        // language) will receive a large boost. 
        // The boost is multiplicative. The 
        // default boost size can be overridden 
        // using the &langw input parameter 
        // described above. This language 
        // abbreviation here is usually 2 letter, 
        // but can be more, like in the case of 
        // zh-cn, for example.
        // "queryLanguageAbbr":"en"
        public string queryLanguageAbbr { get; set; }

        // The language of the query. Just 
        // like above but the language is spelled 
        // out. It may be multiple words.
        // "queryLanguage":"English",
        public string queryLanguage { get; set; }

        // List of space separated words in 
        // the query that were ignored for the 
        // most part. Because they were common 
        // words for the query language they are 
        // in.
        // "ignoredWords":"to the",
        public string ignoredWords { get; set; }

        // There is a maximum limit placed on 
        // the number of query terms we search on 
        // to keep things fast. This can be 
        // changed in the search controls.
        // "queryNumTermsTotal":52,
        public int queryNumTermsTotal { get; set; }
        // "queryNumTermsUsed":20,
        public int queryNumTermsUsed { get; set; }
        // "queryWasTruncated":1,
        public int queryWasTruncated { get; set; }

        // The start of the terms array. Each 
        // query is broken down into a list of 
        // terms. Each term is described here.
        public Term[] terms { get; set; }

    }

    public class Term
    {

        // The term number, starting at 0.
        // "termNum":0
        public int termNum { get; set; }

        // The term as a string.
        // "termStr":"test"
        public string termStr { get; set; }

        // The language the term is from, in 
        // the case of query expansion on the 
        // original query term. Gigablast tries to 
        // find multiple forms of the word that 
        // have the same essential meaning. It 
        // uses the specified query language 
        // (&qlang), however, if a query term is 
        // from a different language, then that 
        // language will be implied for query 
        // expansion.
        // "termLang":"en"
        public string termLang { get; set; }

        // The query term that this term is a 
        // form of.
        // "synonymOf":"test"
        public string synonymOf { get; set; }

        // The term frequency. An estimate of 
        // how many pages in the collection 
        // contain the term. Helps us weight terms 
        // by popularity when scoring the results.
        // "termFreq":425239458
        public long termFreq { get; set; }

        // A 48-bit hash of the term. Used to 
        // represent the term in the index.
        // "termHash48":67259736306430
        public long termHash48 { get; set; }

        // A 64-bit hash of the term.
        // "termHash64":9448336835959712000
        public string termHash64 { get; set; }

        // If the term has a field, like the 
        // term title:cat, then what is the hash 
        // of the field. In this example it would 
        // be the hash of 'title'. But for the 
        // query 'test' there is no field so it is 
        // 0.
        // "prefixHash64":0
        public long prefixHash64 { get; set; }
    }

    public class Gigabit
    {
        // The gigabit as a string in utf8.
        // "term":"Membership"
        public string term { get; set; }

        // The numeric score of the gigabit.
        // "score":240,
        public int score { get; set; }

        // The popularity ranking of the 
        // gigabit. Out of 10000 random documents, 
        // how many documents contain it?
        // "minPop":480,
        public int minPop { get; set; }

        // The gigabit in the context of a 
        // document.
        public Instance instance { get; set; }

    }

    public class Instance
    {
        // A sentence, if it exists, from one 
        // of the search results which also 
        // contains the gigabit and as many 
        // significant query terms as possible. In 
        // UTF-8.
        // "sentence":"Get a free Tested Premium Membership here!"
        public string sentence { get; set; }

        // The url that contained that 
        // sentence. Always starts with http.
        // "url":"http://www.tested.com/"
        public string url { get; set; }

        // The domain of that url.
        // "domain":"tested.com"
        public string domain { get; set; }
    }

    public class Facet
    {
        // The field you are faceting over
        // "field":"Company"
        public string field { get; set; }

        // How many documents in the 
        // collection had this particular field? 
        // 64-bit integer.
        // "totalDocsWithField":148553
        public long totalDocsWithField { get; set; }

        // How many documents in the 
        // collection had this particular field 
        // with the same value as the value line 
        // directly below? This should always be 
        // less than or equal to the 
        // totalDocsWithField count. 64-bit 
        // integer.
        // "totalDocsWithFieldAndValue":44184
        public long totalDocsWithFieldAndValue { get; set; }

        // The value of the field in the case 
        // of this facet. Can be a string or an 
        // integer or a float, depending on the 
        // type described in the gbfacet query 
        // term. i.e. gbfacetstr, gbfacetint or 
        // gbfacetfloat.
        // "value":"Widgets, Inc.",
        public string value { get; set; }

        // Should be the same as 
        // totalDocsWithFieldAndValue, above. 
        // 64-bit integer.
        // "docCount":44184
        public long docCount { get; set; }
    }

    public class Result
    {

        // The title of the result. In UTF-8.
        // "title":"This is the title."
        public string title { get; set; }

        // A DMOZ entry. One result can have 
        // multiple DMOZ entries.
        public DmozEntry dmozEntry { get; set; }

        // The content type of the url. Can be 
        // html, pdf, text, xml, json, doc, xls or 
        // ps.
        // "contentType":"html"
        public string contentType { get; set; }

        // The summary excerpt of the result. 
        // In UTF-8.
        // "sum":"Department of the Interior protects America's natural resources."
        public string sum { get; set; }

        // The url of the result. If it starts 
        // with http:// then that is omitted. Also 
        // omits the trailing / if the urls is 
        // just a domain or subdomain on the root 
        // path.
        // "url":"www.doi.gov",
        public string url { get; set; }

        // The hopcount of the url. The 
        // minimum number of links we would have 
        // to click to get to it from a root url. 
        // If this is 0 that means the url is a 
        // root url, like http://www.root.com/.
        // "hopCount":0
        public int hopCount { get; set; }

        // The size of the result's content. 
        // Always in kilobytes. k stands for 
        // kilobytes. Could be a floating point 
        // number or and integer.
        // "size":"  64k"
        public string size { get; set; }

        // The exact size of the result's 
        // content in bytes.
        // "sizeInBytes":64560
        public int sizeInBytes { get; set; }

        // The unique document identifier of 
        // the result. Used for getting the cached 
        // content of the url.
        // "docId":34111603247
        public long docId { get; set; }

        // The site the result comes from. 
        // Usually a subdomain, but can also 
        // include part of the URL path, like, 
        // abc.com/users/brad/. A site is a set of 
        // web pages controlled by the same 
        // entity.
        // "site":"www.doi.gov"
        public string site { get; set; }

        // The time the url was last INDEXED. 
        // If there was an error or the url's 
        // content was unchanged since last 
        // download, then this time will remain 
        // unchanged because the document is not 
        // reindexed in those cases. Time is in 
        // unix timestamp format and is in UTC.
        // "spidered":1404512549
        public long spidered { get; set; }

        // The first time the url was 
        // successfully INDEXED. Time is in unix 
        // timestamp format and is in UTC.
        // "firstIndexedDateUTC":1404512549
        public long firstIndexedDateUTC { get; set; }

        // A 32-bit hash of the url's content. 
        // It is used to determine if the content 
        // changes the next time we download it.
        // "contentHash32":2680492249
        public long contentHash32 { get; set; }

        // The dominant language that the 
        // url's content is in. The language name 
        // is spelled out in its entirety.
        // "language":"English"
        public string language { get; set; }

        // A convenient abbreviation of the 
        // above language. Most are two 
        // characters, but some, like zh-cn, are 
        // more.
        // "langAbbr":"en"
        public string langAbbr { get; set; }

        // If the result has an associated 
        // image then the image thumbnail is 
        // encoded in base64 format here. It is a 
        // jpg image.
        // "imageBase64":"/9j/4AAQSkZJR...",
        public string imageBase64 { get; set; }

        // If the result has an associated 
        // image then what is its height and width 
        // of the above jpg thumbnail image in 
        // pixels?
        // "imageHeight":223,
        public int imageHeight { get; set; }

        // "imageWidth":350,
        public int imageWidth { get; set; }

        // If the result has an associated 
        // image then what are the dimensions of 
        // the original image in pixels?
        // "origImageHeight":300,
        // "origImageWidth":470
        public int origImageHeight { get; set; }
        public int origImageWidth { get; set; }
    }

    public class DmozEntry
    {
        // The DMOZ category ID.
        // "dmozCatId":374449,
        public int dmozCatId { get; set; }

        // The DMOZ direct category ID.
        // "directCatId":1
        public int directCatId { get; set; }

        // The DMOZ category as a UTF-8 
        // string.
        // "dmozCatStr":"Top: Computers: Security: Malicious 
        //         Software: Viruses: Detection and Removal Tools: 
        //         Reviews"
        public string dmozCatStr { get; set; }

        // What title some DMOZ editor gave 
        // to this url.
        // "dmozTitle":"The DMOZ Title"
        public string dmozTitle { get; set; }

        // What summary some DMOZ editor gave 
        // to this url.
        // "dmozSum":"A great web page.",
        public string dmozSum { get; set; }

        // The DMOZ anchor text, if any.
        // "dmozAnchor":""
        public string dmozAnchor { get; set; }
    }
    
}
