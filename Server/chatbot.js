var http = require('http');
const {Wit, log} = require('node-wit');
var youtube = require('./youTube.js');
const { StringDecoder } = require('string_decoder');
const decoder = new StringDecoder('utf8');

//create socket to connect with python process
var socket = require('net').Socket();
socket.connect(12452);

var port = 3000;

//create a server object:
http.createServer(function (req, response) {
  //next get message from post request
  var message = '';
  req.on('data', function (data) {
    message += data;
  });

  req.on('end', async function () {

      message = unescape(message);

	   if (message !== null && message.length > 0) {

          //create object to return
          var botData = {};

          //first run throught wit.ai
          const client = new Wit({accessToken: 'L7BLOIF6QU3XCS3AVRXLDNTIK4Q5KCME'});
          await client.message(message, {})
          .then((data) => {

            botData.entities = data.entities;
            botData.speech = '';

          }).catch(console.error);

          var gotWitResult = Object.keys(botData.entities).length > 0;

          if (!gotWitResult){
            //Send message to chatbot brain
            socket.write(message);
            await socket.on('data', function(d){
              botData.speech = d.toString();
              console.log("Recieved: " + botData.speech);
              socket.removeAllListeners(['data']);
            });
          }

          response.end(JSON.stringify(botData));

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