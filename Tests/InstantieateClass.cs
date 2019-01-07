using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpeedTesting.Tests
{
	class TestClass
	{
		readonly int o = 20;
		public TestClass()
		{
			o = 20;
		}

		public TestClass(int i)
		{
			o = i;
		}
	}

	class InstantieateClass
	{
		public static IEnumerable<TimeStamp> DoTest(long timesToRun)
		{
			SpeedTester.Start("Standard new no params");

			for (long i = 0; i < timesToRun; i++)
			{
				TestClass s = new TestClass();
			}

			yield return SpeedTester.Finish();


			SpeedTester.Start("Standard new with params");

			for (long i = 0; i < timesToRun; i++)
			{
				TestClass s = new TestClass(420);
			}

			yield return SpeedTester.Finish();


			SpeedTester.Start("Activator.Createinstance no params");

			for (long i = 0; i < timesToRun; i++)
			{
				TestClass a = (TestClass)Activator.CreateInstance(typeof(TestClass));
			}

			yield return SpeedTester.Finish();


			SpeedTester.Start("Activator.Createinstance with params");

			for (long i = 0; i < timesToRun; i++)
			{
				TestClass a = (TestClass)Activator.CreateInstance(typeof(TestClass), 420);
			}

			yield return SpeedTester.Finish();


			ConstructorInfo ctorNoArgs = typeof(TestClass).GetConstructor(new Type[0]);

			ConstructorInfo ctorWithArgs = typeof(TestClass).GetConstructor(new Type[] { typeof(int) });

			ObjectActivator<TestClass> noArgs = GetActivator<TestClass>(ctorNoArgs);
			ObjectActivator<TestClass> Args = GetActivator<TestClass>(ctorWithArgs);

			SpeedTester.Start("Compiled lambda no args");

			for (long i = 0; i < timesToRun; i++)
			{
				TestClass test = noArgs.Invoke();
			}

			yield return SpeedTester.Finish();


			SpeedTester.Start("Compiled lambda null args");

			for (long i = 0; i < timesToRun; i++)
			{
				TestClass test = noArgs.Invoke(null);
			}

			yield return SpeedTester.Finish();


			SpeedTester.Start("Compiled lambda with args");

			for (long i = 0; i < timesToRun; i++)
			{
				TestClass test = Args.Invoke(420);
			}

			yield return SpeedTester.Finish();
		}

		delegate T ObjectActivator<T>(params object[] args);

		static ObjectActivator<T> GetActivator<T>(ConstructorInfo ctor)
		{
			Type type = ctor.DeclaringType;
			ParameterInfo[] paramsInfo = ctor.GetParameters();

			//create a single param of type object[]
			ParameterExpression param =
				Expression.Parameter(typeof(object[]), "args");

			Expression[] argsExp =
				new Expression[paramsInfo.Length];

			//pick each arg from the params array 
			//and create a typed expression of them
			for (int i = 0; i < paramsInfo.Length; i++)
			{
				Expression index = Expression.Constant(i);
				Type paramType = paramsInfo[i].ParameterType;

				Expression paramAccessorExp =
					Expression.ArrayIndex(param, index);

				Expression paramCastExp =
					Expression.Convert(paramAccessorExp, paramType);

				argsExp[i] = paramCastExp;
			}

			//make a NewExpression that calls the
			//ctor with the args we just created
			NewExpression newExp = Expression.New(ctor, argsExp);

			//create a lambda with the New
			//Expression as body and our param object[] as arg
			LambdaExpression lambda =
				Expression.Lambda(typeof(ObjectActivator<T>), newExp, param);

			//compile it
			ObjectActivator<T> compiled = (ObjectActivator<T>)lambda.Compile();
			return compiled;
		}
	}
}
