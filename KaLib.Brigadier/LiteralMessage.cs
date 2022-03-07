namespace KaLib.Brigadier
{
    public class LiteralMessage : IMessage {
        private readonly string _str;

        public LiteralMessage(string str) {
            this._str = str;
        }

        public string GetString() {
            return _str;
        }

        public override string ToString() {
            return _str;
        }
    }
}