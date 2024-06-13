//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using ToyoMcMfg.Staging.DataBaseConfig;

//namespace ToyoStageService
//{
//    public static class SQLSearchConditions
//    {
//        public static string Create(List<SqlSearchStringValue> searchKeyValues)
//        {
//            string CommandText = null;

//            foreach (var searchKeyValue in searchKeyValues)
//            {
//                var Field = searchKeyValue.Field;
//                var Ooperator = searchKeyValue.Ooperator;
//                var Value = searchKeyValue.Value;
//                var Logic = searchKeyValue.Logic;


//                var Operator_keylist = new List<string>() { "IS", "IS NOT", "LIKE", "<", "<=", ">", ">=", "=" };
//                if (string.IsNullOrWhiteSpace(Ooperator) == false && Operator_keylist.Contains(Ooperator.ToUpper().TrimStart().TrimEnd()) == false)
//                    throw new Exception($"SqlSearchStringValueオブジェクトの組立過程にて Ooperator キーワードが想定外です \"IS\", \"LIKE\", \"<\", \"<=\", \">\", \">=\", \"=\" のみ使用可能。 \"{Ooperator}\"");

//                var Logic_keylist = new List<string>() { "(", ")", "AND", "OR", "NOT" };
//                if (string.IsNullOrWhiteSpace(Logic) == false && Logic_keylist.Contains(Logic.ToUpper().TrimStart().TrimEnd()) == false)
//                    throw new Exception($"SqlSearchStringValueオブジェクトの組立過程にて Logic キーワードが想定外です \"(\", \")\", \"AND\", \"OR\", \"NOT\" のみ使用可能。 \"{Logic}\"");

//                if ((string.IsNullOrWhiteSpace(Field) == false) && (string.IsNullOrWhiteSpace(Ooperator) == false) && (string.IsNullOrWhiteSpace(Value) == false))
//                    CommandText += $"( {Field} {Ooperator} {Value} ) {Logic} ";
//                else if ((string.IsNullOrWhiteSpace(Field) == true) && (string.IsNullOrWhiteSpace(Ooperator) == true) && (string.IsNullOrWhiteSpace(Value) == true) && (string.IsNullOrWhiteSpace(Logic) == false))
//                    CommandText += $"{Logic} ";
//                else
//                    throw new Exception($"SqlSearchStringValueオブジェクトの組立過程にて想定外の組合せ。Field: \"{Field}\" Ooperator: \"{Ooperator}\" Value: \"{Value}\" Logic* \"{Logic}\"");
//            }
//            return CommandText;
//        }

//    }
//}
