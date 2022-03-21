import cv2
import math

import socket
import struct
import os

import time
import cv2.aruco as A
import numpy as np

dictionary = cv2.aruco.getPredefinedDictionary(cv2.aruco.DICT_4X4_50)
board = cv2.aruco.CharucoBoard_create(3,3,.025,.0125,dictionary)

#Start capturing images for calibration
cap = cv2.VideoCapture(0, cv2.CAP_DSHOW)

allCorners = []
allIds = []
decimator = 0
while True:

	ret,frame = cap.read()
	gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)
	res = cv2.aruco.detectMarkers(gray,dictionary)

	if len(res[0])>0:
		res2 = cv2.aruco.interpolateCornersCharuco(res[0],res[1],gray,board)
		if res2[1] is not None and res2[2] is not None and len(res2[1])>3 and decimator%3==0:
			allCorners.append(res2[1])
			allIds.append(res2[2])

		cv2.aruco.drawDetectedMarkers(gray,res[0],res[1])

	cv2.imshow('frame',gray)
	
	if cv2.waitKey(5) & 0xFF == 27:
		  break
		  
	decimator+=1


cap.release()
cv2.destroyAllWindows()