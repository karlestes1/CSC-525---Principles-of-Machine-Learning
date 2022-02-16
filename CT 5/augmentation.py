"""
Karl Estes
CSC 525: Critical Thinking 5 (Option 2) - Image Dataset Augmentation
Created: January 17th, 2022
Due: January 23rd, 2022

Asignment Prompt
----------------
For your assignment, submit a Python script that will take any image dataset and augment it in some way to expand the 
dataset. Submission must include a script that will augment any image files within its folder. Please include a small 
sample un-augmented dataset with your submission.

File Description
----------------
The following is a simple script to perform a sequences of image augmentation to generated an augmented dataset.
For detailed information on different arguments, run 'python augmentation.py -h'

When calling the script, one integer argument is required denoting the number of augmented images to generate

The augmentation pipeline is created using the Augmentor libray https://augmentor.readthedocs.io/en/master/userguide/mainfeatures.html

Comment Anchors
---------------
I am using the Comment Anchors extension for Visual Studio Code which utilizes specific keywords
to allow for quick navigation around the file by creating sections and anchor points. Any use
of "anchor", "todo", "fixme", "stub", "note", "review", "section", "class", "function", and "link" are used in conjunction with 
this extension. To trigger these keywords, they must be typed in all caps. 
"""

import argparse
import Augmentor

def create_pipeline(args):
    print("Creating pipeline:")
    pipe = Augmentor.Pipeline(source_directory=args.path_to_images, output_directory=args.output_dir)
    print("");
    
    # Check if adding grayscale
    if args.grayscale == True:
        print("\tAdded greyscale conversion at 100% probability")
        pipe.greyscale(1.0)
    
    # Check if resizing
    if args.resize != None:
        print(f"\tAdded image resizing to {args.resize[0]} by {args.resize[1]} at 100% probability")
        pipe.resize(1.0, args.resize[0], args.resize[1])

    # Other operations
    print("\tAdded image mirroring at 25 % probability")
    pipe.flip_left_right(0.25)

    print("\tAdded random brightness adjusting at 50% probability")
    pipe.random_brightness(0.5, 0.5, 1.5)

    print("\tAdded random contrast adjustments at 50% probability")
    pipe.random_contrast(0.5, 0.5, 1.5)
    # pipe.random_erasing(0.5, 0.15)

    print("\tAdded random image rotation at 80% probability")
    pipe.rotate(0.8, 5, 20)

    print("\tAdded random skew at 50% probability")
    pipe.skew(0.5, magnitude=0.2)

    return pipe

if __name__ == "__main__":
    
    # Arg parsing
    parser = argparse.ArgumentParser()
    parser.add_argument("--path_to_images", type=str, default="images/", help="Path to a folder containing images")
    parser.add_argument("--output_dir", type=str, default="../output/", help="Path to output folder. IMPORTANT: When passing a relative path, it will be relative to the input folder (i.e. An output path of output/ will be placed at path_to_images/output)")
    parser.add_argument("-n", type=int, required=True, help="Number of augmented images to produce")
    parser.add_argument("-g", "--grayscale", action="store_true", default=False, required=False, help="Flag for toggling adding grayscale conversion to pipeline")
    parser.add_argument("-r", "--resize", type=int, default=None, nargs="+", help="Width and height (in pexels) to resize an image to")

    args = parser.parse_args()

    # Create pipeline
    p = create_pipeline(args)

    # Generate images
    p.sample(args.n)


