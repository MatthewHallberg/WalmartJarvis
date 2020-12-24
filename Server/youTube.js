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

     message = unescape(message);

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

    //grab first
    var info = await ytdl.getInfo(videoId);
    var hardlink = '';
    for (var i = 0; i < info.formats.length; i++){
      var element = info.formats[i];
      if (!element.url.includes("manifest")){
          hardlink = element.url;
          break;
        }
    }

    var videoURL = 'https://www.youtube.com/watch?v=' + videoId;
    console.log(videoURL);
    response.end(hardlink);

  });
}).listen(port);

    
