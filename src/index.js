"use strict"

import { app, BrowserWindow, ipcMain } from 'electron';
const path = require('path');
const https = require('https');

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
  });

  // and load the index.html of the app.
  mainWindow.loadURL(`file://${__dirname}/index.html`);

  // Open the DevTools.
  mainWindow.webContents.openDevTools();

  // Emitted when the window is closed.
  mainWindow.on('closed', () => {
    // Dereference the window object, usually you would store windows
    // in an array if your app supports multi windows, this is the time
    // when you should delete the corresponding element.
    mainWindow = null;
  });
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

ipcMain.on("search:request", function(event, arg) {
  var responseObject = {
    data: ''
  };
  var querystring = require('querystring');
  var resData = '';
  console.log(arg);

  if (pendingRequest) {
    pendingRequest.abort();
  }

  const postData = {
    gender: 0,
    req: 'suggest',
    value: arg
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
