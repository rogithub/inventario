using System;
using Xunit.Sdk;
using System.Reflection;


using Xunit.Abstractions;

namespace Ro.Inventario.Web.Tests;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class TestBeforeAfter : BeforeAfterTestAttribute
{
    public TestBeforeAfter()
    {
    }


    public override void Before(MethodInfo methodUnderTest)
    {
        DatabaseProvider.InitDb();
        //_output.WriteLine(methodUnderTest.Name);
    }

    public override void After(MethodInfo methodUnderTest)
    {
        //_output.WriteLine(methodUnderTest.Name);
    }
}