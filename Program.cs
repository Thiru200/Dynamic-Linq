
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace MyConsoleApplication
{

    class Program
    {
        static void Main(string[] args)
        {
            List<MenuUsers> finalDataSet = new List<MenuUsers>();
            string result = string.Empty;
            //string MyRule = "(@FunctionalityID in (10) Or @Band > 1 Or @HasReportees = 1)";
            string MyRule = "(@FunctionalityID in (153,154,155) And @LocationId Not In (2,5,7,8))";
            //string MyRule = "@FunctionalityID in (153, 154, 155) and @Band > 2";
            //string MyRule = "(@LocationId not in (2,5,7,8)) and @FunctionalityID in (153)";
            //string MyRule = "@NT_UserName in ('ramya.p1','ravi.ramachandr','arulsajin.arula','shabnaoviya.m','anbu','babu.j')";
            //string MyRule = "@FunctionalityID in (153) and (@LocationId not in (7,8))";
            //string MyRule = "(@BAND>=3 OR @GRADE IN ('2B1','2B2','2M1','2M2','2IC E1','2IC E2')) or @FunctionalityID in (109,110,111)";
            result = MenuUsers.GetMenuCondition(MyRule);
            KeyValue KeyItem = MenuUsers.GetFilteredMenu(result);
            Console.WriteLine("The Operand" + KeyItem.operand);
            result = MenuUsers.GetReplacedConditionKey(KeyItem.firstFilter);
            result = MenuUsers.GetReplacedConditionValue(result);
            List<Filter> defaultFilters = MenuUsers.SplitKeyValue(result);
            Console.WriteLine(result);
            if (defaultFilters.Count > 0)
            {
                List<MenuUsers> dataSource = Program.DisplayUsers(defaultFilters, finalDataSet);
                foreach (var item in dataSource)
                {
                    Console.WriteLine($" Result is {item.NT_Username}");
                }
                Console.WriteLine(MyRule);
                if ((KeyItem.operand == "and" && dataSource.Count > 0) || (KeyItem.operand == "or" && dataSource.Count == 0))
                    if (KeyItem.secondFilter.Length != 0)
                    {
                        List<MenuUsers> data = new List<MenuUsers>();
                        while (KeyItem.secondFilter.Length != 0 && data.Count == 0)
                        {
                            Console.WriteLine("The First Filter" + KeyItem.firstFilter);
                            Console.WriteLine("The Second Filter" + KeyItem.secondFilter);
                            Console.WriteLine("The Operand" + KeyItem.operand);
                            result = MenuUsers.GetMenuCondition(KeyItem.secondFilter);
                            KeyItem = MenuUsers.GetFilteredMenu(result);
                            result = MenuUsers.GetReplacedConditionKey(KeyItem.firstFilter);
                            result = MenuUsers.GetReplacedConditionValue(result);
                            List<Filter> filters = MenuUsers.SplitKeyValue(result);
                            if (filters.Count > 0)
                            {
                                data = Program.DisplayUsers(filters, finalDataSet);
                                foreach (var item in data)
                                {
                                    Console.WriteLine($" Result is {item.NT_Username}");
                                }
                                Console.WriteLine(MyRule);
                            }
                        }
                    }
            }
        }
        public static List<MenuUsers> DisplayUsers(List<Filter> filters, List<MenuUsers> dataset)
        {
            List<MenuUsers> myData = new List<MenuUsers>();
            if (dataset.Count > 0)
            {
                foreach (var item in filters)
                {
                    Console.WriteLine("first");
                    Console.WriteLine($"Key is {item.PropertyName}, Operand is {item.Operation}, Operators is {item.Operators},Value is {item.Value}");
                    var deleg = ExpressionBuilder.GetExpression<MenuUsers>(item).Compile();
                    dataset = dataset.Where(deleg).ToList();
                    List<string> newUser = dataset.Select(r => r.NT_Username).ToList();
                    Console.WriteLine(dataset.Count);
                    Console.WriteLine(item.Operators);
                    if (dataset.Count > 0 && (item.Operators == "&" || item.Operators == "!" || item.Operators == "="))
                    {
                        List<MenuUsers> deletedUser = dataset.Where(r => !newUser.Contains(r.NT_Username)).ToList();
                        foreach (var user in deletedUser)
                        {
                            dataset.Remove(user);
                        }
                        foreach (var data in myData)
                        {
                            var match = dataset.Where(r => r.NT_Username != data.NT_Username).FirstOrDefault();
                            if (match == null)
                            {
                                dataset.Add(data);
                            }
                        }
                    }
                }
                return dataset;
            }
            else if (filters.Count > 0 && dataset.Count == 0)
            {
                foreach (var item in filters)
                {
                    Console.WriteLine("second");
                    Console.WriteLine($"Key is {item.PropertyName}, Operand is {item.Operation}, Operators is {item.Operators},Value is {item.Value}");
                    List<MenuUsers> menuUsers = MenuUsers.GetMenuUsers();
                    var deleg = ExpressionBuilder.GetExpression<MenuUsers>(item).Compile();
                    myData = menuUsers.Where(deleg).ToList();
                    Console.WriteLine("Operator" + item.Operators);
                    if (myData.Count > 0 && (item.Operators == "&" || item.Operators == "!" || item.Operators == "="))
                    {
                        foreach (var result in myData)
                        {
                            List<string> NTName = dataset.Select(r => r.NT_Username).ToList();
                            if (dataset.Count > 0 && !NTName.Contains(result.NT_Username))
                            {
                                dataset.Add(result);
                            }
                            if (dataset.Count == 0)
                            {
                                dataset.Add(result);
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine("Else Part");
                        foreach (var result in myData)
                        {
                            if (dataset.Count > 0 && dataset.Any(r => r.NT_Username != result.NT_Username))
                            {
                                Console.WriteLine("Count Increased" + dataset.Count);
                                dataset.Add(result);
                            }
                            else
                            {
                                dataset.Add(result);
                            }
                        }
                    }
                    if (myData.Count == 0)
                    {
                        Console.WriteLine("No data in filter");
                    }
                }
            }
            else
            {
                Console.WriteLine("no filter applied");
            }
            return dataset;
        }
    }
    public class KeyValue
    {
        public string operand { get; set; } = string.Empty;
        public string firstFilter { get; set; } = string.Empty;
        public string secondFilter { get; set; } = string.Empty;
    }
    public class MenuProperty
    {
        public string ColumnName { get; set; } = string.Empty;
        public string ColumnValue { get; set; } = string.Empty;
    }
    public class MenuUsers
    {
        // public string NT_Username { get; set; } = string.Empty;
        // public string FUNCTIONALITY_ID { get; set; } = string.Empty;
        // public string Band { get; set; } = string.Empty;
        // public string Grade { get; set; } = string.Empty;
        // public string CLIENT_ID { get; set; } = string.Empty;
        // public string LOB { get; set; } = string.Empty;
        // public string Designation_Id { get; set; } = string.Empty;
        // public string LocationID { get; set; } = string.Empty;
        // public string ACTIVE { get; set; } = string.Empty;
        // public string HasReportees { get; set; } = string.Empty;
        // public string Cont_EntityID { get; set; } = string.Empty;

        public string NT_Username { get; set; } = string.Empty;
        public int FUNCTIONALITY_ID { get; set; } = 0;
        public int Band { get; set; } = 0;
        public string Grade { get; set; } = string.Empty;
        public int CLIENT_ID { get; set; } = 0;
        public int LOB { get; set; } = 0;
        public int Designation_Id { get; set; } = 0;
        public int LocationID { get; set; } = 0;
        public int ACTIVE { get; set; } = 0;
        public int HasReportees { get; set; } = 0;
        public int Cont_EntityID { get; set; } = 0;

        public static List<string> Operators = new List<string>(){
            "&","!","{","<","}",">","=","<>"
        };
        public static List<MenuUsers> GetMenuUsers()
        {
            List<MenuUsers> menuUsers = new List<MenuUsers>()
            {
                    new MenuUsers(){
                        NT_Username="anbu",
                        FUNCTIONALITY_ID=154,
                        Band=1,
                        Grade="",
                        CLIENT_ID=16,
                        LOB=4,
                        Designation_Id=2589,
                        HasReportees=0,
                        Cont_EntityID=2,
                        LocationID=3,
                        ACTIVE=1
                    },
                    new MenuUsers(){
                        NT_Username="babu.j",
                        FUNCTIONALITY_ID=1,
                        Band=1,
                        Grade="2B1",
                        CLIENT_ID=16,
                        LOB=4,
                        Designation_Id=2589,
                        HasReportees=1,
                        Cont_EntityID=2,
                        LocationID=1,
                        ACTIVE=1
                    },
                    new MenuUsers(){
                        NT_Username="charile",
                        FUNCTIONALITY_ID=111,
                        Band=1,
                        Grade="21",
                        CLIENT_ID=16,
                        LOB=4,
                        Designation_Id=2589,
                        HasReportees=0,
                        Cont_EntityID=2,
                        LocationID=8,
                        ACTIVE=1
                    }
                    // new MenuUsers(){
                    //     NT_Username="Charles",
                    //     FUNCTIONALITY_ID=153,
                    //     Band=2,
                    //     Grade="2IC B1",
                    //     CLIENT_ID=16,
                    //     LOB=4,
                    //     Designation_Id=2589,
                    //     HasReportees=1,
                    //     Cont_EntityID=2,
                    //     LocationID=1,
                    //     ACTIVE=1
                    // },
                    // new MenuUsers(){
                    //     NT_Username="David",
                    //     FUNCTIONALITY_ID=153,
                    //     Band=3,
                    //     Grade="3IC B1",
                    //     CLIENT_ID=16,
                    //     LOB=4,
                    //     Designation_Id=2589,
                    //     HasReportees=1,
                    //     Cont_EntityID=3,
                    //     LocationID=1,
                    //     ACTIVE=1
                    // },
                    //  new MenuUsers(){
                    //     NT_Username="Elango",
                    //     FUNCTIONALITY_ID=153,
                    //     Band=3,
                    //     Grade="3IC B1",
                    //     CLIENT_ID=16,
                    //     LOB=4,
                    //     Designation_Id=2589,
                    //     HasReportees=1,
                    //     Cont_EntityID=3,
                    //     LocationID=1,
                    //     ACTIVE=1
                    // },
            };
            return menuUsers;
        }
        //1
        public static string GetMenuCondition(string condition)
        {
            string currentCondition = condition;
            if (currentCondition.StartsWith("("))
            {
                currentCondition = condition.Replace("(", "");
            }
            return currentCondition;

        }
        //2
        public static KeyValue GetFilteredMenu(string condition)
        {
            string returnMsg = condition;
            KeyValue MyItem = new KeyValue();

            if (condition.ToLower().IndexOf(" and ") != -1)
            {
                var Operand = "and";
                int currentIndex = condition.ToLower().IndexOf(" and ");
                returnMsg = condition.Substring(0, currentIndex);
                MyItem.operand = Operand;
                MyItem.firstFilter = returnMsg;
                MyItem.secondFilter = condition.Substring(currentIndex + Operand.Length + 2);
            }
            else if (condition.ToLower().IndexOf(" or ") != -1)
            {
                var Operand = "or";
                int currentIndex = condition.ToLower().IndexOf(" or ");
                returnMsg = condition.Substring(0, currentIndex);
                MyItem.operand = Operand;
                MyItem.firstFilter = returnMsg;
                MyItem.secondFilter = condition.Substring(currentIndex + Operand.Length + 2);
            }
            else
            {
                MyItem.firstFilter = returnMsg;
                MyItem.secondFilter = "";
            }
            return MyItem;
        }
        // public static List<KeyValue> GetFilteredMenu(string condition)
        // {
        //     List<KeyValue> keyValues = new List<KeyValue>();
        //     string returnMsg = condition;
        //     if (condition.ToLower().IndexOf("and") != -1)
        //     {
        //         KeyValue MyItem = new KeyValue();
        //         int currentIndex = condition.ToLower().IndexOf("and");
        //         returnMsg = condition.Substring(0, currentIndex);
        //         MyItem.key = "And";
        //         MyItem.value = returnMsg;
        //         keyValues.Add(MyItem);
        //     }
        //     else if (condition.ToLower().IndexOf("or") != -1)
        //     {
        //         KeyValue MyItem = new KeyValue();
        //         int currentIndex = condition.ToLower().IndexOf("or");
        //         returnMsg = condition.Substring(0, currentIndex);
        //         MyItem.key = "And";
        //         MyItem.value = returnMsg;
        //         keyValues.Add(MyItem);
        //     }
        //     return keyValues;
        // }
        //3
        public static string GetReplacedConditionValue(string condition)
        {
            Console.WriteLine("Inside " + condition);
            string replacedCondition = string.Empty;
            if (condition.ToLower().IndexOf("not in") != -1)
            {
                Console.WriteLine("inner " + condition);
                condition = ReplaceCaseInsensitive(condition, "not in", "!");
                Console.WriteLine(condition);
                return condition;
                //replacedCondition = Regex.Replace(condition, "not in", "&", RegexOptions.IgnoreCase);
                //replacedCondition = condition.ToLower().Replace("not in", "!");
            }
            if (condition.ToLower().IndexOf("in") != -1)
            {
                Console.WriteLine("innerss");
                condition = ReplaceCaseInsensitive(condition, "in", "&");
                return condition;
                //replacedCondition = Regex.Replace(condition, "in", "&", RegexOptions.IgnoreCase);
                // replacedCondition = condition.ToLower().Replace("in", "&");
            }
            if (condition.ToLower().IndexOf("<>") != 1)
            {
                condition = ReplaceCaseInsensitive(condition, "<>", "!");
                //replacedCondition = Regex.Replace(condition, "<>", "!", RegexOptions.IgnoreCase);
                //replacedCondition = condition.ToLower().Replace("<>", "!");
            }
            if (condition.ToLower().IndexOf(">=") != -1)
            {
                condition = ReplaceCaseInsensitive(condition, ">=", "}");
                //replacedCondition = Regex.Replace(condition, ">=", "}", RegexOptions.IgnoreCase);
                //replacedCondition = condition.ToLower().Replace(">=", "}");
            }
            if (condition.ToLower().IndexOf("<=") != -1)
            {
                condition = ReplaceCaseInsensitive(condition, "<=", "{");
                //replacedCondition = Regex.Replace(condition, "<=", "{}", RegexOptions.IgnoreCase);
                //replacedCondition = condition.ToLower().Replace("<=", "{");
            }
            Console.WriteLine("The Replaced Condition " + condition);
            //Console.WriteLine("The Replaced ConditionIndex " + condition.ToLower().IndexOf("in"));
            return condition;
        }
        //4
        public static string GetReplacedConditionKey(string condition)
        {
            string replacedCondition = string.Empty;
            if (condition.Contains("@FunctionalityID"))
            {
                replacedCondition = condition.Replace("@FunctionalityID", "FUNCTIONALITY_ID");
            }
            else if (condition.ToLower().Contains("@hasreportees"))
            {
                replacedCondition = condition.Replace("@HasReportees", "HasReportees");
            }
            else if (condition.ToLower().Contains("@band"))
            {
                replacedCondition = condition.Replace("@Band", "Band");
            }
            else if (condition.ToLower().Contains("@grade"))
            {
                replacedCondition = condition.Replace("@Grade", "Grade");
            }
            else if (condition.ToLower().Contains("@client_id"))
            {
                replacedCondition = condition.Replace("@Client_Id", "CLIENT_ID");
            }
            else if (condition.ToLower().Contains("@lob"))
            {
                replacedCondition = condition.Replace("@LOB", "LOB");
            }
            else if (condition.ToLower().Contains("@designation_id"))
            {
                replacedCondition = condition.Replace("@Designation_Id", "Designation_Id");
            }
            else if (condition.ToLower().Contains("@locationid"))
            {
                replacedCondition = condition.Replace("@LocationId ", "LocationID ");
            }
            else if (condition.ToLower().Contains("@active"))
            {
                replacedCondition = condition.Replace("@Active", "ACTIVE");
            }
            else if (condition.ToLower().Contains("@cont_entityid"))
            {
                replacedCondition = condition.Replace("@Cont_EntityID", "Cont_EntityID");
            }
            else if (condition.ToLower().Contains("@nt_username"))
            {
                replacedCondition = condition.Replace("@NT_UserName", "NT_USERNAME");
            }
            else
            {
                replacedCondition = "";
            }
            return replacedCondition;
        }
        //5
        public static List<Filter> SplitKeyValue(string condition)
        {
            Console.WriteLine("The Condition" + condition);
            List<Filter> menuFilter = new List<Filter>();
            var CurrentOperators = "";
            var paramOperator = "";
            if (Operators.Count > 0)
            {
                foreach (var item in Operators)
                {
                    if (condition.IndexOf(item) != -1)
                    {
                        CurrentOperators = item;
                        paramOperator = item;
                        break;
                    }
                }
            }
            if (CurrentOperators != "")
            {
                string[] values = condition.Split(CurrentOperators);
                if (values.Length > 1)
                {
                    string ItemVal = values[1].Replace("(", "").Replace(")", "").Replace("'", "").Replace(")", "");
                    List<string> items = ItemVal.Split(',').ToList<string>();
                    Console.WriteLine("Items Count" + items.Count);
                    Console.WriteLine("Items Val" + condition);
                    if (items.Count > 0)
                    {
                        foreach (var item in items)
                        {
                            Filter property = new Filter();
                            property.PropertyName = values[0].Trim().ToString();
                            if (property.PropertyName.ToLower().Contains("grade") || property.PropertyName.ToLower().Contains("nt_username"))
                            {
                                if (CurrentOperators == "&")
                                {
                                    CurrentOperators = "=";
                                }
                                property.Operation = (Op)GetOperarend(CurrentOperators);
                                property.Operators = "&";
                                property.Value = item.Trim().ToString();
                            }
                            else
                            {
                                if (CurrentOperators == "&")
                                {
                                    CurrentOperators = "=";
                                }

                                property.Operators = CurrentOperators;
                                property.Operation = (Op)GetOperarend(CurrentOperators);
                                property.Value = Convert.ToInt32(item.Trim().ToString());
                                Console.WriteLine("Query filtred" + property.Value);
                            }
                            menuFilter.Add(property);
                        }
                    }
                }
            }
            return menuFilter;
        }
        //6
        public static int GetOperarend(string Operators)
        {
            int returnVal = 0;
            if (Operators == "&")
            {
                returnVal = (int)Op.Contains;
            }
            if (Operators == "=")
            {
                returnVal = (int)Op.Equals;
            }
            if (Operators == "!")
            {
                returnVal = (int)Op.NotEquals;
            }
            if (Operators == "}")
            {
                returnVal = (int)Op.GreaterThanOrEqual;
            }
            if (Operators == ">")
            {
                returnVal = (int)Op.GreaterThan;
            }
            if (Operators == "{")
            {
                returnVal = (int)Op.LessThanOrEqual;
            }
            if (Operators == "<")
            {
                returnVal = (int)Op.LessThan;
            }
            return returnVal;
        }
        private static string ReplaceCaseInsensitive(string input, string search, string replacement)
        {
            var regex = new Regex(Regex.Escape(search), RegexOptions.IgnoreCase);
            // string result = Regex.Replace(
            //     input,
            //     Regex.Escape(search),
            //     replacement,
            //     RegexOptions.IgnoreCase,
            // );
            string result = regex.Replace(input, replacement, 1, (int)RegexOptions.IgnoreCase);
            return result;
        }
    }

}
