import cv2
import mediapipe as mp
import time
import math

import socket
import struct
import os

UDP_IP = "127.0.0.1"
UDP_PORT = 11000

mp_drawing = mp.solutions.drawing_utils
mp_drawing_styles = mp.solutions.drawing_styles
mp_pose = mp.solutions.pose


sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM) # UDP

cap = cv2.VideoCapture(0, cv2.CAP_DSHOW)
#cap = cv2.VideoCapture("pose_segmentation.mp4")
#cap = cv2.VideoCapture("test.gif")
#cap = cv2.VideoCapture("pose_squats.mp4")

with mp_pose.Pose(
	min_detection_confidence=0.5,
	min_tracking_confidence=0.5,
	enable_segmentation=False,
	smooth_segmentation=False,
	smooth_landmarks=True,
	
	model_complexity=1) as pose:
	
	last_fps = 0
	
	font				   = cv2.FONT_HERSHEY_SIMPLEX
	bottomLeftCornerOfText = (20,40)
	fontScale			   = 1
	fontColor			   = (255,255,255)
	thickness			   = 2
	lineType			   = 2
	cool = False
	
	while cap.isOpened():
		
		success, image = cap.read()
		if not success:
		  cap.set(cv2.CAP_PROP_POS_FRAMES, 0)
		  print("Replay")
		  continue

		c_ticks = time.time()


		pose_image = cv2.cvtColor(image, cv2.COLOR_BGR2RGB)
		results = pose.process(pose_image)
		
		last_fps = 1.0 / (time.time() - c_ticks) 
		

		idx = 0
		packet_count = 0
		
		struct_datatype = "bddd"
		packed_entries = []

	
		if results.pose_world_landmarks != None:
		
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
			udp_data_array = bytearray(struct.pack(struct_datatype, last_fps, *packed_entries))
			
			sock.sendto(udp_data_array , (UDP_IP, UDP_PORT))

		last_fps = str(math.floor(last_fps) ) + " - " + str(packet_count)
		
		mp_drawing.draw_landmarks(
			image,
			results.pose_landmarks,
			mp_pose.POSE_CONNECTIONS,
			landmark_drawing_spec=mp_drawing_styles.get_default_pose_landmarks_style())
			

		cv2.putText(image,str(last_fps), 
			bottomLeftCornerOfText, 
			font, 
			fontScale,
			fontColor,
			thickness,
			lineType)
		
		
		cv2.imshow('MediaPipe Pose & Benchmark', image)
		
		key = cv2.waitKey(1)
		
		if key == ord('q'):
		  break
		elif key == ord('p'):
		  cv2.waitKey(-1)
		  
cap.release()
