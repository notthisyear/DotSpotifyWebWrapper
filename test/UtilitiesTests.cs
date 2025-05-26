using System.Collections.Generic;
using DotSpotifyWebWrapper.Utilities;

namespace DotSpotifyWebWrapperTests;

[TestClass]
public sealed class UtilitiesTests
{
    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public void VerifyQueryStringEmptyIfEmpty(bool includeQuestionMark)
    {
        Assert.AreEqual(string.Empty, HttpRequestUriUtilities.GetQueryStringOrEmpty("key", string.Empty, includeQuestionMark));
    }

    [TestMethod]
    public void VerifyQueryStringEmptyIfEmptyDictionary()
    {
        Assert.AreEqual(string.Empty, HttpRequestUriUtilities.GetQueryString([]));
    }

    [TestMethod]
    [DataRow("some_key", "value_1", true, "?some_key=value_1")]
    [DataRow("some_other_key", "other_value", false, "some_other_key=other_value")]
    public void VerifyQueryStringOfSingleValueCorrect(string key, string value, bool includeQuestionMark, string expectedResult)
    {
        Assert.AreEqual(expectedResult, HttpRequestUriUtilities.GetQueryStringOrEmpty(key, value, includeQuestionMark));
    }

    [TestMethod]
    public void VerifyQueryStringOfSingleDictionaryValueCorrect()
    {
        Assert.AreEqual("?key=value", HttpRequestUriUtilities.GetQueryString(new() { { "key", "value" } }));
    }

    [TestMethod]
    public void VerifyQueryStringOfFullyPopulatedDict()
    {
        Dictionary<string, string> parameters = new() {
            { "response_type", "code" },
            { "client_id", "id" },
            { "scope", "scope 1 scope 2" },
            { "code_challenge_method", "S256" },
        };
        var expectedResult = "?response_type=code&client_id=id&scope=scope 1 scope 2&code_challenge_method=S256";
        Assert.AreEqual(expectedResult, HttpRequestUriUtilities.GetQueryString(parameters));
    }

    [TestMethod]
    public void VerifyQueryStringOfDictWithEmptyMembers()
    {
        Dictionary<string, string> parameters = new() {
            { "response_type", "code" },
            { "client_id", "id" },
            { "scope", "scope 1 scope 2" },
            { "redirect_uri", string.Empty },
            { "code_challenge_method", "S256" },
            { "code_challenge", string.Empty },
        };
        var expectedResult = "?response_type=code&client_id=id&scope=scope 1 scope 2&code_challenge_method=S256";
        Assert.AreEqual(expectedResult, HttpRequestUriUtilities.GetQueryString(parameters));
    }

    [TestMethod]
    [DataRow("http://localhost:3000/?code=some_long_code&state=some_state", "code", "some_long_code", "state", "some_state")]
    [DataRow("http://localhost:3000/")]
    [DataRow("http://localhost:3000/?another_key=some_other_value", "another_key", "some_other_value")]
    public void VerifyQueryUrlParsing(string url, params string[] expectedParameters)
    {
        var result = HttpRequestUriUtilities.ParseQueryUri(url);
        if (expectedParameters.Length == 0)
        {
            Assert.IsEmpty(result);
        }
        else
        {
            Dictionary<string, string> expectedResult = [];
            for (var i = 0; i < expectedParameters.Length; i += 2)
                expectedResult.Add(expectedParameters[i], expectedParameters[i + 1]);
            CollectionAssert.AreEquivalent(expectedResult, result);
        }
    }
}
