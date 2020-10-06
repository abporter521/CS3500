This is the ReadME text file required for PS6.  Within this text file there will be commentary 
of design decisions, implementations, problems, external code resources, and added features.
Andrew Porter 5 October 2020

ADDITIONAL FEATURES

5 Oct-- I added a text box to the bottom of the spreadsheet that indicates to the user when the files
have been changed, and if those changes have been saved or not.  This requires no additional action from 
the user outside of the normal spreadsheet operations that are expected. Message will change when spreadsheet
is saved or changed.

5 Oct -- I added shortcut keys.  Ctrl + S will save the document.  Ctrl + O will open a new spreadsheet

PROBLEMS

5 Oct -- I am having issues accessing the scroll bar.  I would like to attach this to the mouse. 
Also having the issue the the horizontal scroll bar is hidden from the user until the window is 
resized.

DESIGN DECISIONS

IMPLEMENTATIONS

NEXT GOALS

5 Oct -- I would really like to set up a controller object that can help link the spreadsheet logic
to the UI.  Also need to implement the opening of more than one spreadsheet.  I would like to attach
the scrollbar to the mousewheel and move between cells with the arrow keys.