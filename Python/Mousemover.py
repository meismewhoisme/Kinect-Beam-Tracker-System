import mouse
import argparse
import inspect
import ctypes

#used to find screen size, ensure resolution scaling is diabled
user32 = ctypes.windll.user32
screensize = user32.GetSystemMetrics(0), user32.GetSystemMetrics(1)

try: #tries to set it to the set values
    monwidth = screensize[0]
    monheight = screensize[0]
except: #if this fails it sets the width and height to these default values
    monwidth = 1920     #resolution width of monitor being used (change if resolution size changes)
    monheight = 1080    #resolution height of monitor being used (change if resolution size changes)

    
# construct the argument parse and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-xcoor", "--X_Coordinate", type = int, help = "X coordinate on the screen")
ap.add_argument("-ycoor", "--Y_Coordinate", type = int, help = "Y coordinate on the screen")
args = vars(ap.parse_args())

x = args["X_Coordinate"]
y = args["Y_Coordinate"]

#sets values for the screens vertical and horizontal sizes


try:
    x_pos_start = 0
    x_pos_end = 640
    y_pos_start = 0
    y_pos_end = 480

    cam_width = x_pos_end - x_pos_start 
    cam_height = y_pos_end - y_pos_start

    x = x - x_pos_start
    y = y - y_pos_start


except:
    cam_width = 640
    cam_height = 480


x = x/cam_width
y = y/cam_height


x = x * monwidth
y = y * monheight

x = round(x)
y = round(y)

print("x pos: " + str(x) )
print("y pos: " + str(y) )

mouse.move(x, y, absolute=True, duration=0)