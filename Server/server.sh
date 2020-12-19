#make sure both processes get killed with control c
trap 'kill %1; kill %2' SIGINT
#start the chat bot brain and node server
python ChatBrain.py & node chatbot.js