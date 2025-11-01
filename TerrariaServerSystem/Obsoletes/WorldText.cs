namespace TerrariaServerSystem.Obsoletes;

[Obsolete(message: "已过时")]
public class WorldText(string text)
{
    public string Text { get; set; } = text;
    public (int index, string name)? IndexName
    {
        get
        {
            var indexText = Text[0..2].Trim();
            if(!int.TryParse(indexText, out int index)) {
                return null;
            }

            var worldName = Text[2..].Trim();
            return (index, worldName);
        }
    }
}
