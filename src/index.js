"use strict"

import { app, BrowserWindow, ipcMain } from 'electron';
import { setTimeout } from 'timers';
const fs = require('fs');
const path = require('path');
const https = require('https');
const request = require('request');
const progress = require('request-progress');

process.env.NODE_ENV = 'production';

// Handle creating/removing shortcuts on Windows when installing/uninstalling.
if (require('electron-squirrel-startup')) { // eslint-disable-line global-require
  app.quit();
}

if (process.env.NODE_ENV !== 'production') {
  require('electron-reload')(__dirname, {
    electron: path.join(__dirname, '..\\', 'node_modules', '.bin', 'electron')
  });
}

// Keep a global reference of the window object, if you don't, the window will
// be closed automatically when the JavaScript object is garbage collected.
let mainWindow;

const createWindow = () => {
  // Create the browser window.
  mainWindow = new BrowserWindow({
    width: 800,
    height: 600,
    show: false
  });

  // and load the index.html of the app.
  mainWindow.loadURL(`file://${__dirname}/index.html`);

  // Open the DevTools.
  if (process.env.NODE_ENV !== 'production') {
    mainWindow.webContents.openDevTools();
  }

  mainWindow.on('close', function(event) {
    canceledImageDownload = true;

    if (pendingDownload) {
      event.preventDefault();
      setTimeout(function() {
        mainWindow.close();
      }, 100);
    }
  });

  // Emitted when the window is closed.
  mainWindow.on('closed', () => {
    // Dereference the window object, usually you would store windows
    // in an array if your app supports multi windows, this is the time
    // when you should delete the corresponding element.
    mainWindow = null;
  });

  mainWindow.on('ready-to-show', () => {
    mainWindow.show();
  })
};

// This method will be called when Electron has finished
// initialization and is ready to create browser windows.
// Some APIs can only be used after this event occurs.
app.on('ready', createWindow);

// Quit when all windows are closed.
app.on('window-all-closed', () => {
  // On OS X it is common for applications and their menu bar
  // to stay active until the user quits explicitly with Cmd + Q
  if (process.platform !== 'darwin') {
    app.quit();
  }
});

app.on('activate', () => {
  // On OS X it's common to re-create a window in the app when the
  // dock icon is clicked and there are no other windows open.
  if (mainWindow === null) {
    createWindow();
  }
});

// In this file you can include the rest of your app's specific main process
// code. You can also put them in separate files and import them here.

var pendingRequest;
var canceledImageDownload = false;
var pendingDownload = false;

ipcMain.on("search:request", function(event, searchText) {
  var responseObject = {
    data: ''
  };
  var querystring = require('querystring');
  var resData = '';
  console.log(searchText);

  if (pendingRequest) {
    pendingRequest.abort();
  }

  const postData = {
    gender: 0,
    req: 'suggest',
    value: searchText
  }

  console.log(postData);

  const postDataString = querystring.stringify(postData);

  console.log(postDataString);

  const options = {
    hostname: 'www.wikifeet.com',
    path: '/perl/ajax.fpl',
    method: 'POST',
    headers: {
      'Content-Type': 'application/x-www-form-urlencoded',
      'Content-Length': Buffer.byteLength(postDataString)
    }
  };

  pendingRequest = https.request(options, function(res) {
    console.log(`STATUS: ${res.statusCode}`);
    console.log(`HEADERS: ${JSON.stringify(res.headers)}`);
    res.setEncoding('utf-8');
    res.on('data', function(data) {
      resData += data;
    });

    res.on('end', function() {
      responseObject.data = resData;
      event.sender.send("search:response:end", responseObject);
    })
  });

  pendingRequest.on('error', function(e) {
    if (e.code === "ECONNRESET") {
      return;
    }
    responseObject.error = {};
    responseObject.error.message = e.message;
    responseObject.error.code = e.code;
    responseObject.error.stack = e.stack;
    console.error(`problem with request: ${e}`);
    event.sender.send("search:response:end", responseObject);
  });

  pendingRequest.write(postDataString);
  pendingRequest.end();
});

ipcMain.on("downloadImages:request", function(event, requestData) {
  var resData = '';
  const options = {
    hostname: 'www.wikifeet.com',
    path: '/' + requestData.nameObj.uriName,
    method: 'GET'
  };

  canceledImageDownload = false;
  pendingDownload = true;
  pendingRequest = https.request(options, function(res) {
    console.log(`STATUS: ${res.statusCode}`);
    console.log(`HEADERS: ${JSON.stringify(res.headers)}`);
    res.setEncoding('utf-8');
    res.on('data', function(data) {
      resData += data;
    });

    res.on('end', function() {
      console.log(`resData.length = ${resData.length}`);
      const cfnameRegex = /messanger\.cfname[ =']+(.*?)';/;
      const pidRegex = /"pid":"(\d+)"/g;
      var match;
      var imageInfo = {
        basePath: requestData.basePath,
        imageCount: 0,
        name: requestData.nameObj.name,
        cfname: '',
        pids: []
      }
      console.log('FINISHED!');
      // console.log(resData);

      if (match = cfnameRegex.exec(resData)) {
        imageInfo.cfname = match[1];
      }

      while (match = pidRegex.exec(resData)) {
        imageInfo.pids.push(match[1]);
      }
      imageInfo.imageCount = imageInfo.pids.length;

      event.sender.send("downloadImages:response:imageCount", imageInfo.imageCount);
      
      // console.log(imageInfo);

      downloadFromImageInfo(imageInfo, event);
    });
  });

  pendingRequest.on('error', function(e) {
    pendingDownload = false;
    console.error(`problem with request: ${e.message}`);
  });
  
  pendingRequest.end();
})

function downloadFromImageInfo(imageInfo, event) {
  if (imageInfo.pids.length === 0) {
    pendingDownload = false;
    return;
  }

  var progressInfo = {
    imageNumber: 0,
    imageCount: imageInfo.imageCount,
    percentage: 0,
    imageUri: ''
  };
  var imageUri = imageInfo.cfname + '-Feet-' + imageInfo.pids.shift() + '.jpg';
  progressInfo.imageUri = imageUri;
  progressInfo.imageNumber = imageInfo.imageCount - imageInfo.pids.length;
  var fullImageUri = 'https://pics.wikifeet.com/' + imageUri;
  var savePath = path.join(imageInfo.basePath, imageInfo.name, imageUri);

  while (fs.existsSync(savePath)) {
    if (imageInfo.pids.length === 0) {
      pendingDownload = false;
      event.sender.send("downloadImages:response:end", {
        progressInfo: progressInfo,
        canceled: false
      });
      return;
    }

    imageUri = imageInfo.cfname + '-Feet-' + imageInfo.pids.shift() + '.jpg';
    progressInfo.imageUri = imageUri;
    progressInfo.imageNumber = imageInfo.imageCount - imageInfo.pids.length;
    fullImageUri = 'https://pics.wikifeet.com/' + imageUri;
    savePath = path.join(imageInfo.basePath, imageInfo.name, imageUri);

    event.sender.send("downloadImages:response:progress", progressInfo);
  }

  try {
    fs.mkdirSync(path.dirname(savePath));
  } catch (err) {
    if (err.code !== 'EEXIST') {
      console.error(err);
    }
  }

  event.sender.send("downloadImages:response:progress", progressInfo);
  
  pendingDownload = true;
  var req = progress(request(fullImageUri), { throttle: 500 })
  .on('progress', function onProgress(state) {
    // if (canceledImageDownload === true) {
    //   req.abort();
    //   return;
    // }

    progressInfo.percentage = Math.round(state.percent * 100);
    console.log(`percentage: ${progressInfo.percentage}`);
    event.sender.send("downloadImages:response:progress", progressInfo);
  })
  .on('error', function onError(err) {
    console.log(err);
    event.sender.send("downloadImages:response:error", {
      code: err.code,
      message: err.message,
      stack: err.stack
    });
  })
  .on('abort', function onAbort() {
    console.log('Download canceled!');
  })
  .on('end', function onEnd() {
    console.log('Download finished!');
    if (canceledImageDownload === true) {
      event.sender.send("downloadImages:response:end", {
        progressInfo: progressInfo,
        canceled: true
      });
      // pendingDownload = false;
      return;
    }

    progressInfo.percentage = 100;
    event.sender.send("downloadImages:response:end", {
      progressInfo: progressInfo,
      canceled: false
    });
    downloadFromImageInfo(imageInfo, event);
  })
  .pipe(fs.createWriteStream(savePath))
  .on('error', function onFileStreamWriteError(error) {
    console.error(error);
    pendingDownload = false;
  })
  .on('finish', function() {
    pendingDownload = false;
  });
}

ipcMain.on("downloadImages:request:cancel", function(event) {
  canceledImageDownload = true;
});
