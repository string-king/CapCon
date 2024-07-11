using Helpers;

namespace Base.Test;

public class TestFooTests
{

    private TestFoo testFoo = new TestFoo();
    
    public TestFooTests()
    {
        
    }
    
    [Fact]
    public void InitialValueZero()
    {
        Assert.Equal(0, testFoo.State);
    }
    
    [Fact]
    public void AddTwoNumbers()
    {
        testFoo.State = -1;
        testFoo.Add(2);
        Assert.Equal(1, testFoo.State);
    }
    
    [Theory]
    [InlineData(0,1, 1)]
    [InlineData(-1,-1, -2)]
    [InlineData(1,2, 3)]
    public void AddTwoNumbersWithParams(int initialValue, int valueToAdd, int expectedValue)
    {
        testFoo.State = initialValue;
        testFoo.Add(valueToAdd);
        Assert.Equal(expectedValue, testFoo.State);
    }
    
    [Fact]
    public void DivideByZero()
    {
        testFoo.State = 1;
        Assert.Throws<DivideByZeroException>(() => testFoo.Div(0));
    }
}