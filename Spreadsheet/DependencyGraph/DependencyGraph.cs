// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta 
//               (Clarified meaning of dependent and dependee.)
//               (Clarified names in solution/project structure.)
// Version 1.3 - Andrew Porter, 7 Sept 2020
//               Filled in skeleton for PS2  
//               Uses 2 dictionaries to keep track of dependencies between 2 string variables

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpreadsheetUtilities
{

    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    /// 
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings.  Two ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1 equals t2.
    /// Recall that sets never contain duplicates.  If an attempt is made to add an element to a 
    /// set, and the element is already in the set, the set remains unchanged.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
    ///        (The set of things that depend on s)    
    ///        
    ///    (2) If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
    ///        (The set of things that s depends on) 
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    //     dependents("a") = {"b", "c"}
    //     dependents("b") = {"d"}
    //     dependents("c") = {}
    //     dependents("d") = {"d"}
    //     dependees("a") = {}
    //     dependees("b") = {"a"}
    //     dependees("c") = {"a"}
    //     dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        private Dictionary<string, HashSet<string>> dependents;
        private Dictionary<string, HashSet<string>> dependees;
        private int dependentPairs;
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
            dependentPairs = 0;
        }


        /// <summary>
        /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return this.dependentPairs; }
        }

        /// <summary>
        /// The size of dependees(s).
        /// This property is an example of an indexer.  If dg is a DependencyGraph, you would
        /// invoke it like this:
        /// dg["a"]
        /// It should return the size of dependees("a")
        /// </summary>
        public int this[string s]
        {
            get { return dependees[s].Count(); }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.
        /// </summary>
        public bool HasDependents(string s)
        {
            if(dependents[s].Count() == 0)
                return true;
            return false;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.
        /// </summary>
        public bool HasDependees(string s)
        {        
            if(dependees[s].Count() == 0)
                return true;
            return false;
        }
        
        /// <summary>
        /// Enumerates dependents(s).
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if(dependents.ContainsKey(s))
                return dependents[s];
            return new HashSet<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if(dependees.ContainsKey(s))
                return dependees[s];
            return new HashSet<string>();
        }

        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist</para>
        /// 
        /// <para>This should be thought of as:</para>   
        /// 
        ///   t depends on s
        ///
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param>        /// 
        public void AddDependency(string s, string t)
        {
            bool containsS = dependents.ContainsKey(s);
            bool containsT = dependees.ContainsKey(t);

            //Creates an entry in the dictionary if s or t does
            //not already exist
            if (!containsS)
                dependents.Add(s, new HashSet<string>());
            if (!containsT)
                dependees.Add(t, new HashSet<string>());

            //Checks to see if the relation already occurs
            //if not, it is created
            if (!dependents[s].Contains(t))
            {
                dependentPairs++;
                dependents[s].Add(t);
            }
            if (!dependees[t].Contains(s))
            {
                dependees[t].Add(s);
            }
        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t) //Test if s doesnt exist
        {
            //If s exists in the graph, it will remove its
            //dependent and s will be taken off t's dependee list
            if (dependents[s].Contains(t))
            {
                dependentPairs--;
                dependents[s].Remove(t);
                dependees[t].Remove(s);
            }
        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r).  Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if (dependents.ContainsKey(s))
            {
                int k = dependents[s].Count();
                //Removes s from the dependees list
                for (int i = 0; i < k; i++)
                {
                    string dpee = dependents[s].ElementAt(i);
                    RemoveDependency(s, dpee);
                }
            }
            //Adds the new dependents to s
            for (int i = 0; i < newDependents.Count(); i++)
                AddDependency(s, newDependents.ElementAt(i));
            if (newDependents.Count() == 0)
            {
                if (dependents.ContainsKey(s))
                    dependents[s] = new HashSet<string>();
                else
                    dependents.Add(s, new HashSet<string>());
            }

        }

        /// <summary>
        /// Removes all existing ordered pairs of the form (r,s).  Then, for each 
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {
            if (dependees.ContainsKey(s))
            {
                int k = dependees[s].Count();
                //Removes every instance of s in the dependents list
                for (int i = 0; i < k; i++)
                {
                    string dpt = dependees[s].ElementAt(0);
                    RemoveDependency(dpt, s);
                }
            }
            //Adds dependees to s
            for (int i = 0; i < newDependees.Count(); i++)
               AddDependency(newDependees.ElementAt(i), s);
            if (newDependees.Count() == 0)
            {
                if (dependees.ContainsKey(s))
                    dependees[s] = new HashSet<string>();
                else
                    dependees.Add(s, new HashSet<string>());
            }
        }
    }
}
