using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class VWSCloudConnectTestSuite
{
    // A Test behaves as an ordinary method
    [Test]
    public async Task VWSCloudConnectTestSuiteSimplePasses()
    {
        //VWSCloudConnecterService cloudConnecterService = new VWSCloudConnecterService();
        //// Use the Assert class to test conditions
        //await VWSCloudConnecterService.
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator VWSCloudConnectTestSuiteWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
