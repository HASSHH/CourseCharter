# CourseCharter
This is a simple app that allows course plotting on user-defined maps.

The idea for this app came while reading The Wheel of Time and I wanted to know the distances they were traveling in those books. Sometimes those distances were stated, but more often they were not. I have to mention here that there are resources like [wheeloftimelines.com](https://wheeloftimelines.com/map), which offer a very comprehensive map with routes for most major stories from the series. However, they do not show the length of those routes. It might seem like a silly thing to get fixated on, and maybe it is, but I always like to compare distances and world sizes from fiction with distances that I am more familiar with from real life, to get a better sense of the scale of those worlds (the Westlands is immense!). I wanted something like what Google Maps offers: plotting a path and getting its distance, but for any map (supplied from an image) and with custom units of measurement. (did you know that a Westlands league is 4 miles, but those are not equivalent to 4 real miles, but more like 4.5 miles and approx. 7.3 km.? ...)

So that is why I started working on this project, which also explains its original name, but in time it became a more general route planner/course charter app where you can define any map you want (given a high-quality image of said map
and a "known" distance between two points on that map).

## Jump to section
* [How to define a map](#new_map)
* [Node naming and filtering out unnamed nodes](#naming)
* [Autosave](#autosave)
* [Customize path markers](#customize)
* [Save location](#save)
* [System requirements](#sys_req)

### How to define a map<a name="new_map"></a>
To create a new map definition you will need an image of that map (the higher the resolution the better) and a known distance between two locations on that map.

For this example let's use the map of [Elendel Basin](https://www.brandonsanderson.com/wp-content/uploads/2023/01/TLM_basin_map-scaled.jpg) from Wax&Wayne(Mistborn 2) series by Brandon Sanderson. The nice thing about this example is that the "known distance on the map" is provided in the image in the form of a bar scale. It's not always that easy, sometimes the known distance is between two cities or other points of interest, other times the distances provided are vague and you have to do with what you have. Now we need to find out the distance in pixels of that "known distance" on our image of the map. For this you can use the measure tool of an image editor like Gimp, or better yet you can use this app to do the same thing, and much easier and faster.

So let's do that. First, we need to create a map (which we will delete later) using our map image. The important thing here is that we set `1 unit/1 pixel` so that the distance we get will be in pixels. Then we plot a simple line path with the ends at the start and end of the bar scale. (For a guide on how to add path nodes and other stuff check out the "Guide" page inside the app).

<img src="https://github.com/HASSHH/CourseCharter/assets/1902707/3c71a23e-20ba-41e4-9fd7-f0dbebb451f2" height="250"/>
<img src="https://github.com/HASSHH/CourseCharter/assets/1902707/3799f14b-8e29-4b68-9451-6885d22487fc" height="250"/>

Now we know that for our map we will have `100 miles/255 pixels` which are the values for _`Sample length units`_ and _`Sample length pixels`_ respectively. Let's create our actual map.

<img src="https://github.com/HASSHH/CourseCharter/assets/1902707/ae5027bf-24f1-470b-a103-7fdeb50594ad" height="250"/>

All we have to do now is delete the first map we created, and then we can start charting our course!

<img src="https://github.com/HASSHH/CourseCharter/assets/1902707/26f6f1bc-f666-472c-b0c8-e8fd4f43deb4" height="80"/>


### Node naming and filtering out unnamed nodes<a name="naming"></a>
Node names can be changed by double-clicking their name in the node list to enable editing, then pressing Enter to save the change or Esc to cancel.

<img src="https://github.com/HASSHH/CourseCharter/assets/1902707/6911c791-a1ac-4f1a-8a2c-77640f721952" height="350"/>

To clear up the clutter that can arise from adding multiple intermediary nodes between _important_ nodes, check the `Show only named nodes` box above the node list to filter out nodes that have not been given a specific name by the user. The distances will also be updated to show the correct values for the total distance between visible nodes in the list.

<img src="https://github.com/HASSHH/CourseCharter/assets/1902707/e7fc57e3-f819-474d-8b63-7f8d6e4c3328" height="350"/>

### Autosave<a name="autosave"></a>
Every time the path is modified in any way the app will automatically save it to the disk so if you closed the app by accident or forgot to save the path before exiting the program your path data won't be lost, and you can continue where you left off. This feature can be disabled in the settings menu.

<img src="https://github.com/HASSHH/CourseCharter/assets/1902707/d1342d73-7673-49ea-9deb-e3d050baa6d1" height="50"/>

### Customize path markers<a name="customize"></a>
The color and size of the map pins and path line can be changed in the settings menu. The pattern of the path line can also be changed.

<img src="https://github.com/HASSHH/CourseCharter/assets/1902707/04243d7b-1a5d-44ff-b322-55d11d13a9d0" height="250"/>

### Save location<a name="save"></a>
User-defined maps, saved paths and user settings file are saved in __`%USERPROFILE%/Documents/CourseCharter`__

### System requirements<a name="sys_req"></a>
* Windows x64 OS
* [.NET8 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-8.0.204-windows-x64-installer)
