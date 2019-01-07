using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedTesting.Tests;

namespace SpeedTesting.Testing
{
	class TestMaster
	{
		public static void InitializeTests()
		{
			TestRunner.maxIterationTime = 0.002f;
			TestRunner.timesToPerformAllTests = 1000;
			TestRunner.decimalDigitsRounding = 2;
			TestRunner.TestName = "Stringbuilder vs string.Join";
			TestRunner.testType = TestType.Comparison;


			Sample testObject = new Sample();
			TestRunner.Test(new Testable(Name: "Stringbuilder", Test: testObject.test1, TestSetup: null));
			TestRunner.Test(new Testable(Name: "String.Join", Test: testObject.test2, TestSetup: testObject.Preparation));
		}
	}
}
