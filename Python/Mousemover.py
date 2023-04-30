# moves the mouse to a set position
#from screeninfo import get_monitors
import mouse
import argparse
import inspect

#Primemon = 0
#monwidth = ""
#monheight = ""

#for m in get_monitors():
 #   string_monitor = str(m)
   # isprimary = string_monitor.split("is_primary=", 1)[1]
    
 #   if (isprimary == "True)"):
   #     Primemon = m
#        string_monitor = str(m)
 #       monwidth = string_monitor.split("width=", 1)[1]
 #       monwidth = monwidth[:4]
#
 #       monheight = string_monitor.split("height=", 1)[1]
#        monheight = monheight[:4]


        

#monwidth = int(monwidth)
#monheight = int(monheight)
import ctypes
user32 = ctypes.windll.user32
screensize = user32.GetSystemMetrics(0), user32.GetSystemMetrics(1)

try:
    monwidth = screensize[0]
    monheight = screensize[0]
except:
    monwidth = 1920
    monheight = 1080

    
# construct the argument parse and parse the arguments
ap = argparse.ArgumentParser()
ap.add_argument("-xcoor", "--X_Coordinate", type = int, help = "X coordinate on the screen")
ap.add_argument("-ycoor", "--Y_Coordinate", type = int, help = "Y coordinate on the screen")
args = vars(ap.parse_args())

x = args["X_Coordinate"]
y = args["Y_Coordinate"]

screen_width = 640
screen_height = 480


x = x/screen_width
y = y/screen_height


x = x * monwidth
y = y * monheight

x = round(x)
y = round(y)

print("x pos: " + str(x) )
print("y pos: " + str(y) )

mouse.move(x, y, absolute=True, duration=0)