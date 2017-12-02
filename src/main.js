"use strict"

import { ipcRenderer } from 'electron';
window.$ = window.jQuery = require('jquery');
const _ = require('underscore');

var throttled_searchTextChanged = _.throttle(searchTextChanged, 1000, {leading: false});

var $searchBox = $("#search");
$searchBox.on('input', throttled_searchTextChanged);

function searchTextChanged(e) {
  ipcRenderer.send("search:request", e.target.value);
}

ipcRenderer.on("search:response:end", function(event, arg) {
  if (arg.error) {
    console.error(arg.error);
    return;
  }

  const nameRegex = /value='(.*?)';parent.*?encodeURI\('(.*?)'\)/g;
  var names = [];
  var match;
  console.log('FINISHED!');
  // console.log(resData);

  while (match = nameRegex.exec(arg.data)) {
    names.push({
      name: match[1],
      uriName: encodeURI(match[2])
    });
  }
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

function downloadImages(event) {
  event.preventDefault();
  var $downloadInfo = $("#selected_element p");
  var $button = $(this);
  var nameObj = event.data;
  var resData = '';

  $button.unbind("click");
  $button.click(nameObj, cancelDownload);
  // console.log($button);
  $searchBox.prop('disabled', true);
  $button.text("Cancel");
  // console.log($(this));

  $downloadInfo.text("Requesting needed information ...");
  ipcRenderer.send("downloadImages:request", nameObj);
}

ipcRenderer.on("downloadImages:response:imageCount", function(event, imageCount) {
  var $downloadInfo = $("#selected_element p");
  $downloadInfo.text(`There are ${imageCount} images available!`);
});

ipcRenderer.on("downloadImages:response:progress", function(event, progressInfo) {
  var $downloadInfo = $("#selected_element p");
  $downloadInfo.html(`<p>#${progressInfo.imageNumber} of ${progressInfo.imageCount}</p><p>${progressInfo.imageUri} (${progressInfo.percentage}%)</p>`);
});

ipcRenderer.on("downloadImages:response:end", function(event, reason) {
  if (reason.canceled === true) {
    console.log("Download canceled!");
    return;
  }

  var progressInfo = reason.progressInfo;
  var $downloadInfo = $("#selected_element p");
  $downloadInfo.html(`<p>#${progressInfo.imageNumber} of ${progressInfo.imageCount}</p><p>${progressInfo.imageUri} (${progressInfo.percentage}%)</p>`);
  console.log(`Finished download! (${progressInfo.percentage})`);
});

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
  ipcRenderer.send("downloadImages:request:cancel");
}