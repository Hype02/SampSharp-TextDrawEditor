
using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;
using SampSharp.GameMode.SAMP;
using SampSharp.GameMode.Definitions;
using System.IO;


namespace TextDrawEditorModel
{
    [Controller]
    public class TextDrawEditorController : IEventListener
    {
        public void RegisterEvents(BaseMode gameMode)
        {
            Console.WriteLine($">>> S#TDE v1.0 initialized. Your textdraws will be saved at: {TextDrawEditorModel.TextDrawsPath}");

            gameMode.PlayerConnected += onConnected;
            gameMode.PlayerCommandText += onCommandText;
        }
        void onConnected(object sender, EventArgs args)
        {
            BasePlayer thisPlayer = sender as BasePlayer;
            thisPlayer.SendClientMessage($"{Color.Red}(!) Welcome {thisPlayer.Name}. Only one command to remember is simple {Color.White}/txd{Color.Red}. Use it to start, and have fun!");
        }



        void onCommandText(object sender, CommandTextEventArgs args)
        {
            BasePlayer thisPlayer = sender as BasePlayer;

            string[] cmdArgs = args.Text.Split(" ");

            string cmd = cmdArgs[0].Replace("/", String.Empty);

            if (cmd == "txd")
            {
                ListDialog options = new ListDialog("What will you do next?", "Choose", "Cancel");
                options.AddItem($"1 {Color.Green}Create {Color.White}brand new TextDraw\n");
                options.AddItem($"2 {Color.LightBlue}Open {Color.White}an existing TextDraw\n");
                options.AddItem($"3 {Color.Yellow}Edit {Color.White}TextDraw visible on screen now\n");
                options.AddItem($"4 {Color.Red}Hide textdraw from screen\n");
                options.AddItem($"5 {Color.Red}Delete {Color.White}TextDraw that is saved on disk forever\n");
                options.AddItem($"6 {Color.Gray}See {Color.White}See all avaliable commands\n");
                options.AddItem($"6 {Color.Gray}Copy textdraw you are currently editing\n");
                options.Show(thisPlayer);

                options.Response += onOptionResponse;
                void onOptionResponse(object sender, DialogResponseEventArgs args)
                {
                    // I use switchcase because of the YandereDev drama
                    if (args.DialogButton == DialogButton.Left)
                    {
                        switch (args.ListItem)
                        {
                            // Create TextDraw
                            case 0:
                                {
                                    InputDialog createTXD = new InputDialog("Create brand new TextDraw, that is yours.", "How will you name it? ('ServerLogo' is a good approach!')", false, "Done!", "Go Back");
                                    createTXD.Show(thisPlayer);
                                    createTXD.Response += onCreateTextDrawResponse;

                                    void onCreateTextDrawResponse(object sender, DialogResponseEventArgs args)
                                    {
                                        if (args.DialogButton == DialogButton.Left)
                                        {
                                            string newTextDrawName = args.InputText;
                                            if (TextDrawEditorModel.TextDrawFileExists(newTextDrawName) == true)
                                            {
                                                thisPlayer.SendClientMessage($"Textdraw '{newTextDrawName}' already exists. Delete the file and try again.");
                                            }
                                            else
                                            {
                                                TextDrawEditorModel.textDrawBeingEdited =  TextDrawEditorModel.CreateTextDraw(newTextDrawName);

                                                for (byte i = 0; i < 20; i++)
                                                {
                                                    thisPlayer.SendClientMessage($" ");
                                                }

                                                thisPlayer.SendClientMessage($"{Color.LightGreen}> Created and now editing textdraw: {newTextDrawName}");
                                                thisPlayer.SendClientMessage($" ");
                                                thisPlayer.SendClientMessage($"{Color.LightBlue}> Simple remainder - all cmds for you: ttext, tshadow, twidth, tusebox, tselectable, tproportional, tpreviewzoom, tpreviewrotation");
                                                thisPlayer.SendClientMessage($"{Color.LightBlue}  tpreviewsecondarycolor, tpreviewprimarycolor, tpreviewmodel, toutline, tlettersize, theight");
                                                thisPlayer.SendClientMessage($"{Color.LightBlue}  tforecolor, tboxcolor, tbackcolor, tfont, talign, tpos");
                                                thisPlayer.SendClientMessage($" ");
                                                thisPlayer.SendClientMessage($"{Color.LightBlue}> Each use of those command will auto-save your textdraw in {Color.Yellow}{TextDrawEditorModel.TextDrawsPath}");
                                            }
                                        }
                                        else
                                        {
                                            options.Show(thisPlayer);
                                        }

                                    }
                                    break;
                                }
                                // Load TextDraw
                            case 1:
                                {
                                    ListDialog loadTXD = new ListDialog("Load your amazing TextDraw", "Load", "Go Back");
                                    var textdraws = Directory.GetFiles(TextDrawEditorModel.TextDrawsPath);
                                    foreach (string textdraw in textdraws)
                                    {
                                        // im sorry
                                        string thisFileExtension = textdraw.Replace(TextDrawEditorModel.TextDrawsPath, String.Empty).Split(".")[1];
                                        string thisFileName = textdraw.Replace(TextDrawEditorModel.TextDrawsPath, String.Empty).Split(".")[0];
                                        if (thisFileExtension != "json")
                                            loadTXD.AddItem(textdraw.Replace(TextDrawEditorModel.TextDrawsPath, String.Empty).Split(".")[0] + "\n");
                                    }
                                    loadTXD.Show(thisPlayer);

                                    loadTXD.Response += onLoadTextDrawResponse;
                                    void onLoadTextDrawResponse(object sender, DialogResponseEventArgs args)
                                    {
                                        if (args.DialogButton == DialogButton.Left)
                                        {
                                            if (TextDrawEditorModel.TextDrawFileExists(args.InputText) == false)
                                            {
                                                thisPlayer.GameText("~r~TEXTDRAW~n~DOES~n~NOT~n~EXIST" + args.InputText, 5000, 4);
                                            }
                                            else
                                            {
                                                var txd = TextDrawEditorModel.LoadTextDraw(args.InputText);
                                                txd.Show(thisPlayer);
                                                TextDrawEditorModel.DoPlayerEditTextdraw(thisPlayer, txd);
                                            }

                                        }
                                        else
                                        {
                                            options.Show(thisPlayer);
                                        }


                                    }
                                    break;
                                }
                                // Edit TextDraw
                            case 2:
                                {
                                        ListDialog txdList = new ListDialog("Edit textdraw", "Select", "Go back");
                                        foreach (EditorTextDraw textDraw in EditorTextDraw.All)
                                        {
                                            txdList.AddItem(textDraw.name + "\n");

                                        }
                                        txdList.Show(thisPlayer);
                                    txdList.Response += onListResponse;                           
                                   
                                    void onListResponse(object sender, DialogResponseEventArgs args)
                                    {
                                        if (args.DialogButton == DialogButton.Left)
                                        {
                                            foreach (EditorTextDraw textDraw in EditorTextDraw.All)
                                            {
                                               if(textDraw.name == args.InputText)
                                                {
                                                    TextDrawEditorModel.DoPlayerEditTextdraw(thisPlayer, textDraw);
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            options.Show(thisPlayer);
                                        }
                                    }
                                }
                                break;
                                // Hide TextDraw from screen
                            case 3:
                                {
                                    ListDialog txdList = new ListDialog("Hide textdraw", "Select", "Go back");
                                    foreach (EditorTextDraw textDraw in EditorTextDraw.All)
                                    {
                                        txdList.AddItem(textDraw.name + "\n");

                                    }
                                    txdList.Show(thisPlayer);
                                    txdList.Response += onListResponse;

                                    void onListResponse(object sender, DialogResponseEventArgs args)
                                    {
                                        if (args.DialogButton == DialogButton.Left)
                                        {
                                            foreach (EditorTextDraw textDraw in EditorTextDraw.All)
                                            {
                                                if (textDraw.name == args.InputText)
                                                {
                                                    textDraw.Dispose();
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            options.Show(thisPlayer);
                                        }
                                    }
                                    break;
                                }
                                // Delete it from disk
                            case 4:
                                {
                                           ListDialog txdList = new ListDialog("Delete textdraw forever", "DELETE", "Go back");
                                        foreach (EditorTextDraw textDraw in EditorTextDraw.All)
                                        {
                                            txdList.AddItem(textDraw.name + "\n");

                                        }
                                        txdList.Show(thisPlayer);
                                    txdList.Response += onListResponse;                           
                                   
                                    void onListResponse(object sender, DialogResponseEventArgs args)
                                    {
                                        if (args.DialogButton == DialogButton.Left)
                                        {
                                            foreach (EditorTextDraw textDraw in EditorTextDraw.All)
                                            {
                                               if(textDraw.name == args.InputText)
                                                {

                                                    thisPlayer.GameText($"{Color.Red}DELETED " + textDraw.name, 5000, 4);
                                                    Directory.Delete(TextDrawEditorModel.TextDrawsPath + textDraw.name + ".json");
                                                    Directory.Delete(TextDrawEditorModel.TextDrawsPath + textDraw.name + ".txt");
                                                    textDraw.Dispose();
                                                    TextDrawEditorModel.textDrawBeingEdited = null;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            options.Show(thisPlayer);
                                        }
                                    }
                                }break;
                            case 5:
                                {
                                    var cmds = new ListDialog($"{Color.Yellow}All available cmds for you to manage your TextDraw:",
                                   "Great");
                                    cmds.AddItem(
                                  @"
                                    tpos [posX] [posY]\n
                                    talign[align number]\n
                                    tfont[font number]\n
                                    tbackcolor[decimal or hex]\n
                                    tboxcolor[decimal or hex]\n
                                    tforecolor[decimal or hex]\n
                                    theight[float - use, instead of.]\n
                                    tlettersize[float - use, instead of.]\n
                                    toutline[outline value]\n
                                    tpreviewmodel[model id]\n
                                    tpreviewprimarycolor[decimal or hex]\n
                                    tpreviewsecondarycolor[decimal or hex]\n
                                    tpreviewrotation[float - use, instead of.]\n
                                    tpreviewzoom[float - use, instead of.]\n
                                    tproportional[proportional value]\n
                                    tselectable[0 - no, 1 - yes]\n
                                    tusebox[0 - no, 1 - yes]\n
                                    twidth[float - use, instead of.]\n
                                    tshadow[shadow value]\n
                                    ttext[your text here]\n
                                   ");
                                    cmds.Show(thisPlayer);
                                    break;
                                }
                                // copy current textdraw
                            case 6:
                                {
                                    if(TextDrawEditorModel.textDrawBeingEdited == null)
                                        thisPlayer.SendClientMessage("You are not editing any textdraw.");
                                    else
                                    {

                                        InputDialog inputDialog = new InputDialog("Enter a name for your copied textdraw", "Insert name below: ", false, "Done", "Go back");

                                        inputDialog.Show(thisPlayer);
                                        inputDialog.Response += onInputResponse;

                                        void onInputResponse(object sender, DialogResponseEventArgs args)
                                        {
                                            if (args.DialogButton == DialogButton.Left)
                                            {
                                                if(TextDrawEditorModel.TextDrawFileExists(args.InputText))
                                                {
                                                    inputDialog.Show(thisPlayer);
                                                    thisPlayer.SendClientMessage("This textdraw name is already taken. Choose other.");
                                                }
                                                else
                                                {
                                                    var currentTxd = TextDrawEditorModel.textDrawBeingEdited;
                                                    EditorTextDraw newTextDraw = new EditorTextDraw(args.InputText, currentTxd.Position, currentTxd.Text);
                                                    newTextDraw.Alignment = currentTxd.Alignment;
                                                    newTextDraw.BackColor = currentTxd.BackColor;
                                                    newTextDraw.BoxColor = currentTxd.BoxColor;
                                                    newTextDraw.Font = currentTxd.Font;
                                                    newTextDraw.ForeColor = currentTxd.ForeColor;
                                                    newTextDraw.Height = currentTxd.Height;
                                                    newTextDraw.LetterSize = currentTxd.LetterSize;
                                                    newTextDraw.Outline = currentTxd.Outline;
                                                    newTextDraw.PreviewModel = currentTxd.PreviewModel;
                                                    newTextDraw.PreviewPrimaryColor = currentTxd.PreviewPrimaryColor;
                                                    newTextDraw.PreviewSecondaryColor = currentTxd.PreviewSecondaryColor;
                                                    newTextDraw.PreviewZoom = currentTxd.PreviewZoom;
                                                    newTextDraw.Proportional = currentTxd.Proportional;
                                                    newTextDraw.Shadow = currentTxd.Shadow;
                                                    newTextDraw.Selectable = currentTxd.Selectable;
                                                    newTextDraw.UseBox = currentTxd.UseBox;
                                                    newTextDraw.Width = currentTxd.Width;

                                                    thisPlayer.GameText($"{Color.Red}COPIED " + newTextDraw.name, 5000, 4);
                                                    TextDrawEditorModel.SaveTextDraw(newTextDraw);
                                                    TextDrawEditorModel.DoPlayerEditTextdraw(thisPlayer,newTextDraw);


                                                   
                                                }
                                              
                                               
                                            }
                                            else
                                            {
                                                options.Show(thisPlayer);
                                            }
                                        }
                             
                                    }
                                    break;
                                }
                        }

                    }
                }


            }
            else if (cmd == "tpos")
            {
                try
                {
                    Vector2 newTextDrawPos = new Vector2(float.Parse(cmdArgs[1]), float.Parse(cmdArgs[2]));
                    TextDrawEditorModel.textDrawBeingEdited.Position = newTextDrawPos;
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tpos [X position] [Y position]");
                }
            }
            else if (cmd == "talign")
            {
                try
                {
                    int alignment = int.Parse(cmdArgs[1]);
                    TextDrawEditorModel.textDrawBeingEdited.Alignment = (TextDrawAlignment)alignment;
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /talign [alignment intiger value]");
                }
            }
            else if (cmd == "tfont")
            {
                try
                {
                    int font = int.Parse(cmdArgs[1]);
                    TextDrawEditorModel.textDrawBeingEdited.Font = (TextDrawFont)font;
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tfont [font intiger value]");
                }
            }
            else if (cmd == "tbackcolor")
            {
                try
                {
                    int backColor = int.Parse(cmdArgs[1], System.Globalization.NumberStyles.HexNumber);

                    TextDrawEditorModel.textDrawBeingEdited.BackColor = backColor;
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tbackcolor [value]");
                }
            }
            else if (cmd == "tboxcolor")
            {
                try
                {

                    int boxColor = int.Parse(cmdArgs[1], System.Globalization.NumberStyles.HexNumber);
                    TextDrawEditorModel.textDrawBeingEdited.BoxColor = boxColor;
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tboxcolor [value]");
                }
            }
            else if (cmd == "tforecolor")
            {
                try
                {

                    uint foreColor = uint.Parse(cmdArgs[1], System.Globalization.NumberStyles.HexNumber);

                    TextDrawEditorModel.textDrawBeingEdited.ForeColor = foreColor;
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tforecolor [value]");
                }
            }
            else if (cmd == "theight")
            {
                try
                {

                    float height = float.Parse(cmdArgs[1]);
                    TextDrawEditorModel.textDrawBeingEdited.Height = height;
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /theight [value]");
                }
            }
            else if (cmd == "tlettersize")
            {
                try
                {

                    float letterSize1 = float.Parse(cmdArgs[1]);
                    float letterSize2 = float.Parse(cmdArgs[2]);
                    TextDrawEditorModel.textDrawBeingEdited.LetterSize = new Vector2(letterSize1, letterSize2);
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tlettersize [value] [value]");
                }
            }
            else if (cmd == "toutline")
            {
                try
                {
                    int outline = int.Parse(cmdArgs[1]);
                    TextDrawEditorModel.textDrawBeingEdited.Outline = outline;
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /toutline [value]");
                }
            }
            else if (cmd == "tpreviewmodel")
            {
                try
                {
                    int previewModel = int.Parse(cmdArgs[1]);
                    TextDrawEditorModel.textDrawBeingEdited.Font = TextDrawFont.PreviewModel;
                    TextDrawEditorModel.textDrawBeingEdited.PreviewModel = previewModel;

                    TextDrawEditorModel.textDrawBeingEdited.Show();

                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tpreviewmodel [value]");
                }
            }
            else if (cmd == "tpreviewprimarycolor")
            {
                try
                {

                    int primaryColor = int.Parse(cmdArgs[1], System.Globalization.NumberStyles.HexNumber);

                    TextDrawEditorModel.textDrawBeingEdited.PreviewPrimaryColor = primaryColor;
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tpreviewprimarycolor [value]");
                }
            }
            else if (cmd == "tpreviewsecondarycolor")
            {
                try
                {

                    int secondaryColor = int.Parse(cmdArgs[1], System.Globalization.NumberStyles.HexNumber);

                    TextDrawEditorModel.textDrawBeingEdited.PreviewSecondaryColor = secondaryColor;
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tpreviewsecondarycolor [value]");
                }
            }
            else if (cmd == "tpreviewrotation")
            {
                try
                {
                    TextDrawEditorModel.textDrawBeingEdited.PreviewRotation = new Vector3(float.Parse(cmdArgs[1]), float.Parse(cmdArgs[2]), float.Parse(cmdArgs[3]));
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tpreviewrotation [X] [Y] [Z]");
                }
            }
            else if (cmd == "tpreviewzoom")
            {
                try
                {
                    TextDrawEditorModel.textDrawBeingEdited.PreviewZoom = float.Parse(cmdArgs[1]);
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tpreviewzoom [value]");
                }
            }
            else if (cmd == "tproportional")
            {
                try
                {
                    int proportional = int.Parse(cmdArgs[1]);
                    if (proportional > 0)
                    {
                        TextDrawEditorModel.textDrawBeingEdited.Proportional = true;
                    }
                    else
                    {
                        TextDrawEditorModel.textDrawBeingEdited.Proportional = false;
                    }
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tproportional [0/1]");
                }
            }
            else if (cmd == "tselectable")
            {
                try
                {
                    int selectable = int.Parse(cmdArgs[1]);
                    if (selectable > 0)
                    {
                        TextDrawEditorModel.textDrawBeingEdited.Selectable = true;
                    }
                    else
                    {
                        TextDrawEditorModel.textDrawBeingEdited.Selectable = false;
                    }
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tselectable [0/1]");
                }
            }
            else if (cmd == "tusebox")
            {
                try
                {
                    int selectable = int.Parse(cmdArgs[1]);
                    if (selectable > 0)
                    {
                        TextDrawEditorModel.textDrawBeingEdited.UseBox = true;
                    }
                    else
                    {
                        TextDrawEditorModel.textDrawBeingEdited.UseBox = false;
                    }
                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tusebox [0/1]");
                }
            }
            else if (cmd == "twidth")
            {
                try
                {
                    float width = float.Parse(cmdArgs[1]);

                    TextDrawEditorModel.textDrawBeingEdited.Width = width;

                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /twidth [value]");
                }
            }
            else if (cmd == "tshadow")
            {
                try
                {
                    int shadow = int.Parse(cmdArgs[1]);

                    TextDrawEditorModel.textDrawBeingEdited.Shadow = shadow;

                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /tshadow [value]");
                }
            }
            else if (cmd == "ttext")
            {
                try
                {
                    string text = "";
                    for (int i = 1; i < cmdArgs.Length; i++)
                    {

                        if (i != 1)
                            text += " ";
                        text += cmdArgs[i];
                    }
                    TextDrawEditorModel.textDrawBeingEdited.Text = text;

                    TextDrawEditorModel.textDrawBeingEdited.Show();
                    TextDrawEditorModel.SaveTextDraw(TextDrawEditorModel.textDrawBeingEdited);
                }
                catch
                {
                    thisPlayer.SendClientMessage($"{Color.DarkGray}Proper usage: /ttext [text]");
                }
            }

        }


    }
}
