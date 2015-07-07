using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace dt
{
	public class DecisionTree
	{
		private Node rootNode = null;		
		private StreamReader trainSr = null;// StreamReader for reading training data file.

		public DecisionTree (string trainingFile)
		{
			trainSr = new StreamReader(trainingFile);
			// parse the first line to specify each name of attributes.
			SetAttributeNames();
			rootNode = new Node();
			// set attribute values by using the rest of lines of training data.
			SetAttributeValues();
			trainSr.Close();
		}

		public void Test (string testFile, string resultFile)
		{
			StreamWriter sw = new StreamWriter(resultFile);
			sw.AutoFlush = true;
			// write the first line, which specifies names of attributes
			foreach (Attribute attr in Globals.attrs)
				sw.Write(attr.name + "\t");
			sw.WriteLine();

			StreamReader sr = new StreamReader(testFile);

			sr.ReadLine();	// ignore first line
			string line = null;
			while ((line = sr.ReadLine()) != null)
			{
				string[] words = line.Split('\t');
				Tuple t = new Tuple();
				for (int i=0; i<words.Length; ++i)
				{
						t.values[i] = Globals.attrs[i].values[words[i]];
						sw.Write(words[i] + "\t");
				}
				int classLabel = ClassifyTuple(t);
				Attribute classAttr = Globals.attrs[Globals.attrCount-1];
				foreach (string val in classAttr.values.Keys)
				{
					if (classLabel == classAttr.values[val])
					{
						sw.WriteLine(val);
					}
				}
			}
			sr.Close();
			sw.Close();
		}

		public int ClassifyTuple (Tuple t)
		{
			return rootNode.Traverse(t);
		}

		/* set each of attribute names globally 
		 * from the FIRST LINE of training file. */
		private void SetAttributeNames ()
		{
			string line = trainSr.ReadLine();	// first line
			string[] attrNames = line.Split(Globals.delim);

			foreach (string attrName in attrNames)
			{
				Globals.attrs.Add(new Attribute(attrName));
				Console.WriteLine(attrName);
			}

			Globals.attrCount = Globals.attrs.Count;
		}

		/* Set attribute values for all attributes.
		 * after this method called, Globals.attrs will have all values for attributes */
		private void SetAttributeValues()
		{
			HashSet<string>[] attrValSets = AttrValSets();
			for (int i=0; i<attrValSets.Length; ++i)
			{
				int index = 1;
				HashSet<string> attrValSet = attrValSets[i];

				foreach (string attrVal in attrValSet) {
					Globals.attrs[i].values.Add(attrVal, index++);
				}
			}
		}

		/* Returns array(list) of set of attribute values,
		 * e.g. [<=30,31...40,>40]-[high,medium,low]-[yes,no]-... */
		private HashSet<string>[] AttrValSets ()
		{
			int attrCount = Globals.attrCount;
			HashSet<string>[] attrValSets = new HashSet<string>[Globals.attrCount];
			for(int i=0; i<attrCount; ++i) {
				attrValSets[i] = new HashSet<string>();
			}

			string line = null;
			while ((line = trainSr.ReadLine()) != null)
			{
				string[] words = line.Split('\t');
				for (int i=0; i<attrCount; ++i) {
					attrValSets[i].Add(words[i]);
				}
			}

			return attrValSets;
		}

		/* Add tuples to the root node */
		private void AddTuplesForRootNode ()
		{
			// return to start position 
			trainSr.BaseStream.Position = 0;
			trainSr.DiscardBufferedData();
			trainSr.ReadLine();	// ignore the first line

			string line = null;
			while ((line = trainSr.ReadLine()) != null)
			{
				Tuple t = new Tuple();
				string[] words = line.Split('\t');

				for (int i=0, length=Globals.attrCount; i<length; ++i)
				{
					try {
						t.values[i] = Globals.attrs[i].values[words[i]];
					} catch (IndexOutOfRangeException e) {
						Console.WriteLine(e.StackTrace);
						System.Environment.Exit(-1);
					}
				}
				rootNode.AddTuple(t);
			}
		}

		public void ConstructTree (string trainingFile)
		{
			trainSr = new StreamReader(trainingFile);
			// add tuples into root node of this instance.
			AddTuplesForRootNode();
			rootNode.SplitNode();
			trainSr.Close();
		}
	}
}