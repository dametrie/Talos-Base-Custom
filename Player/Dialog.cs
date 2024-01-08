using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talos.Player
{
    internal class Dialog
    {
        private Client _client;
        private DateTime _lastReply;

        internal byte DialogType { get; set; }
        internal byte ObjectType { get; set; }
        internal int ObjectID { get; set; }
        internal byte Unknown1 { get; set; }
        internal ushort Sprite1 { get; set; }
        internal byte Color1 { get; set; }
        internal byte Unknown2 { get; set; }
        internal ushort Sprite2 { get; set; }
        internal byte Color2 { get; set; }
        internal ushort PursuitID { get; set; }
        internal ushort DialogID { get; set; }
        internal bool PreviousButton { get; set; }
        internal bool NextButton { get; set; }
        internal byte Unknown3 { get; set; }
        internal string ObjectName { get; set; }
        internal string Message { get; set; }
        internal List<string> Options { get; set; }
        internal string TopCaption { get; set; }
        internal byte InputLength { get; set; }
        internal string BottomCaption { get; set; }

        internal Dialog(byte dialogType, byte objectType, int objectID, byte unknown1, ushort sprite1, byte color1, byte unknown2, ushort sprite2, byte color2, 
            ushort pursuitID, ushort dialogID, bool previousButton, bool nextButton, byte unknown3, string objectName, string message, Client client)
        {
            DialogType = dialogType;
            ObjectType = objectType;
            ObjectID = objectID;
            Unknown1 = unknown1;
            Sprite1 = sprite1;
            Color1 = color1;
            Unknown2 = unknown2;
            Sprite2 = sprite2;
            Color2 = color2;
            PursuitID = pursuitID;
            DialogID = dialogID;
            PreviousButton = previousButton;
            NextButton = nextButton;
            Unknown3 = unknown3;
            ObjectName = objectName;
            Message = message;
            Options = new List<string>();
            TopCaption = string.Empty;
            BottomCaption = string.Empty;
            _client = client;
            _lastReply = DateTime.MinValue;
        }

        internal Dialog(byte dialogType, byte objectType, int objectID, byte unknown1, ushort sprite1, byte color1, byte unknown2, ushort sprite2, byte color2, 
            ushort pursuitID, ushort dialogID, bool previousButton, bool nextButton, byte unknown3, string objectName, string message, List<string> options, Client client) 
            : this(dialogType, objectType, objectID, unknown1, sprite1, color1, unknown2, sprite2, color2, pursuitID, dialogID, previousButton, nextButton, unknown3, objectName, message, client)
        {
            Options = options;
        }

        internal Dialog(byte dialogType, byte objectType, int objectID, byte unknown1, ushort sprite1, byte color1, byte unknown2, ushort sprite2, byte color2, 
            ushort pursuitID, ushort dialogID, bool previousButton, bool nextButton, byte unknown3, string objectName, string mesage, string topCaption, byte inputLength, string bottomCaption, Client client) 
            : this(dialogType, objectType, objectID, unknown1, sprite1, color1, unknown2, sprite2, color2, pursuitID, dialogID, previousButton, nextButton, unknown3, objectName, mesage, client)
        {
            TopCaption = topCaption;
            InputLength = inputLength;
            BottomCaption = bottomCaption;
        }

        internal Dialog(byte dialogType, byte objectType, int objectID, byte unknown1, ushort sprite1, byte color1, byte unknown2, ushort sprite2, byte color2, 
            ushort pursuitID, ushort DialogID, bool previousButton, bool nextButton, byte unknown3, string objectName, string message, List<string> options, string topCaption, 
            byte inputLength, string bottomCaption, Client client) 
            : this(dialogType, objectType, objectID, unknown1, sprite1, color1, unknown2, sprite2, color2, pursuitID, DialogID, previousButton, nextButton, unknown3, objectName, message, client)
        {
            Options = options;
            TopCaption = topCaption;
            InputLength = inputLength;
            BottomCaption = bottomCaption;
        }

        internal void DialogPrevious()
        {
            if (DateTime.UtcNow.Subtract(this._lastReply).TotalSeconds >= 1.0)
            {
                this._client.ReplyDialog(this.ObjectType, this.ObjectID, this.PursuitID, (ushort)(this.DialogID - 1));
                this._lastReply = DateTime.UtcNow;
            }
        }

        internal void DialogNext()
        {
            if (DateTime.UtcNow.Subtract(this._lastReply).TotalSeconds >= 1.0)
            {
                this._client.ReplyDialog(this.ObjectType, this.ObjectID, this.PursuitID, (ushort)(this.DialogID + 1));
                this._lastReply = DateTime.UtcNow;
            }
        }

        internal void DialogNext(byte num)
        {
            if (DateTime.UtcNow.Subtract(this._lastReply).TotalSeconds >= 1.0)
            {
                this._client.ReplyDialog(this.ObjectType, this.ObjectID, this.PursuitID, (ushort)(this.DialogID + 1), num);
                this._lastReply = DateTime.UtcNow;
            }
        }

        internal void DialogNext(string response)
        {
            if (DateTime.UtcNow.Subtract(this._lastReply).TotalSeconds >= 1.0)
            {
                this._client.ReplyDialog(this.ObjectType, this.ObjectID, this.PursuitID, (ushort)(this.DialogID + 1), response);
                this._lastReply = DateTime.UtcNow;
            }
        }

        internal void Reply()
        {
            if (DateTime.UtcNow.Subtract(this._lastReply).TotalSeconds >= 1.0)
            {
                this._client.ReplyDialog(this.ObjectType, this.ObjectID, this.PursuitID, this.DialogID);
                this._lastReply = DateTime.UtcNow;
            }
        }


    }
}
