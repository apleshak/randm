﻿using UnityEngine;
using System;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/* All sorts of useful generic algorithms and goodies. */
public static class Utilities
{
	static GameObject _dummy;
	public static GameObject dummy
	{
		get
		{
			if (_dummy == null)
			{
				_dummy = new GameObject();
			}
				
			return _dummy;
		}
	}
	
	public static HashSet<K> Map<T, K> (IEnumerable<T> collection, Func<T, K> f)
	{
		HashSet<K> result = new HashSet<K>();
		
		foreach (T item in collection)
		{
			result.Add(f(item));
		}
		
		return result;
	}
	
	/// <summary>
	/// Apply f to every element in the collection.
	/// </summary>
	///<returns>A collection where each element is the result of applying f to it.</returns>
	/// <param name="collection">Collection to map.</param>
	/// <param name="f">Function to apply to each element.</param>
	/// <typeparam name="T">The type of each element in the collection.</typeparam>
	/// <typeparam name="K">The resulting type of each element in the collection.</typeparam>
	/// <typeparam name="L">The type of the collection returned.</typeparam>
	public static L Map<T, K, L> (IEnumerable<T> collection, Func<T, K> f) where L : ICollection<K>, new()
	{
		L result = new L();
		
		foreach (T item in collection)
		{
			result.Add(f(item));
		}
		
		return result;
	}
	
	public static K Clone<T, K> (IEnumerable<T> collection) where K : ICollection<T>, new()
	{
		return Map<T, T, K>(collection, (a) => {return a;});
	}
	
	/// <summary>
	/// Gets the best ordering of actors based on an evalutation of their assignment.
	/// </summary>
	/// <returns>Tuple of ordering of actors, assignment and score.</returns>
	/// <param name="actors">Actors.</param>
	/// <param name="assignments">All possible assignments.</param>
	/// <param name="eval">Function to evaluate an ordering of actors given an assignment.</param>
	/// <typeparam name="T">The type of an assignment.</typeparam>
	/// <typeparam name="K">The type of an actor.</typeparam>
	public static Tuple<List<K>, T, float> GetBestOrdering<T, K> (ICollection<K> actors, ICollection<T> assignments, Func<List<K>, T, float> eval)
	{
		HashSet<List<K>> allPerms = Utilities.GetAllPermutations<K>(actors);
		T bestAssignment = default(T);
		List<K> bestOrdering = null;
		float bestScore = 0;
		
		foreach (T assignment in assignments)
		{
			foreach (List<K> ordering in allPerms)
			{
				float orderingScore = eval(ordering, assignment);
				
				if (orderingScore > bestScore)
				{
					bestScore = orderingScore;
					bestAssignment = assignment;
					bestOrdering = ordering;
				}
			}
		}

		return new Tuple<List<K>, T, float>(bestOrdering, bestAssignment, bestScore);
	}

	public static HashSet<List<T>> GetAllPermutations<T> (ICollection<T> collect)
	{
		//Clone the original input to ensure non-destructibility
		HashSet<T> collection = Utilities.Clone<T, HashSet<T>>(collect);
		return GetAllPermutationsDestructive<T>(collection);
	}
	
	static HashSet<List<T>> GetAllPermutationsDestructive<T> (ICollection<T> collection)
	{
		//Base case
		if (collection.Count < 2)
		{
			HashSet<List<T>> ans = new HashSet<List<T>>();
			ans.Add(new List<T>(collection));
			return ans;
		}
		
		//Recursive case
		T item = collection.First<T>();
		collection.Remove(item);
			
		HashSet<List<T>> recursive = GetAllPermutationsDestructive<T>(collection);
		HashSet<List<T>> results = new HashSet<List<T>>();

		foreach (List<T> permutation in recursive)
		{
			for (int i = 0; i < permutation.Count; i++)
			{
				permutation.Insert(i, item);
				results.Add(Utilities.Clone<T, List<T>>(permutation));
				permutation.RemoveAt(i);
				
				if (i == permutation.Count - 1)
				{
					permutation.Add(item);
					results.Add(permutation);
					permutation.RemoveAt(permutation.Count-1);
					//dont even have to remove because we're throwing it away
				}
			}
		}
		
		return results;
	}
	
	/*Returns all possible subsets of the collection using bit vector mask method. */
	public static HashSet<T> GetAllSubsets<T, K> (T collection) where T : ICollection<K>, new()
	{
		int maxPow = collection.Count;
		HashSet<T> allSubsets = new HashSet<T>();
		
		for (int i = 0; i < Math.Pow(2, maxPow); i++)
		{
			int j = i;
			int index = 0;
			T subset = new T();
			
			while (j > 0)
			{
				if (j % 2 == 1)
				{
					subset.Add(collection.ElementAt<K>(index));
				}
				
				index += 1;
				j /= 2;
			}
			
			allSubsets.Add(subset);
		}

		return allSubsets;
	}
	
	/// <summary>
	/// Gets the best grouping of items based on maximizing the sum of eval calls on each group.
	/// </summary>
	/// <returns>A tuple of the best grouping and the sum of eval calls on each group.</returns>
	/// <param name="items">Items.</param>
	/// <param name="eval">Eval function.</param>
	/// <typeparam name="T">The type of each element in a group.</typeparam>
	public static Tuple<HashSet<HashSet<T>>, float> GetBestGrouping<T> (HashSet<T> items, Func<ICollection<T>, float> eval)
	{
		Tuple<HashSet<HashSet<T>>, float> best = new Tuple<HashSet<HashSet<T>>, float>(new HashSet<HashSet<T>>(), 0);
		
		/* Base case 1. */
		if (items.Count == 0)
		{
			return best;
		}
		//Base case 2.
		else if (items.Count == 1)
		{
			best.first.Add(items);
			best.second = eval(items);
			return best;
		}
		/* Recursive case. */
		else
		{
			HashSet<HashSet<T>> allSubSets = GetAllSubsets<HashSet<T>, T>(items);
			
			foreach (HashSet<T> set in allSubSets)
			{
				/* Don't recurse on empty set - it will cause an infintie loop. */
				if (set.Count != 0)
				{
					/* Get set of all other items and find a recursive solution. */
					HashSet<T> otherItems = new HashSet<T>(items.Except<T>(set));
					Tuple<HashSet<HashSet<T>>, float> bestRecursive = GetBestGrouping<T> (otherItems, eval);
					
					/* Improved solution found. */
					if (eval(set) + bestRecursive.second > best.second)
					{
						best.first.Clear();
						bestRecursive.first.Add(set);
						best.first = bestRecursive.first;
						best.second = eval(set) + bestRecursive.second;
					}
				}
				/* Evaluate the empty set here. */
				else
				{
					float value = eval(items);

					/* Improved solution found. */
					if (value > best.second)
					{
						best.first.Clear();
						best.first.Add(items);
						best.second = value;
					}
				}
			}
			
			return best;
		}
	}
	
	public static void PrintCollection<K> (ICollection<K> collection)
	{
		string line = "[";

		foreach (K item in collection)
		{
			line += item.ToString() + ", ";
		}
		
		line += "]\n";
		Debug.Log(line);
	}
	
	public static void PrintCollection<K> (ICollection<K> collection, Func<K, string> formatter)
	{
		string line = "[";
		
		foreach (K item in collection)
		{
			line += formatter(item) + ", ";
		}
		
		line += "]\n";
		Debug.Log(line);
	}
	
	/* K is the inner type, T is the enlosing container type. */
	public static void PrintCollection<T, K> (ICollection<T> collection)
	{
		string line = "";
		Debug.Log ("[");
		
		foreach (T item in collection)
		{
			if (item is ICollection<K>)
			{
				PrintCollection<K>(item as ICollection<K>);
			}
			else
			{
				line += item.ToString() + ", ";
			}
		}
		
		Debug.Log(line);
		Debug.Log("]\n");
	}
	
	public static GameObject GetChildWithName (GameObject parent, string name)
	{
		if (parent.name == name)
			return parent;
			
		Transform t = parent.transform;
		
		for (int i = 0; i < t.childCount; i++)
		{
			GameObject recursive = GetChildWithName(t.GetChild(i).gameObject, name);
			
			if (recursive != null)
				return recursive;
		}
		
		return null;
	}
	
	public static HashSet<string> GetAllParentTags (GameObject obj)
	{
		HashSet<string> tags = new HashSet<string>();
		Transform t = obj.transform;
		
		while (t.parent != null)
		{
			tags.Add(t.parent.tag);
			t = t.parent.transform;
		}
		
		tags.Add(t.tag);
		return tags;
	}
	
	public static HashSet<string> GetAllChildTags (GameObject obj)
	{
		HashSet<string> tags = new HashSet<string>();
		Transform t = obj.transform;
		
		for (int i = 0; i < t.childCount; i++)
		{
			tags.UnionWith(GetAllChildTags(t.GetChild(i).gameObject));
		}
		
		return tags;
	}
	
	public static HashSet<string> GetAllTags (GameObject obj)
	{
		HashSet<string> tags = GetAllParentTags(obj);
		tags.UnionWith(GetAllChildTags(obj));
		tags.Add(obj.tag);
		return tags;
	}
	
	public static GameObject GetParentWithTag (GameObject obj, string tag)
	{
		Transform t = obj.transform;
		
		while (t.parent != null)
		{
			if (t.parent.tag == tag)
				return t.parent.gameObject;
				
			t = t.parent.transform;
		}
		
		return null;
	}
	
	/*
		Classes
	*/
	
	public class GenericEqualityComparer<T> : IEqualityComparer<T>
	{
		readonly Func<T, T, bool> _comparer;
		readonly Func<T, int> _hash;
		
		public GenericEqualityComparer (Func<T, T, bool> comparer)
			: this( comparer, t => 0 ) // NB Cannot assume anything about how e.g., t.GetHashCode() interacts with the comparer's behavior
		{
		}
		
		public GenericEqualityComparer (Func<T, T, bool> comparer, Func<T, int> hash)
		{
			_comparer = comparer;
			_hash = hash;
		}
		
		public virtual bool Equals(T x, T y)
		{
			return _comparer(x, y);
		}
		
		public virtual int GetHashCode(T obj)
		{
			return _hash(obj);
		}
	}
	
	public class Validator
	{
		Func<bool> eval;
		bool _value;
		public bool valid
		{
			get
			{
				if (eval == null)
				{
					return _value;
				}
				else 
				{
					return eval();
				}
			}
			set
			{
				_value = value;
			}
		}
		
		public Validator ()
		{
			valid = false;
		}
		
		public Validator (bool initial)
		{
			valid = initial;
		}
		
		public Validator (Func<bool> Eval)
		{
			eval = Eval;
			valid = eval();
		}
	}
	
	public class Container<T> : IEquatable<Container<T>>, IEqualityComparer<Container<T>>
	{
		public T data;
		
		public Container (T obj)
		{
			data = obj;
		}
		
		public bool Equals (Container<T> obj)
		{
			return data.Equals(obj.data);
		}
		
		public bool Equals (Container<T> a, Container<T> b)
		{
			return a.Equals(b);
		}
		
		public override int GetHashCode ()
		{
			return data.GetHashCode();
		}
		
		public int GetHashCode (Container<T> a)
		{
			return a.data.GetHashCode();
		}
		
		public override string ToString ()
		{
			return string.Format ("[Container({0})", data);
		}
	}

    // K must implement IComparable
    public class Tuple<T, K> : IComparable<Tuple<T,K>>, IEquatable<Tuple<T, K>>, IEqualityComparer<Tuple<T, K>>
    {
	    public T first;
	    public K second;
	
	    public Tuple (T item1, K item2)
	    {
		    first = item1;
		    second = item2;
	    }

	    public bool Equals (Tuple<T, K> obj)
	    {
		    return first.Equals(obj.first) && second.Equals(obj.second);
	    }
	
	    public bool Equals (Tuple<T, K> a, Tuple<T, K> b)
	    {
		    return a.Equals(b);
	    }

	    //As Josh Bloch suggests in Effective Java 
	    public override int GetHashCode ()
	    {
		    int hash = 17;
		    hash = hash * 31 + first.GetHashCode();
		    hash = hash * 31 + second.GetHashCode();
		    return hash;
	    }
	
	    public int GetHashCode (Tuple<T, K> a)
	    {
		    return a.GetHashCode();
	    }
	
	    public override string ToString ()
	    {
		    return string.Format ("[Tuple({0},{1})", first.ToString(), second.ToString());
	    }
	    // Compares by second value
	    public int CompareTo (Tuple<T, K> other)
	    {
		    return Comparer<K>.Default.Compare(second, other.second);
	    }
    }

    public class Tuple<T, K, L> : IComparable<Tuple<T, K, L>>
    {
        public T first;
        public K second;
        public L third;

        public Tuple(T item1, K item2, L item3)
        {
            first = item1;
            second = item2;
            third = item3;
        }
        /* Compares by second value */
        public int CompareTo(Tuple<T, K, L> other)
        {
            return Comparer<L>.Default.Compare(third, other.third);
        }
    }
}