This is the ReadME text file required for PS6.  Within this text file there will be commentary 
of design decisions, implementations, problems, external code resources, and added features.
Also during this week, I went through eye surgery and lost a little time to work on the code.
Andrew Porter 5 October 2020

ADDITIONAL FEATURES

5 Oct-- I added a text box to the bottom of the spreadsheet that indicates to the user when the files
have been changed, and if those changes have been saved or not.  This requires no additional action from 
the user outside of the normal spreadsheet operations that are expected. Message will change when spreadsheet
is saved or changed.

5 Oct -- I added shortcut keys.  Ctrl + S will save the document.  Ctrl + O will open file explorer to open a saved
spreadsheet

8 Oct -- I added the functionality of using the arrow keys to move around and select cells
Cells that have an error value will display the reason for the error when selected

PROBLEMS

5 Oct -- I am having issues accessing the scroll bar.  I would like to attach this to the mouse. 
Also having the issue the the horizontal scroll bar is hidden from the user until the window is 
resized.

8 Oct -- No idea how to work the scroll bars in this.  Also having problems attaching files to my
pull down help tab

DESIGN DECISIONS

6 Oct -- I put a lot of helper methods in my controller class.  The controller class does the variable
and coordinate conversions and holds all the logic for building the backend spreadsheet.  I am still fuzzy
on the MVC model type and I am trying my best to separate concerns. 

8 Oct -- I decided that when a cell's value had a formula error, that I would just display the word error 
in the cell.  This is a straight foward message that can be further explained when the user clicks on the cell
This is better than having other errors that clog up the cell and are unreadable

IMPLEMENTATIONS

6 Oct -- I implemented the form count class from the demo.  This keeps track of the multiple windows open and 
It is a separate class file within the project.  I have also implemented a controller class that acts as a 
separator of the spreadsheet logic and the view.

8 Oct -- Made the option of filtering by sprd files or by all files by modifying the filter property of the 
OpenFile Dialog box.  Made the same change to the save file dialog box. Program will now check if the file
is saved before closing.  This works on multiple cells



NEXT GOALS

5 Oct -- I would really like to set up a controller object that can help link the spreadsheet logic
to the UI.  Also need to implement the opening of more than one spreadsheet.  I would like to attach
the scrollbar to the mousewheel and move between cells with the arrow keys.

6 Oct -- Get the multiple windows to open up saved files and close when needed.  Attach the scroll bar to the mousewheel

8 Oct -- I would like to get an autosave feature that implements threads