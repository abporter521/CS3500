This is the ReadME text file required for PS6.  Within this text file there will be commentary 
of design decisions, implementations, problems, external code resources, and added features.
Also during this week, I went through eye surgery and lost a little time to work on the code.
Andrew Porter 5 October 2020

ADDITIONAL FEATURES

5 Oct-- I added a text box to the bottom of the spreadsheet that indicates to the user when the files
have been changed, and if those changes have been saved or not.  This requires no additional action from 
the user outside of the normal spreadsheet operations that are expected. Message will change when spreadsheet
is saved or changed or unsaved.

5 Oct -- I added shortcut keys.  Ctrl + S will save the document.  Ctrl + O will open file explorer to open a saved
spreadsheet

8 Oct -- I added the functionality of using the arrow keys to move around and select cells
Cells that have an error value will display the reason for the error when selected.

8 Oct -- Besides inputting the formula contents into the cell, the enter button and key will also move automatically
to the next cell.  This allows the user to quickly move through the spreadsheet after plugging in values.

9 Oct -- I added two features.  One is a go to cell block.  This will allow the user to input cell data
directly into a designated cell without having to select it.  This automatically blocks invalid cell locations
from being entered into the box. If this box is empty or does not match a cell location, it will just input
the formula contents directly into the designated cell block.

9 Oct -- I added an autosave feature that automatically saves the file when a new input is inserted.  This 
feature can be toggled on and off with a checkbox at the top of the form.  This autosave is engaged as soon
as the user saves the file for the first time after opening. The program does not know if the user wants to save 
the file when they open it, so it waits until after the first save to start saving automatically. This constant
saving means that the form will automatically close without bugging the user for an overwrite. The status of the
spreadsheet can be seen at the bottom of the screen where the saved indicator displays its message. If the user
has already saved the file and autosave is off, the safety message will display and clicking yes will save the
file without opening the dialog box again.

PROBLEMS

5 Oct -- I am having issues accessing the scroll bar.  I would like to attach this to the mouse. 
Also having the issue the the horizontal scroll bar is hidden from the user until the window is 
resized.

8 Oct -- No idea how to work the scroll bars in this.  Also having problems attaching files to my
pull down help tab

9 Oct -- Gave spreadsheet to family for focus group testing.  Feedback came back positive. Desired changes
to scroll bar access and SUM formula from excel.  The latter feature is beyond my capabilities for the time
allotted for this assignment.

DESIGN DECISIONS

6 Oct -- I put a lot of helper methods in my controller class.  The controller class does the variable
and coordinate conversions and holds all the logic for building the backend spreadsheet.  I am still fuzzy
on the MVC model type and I am trying my best to separate concerns. 

8 Oct -- I decided that when a cell's value had a formula error, that I would just display the word error 
in the cell.  This is a straight foward message that can be further explained when the user clicks on the cell
This is better than having other errors that clog up the cell and are unreadable.

9 Oct -- Based on the feedback from my family, they decided it was more favorable for the Go To Cell box to 
input the contents of the formula bar into the designated cell without having to manually select it. 
The other option that this feature could have had was to simply select and highlight the cell.  This decision means it is important for the user to know
that by hitting enter, they have the possibility of changing the cell contents. 
is important for the user to understand that inputting a cell name.

9 Oct -- I also decided that instead of displaying the confirmation of closure box everytime the user tries to 
close out of the program, it was much easier and fluid if the program can check that the spreadsheet was saved.
If so, there is no need to ask the user.  It is only in the case that it is not saved should the user have to deal
with the pop-up messages.

IMPLEMENTATIONS

6 Oct -- I implemented the form count class from the demo.  This keeps track of the multiple windows open and 
It is a separate class file within the project.  I have also implemented a controller class that acts as a 
separator of the spreadsheet logic and the view. There is a box to the left of the formula bar that indicates
which cell the selector is currently on.

6 Oct -- The assignment was implemented with a helper class called controller.  This acts as a liaison between
spreadsheet logic and the user interface.  I tried to separate concerns with this using MVC.  My limited understanding
got me to this point.  As far as I can tell, the code inside the form class deals with the user's view or acting
with the controller object.

8 Oct -- Made the option of filtering by sprd files or by all files by modifying the filter property of the 
OpenFile Dialog box.  Made the same change to the save file dialog box. Program will now check if the file
is saved before closing.  This works on multiple cells

9 Oct -- I decided to use a background worker for the implementation of the autosave.  This was ideal because
in the case of a data dense spreadsheet, this could take time.  The user should still be able to select and 
move around on the spreadsheet even if this is occuring.  

9 Oct -- In trying to figure out how to use the arrow keys to select cells, I conferred with Piazza and online.
I found that in order for my program to work, I had to overwrite the ProcessCmdKey method. Since I have never seen
this before, I borrowed the template from the website and made the small edits so that it changed the cell selected
rather than print a message.  The website URL is located in the contract of the method giving credit.
http://net-informations.com/q/faq/arrowkeys.html 

NEXT GOALS

5 Oct -- I would really like to set up a controller object that can help link the spreadsheet logic
to the UI.  Also need to implement the opening of more than one spreadsheet.  I would like to attach
the scrollbar to the mousewheel and move between cells with the arrow keys.

6 Oct -- Get the multiple windows to open up saved files and close when needed.  Attach the scroll bar to the mousewheel

8 Oct -- I would like to get an autosave feature that implements threads

9 Oct -- turn in and rest my eyes