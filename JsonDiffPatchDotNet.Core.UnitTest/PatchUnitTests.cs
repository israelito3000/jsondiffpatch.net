﻿using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonDiffPatchDotNet.UnitTests
{
	[TestClass]
	public class PatchUnitTests
	{

		[TestMethod]
		public void Patch_ObjectApplyDelete_Success()
		{
			var jdp = new JsonDiffPatch();
			var left = JObject.Parse(@"{ ""p"" : true }");
			var right = JObject.Parse(@"{ }");
			var patch = jdp.Diff(left, right);

			var patched = jdp.Patch(left, patch) as JObject;

			Assert.IsNotNull(patched, "Patched object");
			Assert.AreEqual(0, patched.Properties().Count(), "Property Deleted");
		}

		[TestMethod]
		public void Patch_ObjectApplyAdd_Success()
		{
			var jdp = new JsonDiffPatch();
			var left = JObject.Parse(@"{ }");
			var right = JObject.Parse(@"{ ""p"" : true }");
			var patch = jdp.Diff(left, right);

			var patched = jdp.Patch(left, patch) as JObject;

			Assert.IsNotNull(patched, "Patched object");
			Assert.AreEqual(1, patched.Properties().Count(), "Property");
			Assert.AreEqual(JTokenType.Boolean, patched.Property("p").Value.Type);
			Assert.IsTrue(patched.Property("p").Value.ToObject<bool>(), "Patched Property");
		}

		[TestMethod]
		public void Patch_ObjectApplyEdit_Success()
		{
			var jdp = new JsonDiffPatch();
			var left = JObject.Parse(@"{ ""p"" : false }");
			var right = JObject.Parse(@"{ ""p"" : true }");
			var patch = jdp.Diff(left, right);

			var patched = jdp.Patch(left, patch) as JObject;

			Assert.IsNotNull(patched, "Patched object");
			Assert.AreEqual(1, patched.Properties().Count(), "Property");
			Assert.AreEqual(JTokenType.Boolean, patched.Property("p").Value.Type);
			Assert.IsTrue(patched.Property("p").Value.ToObject<bool>(), "Patched Property");
		}

		[TestMethod]
		public void Patch_ObjectApplyEditText_Success()
		{
			var jdp = new JsonDiffPatch();
			var left = JObject.Parse(@"{ ""p"" : ""bla1h111111111111112312weldjidjoijfoiewjfoiefjefijfoejoijfiwoejfiewjfiwejfowjwifewjfejdewdwdewqwertyqwertifwiejifoiwfei"" }");
			var right = JObject.Parse(@"{ ""p"" : ""blah1"" }");
			var patch = jdp.Diff(left, right);

			var patched = jdp.Patch(left, patch) as JObject;

			Assert.IsNotNull(patched, "Patched object");
			Assert.AreEqual(1, patched.Properties().Count(), "Property");
			Assert.AreEqual(JTokenType.String, patched.Property("p").Value.Type, "String Type");
			Assert.AreEqual("blah1", patched.Property("p").Value.ToString(), "String value");
		}

		[TestMethod]
		public void Patch_ObjectApplyEditTextEfficient_Success()
		{
			var options = new Options { MinEfficientTextDiffLength = 1, TextDiff = TextDiffMode.Efficient };
			var jdp = new JsonDiffPatch(options);
			var left = JObject.Parse(@"{ ""p"" : ""The quick brown fox jumps over the lazy dog."" }");
			var right = JObject.Parse(@"{ ""p"" : ""That quick brown fox jumped over a lazy dog."" }");
			var patch = jdp.Diff(left, right);

			var patched = jdp.Patch(left, patch) as JObject;

			Assert.IsNotNull(patched, "Patched object");
			Assert.AreEqual(1, patched.Properties().Count(), "Property");
			Assert.AreEqual(JTokenType.String, patched.Property("p").Value.Type, "String Type");
			Assert.AreEqual("That quick brown fox jumped over a lazy dog.", patched.Property("p").Value.ToString(), "String value");
		}

		[TestMethod]
		public void Patch_NestedObjectApplyEdit_Success()
		{
			var jdp = new JsonDiffPatch();
			var left = JObject.Parse(@"{ ""i"": { ""p"" : false } }");
			var right = JObject.Parse(@"{ ""i"": { ""p"" : true } }");
			var patch = jdp.Diff(left, right);

			var patched = jdp.Patch(left, patch) as JObject;

			Assert.IsNotNull(patched, "Patched object");
			Assert.AreEqual(1, patched.Properties().Count(), "Property");
			Assert.AreEqual(JTokenType.Object, patched.Property("i").Value.Type);
			Assert.AreEqual(1, ((JObject)patched.Property("i").Value).Properties().Count());
			Assert.AreEqual(JTokenType.Boolean, ((JObject)patched.Property("i").Value).Property("p").Value.Type);
			Assert.IsTrue(((JObject)patched.Property("i").Value).Property("p").Value.ToObject<bool>());
		}

		[TestMethod]
		public void Patch_NestedComplexEdit_Success()
		{
			var jdp = new JsonDiffPatch();
			var left = JObject.Parse(@"{ ""i"": { ""1"" : 1, ""2"": 2 }, ""j"": [0, 2, 4], ""k"": [1] }");
			var right = JObject.Parse(@"{ ""i"": { ""1"" : 1, ""2"": 3 }, ""j"": [0, 2, 3], ""k"": null }");
			var patch = jdp.Diff(left, right);

			var patched = jdp.Patch(left, patch);

			Assert.AreEqual(right.ToString(), patched.ToString());
		}

		[TestMethod]
		public void Patch_NestedComplexEditDifferentLeft_Success()
		{
			var jdp = new JsonDiffPatch();
			var left = JObject.Parse(@"{ ""i"": { ""1"" : 1, ""2"": 2 }, ""j"": [0, 2, 4], ""k"": [1] }");
			var right = JObject.Parse(@"{ ""i"": { ""1"" : 1, ""2"": 3 }, ""j"": [0, 2, 3], ""k"": null }");
			var patch = jdp.Diff(JObject.Parse(@"{ ""k"": { ""i"": [1] } }"), right);

			var patched = jdp.Patch(left, patch);

			Assert.AreEqual(right.ToString(), patched.ToString());
		}

		[TestMethod]
		public void Patch_NullLeft_Exception()
		{
			var jdp = new JsonDiffPatch();
			var patch = JToken.Parse(@"[true]");

			JToken result = jdp.Patch(null, patch);

			Assert.IsNotNull(result);
			Assert.AreEqual(JTokenType.Boolean, result.Type);
			Assert.AreEqual(true, result.ToObject<bool>());
		}

		[TestMethod]
		public void Patch_ArrayPatchAdd_Success()
		{
			var jdp = new JsonDiffPatch(new Options { ArrayDiff = ArrayDiffMode.Efficient });
			var left = JToken.Parse(@"[1,2,3]");
			var right = JToken.Parse(@"[1,2,3,4]");
			var patch = jdp.Diff(left, right);

			var patched = jdp.Patch(left, patch);

			Assert.AreEqual(right.ToString(), patched.ToString());
		}

		[TestMethod]
		public void Patch_ArrayPatchRemove_Success()
		{
			var jdp = new JsonDiffPatch(new Options { ArrayDiff = ArrayDiffMode.Efficient });
			var left = JToken.Parse(@"[1,2,3]");
			var right = JToken.Parse(@"[1,2]");
			var patch = jdp.Diff(left, right);

			var patched = jdp.Patch(left, patch);

			Assert.AreEqual(right.ToString(), patched.ToString());
		}

		[TestMethod]
		public void Patch_ArrayPatchModify_Success()
		{
			var jdp = new JsonDiffPatch(new Options { ArrayDiff = ArrayDiffMode.Efficient });
			var left = JToken.Parse(@"[1,3,{""p"":false}]");
			var right = JToken.Parse(@"[1,4,{""p"": [1] }]");
			var patch = jdp.Diff(left, right);

			var patched = jdp.Patch(left, patch);

			Assert.AreEqual(right.ToString(), patched.ToString());
		}

		[TestMethod]
		public void Patch_ArrayPatchComplex_Success()
		{
			var jdp = new JsonDiffPatch(new Options { ArrayDiff = ArrayDiffMode.Efficient });
			var left = JToken.Parse(@"{""p"": [1,2,[1],false,""11111"",3,{""p"":false},10,10] }");
			var right = JToken.Parse(@"{""p"": [1,2,[1,3],false,""11112"",3,{""p"":true},10,10] }");
			var patch = jdp.Diff(left, right);

			var patched = jdp.Patch(left, patch);

			Assert.AreEqual(right.ToString(), patched.ToString());
		}

		[TestMethod]
		public void Patch_ArrayPatchMoving_Success()
		{
			var jdp = new JsonDiffPatch(new Options { ArrayDiff = ArrayDiffMode.Efficient });
			var left = JToken.Parse(@"[0,1,2,3,4,5,6,7,8,9,10]");
			var right = JToken.Parse(@"[10,0,1,7,2,4,5,6,88,9,3]");
			var patch = JToken.Parse(@"{ ""8"": [88], ""_t"": ""a"", ""_3"": ["""", 10, 3], ""_7"": ["""", 3, 3], ""_8"": [8, 0, 0], ""_10"": ["""", 0, 3] }");

			var patched = jdp.Patch(left, patch);

			Assert.AreEqual(right.ToString(), patched.ToString());
		}

		[TestMethod]
		public void Patch_ArrayPatchMovingNonConsecutive_Success()
		{
			var jdp = new JsonDiffPatch(new Options { ArrayDiff = ArrayDiffMode.Efficient });
			var left = JToken.Parse(@"[0,1,3,4,5]");
			var right = JToken.Parse(@"[0,4,3,1,5]");
			var patch = JToken.Parse(@"{""_t"": ""a"", ""_2"": ["""", 2, 3],""_3"": ["""", 1, 3]}");

			var patched = jdp.Patch(left, patch);

			Assert.AreEqual(right.ToString(), patched.ToString());
		}

		[TestMethod]
		public void Patch_ArrayPatchMoveDeletingNonConsecutive_Success()
		{
			var jdp = new JsonDiffPatch(new Options { ArrayDiff = ArrayDiffMode.Efficient });
			var left = JToken.Parse(@"[0,1,3,4,5]");
			var right = JToken.Parse(@"[0,5,3]");
			var patch = JToken.Parse(@"{""_t"": ""a"", ""_1"": [ 1, 0, 0], ""_3"": [4,0, 0],""_4"": [ """", 1, 3 ]}");

			var patched = jdp.Patch(left, patch);

			Assert.AreEqual(right.ToString(), patched.ToString());
		}
	}
}
