"""
Karl Estes
CSC 525 Principles of Artificial Intelligence
Created: January 5th, 2021
Due: January 9th, 2021

Asignment Prompt
----------------
For your assignment, you will build a polynomial regression model in Python.

Please download the [Positions Salaries](https://s3.us-west-2.amazonaws.com/public.gamelab.fun/dataset/position_salaries.csv) dataset in CSV format:
Using this data, our model should be able to predict the value of an employee candidate given their years of experience.

Consider using Google to conduct your own research for this assignment. Feel free to also use the following documentation and resources:

[Robust nonlinear regression in scipy (Links to an external site.)](https://scipy-cookbook.readthedocs.io/items/robust_regression.html)
[Machine Learning: Polynomial Regression with Python (Links to an external site.)](https://towardsdatascience.com/machine-learning-polynomial-regression-with-python-5328e4e8a386)

Submission should include an executable Python file demonstrating the prediction of employee salary based on years of experience.

File Description
----------------
A simple comparison of linear and polynomial regression. 

Two models (linear and polynomial) are fit using sklearn on salary data. The Root Mean Squared Error (RMSE) is calculated for both and displayed on the graphs

The data should exists in a csv file in the following format:

    Position    Level   Salary
    Title 1     1       10000
    Title 2     2       20000
    ...         ...     ...
    Title N     N       N

Three optional arguments [--path_to_data, --degrees, --years], are optional for the script
    --path_to_data points to a csv file with data contained in the aforementioned format
    --degrees takes a single integer denoting the numby of polynomial degrees with which to fit the polynomial regression model
    --years takes an number and prints the predicted salary for both regressor based on that input

Comment Anchors
---------------
I am using the Comment Anchors extension for Visual Studio Code which utilizes specific keywords
to allow for quick navigation around the file by creating sections and anchor points. Any use
of "anchor", "todo", "fixme", "stub", "note", "review", "section", "class", "function", and "link" are used in conjunction with 
this extension. To trigger these keywords, they must be typed in all caps. 
"""

import pandas as pd
import sklearn
import matplotlib.pyplot as plt
import numpy as np
import argparse
import math
from sklearn.linear_model import LinearRegression
from scipy.interpolate import make_interp_spline

# Creates the argparser
parser = argparse.ArgumentParser()
parser.add_argument("--path_to_data", type=argparse.FileType(mode="r"), default='position_salaries.csv', help="Path to file containing the salary data")
parser.add_argument("--degrees", type=int, default=4, help="Degree for polynomial feature transform")
parser.add_argument('--years', type=float, default=None, help="Pass an number value to print a salary prediction based on linear and polynomial regressors")

def round_dec(num, places = 2):
    """Rounds the provided number to a specific number of decimal places (defaulted to 2)"""

    multiplier = 10 ** places

    return math.trunc(num * multiplier) / multiplier


if __name__ == "__main__":
    args = parser.parse_args()

    try:
        df = pd.read_csv(args.path_to_data)
    except:
        print("Error reading {} into dataframe".format(args.path_to_data))
        exit(1)

    # Split the data
    X = df.iloc[:, 1:2].values # Get the year experience data
    y = df.iloc[:, 2].values # Get the salary data

    # Simple Linear Regression Model for comparison
    lin_reg = LinearRegression()
    lin_reg.fit(X,y) 

    # Polynomial Regression
    poly_fts = sklearn.preprocessing.PolynomialFeatures(degree=args.degrees)
    X_poly = poly_fts.fit_transform(X)
    poly_reg = LinearRegression()
    poly_reg.fit(X_poly, y)

    # Do prediction if provided
    if args.years != None:
        year = np.array([[args.years]])
        print("Years of experience: {}".format(args.years))
        print("Linear Reg Predicted Salary= ${}".format(lin_reg.predict(year)))
        print("Polynomial Reg Predicted Salary= ${}".format(poly_reg.predict(poly_fts.fit_transform(year))))

    # Calculate RMSE
    lin_preds = lin_reg.predict(X)
    poly_preds = poly_reg.predict(X_poly)

    rmse = {"linear": round_dec(np.sqrt(sklearn.metrics.mean_squared_error(y,lin_preds)), 4),
            "polynomial": round_dec(np.sqrt(sklearn.metrics.mean_squared_error(y, poly_preds)),4)}

    # Smooth Polynomial Regression Graph
    spline = make_interp_spline(np.array(np.reshape(X,10)), poly_preds)
    x_spline = np.linspace(X.min(), X.max(), 500)
    y_spline = spline(x_spline)

    # Plot Graphs
    fig, axes = plt.subplots(1, 2, sharex=False, sharey=False, figsize=(12,7))

    axes[0].scatter(X, y, color='red', label='Data')
    axes[0].plot(X, lin_preds, color='blue', label='Predictions')
    axes[0].legend()
    axes[0].set_title("Linear Regression\nRMSE: {}".format(rmse['linear']))
    axes[0].set_ylabel("Salary")
    axes[0].set_xlabel("Level")

    axes[1].scatter(X, y, color='red', label='Data')
    axes[1].plot(x_spline, y_spline, color='blue', label='Predictions')
    axes[1].legend()
    axes[1].set_title("Polynomial Regression (Poly Degree: {})\nRMSE: {}".format(args.degrees, rmse['polynomial']))
    axes[1].set_ylabel("Salary")
    axes[1].set_xlabel("Level")

    plt.show()