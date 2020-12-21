var http = require('http');
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

    var videoURL = await new Promise((resolve, reject) => {
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
                var videoId = response.data.items[0].id.videoId;
                var videoURL = 'https://www.youtube.com/watch?v=' + videoId;
                console.log(videoURL);
                resolve(videoURL);
              }
          });
     });

    response.end(videoURL);

  });
}).listen(port);

    
