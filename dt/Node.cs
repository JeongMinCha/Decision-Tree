using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace dt
{
	public class Node
	{
		protected int classLabel = 0;		// only for leaf node
		protected int splitAttrVal = 0;		// not for root node
		protected int splitAttrIdx = 0;		// not for leaf nodes

		private static int attrCount = 0;
		// represents whether attributes can be selected as a test attribue
		private BitArray availableAttrs = null;
		private List<Tuple> tuples = null;
		private List<Node> childNodes = null;

		#region Constructor Implementation
		public Node ()
		{
			attrCount = Globals.attrCount;
			availableAttrs = new BitArray(attrCount);
			availableAttrs.SetAll(true);
			availableAttrs.Set(attrCount-1, false);
			tuples = new List<Tuple>();
			childNodes = new List<Node>();
		}

		public Node (BitArray parentAvailAttrs)
		{
			attrCount = Globals.attrCount;
			availableAttrs = (BitArray) parentAvailAttrs.Clone();	// !!
			tuples = new List<Tuple>();
			childNodes = new List<Node>();
		}
		#endregion

		public int Traverse (Tuple t)
		{
			int attrIdx = splitAttrIdx;
			int attrVal = t.values[attrIdx];

			/* When there are no child nodes */
			if (childNodes.Count == 0)
				return classLabel;	// majority voting

			foreach (Node childNode in childNodes)
			{
				if (childNode.splitAttrVal == attrVal)	
				{
					return childNode.Traverse(t);
				}
			}

			/* Whgen not matched value with child nodes */
			return classLabel;	// majority voting
		}

		public void AddTuple (Tuple t)
		{
			tuples.Add(t);
		}

		public List<Tuple> GetTuples ()
		{
			return tuples;
		}

		public void DisableAttr (int idx)
		{
			availableAttrs.Set(idx, false);
		}

		public void SetTupleList (List<Tuple> tupleList)
		{
			tuples = tupleList;
			tuples.TrimExcess();
		}

		private bool CheckTuplesAreSameClass ()
		{
			int label = 0;
			int attrIdx = attrCount-1;
			foreach (Tuple t in tuples)
			{
				if (label == 0)
				{
					label = t.values[attrIdx];
				}
				else if (label != t.values[attrIdx])	
				{
					return false;
				}
			}
			return true;
		}

		/* majority voting */
		private int ClassLabel ()
		{
			int attrIdx = attrCount-1;
			int[] counts = new int[Globals.attrs[attrIdx].values.Count];
			foreach (Tuple t in tuples)
			{
				int attrVal = t.values[attrIdx];
				++ counts[attrVal-1];
			}

			int label = 0;
			int maxCount = 0;
			for (int i=0; i<counts.Length; ++i)
			{
				int count = counts[i];
				if (maxCount < count)
				{
					maxCount = count;
					label = i+1;
				}
			}
			return label;
		}

		/* Returns true if splitting this node is possible. 
		   Returns false if it's not possible. */
		public void SplitNode ()
		{
			classLabel = ClassLabel();	// majority voting

			// If tuples in this node are all of the same class
			if (CheckTuplesAreSameClass() == true)
			{
				return;
			}
			// If attribute_list is empty
			if (CheckRemainingAttrs() == false)
			{
				return;
			}

			int attrIdx = splitAttrIdx = SelectAttribute();
			int attrValCnt = Globals.attrs[attrIdx].values.Count;
			List<Tuple>[] tupleLists = new List<Tuple>[attrValCnt];
			for (int i=0; i<attrValCnt; ++i)
				tupleLists[i] = new List<Tuple>();

			foreach (Tuple t in tuples)
			{
				int attrVal = t.values[attrIdx];
				tupleLists[attrVal-1].Add(t);
			}
			for (int i=0; i<tupleLists.Length; ++i)
			{
				if (tupleLists[i].Count != 0)
				{
					Node node = new Node(this.availableAttrs);
					node.DisableAttr(attrIdx);
					node.SetTupleList(tupleLists[i]);
					node.splitAttrVal = (i+1);
					childNodes.Add(node);
				}
			}
			tuples.Clear();
				
			foreach (Node childNode in childNodes)
				childNode.SplitNode();
		}

		private bool CheckRemainingAttrs ()
		{
			int remainingAttrs = 0;
			foreach (bool availablity in availableAttrs)
			{
				if (availablity.Equals(true))
					++ remainingAttrs;
			}
			if (remainingAttrs == 0)
				return false;
			else
				return true;
		}

		private int SelectAttribute()
		{
			int index = 0;
			double maxGain = 0D;
			double infoD = Info(this.tuples);

			for (int i=0; i<attrCount; ++i)
			{
				if (availableAttrs[i] == true)
				{
					double infoAttr = Info(this.tuples, i);
					// Gain(A) = Info(D) - InfoA(D)
					double gain = infoD - infoAttr;

					if (gain > maxGain) {
						maxGain = gain;
						index = i;
					}
				}
			}
			return index;
		}

		public double Info (List<Tuple> tuples, int attrIdx)
		{
			double infoAttr = 0D;
			int totalCnt = tuples.Count;
			int[] counts = new int[Globals.attrs[attrIdx].values.Count];
			List<Tuple>[] tupleLists = new List<Tuple>[counts.Length];
			for(int i=0; i<tupleLists.Length; ++i)
				tupleLists[i] = new List<Tuple>();

			foreach (Tuple t in tuples)
			{
				int valIdx = t.values[attrIdx]-1;
				++ counts[valIdx];
				tupleLists[valIdx].Add(t);
			}
			for (int i=0; i<counts.Length; ++i)
			{
				int count = counts[i];
				if (count != 0)
				{
					double frac = count/(double)totalCnt;
					double subInfo = Info(tupleLists[i]);
					infoAttr += (frac) * subInfo;
				}
			}
			return infoAttr;
		}

		public double Info (List<Tuple> tuples)
		{
			double infoD = 0D;
			int attrIdx = attrCount-1;
			int totalCnt = tuples.Count;
			int[] counts = new int[Globals.attrs[attrIdx].values.Count];

			foreach (Tuple t in tuples)
			{
				int valIdx = t.values[attrIdx]-1;
				++ counts[valIdx];
			}

			foreach (int count in counts)
			{
				if (count != 0)
				{
					double frac = count/(double)totalCnt;
					infoD -= (frac)*(Math.Log10(frac)/Math.Log10(2));
				}
			}
			return infoD;
		}
	}
}