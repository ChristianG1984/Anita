# Anita - The (wikifeet.com) Picture Grabber #

Yes, _Anita_ is currently a tool to easily download pictures from [wikifeet.com]().
But the long term goal is, that _Anita_ should be a multipurpose picture grabber. (To know more about the planned features, you should continue reading. :wink:)

## Motivation ##

Why such a tool? Simply put: If you like to collect pics (like others collect stamps), you want hardly do it manually picture by picture. Especially, if you want to actualize your collection later.

I searched for browser extensions, which could do the job. They may be good for general batch downloads of pictures. But [wikifeet.com]() is a bit special in that form, that it only loads more pics on demand through [Ajax][1].

But here is _Anita_ and comes for rescue. :smile:

## Download the executable ##

You can download the latest executable there: **<https://drive.google.com/folderview?id=0B0p6jU6QiR9-MVpkSXhZRFh6TVE&usp=sharing>**

## Usage Guide ##

After downloading the Zip-Archive, simply extract it and run the executable `Anita.exe`.

You should see the following window:
![Main window after starting the executable](/readme_stuff/shot_02.png)

On the bottom, there is a textbox, where you can enter the base path for storing the pictures. (You can also click on the button with the three dots to select the path through the folder select dialog.)

If you leave the textbox empty, the pictures will be stored in the same folder from where the executable was started.

The textbox above is reserved for possible error messages.

Now to the more interesting part, the right side. :wink:

From top to the bottom, there is the search textbox, the search result list and the Fetch Data button.

When you type something into the search textbox, the search on wikifeet.com is instantly started and the results will show up within the result list.

You can see this in the next screenshot:
![Main window with search results](/readme_stuff/shot_00.png)

After you selected a result, the Fetch Data button will be enabled and you can click on it to automatically download all available pictures of the celebrity in a sub folder which has the same name as the search result.

After you clicked on Fetch Data, you can see the progress of the running download:
![Main window with search results](/readme_stuff/shot_01.png)

You have not to wait, until the download has finished. You can cancel at any time and continue later. Pictures, which are already downloaded, will be skipped.


That's all to say about how the application currently works. :smiley:

----------
## Future Plans ##

**Will come soon! Stay tuned!**

## For Developers ##

This application is more a **spike**. The code is not so well as it could be. (Think of clean code, for example.)
It was my first time, where I used the HttpRequest/HttpResponse stuff.



[1]: http://en.wikipedia.org/wiki/Ajax_(programming)