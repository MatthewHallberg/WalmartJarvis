#!/usr/bin/python3
import os
import aiml
import sys
import socket
import atexit  

BRAIN_FILE="brain.dump"

k = aiml.Kernel()

def LoadBrain():
	if os.path.exists(BRAIN_FILE):
	    print("Loading from brain file: " + BRAIN_FILE)
	    k.loadBrain(BRAIN_FILE)
	else:
	    print("Parsing aiml files")
	    k.bootstrap(learnFiles="std-startup.aiml", commands="load aiml b")
	    print("Saving brain file: " + BRAIN_FILE)
	    k.saveBrain(BRAIN_FILE)

def MakeRequest(message):
	print(message);
	return k.respond(message)

# next create a socket object and listen
HOST = '127.0.0.1'	# Standard loopback interface address (localhost)
PORT = 12452	# Port to listen on (non-privileged ports are > 1023)
s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
s.bind((HOST, PORT))
s.listen(1)
print('listening...')
conn, addr = s.accept()

#load AIML files after connection
LoadBrain()

def exit_handler():
	conn.close()
	print ('closing connection...')

#register exit handler
atexit.register(exit_handler)

while 1:
	data = conn.recv(1024).decode()
	if data:
		conn.send(MakeRequest(data).encode())
conn.close()
print("closing connection")