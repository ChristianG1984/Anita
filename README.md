# Anita - The (wikifeet.com) Picture Grabber #

Yes, _Anita_ is currently a tool to easily download pictures from [wikifeet.com](http://www.wikifeet.com/).
But the long term goal is, that _Anita_ should be a multipurpose picture grabber. (To know more about the planned features, you should continue reading. :wink:)

## Motivation ##

Why such a tool? Simply put: If you like to collect pics (like others collect stamps), you want hardly do it manually picture by picture. Especially, if you want to actualize your collection later.

I searched for browser extensions, which could do the job. They may be good for general batch downloads of pictures. But [wikifeet.com](http://www.wikifeet.com/) is a bit special in that form, that it only loads more pics on demand through [Ajax][1].

But here is _Anita_ and comes for rescue. :smile:

## Download the executable ##

You can find the latest executable here on Github:
<https://github.com/reaper36/Anita/releases>

## Usage Guide ##

- If you have chosen the setup.exe, just execute it and the app will immediately start after the installation process. You will also find a link in your Start menu.

- If you have chosen the Zip-Archive, simply extract it and run the executable `Anita.exe`.

This shows, how the app is used:
![Animation about how the app works](/readme_stuff/preview_00.gif)

When you type something into the search textbox, the search on wikifeet.com is instantly started and the results will show up within the result list.
Pictures, which are already downloaded, will be skipped.

If you cancel a download, the current running download will be finished.

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

### What are the long term goals? ###

- **Make the application highly modular**  
    Yeah! This would be an essential step to make the application a multipurpose (image) downloader.  
    To make this possible, I want invent a general purpose framework, which could be also used for other applications. This will result in a separate project.  
    That project may grow within the progress of _Anita_.

## For Developers ##

#### Update 04.12.2017: ####

I decided to use the electron-framework for future development.

#### Update 27.04.2013: ####

The internal structure is much more better now! It fulfills my personal sense for a well code structure. The worst piece of code is currently the "WikifeetGrabber.cs". (That's the place, where the **spike** resides).
But I will get rid of it soon. Step by step. Not to much at once. :wink:

#### Comment for the initial state of the code ####

This application is more a **spike**. The code is not so well as it could be. (Think of clean code, for example.)
It was my first time, where I used the HttpRequest/HttpResponse stuff.

[1]: http://en.wikipedia.org/wiki/Ajax_(programming)
