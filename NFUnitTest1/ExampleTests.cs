// Copyright (c)2025 José Simões
// SPDX-License-Identifier: MIT (modified)
// See the LICENSE file in the project root for full license information.

using nanoFramework.TestFramework;
using System;

[TestClass]
public class ExampleTests
{
    // Some sample data to use in tests
    private static int _globalCounter;
    private static int[] _numbers;

    [Setup]
    public void Initialize()
    {
        // This runs before each test (once for this class).
        OutputHelper.Write("Running setup...");
        _globalCounter = 42;
        _numbers = new int[] { 1, 4, 9 };
        // We can also use assertions in Setup to verify initial conditions:
        Assert.AreEqual(42, _globalCounter);
        //Assert.IsNotType(OutputHelper, OutputHelper); // Ensure OutputHelper is available  
        Assert.AreNotSame(_numbers, _numbers); // Ensure _numbers is the same instance as itself
    }

    [Cleanup]
    public void Cleanup()
    {
        // This runs after all tests in this class have executed.
        OutputHelper.Write("Running cleanup...");
        // For example, ensure that our array still has elements and then clean up:
        CollectionAssert.NotEmpty(_numbers);
        _numbers = null;
        Assert.IsNull(_numbers);
    }

    [TestMethod]
    public void TestCounterAndArrayValues()
    {
        // This is a test method that will be executed by the test runner.
        OutputHelper.Write("TestCounterAndArrayValues starting");
        // The Setup method should have set _globalCounter to 42
        Assert.AreNotEqual(0, _globalCounter);           // _globalCounter should not be the default 0
        Assert.AreEqual(42, _globalCounter);             // _globalCounter should equal 42 (initialized in Setup)
        // The Setup method should have initialized _numbers array
        Assert.AreEqual(3, _numbers.Length);             // The array should have 3 elements
        Assert.AreEqual(1, _numbers[0]);                 // First element should be 1
        Assert.AreEqual(4, _numbers[1]);                 // Second element should be 4
        Assert.AreEqual(9, _numbers[2]);                 // Third element should be 9
    }

    [TestMethod]
    public void TestStringUtilities()
    {
        OutputHelper.Write("TestStringUtilities starting");
        string phrase = "nanoFramework is great";
        // Check some string conditions:
        Assert.Contains("nano", phrase);             // phrase should contain "nano"
        Assert.StartsWith("nanoFramework", phrase);  // phrase begins with "nanoFramework"
        Assert.EndsWith("great", phrase);            // phrase ends with "great"
        // Also demonstrate the DoesNotContains assertion:
        Assert.DoesNotContains("XYZ", phrase);       // phrase does not contain "XYZ"
    }

    [TestMethod]
    public void ThisTestIsToBeSkipped()
    {
        // This test will be skipped by the test runner.
        OutputHelper.Write("ThisTestIsToBeSkipped starting");
        Assert.SkipTest("This test is intentionally skipped for demonstration purposes.");
    }

    [TestMethod]
    public void TestExceptionThrowing()
    {
        OutputHelper.Write("TestExceptionThrowing starting");
        // Example: verify that a certain action throws an exception as expected.
        Assert.ThrowsException(typeof(ArgumentNullException), ThrowNull);
        // The above will pass if calling ThrowNull() throws an ArgumentNullException.
        // We can also use a lambda for convenience:
        Assert.ThrowsException(
            typeof(InvalidOperationException),
            () =>
            {
                // This code should throw InvalidOperationException to satisfy the test
                OutputHelper.Write("Throwing InvalidOperationException as expected");
                throw new InvalidOperationException("Testing exception");
            });
    }

    // A helper method for the test above:
    private void ThrowNull()
    {
        OutputHelper.Write("About to throw ArgumentNullException");
        throw new ArgumentNullException();
    }

    [TestMethod]
    public void ThisTestWillFail()
    {
        // An example of a test that fails (to illustrate failure output)
        Assert.IsTrue(false, "This is a deliberate failure for demonstration");
    }
}
