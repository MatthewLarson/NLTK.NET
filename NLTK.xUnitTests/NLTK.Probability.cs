using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Xunit;


namespace NLTK.Probability.Tests
{
    public class ConditionalFreqDistTests
    {
        //instance of the ConditionalFreqDist
        public ConditionalFreqDist conditionalFreqDist = new ConditionalFreqDist();

        #region "Indexing Tests"

        [Theory]
        [InlineData(0)]
        public void ConditionalFreqDist_FirstLevelIndexing(int i)
        {
            Assert.Equal(conditionalFreqDist[i], new Dictionary<string, int>());
        }

        [Theory]
        [InlineData(0)]
        public void ConditionalFreqDist_SecondLevelIndexing(int v)
        {
            Assert.Equal(conditionalFreqDist[0, "word"], v);
        }

        [Theory]
        [InlineData(1)]
        public void ConditionalFreqDist_SecondLevelValueSetting(int v)
        {
            Assert.Equal(conditionalFreqDist[0, "word"] = 1, v);
            Assert.Equal(conditionalFreqDist[0, "word"] += 2, v + 2);
        }

        #endregion
    }
}
