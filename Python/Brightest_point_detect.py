# import the necessary packages
import numpy as np
import argparse
import cv2


# construct the argument parse and parse the argumentses
ap = argparse.ArgumentParser()
ap.add_argument("-i", "--image", help = "path to the image file")
ap.add_argument("-r", "--radius", type = int,
	help = "radius of Gaussian blur; must be odd")
args = vars(ap.parse_args())
# load the image and convert it to grayscale

##### brightest point without any gaussian blur#########
image = cv2.imread(args["image"]) # uses the command that opened it to input the images location.

orig = image.copy()
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
# perform a naive attempt to find the (x, y) coordinates of
# the area of the image with the largest intensity value
(minVal, maxVal, minLoc, maxLoc) = cv2.minMaxLoc(gray)
cv2.circle(image, maxLoc, 5, (255, 0, 0), 2)
# display the results of the naive attempt

#cv2.imshow("Naive", image)


##### brightest point with gaussian blur#########
# apply a Gaussian blur to the image then find the brightest region


gray = cv2.GaussianBlur(gray, (args["radius"], args["radius"]), 0)
#finds pixel withhighest value in grey image
(minVal, maxVal, minLoc, maxLoc) = cv2.minMaxLoc(gray)
image = orig.copy()
cv2.circle(image, maxLoc, args["radius"], (255, 0, 0), 2)
# display the results of our newly improved method

#cv2.imshow("Robust", image)
#cv2.waitKey(0)

print(maxLoc)
# prints the maximum location  to the cmd prompt



filename = r"___________________________________" # write the location that you would like the new file to be written to
cv2.imwrite(filename, image)
