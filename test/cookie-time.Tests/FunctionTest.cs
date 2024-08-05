using Xunit;
using Amazon.Lambda.TestUtilities;

namespace cookie_time.Tests;

public class FunctionTest
{
    [Fact]
    public async Task TestToUpperFunction()
    {
        var context = new TestLambdaContext();
        await Function.FunctionHandler(context);
    }
}
