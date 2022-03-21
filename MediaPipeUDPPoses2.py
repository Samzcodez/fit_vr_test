import cv2
import mediapipe as mp
import time
import math

import socket
import struct
import os
import sys

UDP_IP = "127.0.0.1"
UDP_PORT = 11000

mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_pose = mp.solutions.pose
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) # UDP



pose0 = mp_pose.Pose(min_detection_confidence=0.5, min_tracking_confidence=0.5)
#cap0 = cv2.VideoCapture(0, cv2.CAP_DSHOW)


#cap0 = cv2.VideoCapture(1, cv2.CAP_DSHOW)
#cap0 = cv2.VideoCapture("test.gif")
cap0 = cv2.VideoCapture("pose_squats.mp4")

frame_shape 			= [720, 1280]
last_fps 				= 0.0
font				   	= cv2.FONT_HERSHEY_SIMPLEX
bottomLeftCornerOfText 	= (20,40)
fontScale			   	= 1
fontColor			   	= (255,255,255)
thickness			   	= 2
lineType			   	= 2

def send_landmarks(pose_world_landmarks, fps):
	idx = 0
	packet_count = 0
	
	struct_datatype = "bddd"
	packed_entries = []
	
	if pose_world_landmarks != None:
		for landmark in results.pose_world_landmarks.landmark:
			
			if landmark.visibility > 0.5:
				packed_entries.append(idx)
				packed_entries.append(landmark.x)
				packed_entries.append(landmark.y)
				packed_entries.append(landmark.z)
				packet_count = packet_count + 1

			idx = idx + 1
	
	if packet_count > 0:
		struct_datatype = "<" + "d" + struct_datatype*packet_count
		udp_data_array = bytearray(struct.pack(struct_datatype, fps, *packed_entries))
		
		sock.sendto(udp_data_array , (UDP_IP, UDP_PORT))
		
		
	return packet_count
	
	
cap0.set(3, frame_shape[1])
cap0.set(4, frame_shape[0])

while cap0.isOpened():
	
	success, frame0 = cap0.read()
	if not success:
	  cap0.set(cv2.CAP_PROP_POS_FRAMES, 0)
	  print("Replay")
	  continue

	c_ticks = time.time()
	pose_image = cv2.cvtColor(frame0, cv2.COLOR_BGR2RGB)
	
	pose_image.flags.writeable = False
	results = pose0.process(pose_image)
	pose_image.flags.writeable = True
	
	
	last_fps = 1.0 / (time.time() - c_ticks) 
	packet_count = send_landmarks(results.pose_world_landmarks, last_fps)

	mp_drawing.draw_landmarks(
		frame0,
		results.pose_landmarks,
		mp_pose.POSE_CONNECTIONS,
		landmark_drawing_spec=mp_drawing_styles.get_default_pose_landmarks_style())
		

	cv2.putText(frame0,str(math.floor(last_fps) ) + " - " + str(packet_count), 
		bottomLeftCornerOfText, 
		font, 
		fontScale,
		fontColor,
		thickness,
		lineType)
	
	
	cv2.imshow('MediaPipe Pose & Benchmark', frame0)
	
	key = cv2.waitKey(1)
	
	if key == ord('q'):
	  break
	elif key == ord('p'):
	  cv2.waitKey(-1)
		  
cap0.release()
