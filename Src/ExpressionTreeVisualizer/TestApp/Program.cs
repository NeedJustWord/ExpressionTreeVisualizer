using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using ExpressionTreeViewer;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.DebuggerVisualizers;
using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

namespace TestApp
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            //var languages = new[] { "C#", "J#", "VB", "Delphi", "F#", "COBOL", "Python" };
            //var queryable = languages.AsQueryable().Where(l => l.EndsWith("#") && l != "j#")
            //    .Take(3).Select(l => new { Name = l, IsSelected = true });
            //new VisualizerDevelopmentHost(queryable.Expression, typeof(ExpressionTreeVisualizer), typeof(ExpressionTreeObjectSource)).ShowVisualizer();
            new VisualizerDevelopmentHost(GetExpression(), typeof(ExpressionTreeVisualizer), typeof(ExpressionTreeObjectSource)).ShowVisualizer();
        }

        static Expression GetExpression()
        {
            return Block(
                Binary(),
                Conditional(),
                Constant((int?)null),
                Constant((string)null),
                Constant("null"),
                Constant("str"),
                Constant(" "),
                Constant("	"),
                Constant(' '),
                Constant('	'),
                Constant(new int[2][,] { null, new int[,] { { 1, 11 }, { 2, 22 } }, }),
                Constant(new Program[6][,][][,,][][,,,][][,,,,]),
                Constant(new int[] { 0, 1, }),
                Constant(new string[] { null, "null", " ", }),
                Constant(new int[,] { { 1, 11, 111, }, { 2, 22, 222, }, }),
                Constant(new int[,,] { { { 1, 11, }, { 2, 22, }, }, { { 3, 33, }, { 4, 44, }, }, }),
                Constant(new int[,,,] { { { { 1, 11, }, { 2, 22, }, }, { { 3, 33, }, { 4, 44, }, }, }, { { { 5, 55, }, { 6, 66, }, }, { { 7, 77, }, { 8, 88, }, }, }, }),
                DebugInfo(),
                Default<IEnumerable<int>>(),
                Default<Func<int>>(),
                Default<Program>(),
                Default<int>(),
                Dynamic(),
                Goto(),
                Return(),
                Index1(),
                Index2(),
                IndexParam(),
                Invocation(),
                Label(),
                LabelDefault(),
                Lambda(),
                ListInit(),
                Loop(),
                Member(),
                MemberInit(),
                MethodCall(),
                NewArray<int>(),
                New<int>(),
                Parameter<int>("a"),
                RuntimeVariables(),
                Switch(),
                Try(),
                TypeBinary(),
                Unary()
                );
        }

        static BinaryExpression Binary()
        {
            return Expression.MakeBinary(ExpressionType.GreaterThan, Constant(1), Constant(2));
        }

        static BlockExpression Block(params Expression[] expressions)
        {
            return Expression.Block(expressions);
        }

        static ConditionalExpression Conditional()
        {
            return Expression.Condition(Constant(true), Constant("yes"), Constant("no"));
        }

        static ConstantExpression Constant<T>(T t)
        {
            return Expression.Constant(t, typeof(T));
        }

        static DebugInfoExpression DebugInfo()
        {
            return Expression.DebugInfo(Expression.SymbolDocument("debug"), 1, 2, 3, 4);
        }

        static DefaultExpression Default<T>()
        {
            return Expression.Default(typeof(T));
        }

        static DynamicExpression Dynamic()
        {
            //todo:Dynamic
            dynamic aiD = new object();
            //声明参数
            ParameterExpression paramExpr = Expression.Parameter(typeof(object), "o");
            //获取 CallSite 以支持调用时的运行时。
            //CallSiteBinder 是必需的
            //注意这里用的是     Binder.GetMember    
            CallSiteBinder aiBinder = Binder.GetMember(CSharpBinderFlags.None, "Id", typeof(Program),
                    new CSharpArgumentInfo[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
            //定义一个动态表达式
            //Func<CallSite, object, object>声明方式中 CallSite 是动态调用站点的基类。 此类型用作动态站点目标的参数类型。
            //就是上面的 aiBinder，第二个参数是要传入的动态类型 用dynamic关键字也是可以的，
            //后面的参数是实际调用和返回的参数
            //不过操作中发现在返回值一定要用 object 不然会编译不过
            //当然也可用Action<CallSite, object,...>,除了没返回值，参数要求和用Func<CallSite, object, object>是一样的。

            DynamicExpression Dynamic2 =
              Expression.MakeDynamic(typeof(Func<CallSite, object, object>),
                 aiBinder, paramExpr
          );
            ////编译
            //LambdaExpression laExp = Expression.Lambda(
            //    Expression.Block(
            //    new ParameterExpression[] { paramExpr },
            //    Expression.Assign(paramExpr, Expression.Constant(aiD))
            //  , Dynamic2));
            ////执行
            //Console.WriteLine("AiDynamicDo：" + laExp.ToString());
            //Console.WriteLine("结果：" + laExp.Compile().DynamicInvoke());
            //Console.WriteLine();

            return Dynamic2;
        }

        static GotoExpression Goto()
        {
            return Expression.Goto(Expression.Label("label"));
        }

        static GotoExpression Return()
        {
            return Expression.MakeGoto(GotoExpressionKind.Return, Expression.Label(), Constant(1), typeof(long));
        }

        static IndexExpression Index1()
        {
            var array = Constant(new int[] { 0, 1, 2, 3 });
            return Expression.ArrayAccess(array, Constant(2));
        }

        static IndexExpression Index2()
        {
            var array = Constant(new int[,] { { 0, 1, 2, 3 }, { 10, 11, 12, 13, }, });
            return Expression.ArrayAccess(array, Constant(1), Constant(3));
        }

        static IndexExpression IndexParam()
        {
            ParameterExpression arrayExpr = Expression.Parameter(typeof(int[]), "Array");
            ParameterExpression indexExpr = Expression.Parameter(typeof(int), "Index");
            return Expression.ArrayAccess(arrayExpr, indexExpr);
        }

        static InvocationExpression Invocation()
        {
            return Expression.Invoke(Lambda(), Constant(10), Constant(20));
        }

        static LabelExpression Label()
        {
            return Expression.Label(Expression.Label("label"));
        }

        static LabelExpression LabelDefault()
        {
            return Expression.Label(Expression.Label(typeof(string), "labelType"), Constant("str"));
        }

        static LambdaExpression Lambda()
        {
            Expression<Func<int, int, bool>> largeSumTest = (num1, num2) => (num1 + num2) > 1000;
            return largeSumTest;
        }

        static ListInitExpression ListInit()
        {

            Console.WriteLine("//System.Linq.Expressions.ListInitExpression 表示包含集合初始值设定项的构造函数调用。");

            string tree1 = "maple";

            string tree2 = "oak";

            System.Reflection.MethodInfo addMethod = typeof(Dictionary<int, string>).GetMethod("Add");//获得Dictionary的Add方法



            System.Linq.Expressions.ElementInit elementInit1 =     // 表示 IEnumerable 集合的单个元素的初始值设定项。

                System.Linq.Expressions.Expression.ElementInit(

                    addMethod,

                    System.Linq.Expressions.Expression.Constant(tree1.Length),

                    System.Linq.Expressions.Expression.Constant(tree1));

            System.Linq.Expressions.ElementInit elementInit2 =

                System.Linq.Expressions.Expression.ElementInit(

                    addMethod,

                    System.Linq.Expressions.Expression.Constant(tree2.Length),

                    System.Linq.Expressions.Expression.Constant(tree2));



            System.Linq.Expressions.NewExpression newDictionaryExpression =

                System.Linq.Expressions.Expression.New(typeof(Dictionary<int, string>));//构造函数



            System.Linq.Expressions.ListInitExpression listInitExpression =

                System.Linq.Expressions.Expression.ListInit(

                    newDictionaryExpression,

                    elementInit1,

                    elementInit2);



            ElementInit el = listInitExpression.Initializers[0];   //获取ElementInit

            Console.WriteLine("" + el.Arguments[0] + el.Arguments[1]);

            Console.WriteLine(listInitExpression.ToString());

            return listInitExpression;
        }

        static LoopExpression Loop()
        {
            return Expression.Loop(Constant(1));
        }

        static MemberExpression Member()
        {
            return Expression.Field(Constant(new Car()), "Name");
        }

        static MemberInitExpression MemberInit()
        {
            var weightMember = typeof(Car).GetMember("Weight")[0];
            var heightMember = typeof(Car).GetMember("Height")[0];                //成员

            ParameterExpression params1 = Expression.Parameter(typeof(int), "weight");
            ParameterExpression params2 = Expression.Parameter(typeof(int), "height");

            var info = typeof(Car).GetConstructor(new Type[] { typeof(int), typeof(int) });

            NewExpression car = New(info, params1, params2);



            MemberBinding weightMemberBinding = Expression.Bind(weightMember, params1);

            MemberBinding heightMemberBinding = Expression.Bind(heightMember, params2);

            MemberInitExpression memberInitExpression = Expression.MemberInit(car, weightMemberBinding, heightMemberBinding);
            return memberInitExpression;
        }

        static MethodCallExpression MethodCall()
        {
            return Expression.Call(null, typeof(Console).GetMethod("Write", new Type[] { typeof(string) }), Constant("Hello"));
        }

        static NewArrayExpression NewArray<T>()
        {
            Console.WriteLine("//System.Linq.Expressions.NewArrayExpression");

            List<System.Linq.Expressions.Expression> trees =

    new List<System.Linq.Expressions.Expression>()

        { System.Linq.Expressions.Expression.Constant("oak"),

          System.Linq.Expressions.Expression.Constant("fir"),

          System.Linq.Expressions.Expression.Constant("spruce"),

          System.Linq.Expressions.Expression.Constant("alder") };

            System.Linq.Expressions.NewArrayExpression newArrayExpression =

                  System.Linq.Expressions.Expression.NewArrayInit(typeof(string), trees);



            Expression<Func<string>> lambdaExp =

            Expression.Lambda<Func<string>>(Expression.ArrayIndex(newArrayExpression, Expression.Constant(1))

            );



            Console.WriteLine(lambdaExp.Compile().Invoke());

            Console.WriteLine(newArrayExpression.ToString());

            return newArrayExpression;
        }

        static NewExpression New<T>()
        {
            return Expression.New(typeof(T));
        }

        static NewExpression New(ConstructorInfo constructor, params Expression[] arguments)
        {
            return Expression.New(constructor, arguments);
        }

        static ParameterExpression Parameter<T>(string paramName)
        {
            return Expression.Parameter(typeof(T), paramName);
        }

        static RuntimeVariablesExpression RuntimeVariables()
        {
            return Expression.RuntimeVariables();
        }

        static SwitchExpression Switch()
        {
            return Expression.Switch(
                Constant(1)
                , Expression.SwitchCase(Expression.Call(

                    null,

                    typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }),

                    Expression.Constant("Second")

                ), Constant(1)));
        }

        static TryExpression Try()
        {
            return Expression.TryFinally(Constant(3), Expression.Call(

                    null,

                    typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }),

                    Expression.Constant("Second")

                ));
        }

        static TypeBinaryExpression TypeBinary()
        {
            return Expression.TypeIs(Constant(3), typeof(int));
        }

        static UnaryExpression Unary()
        {
            return Expression.MakeUnary(ExpressionType.Convert, Constant(2), typeof(long));
        }
    }

    class Car
    {
        public string Name;

        private int weight;
        public int Weight
        {
            get { return weight; }
            set { weight = value; }
        }

        private int height;
        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        public Car()
        {
            weight = 100;
        }

        public Car(int weight, int height)
        {
            this.weight = weight;
            this.height = height;
        }
    }
}
