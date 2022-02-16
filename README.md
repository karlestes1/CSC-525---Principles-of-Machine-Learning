# CSC 525 - Principles of Machine Learning
**Disclaimer:** These projects were built as a requirement for CSC 525: Principles of Machine Learning at Colorado State University Global under the instruction of Dr. Pubali banerjee. Unless otherwise noted, all programs were created to adhere to explicit guidelines as outlined in the assignment requirements I was given. Descriptions of each [programming assignment](#programming-assignments) and the [portfolio project](#portfolio-project) can be found below.

*****This class has been completed, so this repository is archived.*****
___

### Languages and Tools
[<img align="left" height="32" width="32" src="https://cdn.svgporn.com/logos/python.svg" />](https://www.python.org)
[<img align="left" height="32" width="32" src="https://www.psych.mcgill.ca/labs/mogillab/anaconda2/lib/python2.7/site-packages/anaconda_navigator/static/images/anaconda-icon-512x512.png" />](https://www.anaconda.com/pricing)
[<img align="left" height="32" width="32" src="https://cdn.svgporn.com/logos/visual-studio-code.svg" />](https://code.visualstudio.com)
[<img align="left" height="32" width="32" src="https://cdn.svgporn.com/logos/git-icon.svg" />](https://git-scm.com)
[<img align="left" height="32" width="32" src="https://cdn.svgporn.com/logos/gitkraken.svg" />](https://www.gitkraken.com)
[<img align="left" height="32" width="32" src="https://cdn.freebiesupply.com/logos/large/2x/unity-69-logo-black-and-white.png" />](https://unity.com)
<br />
<br />

- [Unity ML Agents Toolkit](https://github.com/Unity-Technologies/ml-agents)
- [scikit-learn](https://scikit-learn.org/stable/)
### Textbook
The textbook for this class was [**Machine Learning in Python for Everyone**](https://www.oreilly.com/library/view/machine-learning-with/9780134845708/) by **Mark E. Fenner**
### VS Code Comment Anchors Extension
I am also using the [Comment Anchors extension](https://marketplace.visualstudio.com/items?itemName=ExodiusStudios.comment-anchors) for Visual Studio Code which places anchors within comments to allow for easy navigation and the ability to track TODO's, code reviews, etc. You may find the following tags intersperesed throughout the code in this repository: ANCHOR, TODO, FIXME, STUB, NOTE, REVIEW, SECTION, LINK, CELL, FUNCTION, CLASS

For anyone using this extension, please note that CELL, FUNCTION, and CLASS are tags I defined myself. 
<br />
___

## Programming Assignments

### Critical Thinking 2: [Predicting Video Game Preferences with KNN](CT%202/)
- This assignment required the implementation of a KNN to predict video games preferences 
- The *dataset* used is synthetic and was provided as part of the class assignment

### Critical Thinking 3: [Simple Polynomial Regression in Scikit Learn](CT%203/)
- A simple script demonstrating the use of polynomial regression with scikit-learn

### Critical Thinking 5: [Image Dataset Augmentation](CT%205/)
- A program to perform a sequences of image augmentations to generated an augmented image dataset.
- The [Augmentor](https://augmentor.readthedocs.io/en/master/) library was used to implement the augmentation pipeline

### Critical Thinking 6: [Feature Engineering and Hyperparameter Tuning](CT%206/)
- There is no code for this assignment, but I thought the discussion around feature engineering and hyperparameter tuning was interesting. The paper for this assignment discusses a theoretical model for Fake News detection and some feature engineering and hyperparemter tuning approaches it might benefit from. 

___

## Portfolio Project: [ML Agents - Autonomous Vehicle](Portfolio%20Project/)
- The final project for this class required to creation of an Agent utilizing the Unity ML-Agents Toolkit. I chose to create a simple Autonomous Vehicle Agent. The Agent could control thrust, braking, and steering with the goal of avoiding any obstacles in its path on the way to a goal.
- The final version of the agent was trained with simple curriculum learning (learn to drive with on obstacles, and then with) and used Generative Adversarial Imitation Learning (GAIL) in conjunction with extrinsic reward signals.
- While the final agent was far from perfect, it could successfully navigate around a series of randomly placed obstacles

![](Portfolio%20Project/Video/recording.mp4)