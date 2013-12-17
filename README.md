# Anita - The (wikifeet.com) Picture Grabber #

Yes, _Anita_ is currently a tool to easily download pictures from [wikifeet.com](http://www.wikifeet.com/).
But the long term goal is, that _Anita_ should be a multipurpose picture grabber. (To know more about the planned features, you should continue reading. :wink:)

## Motivation ##

Why such a tool? Simply put: If you like to collect pics (like others collect stamps), you want hardly do it manually picture by picture. Especially, if you want to actualize your collection later.

I searched for browser extensions, which could do the job. They may be good for general batch downloads of pictures. But [wikifeet.com](http://www.wikifeet.com/) is a bit special in that form, that it only loads more pics on demand through [Ajax][1].

But here is _Anita_ and comes for rescue. :smile:

## Download the executable ##

You can download the latest executable **Anita_R1.1.zip** there: **<https://drive.google.com/folderview?id=0B0p6jU6QiR9-MVpkSXhZRFh6TVE&usp=sharing>**

### Release Notes ###

#### Anita R1.2 (Date: 17.12.2013) ####

-Bugfix: Issue #10

#### Anita R1.1 (Date: 04.05.2013) ####

- Bugfix: Issue #9
- Internal code restructuring: Issue #6

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

### What will be realized soon? ###

- **Make it possible to download more than one celebrity at once!**  
    Theoretically, from a technical point of view, this is already possible. But the GUI is not yet ready to handle this properly. (Of course, that part was poorly designed at first, because it started as an experiment.)  
    But this will be an high priority feature!
- **"ClickOnce" Synchronization**  
    Imagine, that you have already downloaded hundreds of celebrities. A month later, maybe, you wonder if there are new pictures of your favorite celebrities available. It would be not funny to manually search for them and hit "Fetch Data" for each one.  
    So, it should be possible, that you simply click one time on the "Synchronize" button and let the software handle this for you.

### What would be nice to have, but not a critical feature? ###

- **Show small pictures of the celebrities faces!**  
    When you stumble upon a new celebrity name, you have no clue, how she looks like. A small pic within the search results would be really helpful. (I would grab the pic from imdb.com or so.)
- **Search not only for the real name, but also for the character name from any movie!**  
    Think of a situation, where you watch a movie and in that situation, you only know the current character/role name of the actress. You could just search for that name and would possibly find her real name. (For that, I would also use imdb.com or so.)
- **Provide a more detailed progress information**  
    Some info like "??? MB of ??? MB" downloaded. (But that's not so necessary. I would focus more on the other tasks.)

### What are the long term goals? ###

- **Make the application highly modular**  
    Yeah! This would be an essential step to make the application a multipurpose (image) downloader.  
    To make this possible, I want invent a general purpose framework, which could be also used for other applications. This will result in a separate project.  
    That project may grow within the progress of _Anita_.

## For Developers ##

#### Update 27.04.2013: ####

The internal structure is much more better now! It fulfills my personal sense for a well code structure. The worst piece of code is currently the "WikifeetGrabber.cs". (That's the place, where the **spike** resides).
But I will get rid of it soon. Step by step. Not to much at once. :wink:

#### Comment for the initial state of the code ####

This application is more a **spike**. The code is not so well as it could be. (Think of clean code, for example.)
It was my first time, where I used the HttpRequest/HttpResponse stuff.

[1]: http://en.wikipedia.org/wiki/Ajax_(programming)