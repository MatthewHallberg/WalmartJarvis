var http = require('http');
const ytdl = require('ytdl-core');
var {google} = require('googleapis'),
    youtubeV3 = google.youtube( { version: 'v3', auth: 'AIzaSyD8mXzSopdZzBdTtBQplNlaHL4WBDiH-9A' } );

var port = 3001;

http.createServer(function (req, response) {
  //next get message from post request
  var message = '';
  req.on('data', function (data) {
    message += data;
  });

  req.on('end', async function () {

    console.log("Searching for: " + message);

    var videoId = await new Promise((resolve, reject) => {
       youtubeV3.search.list({
              part: 'snippet',
              type: 'video',
              q: message,
              maxResults: '1',
          }, (err,response) => {
              if (err) {
                 console.log("ERROR");
                 reject(err);
              } else {
                resolve(response.data.items[0].id.videoId);
              }
          });
     });

    var info = await ytdl.getInfo(videoId);
    var hardlink = info.formats[0].url;
    var videoURL = 'https://www.youtube.com/watch?v=' + videoId;
    console.log(videoURL);
    response.end(hardlink);

  });
}).listen(port);

    
