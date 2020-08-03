using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using NLTK.Collections;
using NLTK.Internals;

namespace NLTK.Probability
{
    public class FreqDist : Counter
    {
        public FreqDist()
        {

        }
    }

    public class ConditionalFreqDist : DefaultDictionary<int, FreqDist>
    {
        

        public Dictionary<int, Dictionary<string, int>> DefaultDict = new Dictionary<int, Dictionary<string, int>>();

        public Dictionary<string, int> this[int i]
        {
            get {
                if(DefaultDict.ContainsKey(i))
                {
                    return DefaultDict[i];
                } else
                {
                    DefaultDict[i] = new Dictionary<string, int>();
                    return DefaultDict[i];
                }
            }
            set { 
                if(DefaultDict.ContainsKey(i))
                {
                    DefaultDict[i] = value;
                } else
                {
                    DefaultDict[i] = new Dictionary<string, int>();
                    DefaultDict[i] = value;
                } 
            }
        }

        public int this[int i, string s]
        {
            get
            {
                if (this[i].ContainsKey(s))
                {
                    return this[i][s];
                } else
                {
                    this[i][s] = 0;
                    return this[i][s];
                }
            }
            set
            {
                this[i][s] = value;
            }
        }

        //public ConditionalFreqDist()
        //{
        //    throw new NotImplementedException();
        //}

        //__init__
        public ConditionalFreqDist(Tuple<string, string[]> CondSamples = null)
        {
            if(CondSamples == null)
            {

            }
        }

        //__reduce__
        //some kind of serialization function

        //


        /*
         * class ConditionalFreqDist(defaultdict):
    """
    A collection of frequency distributions for a single experiment
    run under different conditions.  Conditional frequency
    distributions are used to record the number of times each sample
    occurred, given the condition under which the experiment was run.
    For example, a conditional frequency distribution could be used to
    record the frequency of each word (type) in a document, given its
    length.  Formally, a conditional frequency distribution can be
    defined as a function that maps from each condition to the
    FreqDist for the experiment under that condition.
    Conditional frequency distributions are typically constructed by
    repeatedly running an experiment under a variety of conditions,
    and incrementing the sample outcome counts for the appropriate
    conditions.  For example, the following code will produce a
    conditional frequency distribution that encodes how often each
    word type occurs, given the length of that word type:
        >>> from nltk.probability import ConditionalFreqDist
        >>> from nltk.tokenize import word_tokenize
        >>> sent = "the the the dog dog some other words that we do not care about"
        >>> cfdist = ConditionalFreqDist()
        >>> for word in word_tokenize(sent):
        ...     condition = len(word)
        ...     cfdist[condition][word] += 1
    An equivalent way to do this is with the initializer:
        >>> cfdist = ConditionalFreqDist((len(word), word) for word in word_tokenize(sent))
    The frequency distribution for each condition is accessed using
    the indexing operator:
        >>> cfdist[3]
        FreqDist({'the': 3, 'dog': 2, 'not': 1})
        >>> cfdist[3].freq('the')
        0.5
        >>> cfdist[3]['dog']
        2
    When the indexing operator is used to access the frequency
    distribution for a condition that has not been accessed before,
    ``ConditionalFreqDist`` creates a new empty FreqDist for that
    condition.
    """

    def __init__(self, cond_samples=None):
        """
        Construct a new empty conditional frequency distribution.  In
        particular, the count for every sample, under every condition,
        is zero.
        :param cond_samples: The samples to initialize the conditional
            frequency distribution with
        :type cond_samples: Sequence of (condition, sample) tuples
        """
        defaultdict.__init__(self, FreqDist)

        if cond_samples:
            for (cond, sample) in cond_samples:
                self[cond][sample] += 1

    def __reduce__(self):
        kv_pairs = ((cond, self[cond]) for cond in self.conditions())
        return (self.__class__, (), None, None, kv_pairs)

    def conditions(self):
        """
        Return a list of the conditions that have been accessed for
        this ``ConditionalFreqDist``.  Use the indexing operator to
        access the frequency distribution for a given condition.
        Note that the frequency distributions for some conditions
        may contain zero sample outcomes.
        :rtype: list
        """
        return list(self.keys())

    def N(self):
        """
        Return the total number of sample outcomes that have been
        recorded by this ``ConditionalFreqDist``.
        :rtype: int
        """
        return sum(fdist.N() for fdist in self.values())

    def plot(self, *args, **kwargs):
        """
        Plot the given samples from the conditional frequency distribution.
        For a cumulative plot, specify cumulative=True.
        (Requires Matplotlib to be installed.)
        :param samples: The samples to plot
        :type samples: list
        :param title: The title for the graph
        :type title: str
        :param conditions: The conditions to plot (default is all)
        :type conditions: list
        """
        try:
            import matplotlib.pyplot as plt #import statment fix
        except ImportError as e:
            raise ValueError(
                "The plot function requires matplotlib to be installed."
                "See http://matplotlib.org/"
            ) from e

        cumulative = _get_kwarg(kwargs, 'cumulative', False)
        percents = _get_kwarg(kwargs, 'percents', False)
        conditions = [c for c in _get_kwarg(kwargs, 'conditions', self.conditions()) if c in self] # conditions should be in self
        title = _get_kwarg(kwargs, 'title', '')
        samples = _get_kwarg(
            kwargs, 'samples', sorted(set(v
                                            for c in conditions
                                            for v in self[c]))
        )  # this computation could be wasted
        if "linewidth" not in kwargs:
            kwargs["linewidth"] = 2
        ax = plt.gca()
        if (len(conditions) != 0):
            freqs = []
            for condition in conditions:
                if cumulative:
                    # freqs should be a list of list where each sub list will be a frequency of a condition
                    freqs.append(list(self[condition]._cumulative_frequencies(samples)))
                    ylabel = "Cumulative Counts"
                    legend_loc = 'lower right'
                    if percents:
                        freqs[-1] = [f / freqs[len(freqs) - 1] * 100 for f in freqs]
                        ylabel = "Cumulative Percents"
                else:
                    freqs.append([self[condition][sample] for sample in samples])
                    ylabel = "Counts"
                    legend_loc = 'upper right'
                # percents = [f * 100 for f in freqs] only in ConditionalProbDist?

            i = 0
            for freq in freqs:
                kwargs['label'] = conditions[i] #label for each condition
                i += 1
                ax.plot(freq, *args, **kwargs)
            ax.legend(loc=legend_loc)
            ax.grid(True, color="silver")
            ax.set_xticks(range(len(samples)))
            ax.set_xticklabels([str(s) for s in samples], rotation=90)
            if title:
                ax.set_title(title)
            ax.set_xlabel("Samples")
            ax.set_ylabel(ylabel)
        plt.show()

        return ax

    def tabulate(self, *args, **kwargs):
        """
        Tabulate the given samples from the conditional frequency distribution.
        :param samples: The samples to plot
        :type samples: list
        :param conditions: The conditions to plot (default is all)
        :type conditions: list
        :param cumulative: A flag to specify whether the freqs are cumulative (default = False)
        :type title: bool
        """

        cumulative = _get_kwarg(kwargs, "cumulative", False)
        conditions = _get_kwarg(kwargs, "conditions", sorted(self.conditions()))
        samples = _get_kwarg(
            kwargs,
            "samples",
            sorted(set(v for c in conditions if c in self for v in self[c])),
        )  # this computation could be wasted

        width = max(len("%s" % s) for s in samples)
        freqs = dict()
        for c in conditions:
            if cumulative:
                freqs[c] = list(self[c]._cumulative_frequencies(samples))
            else:
                freqs[c] = [self[c][sample] for sample in samples]
            width = max(width, max(len("%d" % f) for f in freqs[c]))

        condition_size = max(len("%s" % c) for c in conditions)
        print(" " * condition_size, end=" ")
        for s in samples:
            print("%*s" % (width, s), end=" ")
        print()
        for c in conditions:
            print("%*s" % (condition_size, c), end=" ")
            for f in freqs[c]:
                print("%*d" % (width, f), end=" ")
            print()

    # Mathematical operators

    def __add__(self, other):
        """
        Add counts from two ConditionalFreqDists.
        """
        if not isinstance(other, ConditionalFreqDist):
            return NotImplemented
        result = ConditionalFreqDist()
        for cond in self.conditions():
            newfreqdist = self[cond] + other[cond]
            if newfreqdist:
                result[cond] = newfreqdist
        for cond in other.conditions():
            if cond not in self.conditions():
                for elem, count in other[cond].items():
                    if count > 0:
                        result[cond][elem] = count
        return result

    def __sub__(self, other):
        """
        Subtract count, but keep only results with positive counts.
        """
        if not isinstance(other, ConditionalFreqDist):
            return NotImplemented
        result = ConditionalFreqDist()
        for cond in self.conditions():
            newfreqdist = self[cond] - other[cond]
            if newfreqdist:
                result[cond] = newfreqdist
        for cond in other.conditions():
            if cond not in self.conditions():
                for elem, count in other[cond].items():
                    if count < 0:
                        result[cond][elem] = 0 - count
        return result

    def __or__(self, other):
        """
        Union is the maximum of value in either of the input counters.
        """
        if not isinstance(other, ConditionalFreqDist):
            return NotImplemented
        result = ConditionalFreqDist()
        for cond in self.conditions():
            newfreqdist = self[cond] | other[cond]
            if newfreqdist:
                result[cond] = newfreqdist
        for cond in other.conditions():
            if cond not in self.conditions():
                for elem, count in other[cond].items():
                    if count > 0:
                        result[cond][elem] = count
        return result

    def __and__(self, other):
        """
        Intersection is the minimum of corresponding counts.
        """
        if not isinstance(other, ConditionalFreqDist):
            return NotImplemented
        result = ConditionalFreqDist()
        for cond in self.conditions():
            newfreqdist = self[cond] & other[cond]
            if newfreqdist:
                result[cond] = newfreqdist
        return result

    # @total_ordering doesn't work here, since the class inherits from a builtin class
    def __le__(self, other):
        if not isinstance(other, ConditionalFreqDist):
            raise_unorderable_types("<=", self, other)
        return set(self.conditions()).issubset(other.conditions()) and all(
            self[c] <= other[c] for c in self.conditions()
        )

    def __lt__(self, other):
        if not isinstance(other, ConditionalFreqDist):
            raise_unorderable_types("<", self, other)
        return self <= other and self != other

    def __ge__(self, other):
        if not isinstance(other, ConditionalFreqDist):
            raise_unorderable_types(">=", self, other)
        return other <= self

    def __gt__(self, other):
        if not isinstance(other, ConditionalFreqDist):
            raise_unorderable_types(">", self, other)
        return other < self

    def __repr__(self):
        """
        Return a string representation of this ``ConditionalFreqDist``.
        :rtype: str
        """
        return "<ConditionalFreqDist with %d conditions>" % len(self)
         * */
    }
}
