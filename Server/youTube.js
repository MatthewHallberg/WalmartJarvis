var {google} = require('googleapis'),
    youtubeV3 = google.youtube( { version: 'v3', auth: 'AIzaSyD8mXzSopdZzBdTtBQplNlaHL4WBDiH-9A' } );

module.exports = {
    GetYouTubeLink : async function (query){

    // Create a instance of promise and return it.
    return new Promise(function(resolve,reject){ 
      var request = youtubeV3.search.list({
          part: 'snippet',
          type: 'video',
          q: query,
          maxResults: '1',
      }, (err,response) => {
          if (err) {
             reject(err);
          } else {
            var videoId = response.data.items[0].id.videoId;
            var videoURL = 'https://www.youtube.com/watch?v=' + videoId;
            console.log(videoURL);
            resolve(videoURL);
          }
      });
    });
    }
};