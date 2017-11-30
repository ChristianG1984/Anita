"use strict"

import { ipcRenderer } from 'electron';
window.$ = window.jQuery = require('jquery');
const _ = require('underscore');
const request = require('request');
const progress = require('request-progress');

var pendingRequest;
var canceledImageDownload = false;

var throttled_searchTextChanged = _.throttle(searchTextChanged, 1000, {leading: false});

var $searchBox = $("#search");
$searchBox.on('input', throttled_searchTextChanged);

ipcRenderer.on("search:response:end", function(event, arg) {
  if (arg.error) {
    console.error(arg.error);
    return;
  }

  const nameRegex = /value='(.*?)';parent.*?encodeURI\('(.*?)'\)/g;
  var names = [];
  console.log('FINISHED!');
  // console.log(resData);
  arg.data.replace(nameRegex, function nameSelector(match, p1, p2) {
    names.push({
      name: p1,
      uriName: encodeURI(p2)
    });
    return null;
  });
  // console.log(names);

  var $selectedElement = $("#selected_element");
  var $searchResults = $("#search_results");
  var $ul = $("<ul></ul>");

  $searchResults.empty();
  names.forEach(function(nameObj) {
    var $li = $(`<li><a href="#">${nameObj.name}</a></li>`);
    var $a = $li.has("a").first();

    $a.click(function(e) {
      e.preventDefault();
      console.log("href-Click abgefangen!");
      if ($searchBox.is(":disabled")) {
        var $info = $("#info");
        $info.empty();
        $info.append("<p><strong>You have to cancel the pending Download first!</strong></p>");
        return false;
      }

      $selectedElement.empty();
      $selectedElement.append(`<h1>${nameObj.name}</h1>`);
      $selectedElement.append(`<p></p>`);
      var $downloadButton = $(`<button>Download</button>`);
      $selectedElement.append($downloadButton);
      // console.log($downloadButton);
      $downloadButton.click(nameObj, downloadImages);

    });

    $ul.append($li);
  });
  $searchResults.append($ul);
});

function searchTextChanged(e) {
  ipcRenderer.send("search:request", e.target.value);
}

function downloadImages(event) {
  event.preventDefault();
  var $downloadInfo = $("#selected_element p");
  var $button = $(this);
  var nameObj = event.data;
  var resData = '';

  canceledImageDownload = false;
  $button.unbind("click");
  $button.click(nameObj, cancelDownload);
  // console.log($button);
  $searchBox.prop('disabled', true);
  $button.text("Cancel");
  // console.log($(this));

  const options = {
    hostname: 'www.wikifeet.com',
    path: '/' + nameObj.uriName,
    method: 'GET'
  };

  pendingRequest = https.request(options, function(res) {
    console.log(`STATUS: ${res.statusCode}`);
    console.log(`HEADERS: ${JSON.stringify(res.headers)}`);
    res.setEncoding('utf-8');
    res.on('data', function(data) {
      resData += data;
    });

    res.on('end', function() {
      const cfnameRegex = /messanger\.cfname[ =']+(.*?)';/g;
      const pidRegex = /"pid":"(\d+)",/g;
      var imageInfo = {
        cfname: '',
        pids: []
      }
      console.log('FINISHED!');
      // console.log(resData);

      resData.replace(cfnameRegex, function cfnameSelector(match, p1) {
        imageInfo.cfname = p1;
      });

      resData.replace(pidRegex, function pidSelector(match, p1) {
        imageInfo.pids.push(p1);
      });

      $downloadInfo.text(`There are ${imageInfo.pids.length} images available!`);
      console.log(imageInfo);

      downloadFromImageInfo(imageInfo, 1);
    });
  });

  pendingRequest.on('error', function(e) {
    console.error(`problem with request: ${e.message}`);
  });

  $downloadInfo.text("Requesting needed information ...");
  
  pendingRequest.end();
}

function downloadFromImageInfo(imageInfo, nextImageNumber) {
  if (imageInfo.pids.length === 0) {
    return;
  }

  var $downloadInfo = $("#selected_element p");
  var imageUri = imageInfo.cfname + '-Feet-' + imageInfo.pids.shift() + '.jpg';
  var fullImageUri = 'https://pics.wikifeet.com/' + imageUri;
  $downloadInfo.html(`<p>#${0} of ${0}</p><p>${imageUri}</p>`);

  var req = progress(request(fullImageUri), { throttle: 500 })
  .on('progress', function onProgress(state) {
    if (canceledImageDownload === true) {
      req.abort();
    }

    $downloadInfo.html(`<p>#${0} of ${0}</p><p>${imageUri} (${Math.round(state.percent * 100)}%)</p>`);
  })
  .on('error', function onError(err) {
    console.log(err);
  })
  .on('abort', function onAbort() {
    console.log('Download canceled!');
  })
  .on('end', function onEnd() {
    console.log('Download finished!');
    if (canceledImageDownload === true) {
      return;
    }

    downloadFromImageInfo(imageInfo);
  });
}

function cancelDownload(event) {
  event.preventDefault();
  var $button = $(this);
  var $info = $("#info");

  $button.unbind("click");
  $button.click(event.data, downloadImages);
  // console.log($button);
  $searchBox.prop('disabled', false);
  $button.text("Download");
  $info.empty();

  console.log("Cancel clicked!");
  canceledImageDownload = true;
}