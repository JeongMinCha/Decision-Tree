# Decision-Tree Project
The purpose of this project is to build **a decision tree** by using the **information gain**, and then classify the test set using it.



## Requirements

#### Execution
* Execution file name: dt.exe
* Execute program with two arguments:
* **training file name, test file name**
* Example: dt.exe dt_train.txt dt_test.txt
* You should create the output file with predicted(classified) class label.


#### File format for training set
[attribute_name_1]\t[attribute_name_2]\t ... [attribute_name_n]\n

[attribute_1]\t[attribute_2]\t ... [attribute_n]\n

[attribute_1]\t[attribute_2]\t ... [attribute_n]\n 

[attribute_1]\t[attribute_2]\t ... [attribute_n]\n


* [attribute_name_1] ~ [attribute_name_n]: n attribute names* [attribute_1] ~ [attribute_n-1]	* n-1 attribute values of the corresponding tuple	* All the attributes are categorical (not continuous-valued)* [attribute_n]: a class label that the corresponding tuple belongs to

#### File format for test set
[attribute_name_1]\t[attribute_name_2]\t ... [attribute_name_n-1]\n

[attribute_1]\t[attribute_2]\t ... [attribute_n-1]\n

[attribute_1]\t[attribute_2]\t ... [attribute_n-1]\n

[attribute_1]\t[attribute_2]\t ... [attribute_n-1]\n

* The test set does not have [attribute_name_n] (class label)

#### File format for output file
[attribute_name_1]\t[attribute_name_2]\t ... [attribute_name_n]\n

[attribute_1]\t[attribute_2]\t ... [attribute_n]\n

[attribute_1]\t[attribute_2]\t ... [attribute_n]\n

[attribute_1]\t[attribute_2]\t ... [attribute_n]\n

* Output file name: dt_result.txt
* You must print the following values:	* [attribute_1] ~ [attribute_n-1]: given attribute values in the test set	* [attribute_n]: a class label predicted by your model for the corresponding tuple