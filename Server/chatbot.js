var http = require('http');
const {Wit, log} = require('node-wit');
var youtube = require('./youTube.js');
const { StringDecoder } = require('string_decoder');
const decoder = new StringDecoder('utf8');

//create socket to connect with python process
var socket = require('net').Socket();
socket.connect(12452);

var port = 3000;

const firstEntityValue = (entities, entity) => {
  const val = entities && entities[entity] &&
    Array.isArray(entities[entity]) &&
    entities[entity].length > 0 &&
    entities[entity][0].value
  ;
  if (!val) {
    return null;
  }
  return typeof val === 'object' ? val.value : val;
};

//create a server object:
http.createServer(function (req, response) {
  //next get message from post request
  var message = '';
  req.on('data', function (data) {
    message += data;
  });

  var botData = {
    youtube: '',
    location: '',
    speech: ''
  }; 

  req.on('end', async function () {

      message = unescape(message);

	   if (message !== null && message.length > 0) {

        //Send message to chatbot brain
        socket.write(message);

        //wait for response
        await socket.on('data', function(d){
          botData.speech = d.toString();
          console.log("Recieved: " + botData.speech);
          response.end(JSON.stringify(botData));
          socket.removeAllListeners(['data']);
        });
          
	   } else {
	   		response.end("invalid question");
	   }
  });
}).listen(port);

process.on('SIGINT', function() {
  console.log("\nclosing connection...");
  socket.destroy();
  socket.on('close', function () {
    console.log("socket closed");
    process.exit();
  });
});

console.log('Starting Chatbot Server');