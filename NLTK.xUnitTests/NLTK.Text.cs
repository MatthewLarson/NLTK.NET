using System;
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
}
