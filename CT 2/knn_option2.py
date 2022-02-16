"""
Karl Estes
CSC 525: Critical Thinking 2 - Option 2
Created: December 22, 2021
Due: January 2nd, 2021

Asignment Prompt
----------------
here are several advantages of KNN classification, one of them being simple implementation. 
Search space is robust as classes do not need to be linearly separable. 
It can also be updated online easily as new instances with known classes are presented.

A KNN model can be implemented using the following steps:

1.	Load the data;
2.	Initialise the value of k;
3.	For getting the predicted class, iterate from 1 to total number of training data points;
4.	Calculate the distance between test data and each row of training data;
5.	Sort the calculated distances in ascending order based on distance values;
6.	Get top k rows from the sorted array;
7.	Get the most frequent class of these rows; and
8.	Return the predicted class.

For your assignment, you will build a KNN classifier in Python.
Download the class [data](https://gist.githubusercontent.com/dhar174/14177e1d874a33bfec565a07875b875a/raw/7aa9afaaacc71aa0e8bc60b38111c24e584c74d8/data.csv) in CSV format:

Using this data, your classifier should be able to predict a person's favorite video game genre based on their age, height, weight, and gender. 
(Do not worry about real-world accuracy here. This is to provide you an opportunity to practice.)

Submission should include an executable Python file that accepts input of 4 floating point numbers representing, respectively, age (in years), height (in inches),
weight (in lbs), and gender (females represented by 0s and males represented by 1s).

File Description
----------------
A simple K-Nearest Neighbor classifier is implemented below. The python script takes 4 positional arguments corresponding to the individuals age, height, weight, and gender (0=F, 1=M)

Two optional arguments [--path_to_data, -k], are also available where
    --path_to_data points to a csv file with data for the K-NN comparisons
    -k sets how many neighbors to compare (will defaults to 5)

Distance is calculated for each feature by simpling taking the absolute value of the difference between the provided value and the data-point'v value. 
The total distance is the sum of all the feature differences.

Comment Anchors
---------------
I am using the Comment Anchors extension for Visual Studio Code which utilizes specific keywords
to allow for quick navigation around the file by creating sections and anchor points. Any use
of "anchor", "todo", "fixme", "stub", "note", "review", "section", "class", "function", and "link" are used in conjunction with 
this extension. To trigger these keywords, they must be typed in all caps. 
"""
import pandas as pd
import argparse


if __name__ == '__main__':

    # Create the argparser
    parser = argparse.ArgumentParser()
    parser.add_argument("age", type=float, help="Age of the individual in years")
    parser.add_argument("height", type=float, help="Height of the individual in inches (rounded to nearest whole)")
    parser.add_argument("weight", type=float, help="Weight of the individual in pounds (rounded to the nearest whole)")
    parser.add_argument("gender", type=float, help="Gender: F=0, M=1")
    parser.add_argument('-k', type=int, default=5, help="Number of nearest neighbors to compare too")
    parser.add_argument("--path_to_data", type=str, default="data.csv", help="Path to file containing the training data")

    args = parser.parse_args()


    if (args.gender < 0) or (args.gender > 1):
        print("Please ensure --gender is either 0 or 1")
        exit(1)

    try:
        df = pd.read_csv(args.path_to_data, names=['age','height','weight','gender','genre'])
    except:
        print("Error loading file: {} into dataframe".format(args.path_to_data))
        exit(2)

    # Calculate distance
    df['distance'] = abs(df['age'] - args.age) + abs(df['height'] - args.height) + abs(df['weight'] - args.weight) + abs(df['gender'] - args.gender)

    # Sort values
    df = df.sort_values('distance')

    # Get prediction
    counts = df.head(args.k)['genre'].value_counts()
    preds = counts.loc[counts == counts.max()].index.values

    # Display output
    gen = "F" if args.gender == 0 else "M"
    print("\n{:>15}: {:<4}\n{:>15}: {:<4} years\n{:>15}: {:<4} inches\n{:>15}: {:<4} pounds\n{:>15}: {:<1} ({})".format('k-NN',args.k,'age',args.age,'height',args.height,'weight',args.weight,'gender',gen,int(args.gender)))
    print("Predicted Genre: {}\n".format(preds))

    print(" * * Value Counts * *")
    print(counts)
    print("\n")

    print("* * Data Rows * *")
    print(df.head(args.k))
    print("\n")