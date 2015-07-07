using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace dt
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			CheckArguments(args);

			string resultFile = "./dt_result.txt";
			string trainingFile = args[0];
			string testFile = args[1];

			DecisionTree dt = new DecisionTree(trainingFile);
			dt.ConstructTree(trainingFile);
			dt.Test(testFile, resultFile);
		}

		private static void CheckArguments(string[] args)
		{
			if (args.Length != 2) {
				PrintUsage();
			} else if (File.Exists(args[0]) == false || File.Exists(args[1]) == false) {
				PrintUsage();
			}
		}

		private static void PrintUsage()
		{
			Console.WriteLine("dt.exe [training_set_file_name] [test_set_file_name]");
			Console.WriteLine("You should input names of existing files.");
			Console.WriteLine("ex) dt.exe dt_train.txt dt_test.txt");
			System.Environment.Exit(-1);
		}
	}
}