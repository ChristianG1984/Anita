"use strict"

import { ipcRenderer } from 'electron';
const { dialog } = require('electron').remote
window.$ = window.jQuery = require('jquery');
const _ = require('underscore');
const Store = require('electron-store');
const store = new Store();

console.log(store.path);

var throttled_searchTextChanged = _.throttle(searchTextChanged, 1000, {leading: false});

var $searchBox = $("#search");
var $btnSelectBasePath = $("#btn_select_base_path");
var $chkUseWikifeetX = $("#wikifeetX");
var $basePath = $("#base_path");
var $lblSearch = $("#lblSearch");
$basePath.val(store.get('basePath', ''));

$searchBox.on('input', throttled_searchTextChanged);
$btnSelectBasePath.on('click', btnSelectBasePathClicked);


function btnSelectBasePathClicked(e) {
  e.preventDefault();
  var result = dialog.showOpenDialog({properties: ['createDirectory', 'openDirectory']});
  console.log(result);

  if (result === undefined) {
    return;
  }

  $basePath.val(result[0]);
  store.set('basePath', result[0]);

  if ($basePath.val().length !== 0) {
    var $info = $("#info");
    $info.empty();
  }
}

function searchTextChanged(e) {
  var $info = $("#info");
  $info.empty();
  ipcRenderer.send("search:request", e.target.value, $chkUseWikifeetX.prop('checked'));    
}

ipcRenderer.on("search:response:end", function(event, arg) {
  if (arg.error) {
    console.error(arg.error);
    var $info = $("#info");
    $info.append(`<p><strong>${arg.error.code}: ${arg.error.message}</strong></p>`);
    return;
  }

  const nameRegex = /value='(.*?)';parent.*?encodeURI\('(.*?)'\)/g;
  var names = [];
  var match;
  console.log('FINISHED!');

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

  if ($basePath.val().length === 0) {
    var $info = $("#info");
    $info.empty();
    $info.append('<p><strong>You have to select the "Base-Path" first!</strong></p>');
    return;
  }

  $button.unbind("click");
  $button.click(nameObj, cancelDownload);
  // console.log($button);
  $searchBox.prop('disabled', true);
  $button.text("Cancel");
  // console.log($(this));

  $downloadInfo.text("Requesting needed information ...");
  ipcRenderer.send("downloadImages:request", {
    nameObj: nameObj,
    basePath: $basePath.val()
  }, $chkUseWikifeetX.prop('checked'));
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
  if (progressInfo.imageNumber === progressInfo.imageCount) {
    var $selectedElement = $("#selected_element");
    var $celebName = $selectedElement.find("h1").first();
    $selectedElement.empty();
    $selectedElement.append($celebName);
    $selectedElement.append(`<p><strong>Download finished!</strong></p>`);
    $searchBox.prop('disabled', false);
    console.log(`Finished download! (${progressInfo.percentage})`);
  } else {
    $downloadInfo.html(`<p>#${progressInfo.imageNumber} of ${progressInfo.imageCount}</p><p>${progressInfo.imageUri} (${progressInfo.percentage}%)</p>`);
    console.log(`Finished download! (${progressInfo.percentage})`);
  }
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