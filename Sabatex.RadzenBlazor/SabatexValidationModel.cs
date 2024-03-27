using Sabatex.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

//namespace Sabatex.RadzenBlazor
//{
//    public class SabatexValidationModel<TItem> where TItem : class
//    {
//        /// <summary>
//        /// Hormal result
//        /// </summary>
//        /// <param name="statusCode"></param>
//        /// <param name="item"></param>
//        public SabatexValidationModel(TItem item)
//        {
//            Item = item;
//        }

//        public SabatexValidationModel(string error)
//        {
//            ValidationsErrors = new Dictionary<string, List<string>>
//            {
//                { string.Empty, [error] }
//            };
//        }

//        public SabatexValidationModel(Dictionary<string, List<string>> validationsErrors)
//        {
//            ValidationsErrors = validationsErrors;
//        }
//        public SabatexValidationModel(Dictionary<string, List<string>> validationsErrors,string error)
//        {
//            ValidationsErrors = validationsErrors;

//        }

//        public TItem? Item { get; set; }
//        public Dictionary<string,List<string>>? ValidationsErrors { get; set; }


//    }
//}
