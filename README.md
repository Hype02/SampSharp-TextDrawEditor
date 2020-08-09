# SampSharpTextDrawEditor

INSTALLATION:
1. Put two .cs files in your project's directory
2. Include namespace: TextDrawEditorModel in your file where you keep "LoadControllers" function
3. Add Controller by copying-pasting this line of code:
controllers.Add(new StreamerController());

Finally it should look like this:

  public partial class GameMode : BaseMode
    {
        protected override void LoadControllers(ControllerCollection controllers)
        {
            controllers.Add(new TextDrawController());
            base.LoadControllers(controllers);
        }
     }
     
     That's all, if you meet any issue please message pull an Issue

SampSharp TextDrawEditor made for writing SA:MP gamemodes in C# faster (https://github.com/Ikkentim/SampSharp)

This TextDraw editor was made in rush for in rush TextDraw making with C# framework for SA:MP gamemodes (https://github.com/ikkentim/SampSharp).
TextDrawEditor abilities:
- Auto-save on everything you do so don't worry about anything, just keep making your textdraw.
- Copying actual editing textdraw
- Save TextDraw / Load it
- Switch between editing TextDraw
- Hide textdraw from screen
- Lots of commands to make your editing faster and easier
- SampSharp controller support

TO DO:
- Deleting textdraw from in game menu (for now delete files  by hand - [your_txd_name].txd and  [your_txd_name].json)
- TextDraw rename from in game menu (for now do it manually in your "textdraws" folder)

https://i.imgur.com/SmV54xF.png
