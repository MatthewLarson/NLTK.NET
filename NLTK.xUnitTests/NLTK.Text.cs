using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Xunit;


namespace NLTK.Text.Tests
{
    public class ContextIndexTests
    {
        //public string[] aTokens = new string[] { "I", "am", "a", "silly", "goose", "!" };
        public ContextIndex contextIndex = new ContextIndex(new string[] { "I", "am", "a", "silly", "goose", "!" });

        [Theory]
        [InlineData(new string[] { "I", "am", "a", "silly", "goose", "!" }, true)]
        [InlineData(new string[] { "I", "am", "a", "silly", "goose" }, false)]
        public void ContextIndex_Tokens(string[] tokens, bool isEqual)
        {
            if (isEqual)
            {
                Assert.Equal(tokens, contextIndex.Tokens());
            } else
            {
                Assert.NotEqual(tokens, contextIndex.Tokens());
            }
            
        }
    }

    public class TextTests
    {
        public Text text = new Text(new string[] { "matt", "anthony", "brad" });
        
        [Theory]
        [InlineData(0, new string[] { "matt", "anthony" })]
        [InlineData(1, new string[] { "anthony", "brad" })]
        public void Text_RangeFunctionality(int i, string[] tokens)
        {
            List<string[]> tests = new List<string[]> { 
                text[..2], 
                text[1..]
            };

            Assert.Equal(tokens, tests[i]);
        }
    }
}
