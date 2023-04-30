import mouse
import argparse
import inspect
import ctypes

#used to find screen size, ensure resolution scaling is diabled
user32 = ctypes.windll.user32
screensize = user32.GetSystemMetrics(0), user32.GetSystemMetrics(1)

try: #tries to set it to the set values
    monwidth = screensize[0]
    monheight = screensize[1]
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

    import configparser


    # creating the object of configparser
    config_data = configparser.ConfigParser()


    # reading data
    config_data.read("config.ini")


    # app configuration data
    mouse_movement = config_data["mouse_movement"]


    for mouse_movement_data in mouse_movement:
        if mouse_movement_data == "x_start_pos":
            x_pos_start = int(mouse_movement.get(mouse_movement_data))
        elif mouse_movement_data == "x_end_pos":
            x_pos_end = int(mouse_movement.get(mouse_movement_data))
        elif mouse_movement_data == "y_start_pos": 
            y_pos_start = int(mouse_movement.get(mouse_movement_data))
        elif mouse_movement_data == "y_end_pos":
            y_pos_end = int(mouse_movement.get(mouse_movement_data))

    cam_width = x_pos_end - x_pos_start
    
    cam_height = y_pos_end - y_pos_start
    

    x = x - x_pos_start
    y = y - y_pos_start

    

except:

    cam_width = 640
    cam_height = 480

#end tab

x = x/cam_width
y = y/cam_height



x = x * monwidth
y = y * monheight

x = round(x)
y = round(y)

print("x pos: " + str(x) )
print("y pos: " + str(y) )

mouse.move(x, y, absolute=True, duration=0)