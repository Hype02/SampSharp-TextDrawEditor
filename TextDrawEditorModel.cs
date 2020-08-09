using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using SampSharp.GameMode.World;
using SampSharp.GameMode.Display;
using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using System.Text.Json.Serialization;
using System.Text.Json;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Pools;


namespace staterp
{
    public class TempTextDraw
    {
        public float positionX { get; set; }
        public float positionY{ get; set; }
        public string text{ get; set; }
        public int alignment{ get; set; }
        public int boxcolor{ get; set; }
        public uint font{ get; set; }
        public int forecolor{ get; set; }

        public float rotationX{ get; set; }
        public float rotationY{ get; set; }
        public float height{ get; set; }

        public float lettersizeX{ get; set; }
        public float lettersizeY{ get; set; }
        public int outline{ get; set; }
       

        public int previewModel{ get; set; }
        public int previewPrimaryColor{ get; set; }
        public float previewRotationX{ get; set; }
        public float previewRotationY{ get; set; }
        public float previewRotationZ{ get; set; }
        public int previewSecondaryColor{ get; set; }
        public float previewZoom{ get; set; }
        public bool proportional{ get; set; }

        public bool selectable{ get; set; }
        public int shadow{ get; set; }
        public bool usebox{ get; set; }
        public int backcolor{ get; set; }
        public float width{ get; set; }
    }

    public class EditorTextDraw : TextDraw
    {
        public static List<EditorTextDraw> All = new List<EditorTextDraw>();
        public string name { get; set; }
        public EditorTextDraw(string _name, Vector2 pos, string text) : base (pos, text)
        {
            name = _name;
            All.Add(this);
        }
    }
    public static class TextDrawEditorModel
    {
 
        public static string TextDrawsPath
        {
            get
            {
                if (!Directory.Exists(Directory.GetCurrentDirectory() + "/s#tde/textdraws/"))
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + "/s#tde/textdraws/");
                return Directory.GetCurrentDirectory() + "/s#tde/textdraws/";
            }
        }
        
        public static EditorTextDraw textDrawBeingEdited;
        public static void DoPlayerEditTextdraw(BasePlayer thisPlayer, EditorTextDraw txd)
        {
            thisPlayer.GameText("~g~~h~EDITING~n~~w~~n~" + txd.name, 3000, 4);
            thisPlayer.SendClientMessage("You are now editing textdraw: " + txd.name);
            textDrawBeingEdited = txd;
        }
        public static bool TextDrawFileExists(string fileName)
        {
            return File.Exists(TextDrawsPath + fileName + ".txt");
        }
        public static EditorTextDraw CreateTextDraw(string textDrawName)
        {
            EditorTextDraw newTextDraw = new EditorTextDraw(textDrawName, new Vector2(250, 250), "ChangeMeIfYouWant");
            newTextDraw.Width = 10000.0f; // so you see your textdraw
            newTextDraw.Show();
            SaveTextDraw(newTextDraw);
            return newTextDraw;
        }
        public static void SaveTextDraw(EditorTextDraw textDraw)
        {
            File.WriteAllText(TextDrawsPath + textDraw.name + ".txt", 
                $"public TextDraw {textDraw.name} = new TextDraw(new Vector2({textDraw.Position.X}, {textDraw.Position.Y}), '{textDraw.Text}')"+"{" +"\n" +
                $"Alignment = (SampSharp.GameMode.Definitions.TextDrawAlignment){(int)textDraw.Alignment},\n" +
                $"BoxColor = {(int)textDraw.BoxColor},\n"+
                $"Font = (SampSharp.GameMode.Definitions.TextDrawFont){(uint)textDraw.Font},\n" +
                $"ForeColor = {(int)textDraw.ForeColor},\n" +
                $"Height = {(float)textDraw.Height},\n" +
                $"LetterSize = new Vector2({(float)textDraw.LetterSize.X}, {(float)textDraw.LetterSize.Y}),\n" +
                $"Outline = {(int)textDraw.Outline},\n" +
                $"PreviewModel = {(int)textDraw.PreviewModel},\n" +
                $"PreviewPrimaryColor = {(int)textDraw.PreviewPrimaryColor},\n" +
                $"PreviewRotation = new Vector3({textDraw.PreviewRotation.X}, {textDraw.PreviewRotation.Y}, {textDraw.PreviewRotation.Z}),\n" +
                $"PreviewSecondaryColor = {(int)textDraw.PreviewSecondaryColor},\n" +
                $"PreviewZoom = {(float)textDraw.PreviewZoom},\n" +
                $"Proportional = true,\n" +
                $"Selectable = {((bool)textDraw.Selectable).ToString().ToLower()},\n" +
                $"Shadow = {(int)textDraw.Shadow},\n" +
                $"UseBox = { ((bool)textDraw.UseBox).ToString().ToLower() },\n" +
                $"Width = {(float)textDraw.Width},\n" +
                $"BackColor = {(int)textDraw.BackColor}"+"};");



            TempTextDraw tmpTxd = new TempTextDraw();
            tmpTxd.text = textDraw.Text;
            tmpTxd.positionX = textDraw.Position.X;
            tmpTxd.positionY = textDraw.Position.Y;
            tmpTxd.alignment = (int)textDraw.Alignment;
            tmpTxd.boxcolor = textDraw.BoxColor;
            tmpTxd.font = (uint)textDraw.Font;
            tmpTxd.forecolor = textDraw.ForeColor;
            tmpTxd.height = textDraw.Height;
            tmpTxd.lettersizeX = textDraw.LetterSize.X;
            tmpTxd.lettersizeY = textDraw.LetterSize.Y;
            tmpTxd.outline = textDraw.Outline;
            tmpTxd.previewModel = textDraw.PreviewModel;
            tmpTxd.previewPrimaryColor = textDraw.PreviewPrimaryColor;
            tmpTxd.previewRotationX = textDraw.PreviewRotation.X;
            tmpTxd.previewRotationY = textDraw.PreviewRotation.Y;
            tmpTxd.previewRotationZ = textDraw.PreviewRotation.Z;
            tmpTxd.previewSecondaryColor = textDraw.PreviewSecondaryColor;
            tmpTxd.previewZoom = textDraw.PreviewZoom;
            tmpTxd.proportional = textDraw.Proportional;
            tmpTxd.selectable = textDraw.Selectable;
            tmpTxd.usebox = textDraw.UseBox;
            tmpTxd.width = textDraw.Width;
            tmpTxd.backcolor = (int)textDraw.BackColor;


            string json = JsonSerializer.Serialize(tmpTxd);
            Console.WriteLine(json);
          File.WriteAllText(TextDrawsPath+ textDraw.name + ".json", json);
        }
        public static EditorTextDraw LoadTextDraw(string textDrawName)
        {
            string jsonPath = File.ReadAllText(TextDrawsPath + textDrawName + ".json");


             TempTextDraw textdraw = JsonSerializer.Deserialize<TempTextDraw>(jsonPath);

            EditorTextDraw loadedTextDraw = new EditorTextDraw(textDrawName, new Vector2(textdraw.positionX, textdraw.positionY), textdraw.text);
            loadedTextDraw.Alignment = (TextDrawAlignment)textdraw.alignment;
            loadedTextDraw.BackColor = textdraw.backcolor;
            loadedTextDraw.BoxColor = textdraw.boxcolor;
            loadedTextDraw.Font =(TextDrawFont)textdraw.font;
            loadedTextDraw.ForeColor = textdraw.forecolor;
            loadedTextDraw.Height = textdraw.height;
            loadedTextDraw.LetterSize = new Vector2(textdraw.lettersizeX, textdraw.lettersizeY);
            loadedTextDraw.Outline = textdraw.outline;
            loadedTextDraw.PreviewModel = textdraw.previewModel;
            loadedTextDraw.PreviewPrimaryColor = textdraw.previewPrimaryColor;
            loadedTextDraw.PreviewRotation = new Vector3(textdraw.previewRotationX, textdraw.previewRotationY, textdraw.previewRotationZ);
            loadedTextDraw.PreviewSecondaryColor = textdraw.previewSecondaryColor;
            loadedTextDraw.PreviewZoom = textdraw.previewZoom;
            loadedTextDraw.Proportional = textdraw.proportional;
            loadedTextDraw.Selectable = textdraw.selectable;
            loadedTextDraw.Shadow = textdraw.shadow;
            loadedTextDraw.UseBox = textdraw.usebox;
            loadedTextDraw.Width = textdraw.width;
    
                    
            return loadedTextDraw;
        }
    }
}
